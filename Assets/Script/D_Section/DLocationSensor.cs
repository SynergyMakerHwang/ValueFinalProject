using System.Collections;
using UnityEngine;

public class DLocationSensor : MonoBehaviour
{
    [SerializeField] Transform Hand;
    public bool RightSensorPLC;

    Coroutine coroutine;
    public static DLocationSensor Instance;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    IEnumerator RotateGO()
    {
        float CurrentTime = 0;
        float duration = 1;
        Quaternion StartPos = Quaternion.Euler(-50, 0, 180);
        Quaternion TarPos = Quaternion.Euler(20, 0, 180);


        while (CurrentTime < duration)
        {
            CurrentTime += Time.deltaTime;
            Hand.localRotation = Quaternion.Slerp(StartPos, TarPos, CurrentTime / duration);
            yield return null;
        }
        // 초기화
        coroutine = null;
    }
    IEnumerator RotateBack()
    {
        {
            float CurrentTime = 0;
            float duration = 1;
            Quaternion StartPos = Quaternion.Euler(-50, 0, 180);
            Quaternion TarPos = Quaternion.Euler(20, 0, 180);


            while (CurrentTime < duration)
            {
                CurrentTime += Time.deltaTime;
                if (CurrentTime < duration)
                    Hand.localRotation = Quaternion.Slerp(TarPos, StartPos, CurrentTime / duration);

                yield return null;
            }
            // 초기화
            coroutine = null;
        }
    }

    public void UpperTapingBackPLC()
    {
        if (coroutine == null)
            coroutine = StartCoroutine(RotateBack());

    }
    public void UpperTapingGoPLC()

    {
        if (coroutine == null)
            coroutine = StartCoroutine(RotateGO());

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Box2"))
        {

            RightSensorPLC = true;
         
            print(RightSensorPLC);
       
             
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
