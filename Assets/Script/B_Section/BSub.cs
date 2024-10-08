using UnityEngine;

public class BSub : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("토트박스") && SubLocationSensor.Instance.RightLocationSensorPLC == false)
        {

            other.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("토트박스"))
        {

            other.transform.SetParent(null);
        }
    }
}


