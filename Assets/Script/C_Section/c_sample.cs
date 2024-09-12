using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class c_sample : MonoBehaviour
{
    [SerializeField] bool DoorValue;

    bool DoorCheck = true;

    //2초안에 여닫기 가능
    float duration = 2;
    private void Update()
    {
      
        // 조건에 따라 값을 받는다.
        if (transform.localRotation.eulerAngles.y == 0 && DoorValue == true)
        {  
                StartCoroutine(OpenClose(DoorValue)); 
        }
        if (transform.localRotation.eulerAngles.y == 90 && DoorValue == false)
        {
              StartCoroutine(OpenClose(DoorValue));        
        }
    }


    IEnumerator OpenClose(bool Btn)
    {
        //초기값 설정
        float StartDoorY = transform.localRotation.eulerAngles.y;
        float EndDoorY = Btn ? 90 : 0;
        float CurrentTIme = 0;

        while (CurrentTIme < duration)
        {
            CurrentTIme += Time.deltaTime;
            float NewY = Mathf.Lerp(StartDoorY, EndDoorY, CurrentTIme / duration);
            transform.localRotation = Quaternion.Euler(0, NewY, 0);     
            yield return null;
        }



    }

}
