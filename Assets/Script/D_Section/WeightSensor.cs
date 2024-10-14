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
            print(Cnt);
            if (Cnt == 5)
            {
                Cnt = 0;
               DWeightSensorPLC = true;
                print("Don");
           
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {
           DWeightSensorPLC = false;
            print("Letsgo");
        }
    }
}
