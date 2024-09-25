using System.Collections;
using UnityEngine;

public class SubWeightSensor : MonoBehaviour
{
    public bool WeightSensorPLC;
    private int fruitCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        // ���� �±� Ȯ��
        if (other.CompareTag("Apples") || other.CompareTag("Strawberrys") || other.CompareTag("Oranges"))
        {
            fruitCount++; // ��� Ƚ�� ����
       

            // 5�� ����ߴ��� Ȯ��
            if (fruitCount >= 5)
            {
                // 0���� �ʱ�ȭ
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
            fruitCount++; // ��� Ƚ�� ����


            // 5�� ����ߴ��� Ȯ��
            if (fruitCount >= 5)
            {
                // 0���� �ʱ�ȭ
                fruitCount = 0;
              
                WeightSensorPLC = false;

            }
        }
    }
}
