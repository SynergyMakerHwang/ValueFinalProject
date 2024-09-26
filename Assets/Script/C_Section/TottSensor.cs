using UnityEngine;

public class TottSensor : MonoBehaviour
{
    bool IsTottSensorPLC;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("토트박스"))
        {
            IsTottSensorPLC = true;
      
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("토트박스"))
        {
            IsTottSensorPLC = false;
       
        }
    }
}
