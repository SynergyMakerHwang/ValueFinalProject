using System.Collections;
using UnityEngine;

public class RobotArm : MonoBehaviour
{
    // 로보팔 축
    [SerializeField] Transform Pivot1;
    [SerializeField] Transform Pivot2;
    [SerializeField] Transform Pivot3;
    [SerializeField] Transform Pivot4;
    [SerializeField] Transform Pivot5;
    [SerializeField] Transform Pivot6;

    [Header("지점 따라가기")]
    [SerializeField] Transform StartPoint;
    private void Start()
    {
        aline(Pivot1);
        aline(Pivot2);
        aline(Pivot3);
        aline(Pivot4);
        aline(Pivot5);
        STEPSTART();
    }

    public void STEPSTART()
    {
        StartCoroutine(STEPS());
    }

    IEnumerator STEPS()
    {
        // 1. 박스 집기 
        yield return StartCoroutine(HowToMove(Pivot2, Quaternion.Euler(-50, 0, 0), Pivot3, Quaternion.Euler(-70, 0, 0), Pivot5, Quaternion.Euler(-60, 0, 0), 3));

        // 2.흡착기 가동
        yield return new WaitForSeconds(0.5f);

        // 3. 박스 들어올려서 정렬시키기
        yield return StartCoroutine(HowToMove(Pivot2, Quaternion.Euler(0, 0, 0), Pivot3, Quaternion.Euler(-90, 0, 0), Pivot5, Quaternion.Euler(-90, 0, 0), 3));

   
        // 4. 박스 정렬기 사용하여 박스 정렬
        yield return StartCoroutine(HowToMove(Pivot6, Quaternion.Euler(0, 0, 0), null, Quaternion.identity, null, Quaternion.identity, 0.2f));
        yield return new WaitForSeconds(1f);

        // 5. 정렬된 로봇팔 폴딩기계쪽으로  옮기기 스타트포인트쪽의 각도 알기
        yield return StartCoroutine(HowToMove(Pivot1, Quaternion.Euler(0, 0, 150), null, Quaternion.identity, null, Quaternion.identity, 3f));
    }

    IEnumerator HowToMove(Transform WhichPivot1, Quaternion TarRot1, Transform WhichPivot2, Quaternion TarRot2, Transform WhichPivot3, Quaternion TarRot3, float duration)
    {
        float CurrentTime = 0;

        // 현재 회전 상태를 시작 각도로 설정
        Quaternion StartPivot1 = WhichPivot1 != null ? WhichPivot1.localRotation : Quaternion.identity;
        Quaternion StartPivot2 = WhichPivot2 != null ? WhichPivot2.localRotation : Quaternion.identity; 
        Quaternion StartPivot3 = WhichPivot3 != null ? WhichPivot3.localRotation : Quaternion.identity; 

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            WhichPivot1.localRotation = Quaternion.Lerp(StartPivot1, TarRot1, CurrentTime / duration);

            // 피벗이 null이 아닐 경우에만 회전 업데이트
            if (WhichPivot1 != null)
            {
                WhichPivot1.localRotation = Quaternion.Lerp(StartPivot1, TarRot1, CurrentTime / duration);
            }
            if (WhichPivot2 != null)
            {
                WhichPivot2.localRotation = Quaternion.Lerp(StartPivot2, TarRot2, CurrentTime / duration);
            }
            if (WhichPivot3 != null)
            {
                WhichPivot3.localRotation = Quaternion.Lerp(StartPivot3, TarRot3, CurrentTime / duration);
            }

            yield return null;
        }

        // 최종 회전값 설정
        WhichPivot1.localRotation = TarRot1;
        if (WhichPivot2 != null)
        {
            WhichPivot2.localRotation = TarRot2;
        }
        if (WhichPivot3 != null)
        {
            WhichPivot3.localRotation = TarRot3;
        }
    }

    public void aline(Transform Pivot)
    {
        Pivot.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
