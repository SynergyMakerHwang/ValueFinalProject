using UnityEngine;

public class TottBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apples")||other.CompareTag("Strawberrys")||other.CompareTag("Oranges")) // �±� ��
        {
            other.transform.SetParent(transform);

            // �ڽ� ��ü �� Ȯ��
            if (transform.childCount >= 5)
            {
            }
      
        }

    }
}