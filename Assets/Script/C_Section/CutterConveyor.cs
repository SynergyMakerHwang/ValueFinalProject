using System.Collections;
using UnityEngine;

public class CutterConveyor : MonoBehaviour
{
    [SerializeField] Transform MainStart;
    [SerializeField] Transform MainMiddle1;
    [SerializeField] Transform MainMiddle2;
    [SerializeField] Transform MainEnd;
    [SerializeField] Transform SubStart;
    [SerializeField] Transform SubEnd;

    float MainConveyorSpeed = 1;
    float SubConveyorSpeed = 2;
    public bool IsMain;
    public bool IsSub;

    [SerializeField] GameObject MainBelt;
    [SerializeField] GameObject SubBelt;

    private void Start()
    {
        MainBelt.transform.localPosition = MainStart.localPosition;
        SubBelt.transform.localPosition = SubStart.localPosition;
        StartCoroutine(SubGo());
        StartCoroutine(MainGo());
    }

    void Update()
    {
        
    }

    IEnumerator Go(float Speed, Vector3 from, Vector3 to, GameObject Belt, bool IsOn)
    {
        Belt.transform.localPosition = from;
        float Length = Vector3.Distance(from, to);
        float journeyTime = Length / Speed;
        float CurrentTime = 0;

        while (CurrentTime < journeyTime)
        {
            if (!IsOn) // IsOn이 false일 경우 즉시 코루틴 종료
            {
                if (Belt = MainBelt)
                {
                    Belt.transform.localPosition = MainStart.localPosition;
                }
                else if (Belt = SubBelt)
                {
                    Belt.transform.localPosition = SubStart.localPosition;
                }
                yield break; // 현재 코루틴 종료
            }

            CurrentTime += Time.deltaTime;
            Belt.transform.localPosition = Vector3.Lerp(from, to, CurrentTime / journeyTime);
            yield return null;
        }

        Belt.transform.localPosition = to;

        // 초기화 무한루프
        if (Belt.transform.localPosition == MainEnd.localPosition)
        {
            Belt.transform.localPosition = MainStart.localPosition;
        }
        else if (Belt.transform.localPosition == SubEnd.localPosition)
        {
            Belt.transform.localPosition = SubStart.localPosition;
        }
    }

    IEnumerator MainGo()
    {
        while (IsMain)
        {
            yield return StartCoroutine(Go(MainConveyorSpeed, MainStart.localPosition, MainMiddle1.localPosition, MainBelt, IsMain));
            yield return StartCoroutine(Go(MainConveyorSpeed, MainMiddle2.localPosition, MainEnd.localPosition, MainBelt, IsMain));
        }
    }

    public void MaingGoPLC()
    {
        StartCoroutine(MainGo());
    }

    IEnumerator SubGo()
    {
        while (IsSub)
        {
            yield return StartCoroutine(Go(SubConveyorSpeed, SubStart.localPosition, SubEnd.localPosition, SubBelt, IsSub));
        }
    }

    public void SubGoPLC()
    {
        StartCoroutine(SubGo());
    }
}
