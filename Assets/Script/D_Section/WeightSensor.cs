using UnityEngine;

public class DWeightSensor : MonoBehaviour
{
    int Cnt;
    public bool DWeightSensorPLC;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("FruitPouchs"))
        {
            Cnt++;
            if (Cnt == 5)
            {
                Cnt = 0;
               DWeightSensorPLC = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {
           DWeightSensorPLC = false;
        }
    }
}
