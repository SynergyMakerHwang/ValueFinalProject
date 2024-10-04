using UnityEngine;

public class Absorber : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2") && transform.childCount == 0)
        {


            other.transform.SetParent(transform);

        }
    }
}
