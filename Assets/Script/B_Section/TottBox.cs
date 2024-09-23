using UnityEngine;

public class TottBox : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Apples"))
        {
            other.transform.SetParent(transform);
            if (transform.childCount >= 5)
            {
                WasherController.instance.CommuteTott();

            }

        }

    }
}
