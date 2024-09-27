using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGVManager : MonoBehaviour
{

    public List<Dictionary<int, string>> processList; //전체 설정 공정
    int processCurrentNum = 0;


    public Transform[] pathList;  



    public int waitSec = 1;
    //[Range(0f, 10f)]
    public int duration = 5;
    float currentTime;
    float speed = 5;



    private void Start()
    {
        
        StartCoroutine(MovingRoutine(pathList));

    }

    //공정의 루틴대로 

    //공정 시작점으로 이동
    private IEnumerator moveProcessStartPosion(Transform toPos, float duration)
    {
        float currentTime = 0;
        while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                currentTime = 0;
                break;

            }

            transform.localPosition += transform.right * speed * Time.deltaTime;

            float distance = (toPos.localPosition - transform.localPosition).magnitude;

            if (distance < 0.1f)
            {
                transform.localPosition = toPos.localPosition;
                yield return new WaitForEndOfFrame();
            }
        }

    }

    private IEnumerator MovingRoutine(Transform[] posList)
    {

        int next = 0;
        for (int i = 0; i < posList.Length; i++)
        {
            
            next = (i + 1);
            if (next >= posList.Length)
            {
                next = 0;
            }
            yield return MovingAct( posList[i], posList[next], 1);
        
        }


    }

    private IEnumerator MovingAct( Transform fromPos, Transform toPos, float duration)
    {

        float currentTime = 0;
        while (true)
        {
            currentTime += Time.deltaTime;
            if (currentTime > duration)
            {
                currentTime = 0;
                break;

            }



            // Quaternion rot = Quaternion.LookRotation(toPos.position.normalized);
            //target.transform.Rotate(new Vector3(toPos.position.x, 0, 0) * Time.deltaTime);
            //target.transform.rotation = Quaternion.Lerp(target.transform.rotation, Quaternion.LookRotation(new Vector3(toPos.position.x, toPos.position.y, toPos.position.z)), currentTime / duration);        


            // Quaternion rot = Quaternion.identity;
            //rot.eulerAngles = new Vector3 (toPos.position.x, toPos.position.y, toPos.position.z);            

            //target.transform.rotation = Quaternion.Slerp(toPos.transform.rotation, Quaternion.LookRotation(Vector3.forward), currentTime / duration, Space.World);


            //transform.Translate(toPos.position * speed * Time.deltaTime, Space.World);
            //transform.Translate(toPos.position * speed * Time.deltaTime);


            /* 캐릭터가 특정 속도로 이동 방향을 향해 회전하도록하기 */
            // Quaternion toRotation = Quaternion.LookRotation(toPos.position, Vector3.up);
            //target.transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, currentTime / duration);
            //완
            //target.transform.Rotate(Vector3.up * 90 * currentTime / duration, Space.World);            
            //target.transform.position = Vector3.Lerp(fromPos.position, toPos.position, currentTime / duration);

            /*
            Vector3 speed = Vector3.zero; // (0,0,0) 은 .zero 로도 표현가능
            transform.position = Vector3.SmoothDamp(transform.position, toPos.position, ref speed, 0.1f);        

            transform.Rotate(Vector3.up  * toPos.position.y * currentTime/duration, Space.World);           
            //target.transform.position = Vector3.Lerp(fromPos.position, toPos.position, currentTime / duration);
            print(fromPos.position + "/" +toPos.position);
            print(transform.position);


            yield return new WaitForEndOfFrame();
            */

            transform.localPosition += transform.right * speed * Time.deltaTime;

            float distance = (toPos.localPosition - transform.localPosition).magnitude;

            if (distance < 0.1f)
            {
                //transform.localPosition = toPos.localPosition;
                yield return new WaitForEndOfFrame();
            }
        }


       
    }

    private IEnumerator MovingNextAct( Transform fromPos, Transform toPos) {

        transform.localPosition += transform.right * speed * Time.deltaTime;

        float distance = (toPos.localPosition - transform.localPosition).magnitude;

        if (distance < 0.1f)
        {
            //transform.localPosition = toPos.localPosition;
            yield return new WaitForEndOfFrame();
        }

       

    }

   

}
