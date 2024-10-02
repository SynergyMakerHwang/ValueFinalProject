using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGVManager : MonoBehaviour
{

    public List<Dictionary<int, string>> entireProcessList; //전체 설정 공정
    int entireProcessCurrentNum = 0;


    public Transform[] pathPoints; // 이동할 경로의 포인트들
    private int currentPointIndex = 0;
    private int duration = 10;

    private void Start()
    {
        
    }

    public void OnclickNextPoint()
    {
      
        if (pathPoints.Length > 0 && currentPointIndex < pathPoints.Length)
        {
            StartCoroutine(moveToNextPoint(currentPointIndex));
            currentPointIndex++;
        }

    }

    private IEnumerator moveToNextPoint(int currentPointIndex)
    {


        float currentTime = 0;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime / duration);

            // 이동 처리: Waypoint 방향으로 이동
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex].position, currentTime / duration);
            yield return new WaitForEndOfFrame();
        }


    }


}
