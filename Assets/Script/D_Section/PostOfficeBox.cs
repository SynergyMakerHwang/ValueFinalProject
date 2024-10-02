using UnityEngine;

public class PostOfficeBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tape"))
        {
            other.transform.SetParent(transform);
        }

    }
}
