using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

public class CutterConveyor : MonoBehaviour
{
    [SerializeField] Transform MainStart;
    [SerializeField] Transform MainEnd;
    [SerializeField] Transform SubStart;
    [SerializeField] Transform SubEnd;

    float MainConveyorSpeed = 1;
    float SubConveyorSpeed = 1.5f;
    public bool IsMain;
    public bool IsSub;

    [SerializeField] GameObject MainBelt;
    [SerializeField] GameObject SubBelt;
    Vector3 resumePos1;
    Vector3 resumePos2;
    int isMainCnt = 0;

    private void Start()
    {
        // 초기설정
        MainBelt.transform.localPosition = MainStart.localPosition;
        SubBelt.transform.localPosition = SubStart.localPosition;

        resumePos1 = MainStart.localPosition;
        resumePos2 = SubStart.localPosition;
        MaingGoPLC();
        SubGoPLC();
    }

    void Update()
    {
        if (IsMain)
            StartCoroutine(MainGo());
        if (IsSub)
            StartCoroutine(SubGo());
    }

    IEnumerator Go(float Speed, Vector3 from, Vector3 to, GameObject Belt, bool IsOn)
    {


        float Length = Vector3.Distance(from, to);
        float journeyTime = Length / Speed;
        float CurrentTime = 0;

        while (CurrentTime < journeyTime)
        {
            if (!IsMain && Belt == MainBelt) // IsOn이 false일 경우 즉시 코루틴 종료
            {
                resumePos1 = MainBelt.transform.localPosition;

                yield break;
            }
            else if (!IsSub && Belt == SubBelt)
            {
                resumePos2 = SubBelt.transform.localPosition;
                yield break;
            }

            CurrentTime += Time.deltaTime;
            Belt.transform.localPosition = Vector3.Lerp(from, to, CurrentTime / journeyTime);
            yield return null;
        }


        resumePos1 = MainStart.localPosition;


    }

    IEnumerator MainGo()
    {
        while (IsMain)
        {

            yield return StartCoroutine(Go(MainConveyorSpeed, resumePos1, MainEnd.localPosition, MainBelt, IsMain));
        }


    }

    IEnumerator SubGo()
    {
        while (IsSub)
        {
            yield return StartCoroutine(Go(SubConveyorSpeed, SubStart.localPosition, SubEnd.localPosition, SubBelt, IsSub));
        }
    }

    public bool MaingGoPLC()
    {
        return IsMain = true;
    }
    public bool MainStopPLC()
    {
        return IsMain = false;
    }

    public bool SubGoPLC()
    {
        return IsSub = true;
    }
    public bool SubStopPLC()
    {

        return IsSub = false;

    }
}
