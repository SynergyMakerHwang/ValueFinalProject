using Firebase.Database;
using Google.MiniJSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
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

    [Header("[A] 모니터링 관련")]
    public Transform washer_BTN;
    public TextMeshProUGUI washerBTNtext;// 텍스트를 받아올 부분    
    public Transform dryer_BTN;
    public TextMeshProUGUI dryerBTNtext;// 텍스트를 받아올 부분    
    public Transform freeze_BTN;
    public TextMeshProUGUI freezeBTNtext;// 텍스트를 받아올 부분    
    public Transform cutting_BTN;
    public TextMeshProUGUI cuttingBTNtext;// 텍스트를 받아올 부분    
    public Transform packing_BTN;
    public TextMeshProUGUI packingBTNtext;// 텍스트를 받아올 부분    
    public Transform loading_BTN;
    public TextMeshProUGUI loadingBTNtext;// 텍스트를 받아올 부분    


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




    public void btnOnChangeColorText(string targetIndex, string msg)
    {

        Transform target;
        Button btn;
        ColorBlock colorBlock;
        Color newColr;
        if(msg == "ON" || msg == "on" || msg == "On") {
            newColr = Color.green;
        }
        else if (msg == "ERROR" || msg == "error" || msg == "Error")
        {
            newColr = Color.red;
        }
        else 
        {
            newColr = Color.gray;
        }
        
        

        switch (targetIndex)
        {
            case "30":
                washerBTNtext.text = msg;
                target = washer_BTN;
                btn = target.GetComponent<Button>();
                colorBlock = btn.colors;
                colorBlock.normalColor = newColr;
                btn.colors = colorBlock;
                break;

            case "40":
                dryerBTNtext.text = msg;
                target = dryer_BTN;
                btn = target.GetComponent<Button>();
                colorBlock = btn.colors;
                colorBlock.normalColor = newColr;
                btn.colors = colorBlock;
                break;

            case "50":
                dryerBTNtext.text = msg;
                target = dryer_BTN;
                btn = target.GetComponent<Button>();
                colorBlock = btn.colors;
                colorBlock.normalColor = newColr;
                btn.colors = colorBlock;
                break;
            case "60":
                freezeBTNtext.text = msg;
                target = freeze_BTN;
                btn = target.GetComponent<Button>();
                colorBlock = btn.colors;
                colorBlock.normalColor = newColr;
                btn.colors = colorBlock;
                break;

            default:
                break;
        }
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
