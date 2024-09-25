using System.Collections;
using UnityEngine;

public class SubWeightSensor : MonoBehaviour
{
    public bool WeightSensorPLC;
    private int fruitCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        // 과일 태그 확인
        if (other.CompareTag("Apples") || other.CompareTag("Strawberrys") || other.CompareTag("Oranges"))
        {
            fruitCount++; // 통과 횟수 증가
       

            // 5번 통과했는지 확인
            if (fruitCount >= 5)
            {
                // 0으로 초기화
                fruitCount = 0;
                //X32
                WeightSensorPLC = true;
        
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Apples") || other.CompareTag("Strawberrys") || other.CompareTag("Oranges"))
        {
            fruitCount++; // 통과 횟수 증가


            // 5번 통과했는지 확인
            if (fruitCount >= 5)
            {
                // 0으로 초기화
                fruitCount = 0;
              
                WeightSensorPLC = false;

            }
        }
    }
}
