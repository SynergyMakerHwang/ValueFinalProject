using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    static public UserInterfaceManager instance;


    [SerializeField] GameObject signInPanel;
    [SerializeField] GameObject userSettingPanel;

    [SerializeField] GameObject stepFirstPanel;
    [SerializeField] GameObject stepSecondPanel;
    [SerializeField] GameObject stepEtcPanel;


    Dictionary<string, Dictionary<string, string>> productGroupList = new Dictionary<string, Dictionary<string, string>>();
    Dictionary<string, string> product = new Dictionary<string, string>();
    ProductClass productClass;
    ProcessClass processClass;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        /** ���� ���� ������ ù������ �̿ܿ� ��Ȱ��ȭ **/
        stepFirstPanel.SetActive(true);
        stepSecondPanel.SetActive(false);
        stepEtcPanel.SetActive(false);

    }

    //����� ���� �������� ��ȯ
    public void transUserMode()
    {
        print("transUserMode");
        signInPanel.SetActive(false);
        userSettingPanel.SetActive(true);
    }

    public void transPageMode(GameObject fromPanel, GameObject toPanel)
    {
        fromPanel.SetActive(false);
        toPanel.SetActive(true);
    }

    /*** ������ �������� ***/
    public void getUserProcessData()
    {
        print("������ ��������==1");       

        /*foreach (var item in readProduct)
        {
            string json = item.GetRawJsonValue();
            print(json);
            productClass = JsonConvert.DeserializeObject<ProductClass>(json);
        }*/
        StartCoroutine(FirebaseManager.instance.ReadDataWithNewtonJsonData("product", (returnValue) =>
        {
            print(returnValue);

        }));

        print("������ ��������==2");
    }

    /** ���� ���� - 1�ܰ� START **/

    ///1. infoText �ٲٰ�,
    ///2. ��ǰ����Ʈ ��������

    /** ���� ���� - 1�ܰ� END **/


    /** ���� ���� - 2�ܰ�-1 START **/
    /** ���� ���� - 2�ܰ�-1 END **/


    /** ���� ���� - 2�ܰ�-2 (��Ÿ ����) START **/
    /** ���� ���� - 2�ܰ�-2 (��Ÿ ����) END **/
    
}
