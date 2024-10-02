using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGVManager : MonoBehaviour
{

    public List<Dictionary<int, string>> entireProcessList; //��ü ���� ����
    int entireProcessCurrentNum = 0;


    public Transform[] pathPoints; // �̵��� ����� ����Ʈ��
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

            // ���� Waypoint������ ���� ���
            Vector3 direction = pathPoints[currentPointIndex].position - transform.position;
            direction.y = 0; // ���� ȸ���� �����ϰ� ���� �� Y���� ����

            // ��ü�� Waypoint�� �����ߴ��� Ȯ��
            if (direction.magnitude < 0.1f)
            {
                break;
            }

            // ȸ�� ó��: ��ü�� Waypoint�� ���ϵ��� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime / duration);

            // �̵� ó��: Waypoint �������� �̵�
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex].position, currentTime / duration);
            yield return new WaitForEndOfFrame();
        }


    }


}
