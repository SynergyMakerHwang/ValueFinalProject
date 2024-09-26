using System.Collections;
using UnityEngine;

public class D : MonoBehaviour
{
    [SerializeField] Transform Belt;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] float duration;
    [SerializeField] bool Power;

    private Coroutine moveCoroutine; // �ڷ�ƾ�� ������ ���� �߰�

    private void Start()
    {
    }

    private void Update()
    {
        // Power ���°� ����� ������ MovingGo ȣ��
        if (Power && moveCoroutine == null)
        {
            MovingGo();
        }
        else if (!Power && moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
           // Power�� ������ ���� ��ġ�� �̵�
            moveCoroutine = null; // �ڷ�ƾ ���� �ʱ�ȭ
        }
    }

    IEnumerator Moving(Vector3 from, Vector3 to)
    {
        float CurrentTime = 0;
        Belt.localPosition = from;

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            Belt.localPosition = Vector3.Lerp(from, to, CurrentTime / duration);
            yield return null;
        }

        Belt.localPosition = to; // ���� ��ġ�� �̵�
        moveCoroutine = null; // �ڷ�ƾ�� ������ ���� �ʱ�ȭ
    }

    public void MovingGo()
    {
        moveCoroutine = StartCoroutine(Moving(StartPos.localPosition, EndPos.localPosition));
    }

    public bool DConeyorOnPLC()
    {
        return Power = true;
    }
    public bool DConveyorOffPLC()
    {
        return Power = false;
    }

}
