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
/// 1. �α���: �̸���, �н����� �Է½� ȸ������ ���ο� ���� �α���
/// 2. ȸ������: �̸���, �н����� �Է� �� �̸��� ������ ���� ������ �ȴٸ� ȸ������ �Ϸ�!
/// 3. �ҷ�����: ���ѿ� ���� DB�� Ư�� ������ �ҷ�����
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

    [Header("�α��� UI")]
    [SerializeField] GameObject signInPanel;
    [SerializeField] TMP_InputField signInEmailInput;
    [SerializeField] TMP_InputField signInPWInput;
    [SerializeField] Button signInBtn;
    [SerializeField] Button signUpBtn;        
    
    [Header("ȸ������ UI")]
    [SerializeField] GameObject signUpPanel;    
    [SerializeField] TMP_InputField signUpNameInput;
    [SerializeField] TMP_InputField signUpEmailInput;
    [SerializeField] TMP_InputField signUpPWInput;
    [SerializeField] TMP_InputField signUpPWCheckInput;
    [SerializeField] Button signUpCheckBtn;
    [SerializeField] Button cancelBtn;
    

    [Header("����� UI")]
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
            StartCoroutine(TurnMessagePanel("�α׾ƿ� �Ǿ����ϴ�."));
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








    /****************** �α��� Start********************/

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
            StartCoroutine(TurnMessagePanel($"��й�ȣ�� {wrongPWCnt}ȸ Ʋ�Ƚ��ϴ�. {timerTime}�� �� �ٽ� �õ��� �ּ���."));

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
                    StartCoroutine(TurnMessagePanel("�̹� ��ϵ� �̸����Դϴ�."));
                    print("�̹� ��ϵ� �̸����Դϴ�.");
                    yield break;
                case AuthError.InvalidEmail:
                    StartCoroutine(TurnMessagePanel("��ȿ���� ���� �̸��� �Դϴ�."));
                    print("��ȿ���� ���� �̸��� �Դϴ�.");
                    yield break;
                case AuthError.WrongPassword:
                    wrongPWCnt++;
                    StartCoroutine(TurnMessagePanel($"��й�ȣ�� {wrongPWCnt}ȸ Ʋ�Ƚ��ϴ�. \n{maxWrongPWCnt - wrongPWCnt}ȸ ����"));
                    print($"��й�ȣ�� {wrongPWCnt}ȸ Ʋ�Ƚ��ϴ�. {maxWrongPWCnt - wrongPWCnt}ȸ ����");
                    yield break;
            }
        }

        try
        {
            if (user.IsEmailVerified)
            {
                StartCoroutine(TurnMessagePanel("�α����� ���������� �Ϸ� �Ǿ����ϴ�."));
                print("�α����� �Ǿ����ϴ�.");

                wrongPWCnt = 0;

                //���� �������� ������ ��ȯ
                UserInterfaceManager.instance.transUserMode();

            }
            else
            {
                StartCoroutine(TurnMessagePanel($"���������� {email}�� ���½��ϴ�.\n�̸����� Ȯ���� �ּ���."));
                print($"���������� {email}�� ���½��ϴ�.\n�̸����� Ȯ���� �ּ���.");
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
    //���� ���� �߼�
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

        StartCoroutine(TurnMessagePanel($"���������� {email}�� ���½��ϴ�.\n�̸����� Ȯ���� �ּ���."));

        yield return new WaitForSeconds(3);

        signInPanel.SetActive(true);
        signUpPanel.SetActive(false);
        verificationPanel.SetActive(false);
    }

    /******************�α��� End ********************/



    /******************ȸ������ Start********************/
    /******************ȸ������ End ********************/


    /****************** �˸� ���� Start********************/

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

                print("������ �бⰡ �Ϸ�Ǿ����ϴ�.");
            }
        }
    }
    /****************** �˸� ���� End ********************/



    /****************** ��Ÿ ( ��й�ȣ �缳��) Start********************/

    bool isInvalid = false;
    public IEnumerator SignUp(string email, string password, string passwordCheck)
    {
        if (email == "" || password == "" || passwordCheck == "")
        {
            StartCoroutine(TurnMessagePanel("�̸��� �Ǵ� �н����带 �Է��� �ּ���"));
            print("�̸��� �Ǵ� �н����带 �Է��� �ּ���");
            yield break;
        }

        if (password != passwordCheck)
        {
            StartCoroutine(TurnMessagePanel("��й�ȣ�� Ȯ�� ��й�ȣ�� ���� �ʽ��ϴ�. ��й�ȣ�� Ȯ���� �ּ���."));
            print("��й�ȣ�� Ȯ�� ��й�ȣ�� ���� �ʽ��ϴ�. ��й�ȣ�� Ȯ���� �ּ���.");
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
                    StartCoroutine(TurnMessagePanel("��ȿ���� ���� �̸��� �����Դϴ�."));
                    print("��ȿ���� ���� �̸��� �����Դϴ�.");
                    isInvalid = true;
                    yield break;
                case AuthError.WeakPassword:
                    StartCoroutine(TurnMessagePanel("��й�ȣ�� ����մϴ�."));
                    print("��й�ȣ�� ����մϴ�.");
                    isInvalid = true;
                    yield break;
                case AuthError.EmailAlreadyInUse:
                    StartCoroutine(TurnMessagePanel("�̹� ������� �̸����Դϴ�."));
                    print("�̹� ������� �̸����Դϴ�.");
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
                    print("���� ����� �Ϸ�Ǿ����ϴ�.");
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
                        StartCoroutine(TurnMessagePanel("��ȿ���� ���� �̸��� �Դϴ�."));
                        print("��ȿ���� ���� �̸��� �Դϴ�.");
                        break;
                    case AuthError.WrongPassword:
                        break;
                }
            }
        });

        yield return new WaitUntil(() => task.IsCompleted == true);

        StartCoroutine(TurnMessagePanel($"{email}�� ��й�ȣ �缳�� �̸����� ���½��ϴ�. \n�̸����� Ȯ���� �ּ���."));
    }
    /****************** ��Ÿ ( ��й�ȣ �缳��) End ********************/


}