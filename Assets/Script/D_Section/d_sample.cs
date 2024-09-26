using System.Collections;
using UnityEngine;

public class D : MonoBehaviour
{
    [SerializeField] Transform Belt;
    [SerializeField] Transform StartPos;
    [SerializeField] Transform EndPos;
    [SerializeField] float duration;
    [SerializeField] bool Power;

    private Coroutine moveCoroutine; // 코루틴을 저장할 변수 추가

    private void Start()
    {
    }

    private void Update()
    {
        // Power 상태가 변경될 때마다 MovingGo 호출
        if (Power && moveCoroutine == null)
        {
            MovingGo();
        }
        else if (!Power && moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
           // Power가 꺼지면 시작 위치로 이동
            moveCoroutine = null; // 코루틴 상태 초기화
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

        Belt.localPosition = to; // 최종 위치로 이동
        moveCoroutine = null; // 코루틴이 끝나면 상태 초기화
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
