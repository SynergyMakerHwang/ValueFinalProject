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

    [Header("Ŀ��")]
    [SerializeField] Transform tapecutterPivot;
    [SerializeField] GameObject Tape;



    private float angle = 30f * Mathf.Deg2Rad; // ���� ���� (30��, �������� ��ȯ)
    private float initialXRotation = 30f; // �ʱ� X ȸ�� ����


    private void Start()
    {
        //�ʱ⼳����
        Box.transform.localPosition = startPoint.localPosition;
        Box.transform.localRotation = Quaternion.Euler(30, 0, 0);
        StartCoroutine(AllStep());

    }


    //�׳��� ���´ܰ�
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
    //�ڽ� ���
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
    //��� �� ����� �׹س��� ���´ܰ�
    IEnumerator STEP3()
    {
        Vector3 startPos1 = Box.localPosition;
        Vector3 endPos1 = new Vector3(Box.localPosition.x, 2f, Box.localPosition.z);
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



        Vector3 startPos2 = Box.localPosition;
        Vector3 endPos2 = new Vector3(Box.localPosition.x, Box.localPosition.y, 1f);

        Quaternion underWingStart1Of2 = Quaternion.Euler(0, 0, -15);
        Quaternion underWingStart2Of2 = Quaternion.Euler(0, 0, 15);
        Quaternion underWingEnd1Of2 = Quaternion.Euler(0, 0, -90);
        Quaternion underWingEnd2Of2 = Quaternion.Euler(0, 0, 90);

        float boxDuration = 2f;
        float wingDuration = 0.5f;

        currentTime = 0f; // �ʱ�ȭ


        while (currentTime < boxDuration)
        {
            currentTime += Time.deltaTime;


            Box.transform.localPosition = Vector3.Lerp(startPos2, endPos2, currentTime / boxDuration);

            if (currentTime >= 0.5f && currentTime <= 0.5f + wingDuration)
            {
                float ratio = (currentTime - 0.5f) / wingDuration; // 1.5�ʺ��� ����
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

    //�ڽ� ������ �ܰ�
    IEnumerator STEP4()
    {
        //�ð����� �����ϴ°� ���ϴ�
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
