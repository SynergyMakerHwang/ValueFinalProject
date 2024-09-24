using Unity.VisualScripting;
using UnityEngine;

public class AGVParkingSensor : MonoBehaviour
{
    public bool isAgvParking;




     private void OnTriggerEnter(Collider other)
      {
          print("==============trigger enter");
        print(other.tag);
        print(other);
        if (other.tag == "AGV")
        {
              print("==============trigger agv");
              isAgvParking = true;
          }
      }

      private void OnTriggerExit(Collider other)
      {
          print("==============trigger exit");
          if (other.tag == "AGV")
          {
              print("==============trigger agv exit");
              isAgvParking = false;
          }
      }


    

}
