using System.Collections;
using UnityEngine;

public class Partical : MonoBehaviour
{
    [SerializeField] private Transform centerPoint; // 중심점
    [SerializeField] private float speed = 1f; // 회전 속도
    [SerializeField] private float radius = 1.8f; // 원의 반지름

    [Header("박스날개들 ")]
    [SerializeField] Transform UnderWing4;
    [SerializeField] Transform UnderWing3;

    private float angle = 30f * Mathf.Deg2Rad; // 시작 각도 (30도, 라디안으로 변환)
    private float initialXRotation = 30f; // 초기 X 회전 각도
    private float initialWing3 = 0f;
    private float initialWing4 = 0f;

    private void Start()
    {
        StartCoroutine(CircleMovement());
    }

    private void Update()
    {
    }

    IEnumerator CircleMovement()
    {
        // 목표 각도 (90도, 라디안으로 변환)
        float targetAngle = 90f * Mathf.Deg2Rad; // 90도를 라디안으로 변환
        float targetXRotation = 0f; // 목표 X 회전 각도 (0도)
        float targetXforWing3 = 90f; // UnderWing3의 목표 회전 각도
        float targetXforWing4 = -90f; // UnderWing4의 목표 회전 각도

        while (angle < targetAngle)
        {
            angle += speed * Time.deltaTime;

            // 원운동의 위치 계산
            float y = centerPoint.localPosition.y - Mathf.Sin(angle) * radius;
            float z = centerPoint.localPosition.z - Mathf.Cos(angle) * radius;

            // 질점 위치 업데이트
            transform.localPosition = new Vector3(transform.localPosition.x, y, z);

            // X 회전 각도 업데이트 (30도에서 0도로 감소)
            float xRotation1 = Mathf.Lerp(initialXRotation, targetXRotation, angle / targetAngle);
            transform.localRotation = Quaternion.Euler(xRotation1, 0, 0); // X축 회전 적용

            // UnderWing4 회전 업데이트
            float xRotation2 = Mathf.Lerp(initialWing4, targetXforWing4, angle / targetAngle);
            UnderWing4.localRotation = Quaternion.Euler(xRotation2, 0, 0);

            // 날개 회전 조건식 
            if (xRotation2 <= -30)
            {
                float xRotation3 = Mathf.Lerp(initialWing3, targetXforWing3, angle / targetAngle);
                UnderWing3.localRotation = Quaternion.Euler(xRotation3, 0, 0);
            }

        

            yield return null; // 다음 프레임까지 대기
        }
    }
}
