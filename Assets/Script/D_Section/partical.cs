using System.Collections;
using UnityEngine;

public class Partical : MonoBehaviour
{
    [SerializeField] private Transform centerPoint; // �߽���
    [SerializeField] private float speed = 1f; // ȸ�� �ӵ�
    [SerializeField] private float radius = 1.8f; // ���� ������

    [Header("�ڽ������� ")]
    [SerializeField] Transform UnderWing4;
    [SerializeField] Transform UnderWing3;

    private float angle = 30f * Mathf.Deg2Rad; // ���� ���� (30��, �������� ��ȯ)
    private float initialXRotation = 30f; // �ʱ� X ȸ�� ����
    private float initialWing3 = 0f;
    private float initialWing4 = 0f;

    private void Start()
    {
        StartCoroutine(CircleMovement());
    }

    private void Update()
    {
    }

    IEnumerator CircleMovement()
    {
        // ��ǥ ���� (90��, �������� ��ȯ)
        float targetAngle = 90f * Mathf.Deg2Rad; // 90���� �������� ��ȯ
        float targetXRotation = 0f; // ��ǥ X ȸ�� ���� (0��)
        float targetXforWing3 = 90f; // UnderWing3�� ��ǥ ȸ�� ����
        float targetXforWing4 = -90f; // UnderWing4�� ��ǥ ȸ�� ����

        while (angle < targetAngle)
        {
            angle += speed * Time.deltaTime;

            // ����� ��ġ ���
            float y = centerPoint.localPosition.y - Mathf.Sin(angle) * radius;
            float z = centerPoint.localPosition.z - Mathf.Cos(angle) * radius;

            // ���� ��ġ ������Ʈ
            transform.localPosition = new Vector3(transform.localPosition.x, y, z);

            // X ȸ�� ���� ������Ʈ (30������ 0���� ����)
            float xRotation1 = Mathf.Lerp(initialXRotation, targetXRotation, angle / targetAngle);
            transform.localRotation = Quaternion.Euler(xRotation1, 0, 0); // X�� ȸ�� ����

            // UnderWing4 ȸ�� ������Ʈ
            float xRotation2 = Mathf.Lerp(initialWing4, targetXforWing4, angle / targetAngle);
            UnderWing4.localRotation = Quaternion.Euler(xRotation2, 0, 0);

            // ���� ȸ�� ���ǽ� 
            if (xRotation2 <= -30)
            {
                float xRotation3 = Mathf.Lerp(initialWing3, targetXforWing3, angle / targetAngle);
                UnderWing3.localRotation = Quaternion.Euler(xRotation3, 0, 0);
            }

        

            yield return null; // ���� �����ӱ��� ���
        }
    }
}
