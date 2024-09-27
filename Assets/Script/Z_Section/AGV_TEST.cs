using System.Collections.Generic;
using UnityEngine;

public class AGV_TEST : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public List<Vector3> playerPos = new List<Vector3>();


    void Start()
    {

       /*

        //Store players positions somewhere
        //playerPos.Add(pPos);
        //playerPos.Add(pPos); 
        //playerPos.Add(pPos);


        //Color green = Color.green;
        //LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();    

        //lineRenderer.SetColors(red, red);
        //lineRenderer.SetWidth(0.2F, 0.2F);

        //Change how mant points based on the mount of positions is the List
        //lineRenderer.SetVertexCount(playerPos.Count);

        for (int i = 0; i < playerPos.Count; i++)
        {
            //Change the postion of the lines
           // lineRenderer.SetPosition(i, playerPos[i]);
        }


        */




    }


    public Transform[] pathPoints; // �̵��� ����� ����Ʈ��
    public float speed = 2f; // �̵� �ӵ�
    private int currentPointIndex = 0;

    void Update()
    {
        if (currentPointIndex < pathPoints.Length)
        {
            MoveToNextPoint();
        }
    }



    void MoveToNextPoint()
    {
        Transform targetPoint = pathPoints[currentPointIndex];
        float step = speed * Time.deltaTime;

        // ��ǥ �������� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);

        // ������ ��ǥ �������� �ٲ�
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // ��ǥ ������ ��������� ���� ����Ʈ�� �̵�
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex++;
        }
    }




}
