using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AGV_TEST : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public List<Vector3> playerPos = new List<Vector3>();

    public Transform[] pathPoints; // �̵��� ����� ����Ʈ��
    public float speed = 5f; // �̵� �ӵ�
    public float rotateSpeed = 5f;     // ȸ�� �ӵ�
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
    void SmoothMoveToNextPoint()
    {
        Transform targetPoint = pathPoints[currentPointIndex];
        float step = speed * Time.deltaTime;
        Vector3 zero = Vector3.zero; // (0,0,0) �� .zero �ε� ǥ������
        // ��ǥ �������� �̵�
        transform.position = Vector3.SmoothDamp(transform.position, targetPoint.position, ref zero, 0.1f);


        // ������ ��ǥ �������� �ٲ�
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            //transform.rotation = Quaternion.LookRotation(direction);
            transform.Rotate(Vector3.up * targetPoint.position.y * step, Space.World);

        }

        // ��ǥ ������ ��������� ���� ����Ʈ�� �̵�
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex++;
        }
    }
    void slerpMoveToNextPoint()
    {
       


      // Waypoint�� �����Ǿ� �ִ��� Ȯ��
        if (pathPoints.Length == 0)
            return;

                // ���� Waypoint������ ���� ���
                Vector3 direction = pathPoints[currentPointIndex].position - transform.position;
            direction.y = 0; // ���� ȸ���� �����ϰ� ���� �� Y���� ����

                // ��ü�� Waypoint�� �����ߴ��� Ȯ��
                if (direction.magnitude< 0.1f)
                {
                    // ������ Waypoint�� �ƴ϶�� ���� Waypoint�� �̵�
                    if (currentPointIndex < pathPoints.Length - 1)
                    {
                currentPointIndex++;
                    }
                    else
        {
            // Waypoint�� ������ ���� (���ϴ� ��� ���� �Ǵ� �ٸ� ���� �߰� ����)
            return;
}
        }

        // ȸ�� ó��: ��ü�� Waypoint�� ���ϵ��� ȸ��
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // �̵� ó��: Waypoint �������� �̵�
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime/ duration);

            // �̵� ó��: Waypoint �������� �̵�
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex].position, currentTime/ duration);
            yield return new WaitForEndOfFrame();
        }

           
    }




}
