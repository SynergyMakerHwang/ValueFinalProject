using UnityEngine;

public class AGVParkingSensor : MonoBehaviour
{
    public bool isAgvParking;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("AGV"))
        {
            isAgvParking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AGV"))
        {
            isAgvParking = false;
        }
    }
}
