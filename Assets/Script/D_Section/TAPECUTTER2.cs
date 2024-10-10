using UnityEngine;
using UnityEngine.XR;
using System.Collections;

public class TAPECUTTER2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Box Side 4"))
        {
            StartCoroutine(RotateHand());
        }
    }
    IEnumerator RotateHand()
    {
        float CurrentTime = 0;
        float duration = 1;

        Quaternion StartPos = Quaternion.Euler(0, 0, 0);
        Quaternion TarPos = Quaternion.Euler(0, -35, 0);
        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(StartPos, TarPos, CurrentTime / duration);
            yield return null;
        }
        CurrentTime = 0;
        // 2ÃÊ
        while(CurrentTime < duration*2)
        {
            CurrentTime += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(TarPos, StartPos, CurrentTime / duration*2);
            yield return null;
        }

    }
}
