using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;


public class AGV_RobotArmGripper : MonoBehaviour
{
    public static AGV_RobotArmGripper instance;

    [Header("Grip ��� ����")]
    public bool isGripperMode = false;
    Rigidbody rb;
    [SerializeField] Transform gripper;    


    [Header("Gripper ������ ����")]
    [SerializeField] Transform gripperL;
    [SerializeField] Transform gripperR;
    [SerializeField] float maxRange = 0.6f;
    [SerializeField] float minRange = 0f;
    [SerializeField] float duration = 2;
    [SerializeField] MeshRenderer lsFront;
    [SerializeField] MeshRenderer lsBack;
    [SerializeField] bool isMoving;
    [SerializeField] bool isForward;
    [SerializeField] bool isForwardOn;


    public bool LsForwardOn { get { return isForwardOn; } }
    [SerializeField] bool isBackWardOn;
    public bool LsBackWardOn { get { return isBackWardOn; } }
    float currentTime;       


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


    private void Start()
    {
        isBackWardOn = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        
        print("other"+ other+"//"+ other.tag);
        print("isGripperMode" + isGripperMode);

        rb = other.GetComponent<Rigidbody>();
        if (isGripperMode)
        {
          
            
            if (other.tag.Contains("��Ʈ") || other.tag.Contains("tott"))
            {
                
                rb.isKinematic = true;
                rb.useGravity = false;

                //���� �浹 �� �ӵ��� ���ӵ��� ����
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                other.transform.SetParent(transform);
                //isAttached = true;

                print("gripper");
            }
        }
        else
        {
            if (other.tag.Contains("��Ʈ") || other.tag.Contains("tott"))
                other.transform.SetParent(null);
            //isAttached = false;

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

            }
        }
    }


    public void removeChild(bool isGripperOn)
    {
        isGripperMode = false;
        
        if (!isGripperOn)
        {        
            if (gripper.childCount > 0)
            {
               
                Rigidbody childRb = gripper.GetChild(gripper.childCount-1).GetComponent<Rigidbody>();
        
                if (childRb.tag.Contains("��Ʈ") || childRb.tag.Contains("tott"))
                {
                    childRb.isKinematic = false;
                    childRb.useGravity = true;
                    childRb.transform.SetParent(null);
                    //gripper.DetachChildren();
                    //isAttached = false;
                }
            }
        }
    }

  


    public void OnForwardBtnClkEvent()
    {
        Vector3 startRPos = new Vector3(0, 0, minRange);
        Vector3 endRPos = new Vector3(0, 0, maxRange);
        Vector3 startLPos = new Vector3(0, 0, -minRange);
        Vector3 endLPos = new Vector3(0, 0, -maxRange);

        if (!isMoving && !isForward)
        {
            StartCoroutine(MoviCylinder(gripperR, startRPos, endRPos, duration));
            StartCoroutine(MoviCylinder(gripperL, startLPos, endLPos, duration));
        }
    }

    public void OnBackwardBtnClkEvent()
    {
        Vector3 startRPos = new Vector3(0, 0, minRange);
        Vector3 endRPos = new Vector3(0, 0, maxRange);
        Vector3 startLPos = new Vector3(0, 0, -minRange);
        Vector3 endLPos = new Vector3(0, 0, -maxRange);

        if (!isMoving && isForward)
        {
            StartCoroutine(MoviCylinder(gripperR, endRPos, startRPos, duration));
            StartCoroutine(MoviCylinder(gripperL, endLPos, startLPos, duration));
        }
    }

    IEnumerator MoviCylinder(Transform cylinderRod, Vector3 from, Vector3 to, float duration)
    {
        isMoving = true;

        if (isForward)
        {
            isForwardOn = false;
        }
        else
        {
            isBackWardOn = false;
        }


        while (isMoving)
        {
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                currentTime = 0;
                isMoving = false;
                if (isForward)
                {
                    isBackWardOn = true;
                }
                else
                {
                    isForwardOn = true;
                                    }
                isForward = !isForward;
                break;
            }

            cylinderRod.localPosition = Vector3.Lerp(from, to, currentTime / duration);
            yield return new WaitForEndOfFrame();
        }

    }

}
