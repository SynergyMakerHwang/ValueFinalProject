using UnityEngine;

public class TottBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apples")) // 태그 비교
        {
            other.transform.SetParent(transform);
        }

    }
}