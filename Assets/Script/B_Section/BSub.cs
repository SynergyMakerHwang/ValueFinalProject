using UnityEngine;

public class BSub : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("��Ʈ�ڽ�") && SubLocationSensor.Instance.RightLocationSensorPLC == false)
        {

            other.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("��Ʈ�ڽ�"))
        {

            other.transform.SetParent(null);
        }
    }
}


