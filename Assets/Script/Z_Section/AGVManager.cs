using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class AGVManager : MonoBehaviour
{

    public Transform AGV;
    public Transform[] positionList;


    public int waitSec = 1;
    //[Range(0f, 10f)]
    public int duration = 5;
    float currentTime;


    private void Start()
    {
        StartCoroutine(MovingRoutine(AGV, positionList));
    }

    private IEnumerator MovingRoutine(Transform target, Transform[] posList)
    {

        int next = 0;
        for (int i = 0; i < posList.Length; i++)
        {
            
            next = (i + 1);
            if (next >= posList.Length)
            {
                next = 0;
            }            
            yield return MovingAct(target, posList[i], posList[next], 1);
        }


    }

    private IEnumerator MovingAct(Transform target, Transform fromPos, Transform toPos, float duration)
    {
        currentTime = 0;
        while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                currentTime = 0;
                break;

            }
            
            target.transform.position = Vector3.Lerp(fromPos.position, toPos.position, currentTime / duration);

           // Quaternion rot = Quaternion.LookRotation(toPos.position.normalized);
            target.transform.Rotate(new Vector3(toPos.position.x, 0, 0) * Time.deltaTime);
            //target.transform.rotation = rot;
            //target.transform.rotation = Quaternion.Slerp(toPos.transform.rotation, Quaternion.LookRotation(Vector3.forward), currentTime / duration, Space.World);
            yield return new WaitForEndOfFrame();
        }
    }

}
