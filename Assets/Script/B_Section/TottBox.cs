using UnityEngine;

public class TottBox : MonoBehaviour
{
    bool WeightSensorPLC = false;
    bool rightlocationSensorPLC = false;
    void Start()
    {
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apples")) // �±� ��
        {
            other.transform.SetParent(transform);


            if (transform.childCount >= 5)
            {
                //���Լ��� Ʈ�簪 ��ȯ
                WeightSensorPLC = true;
            }
        }
        else if (other.name.Contains("Sensor"))
        {
            //����ġ���� Ʈ�簪 ��ȯ
            rightlocationSensorPLC = true;
        }


    }
}