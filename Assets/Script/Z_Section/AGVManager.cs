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

    public Transform[] pathPoints; // �̵��� ����� ����Ʈ��    
    public Transform[] washerSpathPoints; // ��ô���� ���� �̵� ���
    public Transform[] washerEpathPoints; // ��ô���� ���� �̵� ���
    
    public Transform[] dryerSpathPoints; // �������� ���� �̵� ���
    public Transform[] dryerEpathPoints; // �������� ���� �̵� ���

    public Transform[] cuttingSpathPoints; // ���ܰ��� ���� �̵� ���
    public Transform[] cuttingMpathPoints; // ���ܰ��� �߰� �Ͽ� �̵� ���
    public Transform[] cuttingEpathPoints; // ���ܰ��� ���� �̵� ���


    public Transform[] packingSpathPoints; // ������� ���� �̵� ���
    public Transform[] packingEpathPoints; // ������� ���� �̵� ���

    public Transform[] loadingSpathPoints; // ������� ���� �̵� ���
    public Transform[] loadingEpathPoints; // ������� ���� �̵� ���

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

        //������ ��� 
        pathRoot.Add("30", washerSpathPoints); //��ô����
        pathRoot.Add("40", cuttingSpathPoints);//���ܰ���
        pathRoot.Add("50", dryerSpathPoints); //��ǳ����
        pathRoot.Add("60", dryerSpathPoints); //�������
        pathRoot.Add("70", packingSpathPoints); //�������
        pathRoot.Add("80", loadingSpathPoints); //�������
    }


    //DB - ���� ���� �� ����
    public void processRootSetting(string[] processList) {
        entireProcessList = processList;      
    }



    //������ ���� ����
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
                            //���� �������� �̵�
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
                            //���� �������� �̵�
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
                            //���� �������� �̵�
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
                            //���� �������� �̵�
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
                            //���� �������� �̵�
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
                            //���� �������� �̵�
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


   


    //������ ���� ����
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

            // ���� Waypoint������ ���� ���
            Vector3 direction = pathPoints[currentPointIndex].position - transform.position;
            direction.y = 0; // ���� ȸ���� �����ϰ� ���� �� Y���� ����

            // ��ü�� Waypoint�� �����ߴ��� Ȯ��
            if (direction.magnitude < 0.1f)
            {
                break;
            }

            // ȸ�� ó��: ��ü�� Waypoint�� ���ϵ��� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime / duration);

            // �̵� ó��: Waypoint �������� �̵�
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

                // ���� Waypoint������ ���� ���
                Vector3 direction = path[index].position - transform.position;
                direction.y = 0; // ���� ȸ���� �����ϰ� ���� �� Y���� ����

                // ��ü�� Waypoint�� �����ߴ��� Ȯ��
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


                // ȸ�� ó��: ��ü�� Waypoint�� ���ϵ��� ȸ��
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime / duration);

                // �̵� ó��: Waypoint �������� �̵�
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

                // ���� Waypoint������ ���� ���
                Vector3 direction = path[index].position - transform.position;
                direction.y = 0; // ���� ȸ���� �����ϰ� ���� �� Y���� ����

                // ��ü�� Waypoint�� �����ߴ��� Ȯ��
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
               

                // ȸ�� ó��: ��ü�� Waypoint�� ���ϵ��� ȸ��
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentTime / duration);

                // �̵� ó��: Waypoint �������� �̵�
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
