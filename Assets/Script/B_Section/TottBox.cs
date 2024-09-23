using UnityEngine;

public class TottBox : MonoBehaviour
{
    bool WeightSensorPLC = false;
    bool rightlocationSensorPLC = false;
    void Start()
    {
    }

    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apples")) // 태그 비교
        {
            other.transform.SetParent(transform);


            if (transform.childCount >= 5)
            {
                //무게센서 트루값 반환
                WeightSensorPLC = true;
            }
        }
        else if (other.name.Contains("Sensor"))
        {
            //정위치센서 트루값 반환
            rightlocationSensorPLC = true;
        }


    }
}