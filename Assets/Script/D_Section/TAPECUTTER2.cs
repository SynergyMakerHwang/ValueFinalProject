using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class TAPECUTTER2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Box Side 4"))
        {
            StartCoroutine(RotateCutter());
        }
    }
    IEnumerator RotateCutter()
    {
        float CurrentTime = 0;
        float firstduration = 1;
        float secondduration = 3;

        Quaternion StartPos = Quaternion.Euler(0, 0, 0);
        Quaternion TarPos = Quaternion.Euler(0, -50, 0);



        while (CurrentTime < firstduration)
        {

            CurrentTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(StartPos, TarPos, CurrentTime / firstduration);
            yield return null;
        }
        CurrentTime = 0;
        while (CurrentTime < secondduration)
        {

            CurrentTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(TarPos, StartPos, CurrentTime / firstduration);
            yield return null;
        }


    }
}
