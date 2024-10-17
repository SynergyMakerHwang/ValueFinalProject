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
      
        if (other.tag.Contains("토트") || other.tag.Contains("tott") || other.tag.Contains("센서부분"))
        {
            if (other.name.Contains("센서부분")) {
                rb = other.transform.parent.GetComponent<Rigidbody>();                
            }
            else {
                rb = other.GetComponent<Rigidbody>();                

            }

            if (rb.transform.parent == null) {
                rb.isKinematic = true;
                rb.useGravity = false;

                //현재 충돌 후 속도와 가속도를 제거
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
        if (other.tag.Contains("토트") || other.tag.Contains("tott") || other.tag.Contains("센서부분"))
        {

            if (other.name.Contains("센서부분"))
            {
                rb = other.transform.parent.GetComponent<Rigidbody>();
                print("agv  == other.name.Contains(센서)");
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
