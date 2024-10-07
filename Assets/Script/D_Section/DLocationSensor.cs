using UnityEngine;

public class DLocationSensor : MonoBehaviour
{
    bool RightSensorPLC;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {

            RightSensorPLC = true;
            print(RightSensorPLC);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        {
            if (other.name.StartsWith("Box2"))
            {
                RightSensorPLC = false;
                print(RightSensorPLC);
            }
        }
    }
}
