using System.Collections;
using UnityEngine;

public class tapecutter : MonoBehaviour
{

    public float Goduration;
    public float Backduration;
    public float waitsecond;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("우체국박스"))
        {

            StartCoroutine(CutterMove());
        }
    }
    IEnumerator CutterMove()
    {
        float CurrentTime = 0;

        Quaternion StartAngle = Quaternion.identity;
        Quaternion EndAngle1 = Quaternion.Euler(0,-60 , 0);
        Quaternion EndAngle2 = Quaternion.Euler(0, 0, 0);
        while (CurrentTime < Goduration)
        {
            CurrentTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(StartAngle, EndAngle1, CurrentTime / Goduration);
            
            yield return null;
           
        }
        yield return new WaitForSeconds(waitsecond);
        CurrentTime = 0;
        while (CurrentTime < Backduration)
        {
            CurrentTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(EndAngle1, EndAngle2, CurrentTime / Backduration);
            yield return null;
        }
    }

}
