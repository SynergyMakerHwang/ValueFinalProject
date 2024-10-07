using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;



public class AGV_RobotArmBase : MonoBehaviour
{
    public static AGV_RobotArmBase instance;

    Rigidbody rb;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }



    private void OnTriggerEnter(Collider other)
    {

        print("other" + other + "//" + other.tag);
        rb = other.GetComponent<Rigidbody>();

        if (other.tag.Contains("��Ʈ") || other.tag.Contains("tott"))
        {
            print("???���� base w��enter");

            rb.isKinematic = true;
            rb.useGravity = true;

            //���� �浹 �� �ӵ��� ���ӵ��� ����
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            other.transform.SetParent(transform);
        

        }        
       
    }


    private void OnTriggerExit(Collider other)
    {
        rb = other.GetComponent<Rigidbody>();
        if (other.tag.Contains("��Ʈ") || other.tag.Contains("tott"))
        {
            other.transform.SetParent(null);

            print("???���� base exit");
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

            }
        }
    }



}
