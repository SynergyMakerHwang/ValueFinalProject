using System.Collections;
using UnityEngine;

public class D2 : MonoBehaviour
{
    // �κ��� ��
    [Header("�κ��� ���� ��")]
    [SerializeField] Transform Pivot1;
    [SerializeField] Transform Pivot2;
    [SerializeField] Transform Pivot3;
    [SerializeField] Transform Pivot4;
    [SerializeField] Transform Pivot5;
    [SerializeField] Transform Pivot6;
    Coroutine Coroutine1;

    [Header("���� ��ü������")]
    [SerializeField] GameObject Box2;
    [SerializeField] GameObject pallet;

    [Header("���������̾Ʈ")]
    [SerializeField] Transform Belt;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] Transform Hand;
    [SerializeField] float speed = 5f;
    [SerializeField] bool Power;
    private Vector3 ResumePos; // ���� ��ġ ����
    bool DIsRunPLC;
    Coroutine Coroutine2;

    public static D2 instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        RobotGoPLC();
        SpawnBox();

        //��Ʈ ��ġ �ʱ�ȭ
        ResumePos = StartPos.localPosition;
        Belt.localPosition = ResumePos;
    }

  
    public void RobotGoPLC()
    {
        if (Coroutine1 == null)
            Coroutine1 = StartCoroutine(STEPS());
    }

    //���� �ʿ�
    public void RobotBackPLC()
    {
        if (Coroutine1 != null)
            Coroutine1 = StartCoroutine(STEPS());
    }

    public void SpawnBox()
    {
        Instantiate(Box2, pallet.transform);
    }
    IEnumerator STEPS()
    {
        // 1. �ڽ� ���� 
        yield return StartCoroutine(HowToMove(Pivot2, Quaternion.Euler(-50, 0, 0), Pivot3, Quaternion.Euler(-70, 0, 0), Pivot5, Quaternion.Euler(-60, 0, 0), null, Quaternion.identity, 50f));

        // 2. ������ ����
        yield return new WaitForSeconds(0.5f);

        // 3. �ڽ� ���÷��� ���Ľ�Ű��
        yield return StartCoroutine(HowToMove(Pivot2, Quaternion.Euler(0, 0, 0), Pivot3, Quaternion.Euler(-90, 0, 0), Pivot5, Quaternion.Euler(-90, 0, 0), null, Quaternion.identity, 50f));

        // 4. �ڽ� ���ı� ����Ͽ� �ڽ� ����
        yield return StartCoroutine(HowToMove(Pivot6, Quaternion.Euler(0, 0, 0), null, Quaternion.identity, null, Quaternion.identity, null, Quaternion.identity, 200f));

        // (������ ����)
        yield return new WaitForSeconds(1f);

        // 5. ���ĵ� �κ��� ������������� �ű��
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 130), Pivot3, Quaternion.Euler(-90, 0, 0), Pivot5, Quaternion.Euler(0, 40, 0), null, Quaternion.identity, 50f));

        // 6. �ڽ� �����ֱ�
        yield return StartCoroutine(HowToMove(Pivot5, Quaternion.Euler(40, 40, 0), null, Quaternion.identity, null, Quaternion.identity, null, Quaternion.identity, 50f));

        // 7. ���� �ӽſ� �о �ؿ� ���� ����
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 120), Pivot2, Quaternion.Euler(-30, 0, 0), Pivot3, Quaternion.Euler(-63, 0, 0), Pivot5, Quaternion.Euler(40, 30, 0), 50f));

        // 8-1 ������ Ÿ��
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 127), Pivot2, Quaternion.Euler(-10, 0, 0), Pivot3, Quaternion.Euler(-100, 0, 0), Pivot5, Quaternion.Euler(45, 45, 15), 30f));

        // 8-2
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 138), Pivot2, Quaternion.Euler(7, 0, 0), Pivot3, Quaternion.Euler(-117, 0, 0), Pivot5, Quaternion.Euler(22, 55, 15.5f), 30f));

        // 8-3
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 150), Pivot2, Quaternion.Euler(25, 0, 0), Pivot3, Quaternion.Euler(-131, 0, 0), Pivot5, Quaternion.Euler(10, 60, 15), 30f));

        // 9-1 ���ʳ��� ����
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 150), Pivot2, Quaternion.Euler(25, 0, 0), Pivot3, Quaternion.Euler(-128, 0, 0), Pivot5, Quaternion.Euler(8, 60, 12), 10f));

        // 9-2
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 170), Pivot2, Quaternion.Euler(35, 0, 0), Pivot3, Quaternion.Euler(-137, 0, 0), Pivot5, Quaternion.Euler(2, 82, 13f), 20f));

        // 9-3
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 195), Pivot2, Quaternion.Euler(35, 0, 0), Pivot3, Quaternion.Euler(-135, 0, 0), Pivot5, Quaternion.Euler(0, 105, 11.5f), 20f));

        // 9-4
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 220), Pivot2, Quaternion.Euler(13, 0, 0), Pivot3, Quaternion.Euler(-120, 0, 0), Pivot5, Quaternion.Euler(-10, 130, 14f), 20f));

        // 10. ������ ������
        yield return StartCoroutine(HowToMove(Pivot3, Quaternion.Euler(-90, 0, 0), null, Quaternion.identity, null, Quaternion.identity, null, Quaternion.identity, 20f));
        yield return new WaitForSeconds(0.3f);

        // 11 ������ �� ���ı� ����
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 225), Pivot2, Quaternion.Euler(10, 0, 0), Pivot3, Quaternion.Euler(-112, 0, 0), Pivot5, Quaternion.Euler(0, -40, 0), 50f));

        // 11-1
        yield return StartCoroutine(HowToMove(Pivot6, Quaternion.Euler(0, 0, -90), null, Quaternion.identity, null, Quaternion.identity, null, Quaternion.identity, 100f));

        // ���ƿ���
        //yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 150), Pivot2, Quaternion.Euler(0, 0, 0), Pivot3, Quaternion.Euler(-90, 0, 0), Pivot5, Quaternion.Euler(0, 0, 0), 50f));

    }

    IEnumerator HowToMove(Transform WhichPivot1, Quaternion TarRot1, Transform WhichPivot2, Quaternion TarRot2, Transform WhichPivot3, Quaternion TarRot3, Transform WhichPivot4, Quaternion TarRot4, float speed)
    {
        // ���� ȸ�� ���¸� ���� ������ ����
        Quaternion StartPivot1 = WhichPivot1 != null ? WhichPivot1.localRotation : Quaternion.identity;
        Quaternion StartPivot2 = WhichPivot2 != null ? WhichPivot2.localRotation : Quaternion.identity;
        Quaternion StartPivot3 = WhichPivot3 != null ? WhichPivot3.localRotation : Quaternion.identity;
        Quaternion StartPivot4 = WhichPivot4 != null ? WhichPivot4.localRotation : Quaternion.identity;

        // �� ȸ���� ���� ���
        float angleDiff1 = Quaternion.Angle(StartPivot1, TarRot1);
        float angleDiff2 = WhichPivot2 != null ? Quaternion.Angle(StartPivot2, TarRot2) : 0f;
        float angleDiff3 = WhichPivot3 != null ? Quaternion.Angle(StartPivot3, TarRot3) : 0f;
        float angleDiff4 = WhichPivot4 != null ? Quaternion.Angle(StartPivot4, TarRot4) : 0f;

        // �ִ� ȸ�� �ð� ���
        float maxAngleDiff = Mathf.Max(angleDiff1, angleDiff2, angleDiff3, angleDiff4);
        float duration = maxAngleDiff / speed;

        float CurrentTime = 0;

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;

            // ȸ�� ���� ���
            float t = CurrentTime / duration;

            // �� �ǹ� ȸ�� ������Ʈ
            if (WhichPivot1 != null)
            {
                WhichPivot1.localRotation = Quaternion.Lerp(StartPivot1, TarRot1, t);
            }
            if (WhichPivot2 != null)
            {
                WhichPivot2.localRotation = Quaternion.Lerp(StartPivot2, TarRot2, t);
            }
            if (WhichPivot3 != null)
            {
                WhichPivot3.localRotation = Quaternion.Lerp(StartPivot3, TarRot3, t);
            }
            if (WhichPivot4 != null)
            {
                WhichPivot4.localRotation = Quaternion.Lerp(StartPivot4, TarRot4, t);
            }

            yield return null;
        }

        // ���� ȸ���� ����
        if (WhichPivot1 != null)
        {
            WhichPivot1.localRotation = TarRot1;
        }
        if (WhichPivot2 != null)
        {
            WhichPivot2.localRotation = TarRot2;
        }
        if (WhichPivot3 != null)
        {
            WhichPivot3.localRotation = TarRot3;
        }
        if (WhichPivot4 != null)
        {
            WhichPivot4.localRotation = TarRot4;
        }
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

                yield return null; // ���� �����ӱ��� ���
            }
            float Distance = Vector3.Distance(Belt.localPosition, EndPos.localPosition);
            if (Distance < 5)
                ResumePos = StartPos.localPosition;


            Belt.localPosition = StartPos.localPosition;
        }

    }

    public void WrapConveyorOnPLC()
    {

        if (Coroutine2 == null)
        {
            Coroutine2 = StartCoroutine(Moving());

        }

    }
    public void WrapConeyorOffPLC()
    {
        if (Coroutine2 != null)
        {
            StopCoroutine(Coroutine2);
            ResumePos = Belt.localPosition;
            Coroutine2 = null;
        }
    }

}

