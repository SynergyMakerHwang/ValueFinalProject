using System.Collections;
using System.Net.Http.Headers;
using Unity.Mathematics;
using UnityEngine;

public class DMachine : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform centerPoint; // 중심점
    [SerializeField] private Transform STEP1Point;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float radius = 1.8f; // 원의 반지름

    [SerializeField] Transform Box;
    [Header("박스날개들 ")]
    [SerializeField] Transform UnderWing1;
    [SerializeField] Transform UnderWing2;
    [SerializeField] Transform UnderWing3;
    [SerializeField] Transform UnderWing4;

    [Header("커터")]
    [SerializeField] Transform tapecutterPivot;
    [SerializeField] GameObject Tape;



    private float angle = 30f * Mathf.Deg2Rad; // 시작 각도 (30도, 라디안으로 변환)
    private float initialXRotation = 30f; // 초기 X 회전 각도


    private void Start()
    {
        //초기설정값
        Box.transform.localPosition = startPoint.localPosition;
        Box.transform.localRotation = Quaternion.Euler(30, 0, 0);
        StartCoroutine(AllStep());

    }


    //및날개 접는단계
    IEnumerator STEP1()
    {
        float Lengh = Vector3.Distance(Box.transform.localPosition, STEP1Point.localPosition);
        float journey = Lengh / speed;
        float CurrentTime = 0;
        Quaternion TargetAngle = Quaternion.Euler(-90, 0, 0);
        quaternion StartAngle = Quaternion.Euler(0, 0, 0);

        while (CurrentTime < journey)
        {
            CurrentTime += Time.deltaTime;

            print(CurrentTime / journey);
            Box.transform.localPosition = Vector3.Lerp(startPoint.localPosition, STEP1Point.localPosition, CurrentTime / journey);
            UnderWing4.localRotation = Quaternion.Lerp(StartAngle, TargetAngle, CurrentTime / journey);

            yield return new WaitForEndOfFrame();
        }
        Box.transform.localPosition = STEP1Point.localPosition;

    }
    //박스 원운동
    IEnumerator STEP2()
    {
        // 목표 각도 (90도, 라디안으로 변환)
        float targetAngle = 90f * Mathf.Deg2Rad; // 90도를 라디안으로 변환
        Quaternion startAngle1 = quaternion.Euler(0.785398f, 0, 0);//라디안 45도 이유가 뭐지
        Quaternion EndAngle1 = Quaternion.Euler(0, 0, 0);
        Quaternion StartAngle2 = quaternion.Euler(-0.785398f, 0, 0);// 라디안 45도 뭔가 유지시켜주는 느낌같다
        Quaternion EndAngle2 = Quaternion.Euler(90, 0, 0);



        while (angle < targetAngle)
        {
            angle += speed * Time.deltaTime;

            // 원운동의 위치 계산
            float y = centerPoint.localPosition.y - Mathf.Sin(angle) * radius;
            float z = centerPoint.localPosition.z - Mathf.Cos(angle) * radius;

            // 질점 위치 업데이트
            Box.transform.localPosition = new Vector3(Box.transform.localPosition.x, y, z);

            //비율 조정을 위한 Mathf.Clamp
            float Ratio = Mathf.Clamp01(angle / targetAngle);
            // 회전 업데이트

            Box.transform.localRotation = Quaternion.Lerp(startAngle1, EndAngle1, Ratio);
            UnderWing3.transform.localRotation = Quaternion.Lerp(StartAngle2, EndAngle2, Ratio);

            yield return null; // 다음 프레임까지 대기
        }

    }
    //원운동 후 직선운동 및밑날개 접는단계
    IEnumerator STEP3()
    {
        Vector3 startPos1 = Box.localPosition;
        Vector3 endPos1 = new Vector3(Box.localPosition.x, 2f, Box.localPosition.z);
        Quaternion underWingStart1Of1 = Quaternion.Euler(0, 0, 0);
        Quaternion underWingEnd1Of1 = Quaternion.Euler(0, 0, -15);
        Quaternion underWingEnd2of1 = Quaternion.Euler(0, 0, 15);

        float duration = 1f;
        float currentTime = 0f;

        // STEP3-1: Box 이동 및 날개 회전
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            // Box의 위치 업데이트
            Box.transform.localPosition = Vector3.Lerp(startPos1, endPos1, currentTime / duration);

            // 날개 회전
            UnderWing1.localRotation = Quaternion.Lerp(underWingStart1Of1, underWingEnd1Of1, currentTime / duration);
            UnderWing2.localRotation = Quaternion.Lerp(underWingStart1Of1, underWingEnd2of1, currentTime / duration);

            yield return null;
        }



        Vector3 startPos2 = Box.localPosition;
        Vector3 endPos2 = new Vector3(Box.localPosition.x, Box.localPosition.y, 1f);

        Quaternion underWingStart1Of2 = Quaternion.Euler(0, 0, -15);
        Quaternion underWingStart2Of2 = Quaternion.Euler(0, 0, 15);
        Quaternion underWingEnd1Of2 = Quaternion.Euler(0, 0, -90);
        Quaternion underWingEnd2Of2 = Quaternion.Euler(0, 0, 90);

        float boxDuration = 2f;
        float wingDuration = 0.5f;

        currentTime = 0f; // 초기화


        while (currentTime < boxDuration)
        {
            currentTime += Time.deltaTime;


            Box.transform.localPosition = Vector3.Lerp(startPos2, endPos2, currentTime / boxDuration);

            if (currentTime >= 0.5f && currentTime <= 0.5f + wingDuration)
            {
                float ratio = (currentTime - 0.5f) / wingDuration; // 1.5초부터 시작
                UnderWing1.localRotation = Quaternion.Lerp(underWingStart1Of2, underWingEnd1Of2, ratio);
                UnderWing2.localRotation = Quaternion.Lerp(underWingStart2Of2, underWingEnd2Of2, ratio);
            }

            yield return null;
        }
        currentTime = 0;
        Vector3 startPos3 = Box.localPosition;
        Vector3 endPos3 = new Vector3(Box.localPosition.x, 1.87f, Box.localPosition.z);
        while (currentTime < boxDuration)
        {
            currentTime += Time.deltaTime;
            Box.transform.localPosition = Vector3.Lerp(startPos3, endPos3, currentTime / boxDuration);
            yield return null;
        }

    }

    //박스 테이프 단계
    IEnumerator STEP4()
    {
        //시간으로 제어하는게 편하다
        float currentTime = 0f;
        float duration = 2.89f;
        Vector3 startPos = Box.localPosition;
        Vector3 endPos = new Vector3(Box.localPosition.x, Box.localPosition.y, 2.6f);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            Box.localPosition = Vector3.Lerp(startPos, endPos, currentTime / duration);
            yield return null;
        }

        Vector3 SpotOfTape = new Vector3(Box.position.x+0.18f, Box.position.y, Box.position.z );
        Instantiate(Tape, SpotOfTape, Quaternion.Euler(-90, 90, 0));
        Tape.transform.SetParent(Box.transform);

    }


    IEnumerator AllStep()
    {
        yield return StartCoroutine(STEP1());
        yield return StartCoroutine(STEP2());
        yield return StartCoroutine(STEP3());
        yield return StartCoroutine(STEP4());
    }
}
