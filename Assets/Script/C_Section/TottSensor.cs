using UnityEngine;

public class TottSensor : MonoBehaviour
{
    public static TottSensor Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public bool IsTottSensorPLC { get; set; }
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
