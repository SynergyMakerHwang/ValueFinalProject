using Unity.VisualScripting;
using UnityEngine;

public class SubLocationSensor : MonoBehaviour
{
    public bool RightLocationSensorPLC;
    //��ҿ��� false

    //���˵Ǹ� true
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("��Ʈ�ڽ�����"))
        {
            RightLocationSensorPLC = true;
        }


    }
    //������ �Ǹ� false
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("��Ʈ�ڽ�����"))
        {
            RightLocationSensorPLC = false;
        }
    }

}
