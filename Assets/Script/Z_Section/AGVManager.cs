using Google.MiniJSON;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AGVManager : MonoBehaviour
{
    public static AGVManager Instance;
    //string[] entireProcessList = new string[]{ "30", "40", "50" , "70", "80"};  
    string[] entireProcessList = new string[]{ "30", "50", "70", "80"};  
    int entireProcessCurrentNum = 0;

    public bool isAGVmoving = false;

    public Transform[] pathPoints; // 이동할 경로의 포인트들    
    public Transform[] washerSpathPoints; // 세척공정 시작 이동 경로
    public Transform[] washerEpathPoints; // 세척공정 종료 이동 경로
    
    public Transform[] dryerSpathPoints; // 건조공정 시작 이동 경로
    public Transform[] dryerEpathPoints; // 건조공정 종료 이동 경로

    public Transform[] cuttingSpathPoints; // 절단공정 시작 이동 경로
    public Transform[] cuttingMpathPoints; // 절단공정 중간 하역 이동 경로
    public Transform[] cuttingEpathPoints; // 절단공정 종료 이동 경로


    public Transform[] packingSpathPoints; // 포장공정 시작 이동 경로
    public Transform[] packingEpathPoints; // 포장공정 종료 이동 경로

    public Transform[] loadingSpathPoints; // 적재공정 시작 이동 경로
    public Transform[] loadingEpathPoints; // 적재공정 종료 이동 경로

    public int duration = 20;
    private int currentPointIndex = 0;

    Dictionary<string, Transform[]> pathRoot = new Dictionary<string, Transform[]>();

    private void Awake()
    {
        if (Instance == null) { 
            Instance = this;
        }
    }

    private void Start()
    {
        entireProcessCurrentNum = 0;

        //공정별 경로 
        pathRoot.Add("30", washerSpathPoints); //세척공정
        pathRoot.Add("40", cuttingSpathPoints);//절단공정
        pathRoot.Add("50", dryerSpathPoints); //열풍건조
        pathRoot.Add("60", dryerSpathPoints); //동결건조
        pathRoot.Add("70", packingSpathPoints); //포장공정
        pathRoot.Add("80", loadingSpathPoints); //적재공정
    }


    //DB - 공정 시작 시 설정
    public void processRootSetting(string[] processList) {
        entireProcessList = processList;      
    }



    //공정의 종료 시점
    public IEnumerator moveProcessEndPostion(string processNum) {

      
        if (!isAGVmoving  && entireProcessCurrentNum < entireProcessList.Length) {
            isAGVmoving = true;
        
            switch (processNum)
            {
                case "30":
                    StartCoroutine(moveLoopPoint(washerEpathPoints, (returnValue) =>
                    {
                        if (returnValue)
                        {
                            //다음 공정으로 이동
                            StartCoroutine(moveProcessStartPostion());
                            isAGVmoving = false;
                        }
                        print(returnValue);

                    }));
                    break;

                case "40":
                    StartCoroutine(moveLoopPoint(cuttingEpathPoints, (returnValue) =>
                    {
                        if (returnValue)
                        {
                            //다음 공정으로 이동
                            StartCoroutine(moveProcessStartPostion());
                            isAGVmoving = false;
                        }
                        print(returnValue);

                    }));
                    break;
                case "41":
                    StartCoroutine(moveLoopPoint(cuttingMpathPoints));
                    break;

                case "50":
                  
                    StartCoroutine(moveLoopPoint(dryerEpathPoints, (returnValue) =>
                    {
                      
                        if (returnValue)
                        {
                            //다음 공정으로 이동
                            StartCoroutine(moveProcessStartPostion());
                          
                            isAGVmoving = false;
                        }
                        print(returnValue);

                    }));
                    break;
                case "60":
                    StartCoroutine(moveLoopPoint(dryerEpathPoints, (returnValue) =>
                    {
                        if (returnValue)
                        {
                            //다음 공정으로 이동
                            StartCoroutine(moveProcessStartPostion());
                            isAGVmoving = false;
                        }
                        print(returnValue);

                    }));
                    break;

                case "70":
                    StartCoroutine(moveLoopPoint(packingEpathPoints, (returnValue) =>
                    {
                        if (returnValue)
                        {
                            //다음 공정으로 이동
                            StartCoroutine(moveProcessStartPostion());
                            isAGVmoving = false;
                        }
                        print(returnValue);

                    }));
                    break;
                case "80":
                    StartCoroutine(moveLoopPoint(loadingEpathPoints, (returnValue) =>
                    {
                        if (returnValue)
                        {
                            //다음 공정으로 이동
                            StartCoroutine(moveProcessStartPostion());
                            isAGVmoving = false;
                        }
                        print(returnValue);

                    }));
                    break;
                default:
                    isAGVmoving = false;
                    break;
            }

            yield return false;
            
        }
       


    }


   


    //공정의 시작 시점
    public IEnumerator moveProcessStartPostion()
    {
       
        if (entireProcessList != null)
        {
           
            if (entireProcessList.Length>0 && entireProcessCurrentNum < entireProcessList.Length) {
                Transform[] tmpRoot = pathRoot[entireProcessList[entireProcessCurrentNum]];
                
                yield return(moveLoopPoint(tmpRoot, (returnValue) =>
                {
                    if (returnValue)
                    {
                                             
                        entireProcessCurrentNum++;
                    }                   

                }));
            }
            
        }
    }

    public void OnclickNextPoint()
    {
      
        if (pathPoints.Length > 0 && currentPointIndex < pathPoints.Length)
        {
            StartCoroutine(moveToNextPoint(currentPointIndex));
            currentPointIndex++;
        }

    }

    private IEnumerator moveToNextPoint(int currentPointIndex)
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

            // 현재 Waypoint까지의 방향 계산
            Vector3 direction = pathPoints[currentPointIndex].position - transform.position;
            direction.y = 0; // 수평 회전만 적용하고 싶을 때 Y축은 고정

            // 물체가 Waypoint에 도착했는지 확인
            if (direction.magnitude < 0.1f)
            {
                break;
            }

            // 회전 처리: 물체가 Waypoint를 향하도록 회전
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime / duration);

            // 이동 처리: Waypoint 방향으로 이동
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[currentPointIndex].position, currentTime / duration);
            yield return new WaitForEndOfFrame();
        }


    }
    private IEnumerator moveLoopPoint(Transform[] path)
    {

      
        float currentTime = 0;
        int index = 0;
        if (path.Length > 0)
        {

            while (true)
            {


                currentTime += Time.deltaTime;
                if (currentTime > duration)
                {
                    currentTime = 0;
                    break;

                }

                // 현재 Waypoint까지의 방향 계산
                Vector3 direction = path[index].position - transform.position;
                direction.y = 0; // 수평 회전만 적용하고 싶을 때 Y축은 고정

                // 물체가 Waypoint에 도착했는지 확인
                if (direction.magnitude < 0.1f)
                {

                    if (index < path.Length - 1)
                    {
                        index++;
                    }
                    else
                    {
                       
                        break;
                    }

                }


                // 회전 처리: 물체가 Waypoint를 향하도록 회전
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime / duration);

                // 이동 처리: Waypoint 방향으로 이동
                transform.position = Vector3.MoveTowards(transform.position, path[index].position, currentTime / duration);
                yield return new WaitForEndOfFrame();

                if (index > path.Length)
                {
                    
                    break;
                }
            }

        }
        
    }

    private IEnumerator moveLoopPoint(Transform[] path , System.Action<bool> callback)
    {

        bool result = false;
        float currentTime = 0;
        int index = 0;
        if (path.Length > 0)
        {

            while (true)
            {
               

                currentTime += Time.deltaTime;
                if (currentTime > duration)
                {
                    currentTime = 0;
                    break;

                }

                // 현재 Waypoint까지의 방향 계산
                Vector3 direction = path[index].position - transform.position;
                direction.y = 0; // 수평 회전만 적용하고 싶을 때 Y축은 고정

                // 물체가 Waypoint에 도착했는지 확인
                if (direction.magnitude < 0.1f)
                {

                    if (index < path.Length - 1)
                    {
                        index++;
                    }
                    else {
                      result = true;
                      break;
                    }
                    
                }
               

                // 회전 처리: 물체가 Waypoint를 향하도록 회전
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime / duration);

                // 이동 처리: Waypoint 방향으로 이동
                transform.position = Vector3.MoveTowards(transform.position, path[index].position, currentTime / duration);
                yield return new WaitForEndOfFrame();

                if (index > path.Length)
                {
                    result = true;                    
                    break;
                }
            }

        }

        callback(result);
    }


}
