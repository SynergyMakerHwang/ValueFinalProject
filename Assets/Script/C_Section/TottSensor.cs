using UnityEngine;

public class TottSensor : MonoBehaviour
{
    public static TottSensor Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
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
