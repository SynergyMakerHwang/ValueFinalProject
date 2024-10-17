using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using static Unity.VisualScripting.Metadata;



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
      
        if (other.tag.Contains("��Ʈ") || other.tag.Contains("tott") || other.tag.Contains("�����κ�"))
        {
            if (other.name.Contains("�����κ�")) {
                rb = other.transform.parent.GetComponent<Rigidbody>();                
            }
            else {
                rb = other.GetComponent<Rigidbody>();                

            }

            if (rb.transform.parent == null) {
                rb.isKinematic = true;
                rb.useGravity = false;

                //���� �浹 �� �ӵ��� ���ӵ��� ����
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.transform.SetParent(transform);
            }

        }        
       
    }


   /* private void OnTriggerExit(Collider other)
    {
        print("agv  == OnTriggerExit");
        rb = other.GetComponent<Rigidbody>();
        if (other.tag.Contains("��Ʈ") || other.tag.Contains("tott") || other.tag.Contains("�����κ�"))
        {

            if (other.name.Contains("�����κ�"))
            {
                rb = other.transform.parent.GetComponent<Rigidbody>();
                print("agv  == other.name.Contains(����)");
            }
            else
            {
                rb = other.GetComponent<Rigidbody>();
                print("agv  == other.name.Contains(else)");
            }

            rb.transform.SetParent(null);

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

            }
        }
    }*/



}
