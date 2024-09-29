using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AGV_TEST : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public List<Vector3> playerPos = new List<Vector3>();

    public Transform[] pathPoints; // 이동할 경로의 포인트들
    public float speed = 5f; // 이동 속도
    public float rotateSpeed = 5f;     // 회전 속도
    private int currentPointIndex = 0;
    private int duration = 10;

    void Start()
    {
       // slerpMoveToNextPointUpdate();
        //slerpMoveToNextPoint();
    }

    public void OnclickNextProcess() {
        print(currentPointIndex+ "<<currentPointIndex");
      
        if (pathPoints.Length > 0 && currentPointIndex < pathPoints.Length)
        {
           
            StartCoroutine(moveToNextPoint(currentPointIndex));
            currentPointIndex++;
        }
        
    }


    void Update()
    {
      // slerpMoveToNextPoint();
     //   print("slerpMoveToNextPoint");
        if (currentPointIndex < pathPoints.Length)
        {
           // MoveToNextPoint();
            // SmoothMoveToNextPoint();
        }
    }



    void MoveToNextPoint()
    {
        Transform targetPoint = pathPoints[currentPointIndex];
        float step = speed * Time.deltaTime;

        // 목표 지점으로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

        // 방향을 목표 지점으로 바꿈
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 목표 지점에 가까워지면 다음 포인트로 이동
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex++;
        }
    }
    void SmoothMoveToNextPoint()
    {
        Transform targetPoint = pathPoints[currentPointIndex];
        float step = speed * Time.deltaTime;
        Vector3 zero = Vector3.zero; // (0,0,0) 은 .zero 로도 표현가능
        // 목표 지점으로 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPoint.position, ref zero, 0.1f);


        // 방향을 목표 지점으로 바꿈
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            //transform.rotation = Quaternion.LookRotation(direction);
            transform.Rotate(Vector3.up * targetPoint.position.y * step, Space.World);

        }

        // 목표 지점에 가까워지면 다음 포인트로 이동
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex++;
        }
    }
    void slerpMoveToNextPoint()
    {
       


      // Waypoint가 설정되어 있는지 확인
        if (pathPoints.Length == 0)
            return;

                // 현재 Waypoint까지의 방향 계산
                Vector3 direction = pathPoints[currentPointIndex].position - transform.position;
            direction.y = 0; // 수평 회전만 적용하고 싶을 때 Y축은 고정

                // 물체가 Waypoint에 도착했는지 확인
                if (direction.magnitude< 0.1f)
                {
                    // 마지막 Waypoint가 아니라면 다음 Waypoint로 이동
                    if (currentPointIndex < pathPoints.Length - 1)
                    {
                currentPointIndex++;
                    }
                    else
        {
            // Waypoint가 끝나면 멈춤 (원하는 경우 루프 또는 다른 동작 추가 가능)
            return;
}
        }

        // 회전 처리: 물체가 Waypoint를 향하도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // 이동 처리: Waypoint 방향으로 이동
        transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex].position, speed * Time.deltaTime);
    }

    private IEnumerator moveToNextPoint(int currentPointIndex)
    {

        
        float currentTime = 0;
        print("start slerpMoveToNextPointUpdate ");

        while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                currentTime = 0;
                break;

            }

            // 현재 Waypoint까지의 방향 계산
            Vector3 direction = pathPoints[currentPointIndex].position - transform.position;
            direction.y = 0; // 수평 회전만 적용하고 싶을 때 Y축은 고정

            // 물체가 Waypoint에 도착했는지 확인
            if (direction.magnitude < 0.1f)
            {
                break;
            }

            // 회전 처리: 물체가 Waypoint를 향하도록 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime/ duration);

            // 이동 처리: Waypoint 방향으로 이동
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex].position, currentTime/ duration);
            yield return new WaitForEndOfFrame();
        }

           
    }




}
