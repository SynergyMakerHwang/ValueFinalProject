using Unity.VisualScripting;
using UnityEngine;

public class SubLocationSensor : MonoBehaviour
{
    public bool RightLocationSensorPLC;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Contains("토트박스"))
        {
            RightLocationSensorPLC = !RightLocationSensorPLC;
        }

    }
    
}
