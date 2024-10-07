using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class D1 : MonoBehaviour
{
    [SerializeField] Transform Belt;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] float speed = 1;
    [SerializeField] bool Power;
    private Vector3 ResumePos; // 현재 위치 저장
    bool DIsRunPLC;
    Coroutine Coroutine;
    private void Start()
    {
        ResumePos = StartPos.localPosition; // 시작 위치 초기화
        Belt.localPosition = ResumePos; // 벨트 초기 위치 설정

    }


    IEnumerator Moving()
    {


        while (true)
        {
            Vector3 from = ResumePos;
            Vector3 to = EndPos.localPosition;
            float Length = Vector3.Distance(from, to);
            float Journey = Length / speed;
            float currentTime = 0;

            while (currentTime < Journey)
            {
                currentTime += Time.deltaTime;
                Belt.localPosition = Vector3.Lerp(from, to, currentTime / Journey);

                yield return null; // 다음 프레임까지 대기
            }
            float Distance = Vector3.Distance(Belt.localPosition, EndPos.localPosition);
            if (Distance < 5)
                ResumePos = StartPos.localPosition;


            Belt.localPosition = StartPos.localPosition;
        }

    }

    public void DConveyorOnPLC()
    {

        if (Coroutine == null)
        {
            Coroutine = StartCoroutine(Moving());
            // if (Belt.localPosition == EndPos.localPosition)
            // {
            //    ResumePos = StartPos.localPosition;
            // }


        }

    }
    public void DConeyorOffPLC()
    {
        if (Coroutine != null)
        {
            StopCoroutine(Coroutine);
            ResumePos = Belt.localPosition;
            Coroutine = null;
        }
    }


}
