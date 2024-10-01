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
        Vector3 startPos1 = Box.localPosition;
        Vector3 endPos1 = new Vector3(Box.localPosition.x, 1.96f, Box.localPosition.z);
        Quaternion underWingStart1Of1 = Quaternion.Euler(0, 0, 0);
        Quaternion underWingEnd1Of1 = Quaternion.Euler(0, 0, -15);
        Quaternion underWingEnd2of1 = Quaternion.Euler(0, 0, 15);

        float duration = 1f;
        float currentTime = 0f;

        // STEP3-1: Box �̵� �� ���� ȸ��
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            // Box�� ��ġ ������Ʈ
            Box.transform.localPosition = Vector3.Lerp(startPos1, endPos1, currentTime / duration);

            // ���� ȸ��
            UnderWing1.localRotation = Quaternion.Lerp(underWingStart1Of1, underWingEnd1Of1, currentTime / duration);
            UnderWing2.localRotation = Quaternion.Lerp(underWingStart1Of1, underWingEnd2of1, currentTime / duration);

            yield return null;
        }

        // STEP3-2: Box�� ���ο� ��ġ�� �̵� �� ���� ȸ��
        Vector3 startPos2 = Box.localPosition;
        Vector3 endPos2 = new Vector3(Box.localPosition.x, Box.localPosition.y, 0.92f);

        Quaternion underWingStart1Of2 = Quaternion.Euler(0, 0, -15);
        Quaternion underWingStart2Of2 = Quaternion.Euler(0, 0, 15);
        Quaternion underWingEnd1Of2 = Quaternion.Euler(0, 0, -90);
        Quaternion underWingEnd2Of2 = Quaternion.Euler(0, 0, 90);

        float boxDuration = 2f; // Box�� 2�� ���� �̵�
        float wingDuration = 0.5f; // ������ 0.5�� ���� ȸ��

        currentTime = 0f; // �ʱ�ȭ

        // Box �̵��� ���� ȸ���� ���ÿ� ����
        while (currentTime < boxDuration)
        {
            currentTime += Time.deltaTime;

            // Box �̵� ������Ʈ
            Box.transform.localPosition = Vector3.Lerp(startPos2, endPos2, currentTime / boxDuration);

            // ���� ȸ�� (0.5�� ���� ȸ��) - ���� �ð��� 1.5�ʷ� ����
            if (currentTime >= 0.5f && currentTime <= 0.5f + wingDuration)
            {
                float ratio = (currentTime - 0.5f) / wingDuration; // 1.5�ʺ��� ����
                UnderWing1.localRotation = Quaternion.Lerp(underWingStart1Of2, underWingEnd1Of2, ratio);
                UnderWing2.localRotation = Quaternion.Lerp(underWingStart2Of2, underWingEnd2Of2, ratio);
            }

            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ȸ�� ����
        UnderWing1.localRotation = underWingEnd1Of2; // ���� ȸ�� ����
        UnderWing2.localRotation = underWingEnd2Of2; // ���� ȸ�� ����
    }


    IEnumerator AllStep()
    {
        yield return StartCoroutine(STEP1());
        yield return StartCoroutine(STEP2());
        yield return StartCoroutine(STEP3());
    }
}
