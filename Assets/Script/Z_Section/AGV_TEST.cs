using System.Collections.Generic;
using UnityEngine;

public class AGV_TEST : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<Vector3> playerPos = new List<Vector3>();


    void Start()
    {

       

        //Store players positions somewhere
        //playerPos.Add(pPos);
        //playerPos.Add(pPos); 
        //playerPos.Add(pPos);


        //Color green = Color.green;
        //LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();    

        //lineRenderer.SetColors(red, red);
        //lineRenderer.SetWidth(0.2F, 0.2F);

        //Change how mant points based on the mount of positions is the List
        //lineRenderer.SetVertexCount(playerPos.Count);

        for (int i = 0; i < playerPos.Count; i++)
        {
            //Change the postion of the lines
           // lineRenderer.SetPosition(i, playerPos[i]);
        }







    }



}
