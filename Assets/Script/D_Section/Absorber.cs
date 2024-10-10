using UnityEngine;

public class Absorber : MonoBehaviour
{
    private int Count;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2") && transform.childCount == 0 && DLocationSensor.Instance.RightSensorPLC == false)
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
