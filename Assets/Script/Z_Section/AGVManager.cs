using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class AGVManager : MonoBehaviour
{

    public Transform AGV;
    public Transform[] positionList;


    public int waitSec = 1;
    //[Range(0f, 10f)]
    public int duration = 10;
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
            // 1. Position 0에서 3초간 대기 
            yield return new WaitForSeconds(3);

            next = (i + 1);
            if (next >= posList.Length)
            {
                next = 0;
            }
            // 2. Position 0 -> Positon 1까지 1초 동안 이동
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
            yield return new WaitForEndOfFrame();
        }
    }

}
