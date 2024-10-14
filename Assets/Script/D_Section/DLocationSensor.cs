using System.Collections;
using UnityEngine;

public class DLocationSensor : MonoBehaviour
{
    [SerializeField] Transform Hand;
    bool DRightSensorPLC;

    Coroutine coroutine;
    public static DLocationSensor Instance;



    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {
            DRightSensorPLC = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        {
            if (other.name.StartsWith("Box2"))
            {
                DRightSensorPLC = false;

            }
        }
    }
}
