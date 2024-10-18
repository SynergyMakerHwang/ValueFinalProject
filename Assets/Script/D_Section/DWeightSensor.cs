using UnityEngine;

public class DWeightSensor : MonoBehaviour
{
    int Cnt;
    public bool DWeightSensorPLC;

    public static DWeightSensor instance;

    public void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FruitPouchs"))
        {
          
                DWeightSensorPLC = true;
            
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
