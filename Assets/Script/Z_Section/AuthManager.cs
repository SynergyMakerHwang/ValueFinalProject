using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1. 로그인: 이메일, 패스워드 입력시 회원가입 여부에 따라 로그인
/// 2. 회원가입: 이메일, 패스워드 입력 후 이메일 인증을 통해 인증이 된다면 회원가입 완료!
/// 3. 불러오기: 권한에 따라 DB의 특정 정보를 불러오기
/// 
/// class diagram
/// (-) void Initialization(void)
/// (+) void SignIn(string, string)
/// (+) void SignUp(string, string)
/// (-) void SendVerificationEmail(string)
/// </summary>
public class AuthManager : MonoBehaviour
{
    [Serializable]
    public class UserInfo
    {
        public string id;
        public string pw;
        public string email;
        public string name;
        public string role;
        public string products;
        public string history;
    }

    [Header("로그인 UI")]
    [SerializeField] GameObject signInPanel;
    [SerializeField] TMP_InputField signInEmailInput;
    [SerializeField] TMP_InputField signInPWInput;
    [SerializeField] Button signInBtn;
    [SerializeField] Button signUpBtn;        
    
    [Header("회원가입 UI")]
    [SerializeField] GameObject signUpPanel;    
    [SerializeField] TMP_InputField signUpNameInput;
    [SerializeField] TMP_InputField signUpEmailInput;
    [SerializeField] TMP_InputField signUpPWInput;
    [SerializeField] TMP_InputField signUpPWCheckInput;
    [SerializeField] Button signUpCheckBtn;
    [SerializeField] Button cancelBtn;
    

    [Header("사용자 UI")]
    [SerializeField] GameObject userPanel;
    [SerializeField] Button signOutBtn;
    [SerializeField] TMP_Text infoTxt;
    [SerializeField] UserInfo userSignedIn;

    [Header("Alert UI")]
    [SerializeField] GameObject verificationPanel;
    [SerializeField] TMP_Text verficationMsg;
    

    FirebaseAuth auth;
    FirebaseUser user;

    void Start()
    {
        Initialization();
    }

    void Initialization()
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;

        AuthStateChanged(this, null);

        //auth.SignOut();
    }


    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool isSignedIn = (user != auth.CurrentUser) && (auth.CurrentUser != null);

            if (isSignedIn == false && user != null)
            {
                print("Signed Out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (isSignedIn == true)
            {
                print("Signed In " + user.UserId);
            }
        }
    }


    /****************** Button Event Start********************/

    public void OnMoveSignUpPageBtnClkEvent()
    {
        signInPanel.SetActive(false);
        signUpPanel.SetActive(true);
    }

    public void OnSignUpBtnClkEvent()
    {
        StartCoroutine(SignUp(signUpEmailInput.text, signUpPWInput.text, signUpPWCheckInput.text));
    }

    public void OnResetPWBtnClkEvent()
    {
        StartCoroutine(ResetPassword(signInEmailInput.text));
    }


    public void OnSignOutBtnClkEvent()
    {
        if (user == auth.CurrentUser)
        {
            auth.SignOut();
            StartCoroutine(TurnMessagePanel("로그아웃 되었습니다."));
        }
    }



    public void OnCancelBtnClkEvent()
    {
        signUpPanel.SetActive(false);
    }

    public void OnExitBtnClkEvent()
    {
        Application.Quit();
    }

    public void OnShowInfoBtnClkEvent()
    {
        StartCoroutine(ShowInfo());
    }

    public void OnSignInBtnClkEvent()
    {
        signInPanel.SetActive(true);
        signUpPanel.SetActive(false);

        StartCoroutine(SignIn(signInEmailInput.text, signInPWInput.text));

    }

    /****************** Button Event End ********************/








    /****************** 로그인 Start********************/

    Coroutine timerCoroutine;
    int timerTime;
    public IEnumerator StartTimer(int time)
    {
        timerTime = 0;

        while (true)
        {
            timerTime++;

            if (timerTime >= time)
                break;

            yield return new WaitForSeconds(1);
        }
    }

    int wrongPWCnt = 0;
    int maxWrongPWCnt = 5;
    public IEnumerator SignIn(string email, string password)
    {
        if (wrongPWCnt == maxWrongPWCnt)
        {
            StartCoroutine(TurnMessagePanel($"비밀번호가 {wrongPWCnt}회 틀렸습니다. {timerTime}초 후 다시 시도해 주세요."));

            if (timerCoroutine == null)
                timerCoroutine = StartCoroutine(StartTimer(60));

            yield break;
        }

        Task task = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => task.IsCompleted == true);

        if (task.Exception != null)
        {
            FirebaseException e = task.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)e.ErrorCode;

            switch (authError)
            {
                case AuthError.EmailAlreadyInUse:
                    StartCoroutine(TurnMessagePanel("이미 등록된 이메일입니다."));
                    print("이미 등록된 이메일입니다.");
                    yield break;
                case AuthError.InvalidEmail:
                    StartCoroutine(TurnMessagePanel("유효하지 않은 이메일 입니다."));
                    print("유효하지 않은 이메일 입니다.");
                    yield break;
                case AuthError.WrongPassword:
                    wrongPWCnt++;
                    StartCoroutine(TurnMessagePanel($"비밀번호가 {wrongPWCnt}회 틀렸습니다. \n{maxWrongPWCnt - wrongPWCnt}회 남음"));
                    print($"비밀번호가 {wrongPWCnt}회 틀렸습니다. {maxWrongPWCnt - wrongPWCnt}회 남음");
                    yield break;
            }
        }

        try
        {
            if (user.IsEmailVerified)
            {
                StartCoroutine(TurnMessagePanel("로그인이 성공적으로 완료 되었습니다."));
                print("로그인이 되었습니다.");

                wrongPWCnt = 0;

                //공정 설정으로 페이지 전환
                UserInterfaceManager.instance.transUserMode();

            }
            else
            {
                StartCoroutine(TurnMessagePanel($"인증메일을 {email}로 보냈습니다.\n이메일을 확인해 주세요."));
                print($"인증메일을 {email}로 보냈습니다.\n이메일을 확인해 주세요.");
            }
        }
        catch (Exception e)
        {
            print(e.ToString());
        }

        yield return new WaitForSeconds(3);

        if (verificationPanel.activeSelf)
        {
            verificationPanel.SetActive(false);
        }
    }
    //인증 메일 발송
    private IEnumerator SendVerificationEmail(string email)
    {
        FirebaseUser user = auth.CurrentUser;
        print(user.UserId);

        Task task = null;
        if (user != null)
        {
            task = user.SendEmailVerificationAsync();

            yield return new WaitUntil(() => task.IsCompleted == true);

            if (task.Exception != null)
            {
                FirebaseException e = task.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)e.ErrorCode;
                print(authError);
            }

        }

        StartCoroutine(TurnMessagePanel($"인증메일을 {email}로 보냈습니다.\n이메일을 확인해 주세요."));

        yield return new WaitForSeconds(3);

        signInPanel.SetActive(true);
        signUpPanel.SetActive(false);
        verificationPanel.SetActive(false);
    }

    /******************로그인 End ********************/



    /******************회원가입 Start********************/
    /******************회원가입 End ********************/


    /****************** 알림 영역 Start********************/

    IEnumerator TurnMessagePanel(string message)
    {
        verificationPanel.SetActive(true);
        verficationMsg.text = message;

        yield return new WaitForSeconds(3);

        verificationPanel.SetActive(false);
    }



    IEnumerator ShowInfo()
    {
        if (user != null)
        {
            string userInfo = string.Empty;

            if (FirebaseManager.instance != null)
            {
                DatabaseReference dbref = FirebaseManager.instance.dbRef;

                Task t = dbref.Child("users").Child(user.UserId).GetValueAsync().ContinueWith(task =>
                {
                    string json = task.Result.GetRawJsonValue();

                    userSignedIn = JsonConvert.DeserializeObject<UserInfo>(json);

                    if (userSignedIn.role == "user")
                    {
                        DataSnapshot userData = task.Result;

                        foreach (var item in userData.Children)
                        {
                            userInfo += $"{item.Key}: {item.Value}\n";
                        }
                    }

                    if (task.Exception != null)
                        print(task.Exception);
                });

                yield return new WaitUntil(() => t.IsCompleted);

                if (userSignedIn.role == "admin")
                {
                    Task getAlluser = dbref.Child("users").GetValueAsync().ContinueWith(adminTask =>
                    {
                        if (adminTask.IsCompleted)
                        {
                            DataSnapshot adminData = adminTask.Result;

                            foreach (var item in adminData.Children)
                            {
                                IDictionary value = (IDictionary)item.Value;

                                userInfo += $"{item.Key}: {value["email"]}, {value["name"]}, {value["role"]}\n";
                            }

                            print(userInfo);
                        }
                    });

                    yield return new WaitUntil(() => getAlluser.IsCompleted);

                }

                infoTxt.text = userInfo;

                print("데이터 읽기가 완료되었습니다.");
            }
        }
    }
    /****************** 알림 영역 End ********************/



    /****************** 기타 ( 비밀번호 재설정) Start********************/

    bool isInvalid = false;
    public IEnumerator SignUp(string email, string password, string passwordCheck)
    {
        if (email == "" || password == "" || passwordCheck == "")
        {
            StartCoroutine(TurnMessagePanel("이메일 또는 패스워드를 입력해 주세요"));
            print("이메일 또는 패스워드를 입력해 주세요");
            yield break;
        }

        if (password != passwordCheck)
        {
            StartCoroutine(TurnMessagePanel("비밀번호와 확인 비밀번호가 같지 않습니다. 비밀번호를 확인해 주세요."));
            print("비밀번호와 확인 비밀번호가 같지 않습니다. 비밀번호를 확인해 주세요.");
            yield break;
        }

        Task task = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => task.IsCompleted == true);

        if (task.Exception != null)
        {
            FirebaseException e = task.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)e.ErrorCode;
            print(authError);
            switch (authError)
            {
                case AuthError.InvalidEmail:
                    StartCoroutine(TurnMessagePanel("유효하지 않은 이메일 포멧입니다."));
                    print("유효하지 않은 이메일 포멧입니다.");
                    isInvalid = true;
                    yield break;
                case AuthError.WeakPassword:
                    StartCoroutine(TurnMessagePanel("비밀번호가 취약합니다."));
                    print("비밀번호가 취약합니다.");
                    isInvalid = true;
                    yield break;
                case AuthError.EmailAlreadyInUse:
                    StartCoroutine(TurnMessagePanel("이미 사용중인 이메일입니다."));
                    print("이미 사용중인 이메일입니다.");
                    isInvalid = true;
                    yield break;
            }
        }

        SetID();

        StartCoroutine(SendVerificationEmail(email));

        signUpNameInput.text = "";
        signUpEmailInput.text = "";
        signUpPWInput.text = "";
        signUpPWCheckInput.text = "";

        void SetID()
        {
            string json = $"{{\"role\":\"admin\",\"name\":\"{signUpNameInput.text}\",\"email\":\"{user.Email}\"}}";

            print(json);

            DatabaseReference dbRef = FirebaseManager.instance.dbRef;
            dbRef.Child("users").Child(user.UserId).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    print("정보 등록이 완료되었습니다.");
                }
            });
        }
    }






    public IEnumerator ResetPassword(string email)
    {
        Task task = auth.SendPasswordResetEmailAsync(email).ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                FirebaseException e = t.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)e.ErrorCode;

                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        StartCoroutine(TurnMessagePanel("유효하지 않은 이메일 입니다."));
                        print("유효하지 않은 이메일 입니다.");
                        break;
                    case AuthError.WrongPassword:
                        break;
                }
            }
        });

        yield return new WaitUntil(() => task.IsCompleted == true);

        StartCoroutine(TurnMessagePanel($"{email}로 비밀번호 재설정 이메일을 보냈습니다. \n이메일을 확인해 주세요."));
    }
    /****************** 기타 ( 비밀번호 재설정) End ********************/


}