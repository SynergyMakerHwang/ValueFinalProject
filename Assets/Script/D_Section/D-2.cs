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


    private float angle = 30f * Mathf.Deg2Rad; // 시작 각도 (30도, 라디안으로 변환)
    private float initialXRotation = 30f; // 초기 X 회전 각도


    private void Start()
    {
        //초기설정값
        Box.transform.localPosition = startPoint.localPosition;
        Box.transform.localRotation = Quaternion.Euler(30, 0, 0);
        StartCoroutine(AllStep());

    }



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
    IEnumerator STEP3()
    {
        Vector3 startPos1 = Box.localPosition;
        Vector3 endPos1 = new Vector3(Box.localPosition.x, 1.96f, Box.localPosition.z);
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

        // STEP3-2: Box의 새로운 위치로 이동 및 날개 회전
        Vector3 startPos2 = Box.localPosition;
        Vector3 endPos2 = new Vector3(Box.localPosition.x, Box.localPosition.y, 0.92f);

        Quaternion underWingStart1Of2 = Quaternion.Euler(0, 0, -15);
        Quaternion underWingStart2Of2 = Quaternion.Euler(0, 0, 15);
        Quaternion underWingEnd1Of2 = Quaternion.Euler(0, 0, -90);
        Quaternion underWingEnd2Of2 = Quaternion.Euler(0, 0, 90);

        float boxDuration = 2f; // Box는 2초 동안 이동
        float wingDuration = 0.5f; // 날개는 0.5초 동안 회전

        currentTime = 0f; // 초기화

        // Box 이동과 날개 회전을 동시에 수행
        while (currentTime < boxDuration)
        {
            currentTime += Time.deltaTime;

            // Box 이동 업데이트
            Box.transform.localPosition = Vector3.Lerp(startPos2, endPos2, currentTime / boxDuration);

            // 날개 회전 (0.5초 동안 회전) - 시작 시간을 1.5초로 조정
            if (currentTime >= 0.5f && currentTime <= 0.5f + wingDuration)
            {
                float ratio = (currentTime - 0.5f) / wingDuration; // 1.5초부터 시작
                UnderWing1.localRotation = Quaternion.Lerp(underWingStart1Of2, underWingEnd1Of2, ratio);
                UnderWing2.localRotation = Quaternion.Lerp(underWingStart2Of2, underWingEnd2Of2, ratio);
            }

            yield return null; // 다음 프레임까지 대기
        }

        // 최종 회전 설정
        UnderWing1.localRotation = underWingEnd1Of2; // 최종 회전 설정
        UnderWing2.localRotation = underWingEnd2Of2; // 최종 회전 설정
    }


    IEnumerator AllStep()
    {
        yield return StartCoroutine(STEP1());
        yield return StartCoroutine(STEP2());
        yield return StartCoroutine(STEP3());
    }
}
