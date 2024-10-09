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

        /** 공정 설정 페이지 첫페이지 이외에 비활성화 **/
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

    //사용자 설정 페이지로 전환
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

    /*** 데이터 가져오기 ***/
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

    /** 공정 설정 - 1단계 START **/

    ///1. infoText 바꾸고,
    ///2. 상품리스트 가져오기

    /** 공정 설정 - 1단계 END **/


    /** 공정 설정 - 2단계-1 START **/
    /** 공정 설정 - 2단계-1 END **/


    /** 공정 설정 - 2단계-2 (기타 설정) START **/
    /** 공정 설정 - 2단계-2 (기타 설정) END **/
    
}
