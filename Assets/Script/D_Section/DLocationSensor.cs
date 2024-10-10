using System.Collections;
using UnityEngine;

public class DLocationSensor : MonoBehaviour
{
    [SerializeField] Transform Hand;
    bool RightSensorPLC;

    Coroutine coroutine;
    public static DLocationSensor Instance;

    IEnumerator RotateHand()
    {
        float CurrentTime = 0;
        float duration = 1;
        float halfduration = duration / 2;
        Quaternion StartPos = Quaternion.Euler(-50, 0, 180);
        Quaternion TarPos = Quaternion.Euler(20, 0, 180);


        yield return new WaitForSeconds(1.5f);

        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime <= halfduration)
                Hand.localRotation = Quaternion.Slerp(StartPos, TarPos, CurrentTime / halfduration);
            else
                Hand.localRotation = Quaternion.Slerp(TarPos, StartPos, (CurrentTime - halfduration) / halfduration);
            yield return null;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {

            RightSensorPLC = true;
            D2.instance.WrapConveyorOnPLC();
            
            coroutine = StartCoroutine(RotateHand());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        {
            if (other.name.StartsWith("Box2"))
            {
                RightSensorPLC = false;
                print(RightSensorPLC);
            }
        }
    }
}
