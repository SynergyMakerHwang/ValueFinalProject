using UnityEngine;
using Unity.VisualScripting;

public class AGV_RobotArmGripper : MonoBehaviour
{

    public bool isSuctionMode = false;
    Rigidbody rb;
    [SerializeField] Transform suction;
    public static AGV_RobotArmGripper instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {

        rb = other.GetComponent<Rigidbody>();

        if (isSuctionMode)
        {
            if (other.tag.Contains("��Ʈ"))
            {

                rb.isKinematic = true;
                rb.useGravity = false;

                //���� �浹 �� �ӵ��� ���ӵ��� ����
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                other.transform.SetParent(transform);
                //isAttached = true;
            }
        }
        else
        {
            if (other.tag.Contains("��Ʈ"))
                other.transform.SetParent(null);
            //isAttached = false;

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

            }
        }
    }


    public void removeChild(bool isSuctionOn)
    {

        if (!isSuctionOn)
        {
            if (suction.childCount > 0)
            {
                Rigidbody childRb = suction.GetChild(0).GetComponent<Rigidbody>();
                if (childRb.tag.Contains("��Ʈ"))
                {
                    childRb.isKinematic = false;
                    childRb.useGravity = true;
                    suction.DetachChildren();
                    //isAttached = false;
                }
            }
        }
    }


}
