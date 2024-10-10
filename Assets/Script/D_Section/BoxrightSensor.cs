using Unity.VisualScripting;
using UnityEngine;

public class BoxrightSensor : MonoBehaviour
{
    public bool BoxRightSensorPLC;
    public static BoxrightSensor Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {
            BoxRightSensorPLC = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {
            BoxRightSensorPLC = false;
        }
    }

}

