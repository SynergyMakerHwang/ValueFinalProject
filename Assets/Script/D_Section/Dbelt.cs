using UnityEngine;

public class Dbelt : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("토트박스") || other.CompareTag("FruitPouchs"))
            other.transform.SetParent(transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("토트박스") || other.CompareTag("FruitPouchs"))
            other.transform.SetParent(null);
    }
}
