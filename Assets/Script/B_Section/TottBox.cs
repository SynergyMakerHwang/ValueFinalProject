using UnityEngine;

public class TottBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apples")||other.CompareTag("Strawberrys")||other.CompareTag("Oranges")) // 태그 비교
        {
            other.transform.SetParent(transform);

            // 자식 개체 수 확인
            if (transform.childCount >= 5)
            {
            }
      
        }

    }
}