using Unity.VisualScripting;
using UnityEngine;

public class SubLocationSensor : MonoBehaviour
{
    public bool RightLocationSensorPLC;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Contains("��Ʈ�ڽ�"))
        {
            RightLocationSensorPLC = !RightLocationSensorPLC;
        }

    }
    
}
