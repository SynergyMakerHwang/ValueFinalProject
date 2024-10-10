using UnityEngine;

public class RightBoxSensor : MonoBehaviour
{
    public bool RightBoxSensorPLC;
    public static RightBoxSensor instacne;
    public void Awake()
    {
        if (instacne == null)
            instacne = this;
    }
    private void OnTrigerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {
            RightBoxSensorPLC = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {
            RightBoxSensorPLC = false;
        }
    }
}

