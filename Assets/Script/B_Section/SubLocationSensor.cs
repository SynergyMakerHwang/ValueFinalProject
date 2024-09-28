using Unity.VisualScripting;
using UnityEngine;

public class SubLocationSensor : MonoBehaviour
{
    public bool RightLocationSensorPLC;
    //평소에는 false

    //접촉되면 true
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("토트박스센서"))
        {
            RightLocationSensorPLC = true;
        }


    }
    //나가게 되면 false
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("토트박스센서"))
        {
            RightLocationSensorPLC = false;
        }
    }

}
