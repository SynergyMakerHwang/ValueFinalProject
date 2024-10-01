using System.Collections;
using System.Net.Http.Headers;
using Unity.Mathematics;
using UnityEngine;

public class DMachine : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform centerPoint; // �߽���
    [SerializeField] private Transform STEP1Point;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float radius = 1.8f; // ���� ������

    [SerializeField] Transform Box;
    [Header("�ڽ������� ")]
    [SerializeField] Transform UnderWing1;
    [SerializeField] Transform UnderWing2;
    [SerializeField] Transform UnderWing3;
    [SerializeField] Transform UnderWing4;


    private float angle = 30f * Mathf.Deg2Rad; // ���� ���� (30��, �������� ��ȯ)
    private float initialXRotation = 30f; // �ʱ� X ȸ�� ����


    private void Start()
    {
        //�ʱ⼳����
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
        // ��ǥ ���� (90��, �������� ��ȯ)
        float targetAngle = 90f * Mathf.Deg2Rad; // 90���� �������� ��ȯ
        Quaternion startAngle1 = quaternion.Euler(0.785398f, 0, 0);//���� 45�� ������ ����
        Quaternion EndAngle1 = Quaternion.Euler(0, 0, 0);
        Quaternion StartAngle2 = quaternion.Euler(-0.785398f, 0, 0);// ���� 45�� ���� ���������ִ� ��������
        Quaternion EndAngle2 = Quaternion.Euler(90, 0, 0);



        while (angle < targetAngle)
        {
            angle += speed * Time.deltaTime;

            // ����� ��ġ ���
            float y = centerPoint.localPosition.y - Mathf.Sin(angle) * radius;
            float z = centerPoint.localPosition.z - Mathf.Cos(angle) * radius;

            // ���� ��ġ ������Ʈ
            Box.transform.localPosition = new Vector3(Box.transform.localPosition.x, y, z);

            //���� ������ ���� Mathf.Clamp
            float Ratio = Mathf.Clamp01(angle / targetAngle);
            // ȸ�� ������Ʈ

            Box.transform.localRotation = Quaternion.Lerp(startAngle1, EndAngle1, Ratio);
            UnderWing3.transform.localRotation = Quaternion.Lerp(StartAngle2, EndAngle2, Ratio);

            yield return null; // ���� �����ӱ��� ���
        }

    }
    IEnumerator STEP3()
    {
        Vector3 StartPos1 = Box.localPosition;
        Vector3 EndPos1 = new Vector3(Box.localPosition.x, 2, Box.localPosition.z);
        Quaternion UnderWingStart = Quaternion.Euler(0, 0, 0);
        Quaternion UnderWingEnd1Of1 = Quaternion.Euler(0, 0, -15);
        Quaternion UnderWingEnd2of1 = Quaternion.Euler(0, 0, 15);


        float duration = 1;
        float CurrentTime = 0;
        //STEP3-1
        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            Box.transform.localPosition = Vector3.Lerp(StartPos1, EndPos1, CurrentTime / duration);
            UnderWing1.localRotation = Quaternion.Lerp(UnderWingStart, UnderWingEnd1Of1, CurrentTime / duration);
            UnderWing2.localRotation = Quaternion.Lerp(UnderWingStart, UnderWingEnd2of1, CurrentTime / duration);
            yield return null;
        }
        //�ʱ�ȭ
        CurrentTime = 0;
        Vector3 StartPos2 = Box.localPosition;
        Vector3 EndPos2 = new Vector3(Box.localPosition.x, Box.localPosition.y, 0.92f);
        Quaternion UnderWingStart1Of2 = Quaternion.Euler(0, 0, -15);
        Quaternion UnderWingStart2Of2 = Quaternion.Euler(0, 0, 15);
        Quaternion UnderWingEnd1Of2 = Quaternion.Euler(0, 0, -90);
        Quaternion UnderWingEnd2Of2 = Quaternion.Euler(0, 0, 90);
        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            Box.transform.localPosition = Vector3.Lerp(StartPos2, EndPos2, CurrentTime / duration);
            UnderWing1.localRotation = Quaternion.Lerp(UnderWingStart1Of2, UnderWingEnd1Of2, CurrentTime / duration);
            UnderWing2.localRotation = Quaternion.Lerp(UnderWingStart2Of2, UnderWingEnd2Of2, CurrentTime / duration);
            yield return null; ;

        }
    }

    IEnumerator AllStep()
    {
        yield return StartCoroutine(STEP1());
        yield return StartCoroutine(STEP2());
        yield return StartCoroutine(STEP3());
    }
}
