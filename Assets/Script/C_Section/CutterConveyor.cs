using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class CutterConveyor : MonoBehaviour
{
    public static CutterConveyor Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    [SerializeField] Transform MainStart;
    [SerializeField] Transform MainEnd;
    [SerializeField] Transform SubStart;
    [SerializeField] Transform SubEnd;

    public float MainConveyorSpeed;
    public float SubConveyorSpeed;

    [SerializeField] Transform MainBelt;
    [SerializeField] Transform SubBelt;
    Vector3 resumePos1;
    Vector3 resumePos2;

    Coroutine MainCoroutine;
    Coroutine SubCoroutine;
    private void Start()
    {
        // 초기설정

        resumePos1 = MainStart.localPosition;
        MainBelt.localPosition = resumePos1;
        resumePos2 = SubStart.localPosition;
        SubBelt.localPosition = resumePos2;

    }


    IEnumerator MainGo()
    {


        while (true)
        {
            Vector3 from = resumePos1;
            Vector3 to = MainEnd.localPosition;
            float Length = Vector3.Distance(from, to);
            float Journey = Length / MainConveyorSpeed;
            float currentTime = 0;

            while (currentTime < Journey)
            {
                currentTime += Time.deltaTime;
                MainBelt.localPosition = Vector3.Lerp(from, to, currentTime / Journey);

                yield return null; // 다음 프레임까지 대기
            }
            float Distance = Vector3.Distance(MainBelt.localPosition, MainEnd.localPosition);
            if (Distance < 5)
                resumePos1 = MainStart.localPosition;


            MainBelt.localPosition = MainStart.localPosition;

        }

    }
    IEnumerator SubGo()
    {


        while (true)
        {
            Vector3 from = resumePos2;
            Vector3 to = SubEnd.localPosition;
            float Length = Vector3.Distance(from, to);
            float Journey = Length / SubConveyorSpeed;
            float currentTime = 0;

            while (currentTime < Journey)
            {
                currentTime += Time.deltaTime;
                SubBelt.localPosition = Vector3.Lerp(from, to, currentTime / Journey);

                yield return null; // 다음 프레임까지 대기
            }
            float Distance = Vector3.Distance(SubBelt.localPosition, SubEnd.localPosition);
            if (Distance < 5)
                resumePos2 = SubStart.localPosition;


            SubBelt.localPosition = SubStart.localPosition;

        }

    }

    public void MaingGoPLC()
    {
        if (MainCoroutine == null)
            MainCoroutine = StartCoroutine(MainGo());
    }
    public void MainStopPLC()
    {
        if (MainCoroutine != null)
        {
            StopCoroutine(MainCoroutine);
            resumePos1 = MainBelt.transform.localPosition;
            MainCoroutine = null;
        }
    }

    public void SubGoPLC()
    {
        if (SubCoroutine == null)
            SubCoroutine = StartCoroutine(SubGo());
    }
    public void SubStopPLC()
    {
        if (SubCoroutine != null)
        {
            StopCoroutine(SubCoroutine);
            resumePos2 = SubBelt.transform.localPosition;
            SubCoroutine = null;
        }


    }
}
