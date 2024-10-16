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

    public GameObject buttonPrefab;
    public Transform IngredientsPanel;


    [Header("[A] ����͸� ����")]
    public Transform washer_BTN;
    public TextMeshProUGUI washerBTNtext;// �ؽ�Ʈ�� �޾ƿ� �κ�    
    public Transform dryer_BTN;
    public TextMeshProUGUI dryerBTNtext;// �ؽ�Ʈ�� �޾ƿ� �κ�    
    public Transform freeze_BTN;
    public TextMeshProUGUI freezeBTNtext;// �ؽ�Ʈ�� �޾ƿ� �κ�    
    public Transform cutting_BTN;
    public TextMeshProUGUI cuttingBTNtext;// �ؽ�Ʈ�� �޾ƿ� �κ�    
    public Transform packing_BTN;
    public TextMeshProUGUI packingBTNtext;// �ؽ�Ʈ�� �޾ƿ� �κ�    
    public Transform loading_BTN;
    public TextMeshProUGUI loadingBTNtext;// �ؽ�Ʈ�� �޾ƿ� �κ�    




    //Dictionary<string, Dictionary<string, string>> productGroupList = new Dictionary<string, Dictionary<string, string>>();
    //Dictionary<string, string> product = new Dictionary<string, string>();
    //ProductClass productClass;
    //ProcessClass processClass;
    //List<ProductClass> productList = new List<ProductClass>();
    [SerializeField] List<Product> IngredientsList = new List<Product>();
    public class Product
    {
        public string Ingredients;
        public List<ProductDetail> productList;
    }

    public class ProductDetail
    {
        public string productName;
        public string[] processList;
    }


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




    public void btnOnChangeColorText(string targetIndex, string msg)
    {

        Transform target;
        Button btn;
        ColorBlock colorBlock;
        Color newColr;
        if (msg == "ON" || msg == "on" || msg == "On")
        {
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
                cuttingBTNtext.text = msg;
                target = cutting_BTN;
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
           case "70":
                packingBTNtext.text = msg;
                target = packing_BTN;
                btn = target.GetComponent<Button>();
                colorBlock = btn.colors;
                colorBlock.normalColor = newColr;
                btn.colors = colorBlock;
                break;
          case "80":
                loadingBTNtext.text = msg;
                target = loading_BTN;
                btn = target.GetComponent<Button>();
                colorBlock = btn.colors;
                colorBlock.normalColor = newColr;
                btn.colors = colorBlock;
                break;
            default:
                break;
        }

    }


    /*** ������ �������� ***/
    public void getUserProcessData()
    {

        closePanel();
        stepFirstPanel.SetActive(true);

        /*StartCoroutine(FirebaseManager.instance.ReadDataWithNewtonJsonData("product", (returnValue) =>
        {
            productClass = JsonConvert.DeserializeObject<ProductClass>(returnValue);
            print(returnValue);          

        }));*/

        StartCoroutine(FirebaseManager.instance.ReadDataWithNewtonJsonDataSnapshot("product", (returnValue) =>
        {

            foreach (Transform child in IngredientsPanel) {
                Destroy(child.gameObject);
; ;          }

            print(returnValue);
            DataSnapshot snapshot = returnValue;
            
            foreach (var productSnapshot in snapshot.Children)
            {
                string json = productSnapshot.GetRawJsonValue();
                Product product = JsonConvert.DeserializeObject<Product>(json);
                IngredientsList.Add(product);

                // �ҷ��� �����͸� ��� (������)
                Debug.Log("Ingredients: " + product.Ingredients);
                GameObject newBtn = Instantiate(buttonPrefab, IngredientsPanel);

                TextMeshProUGUI buttonText = newBtn.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = product.Ingredients;

                foreach (var detail in product.productList)
                {
                    Debug.Log("Product Name: " + detail.productName);
                    foreach (var process in detail.processList)
                    {
                        Debug.Log("Process: " + process);
                    }
                }
            }
        }));


     
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
