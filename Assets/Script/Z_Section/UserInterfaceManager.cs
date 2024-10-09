using Firebase.Database;
using Google.MiniJSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    static public UserInterfaceManager instance;

    [SerializeField] GameObject[] panelList;

    [SerializeField] GameObject signInPanel;
    [SerializeField] GameObject userSettingPanel;

    [SerializeField] GameObject stepFirstPanel;
    [SerializeField] GameObject stepSecondPanel;
    [SerializeField] GameObject stepEtcPanel;

    [SerializeField] GameObject monitoringPanel;




    Dictionary<string, Dictionary<string, string>> productGroupList = new Dictionary<string, Dictionary<string, string>>();
    Dictionary<string, string> product = new Dictionary<string, string>();
    ProductClass productClass;
    ProcessClass processClass;
    List<ProductClass> productList = new List<ProductClass>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        /** ���� ���� ������ ù������ �̿ܿ� ��Ȱ��ȭ **/
        closePanel();
        userSettingPanel.SetActive(true);




    }

    public void closePanel()
    {
        GameObject target = null;
        for (int i = 0; i < panelList.Length; i++)
        {
            target = panelList[i].gameObject;
            target.SetActive(false);
        }
    }

    //����� ���� �������� ��ȯ
    public void transUserMode()
    {

        closePanel();
        userSettingPanel.SetActive(true);
    }

    public void transPageMode(GameObject fromPanel, GameObject toPanel)
    {
        fromPanel.SetActive(false);
        toPanel.SetActive(true);
    }


    public void transUserMonitoringMode()
    {
        closePanel();
        userSettingPanel.SetActive(false);
        monitoringPanel.SetActive(true);
    }

    public void onClkNextStepSetting() {

        closePanel();
        stepSecondPanel.SetActive(true);
    }

    public void onClkEtcStepSetting()
    {

        closePanel();
        stepEtcPanel.SetActive(true);
    }



    public void btnChangeOnColor(Button target)
    {

        ColorBlock colorBlock = target.colors;
        Color newColr = Color.green;
        colorBlock.normalColor = newColr;
        target.GetComponent<Text>().text = "On";

        target.colors = colorBlock;
    }

    public void btnChangeOffColor(Button target)
    {

        ColorBlock colorBlock = target.colors;
        Color newColr = Color.gray;
        colorBlock.normalColor = newColr;
        target.GetComponent<Text>().text = "Off";

        target.colors = colorBlock;
    }

    public void btnOnChangeErrorColor(Button target)
    {

        ColorBlock colorBlock = target.colors;
        Color newColr = Color.red;
        colorBlock.normalColor = newColr;
        target.GetComponent<Text>().text = "Error";

        target.colors = colorBlock;
    }

    /*** ������ �������� ***/
    public void getUserProcessData()
    {

        closePanel();
        stepFirstPanel.SetActive(true);

        StartCoroutine(FirebaseManager.instance.ReadDataWithNewtonJsonData("product", (returnValue) =>
        {
            productClass = JsonConvert.DeserializeObject<ProductClass>(returnValue);
            print(returnValue);          

        }));

       /* StartCoroutine(FirebaseManager.instance.ReadDataWithNewtonJsonDataSnapshot("product", (returnValue) =>
        {
           
            print(returnValue);
            DataSnapshot snapshot = returnValue;

            foreach (var item in snapshot.Children)
            {
                string json = item.GetRawJsonValue();
                print("json");
                print(json);
                productClass = JsonConvert.DeserializeObject<ProductClass>(json);

                productList.Add(productClass);
            }
            print("productList");
            print(productList);
        }));*/


     
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
