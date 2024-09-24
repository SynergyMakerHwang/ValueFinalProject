using Unity.VisualScripting;
using UnityEngine;

public class AGVParkingSensor : MonoBehaviour
{
    public bool isAgvParking;




     private void OnTriggerEnter(Collider other)
      {
     
        if (other.tag == "AGV")
        {
              isAgvParking = true;
          }
      }

      private void OnTriggerExit(Collider other)
      {
       
          if (other.tag == "AGV")
          {  
              isAgvParking = false;
          }
      }


    

}
