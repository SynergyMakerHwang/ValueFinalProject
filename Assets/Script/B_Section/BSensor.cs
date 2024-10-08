using UnityEngine;

public class BSensor : MonoBehaviour
{
    public bool Sensor1;

    public static BSensor Instance;
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("토트박스센서"))
        {
            Sensor1 = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("토트박스센서"))
        {
            Sensor1 = false;
        }
    }
}
