using UnityEngine;

public class TottBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apples")) // �±� ��
        {
            other.transform.SetParent(transform);
        }

    }
}