using UnityEngine;

public class TottSensor : MonoBehaviour
{
    bool IsTottSensorPLC;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("��Ʈ�ڽ�"))
        {
            IsTottSensorPLC = true;
      
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("��Ʈ�ڽ�"))
        {
            IsTottSensorPLC = false;
       
        }
    }
}
