using UnityEngine;

public class Absorber : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2") && transform.childCount == 0 && RightBoxSensor.instacne.RightBoxSensorPLC == true)
        {
            other.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {
            other.transform.SetParent(null);
        }
    }
}
