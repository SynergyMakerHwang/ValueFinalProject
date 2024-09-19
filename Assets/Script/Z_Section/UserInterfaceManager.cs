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


    [SerializeField] Dictionary<string, Dictionary<string, string>> productGroupList = new Dictionary<string, Dictionary<string, string>>();
    [SerializeField] Dictionary<string, string> product = new Dictionary<string, string>();

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
    public void transUserMode() {
        print("transUserMode");
        signInPanel.SetActive(false);
        userSettingPanel.SetActive(true);
    }

    public void transPageMode(GameObject fromPanel, GameObject toPanel)
    {
        fromPanel.SetActive(false);
        toPanel.SetActive(true);
    }


    /** ���� ���� - 1�ܰ� START **/

    /** ���� ���� - 1�ܰ� END **/


    /** ���� ���� - 2�ܰ�  START **/
    /** ���� ���� - 2�ܰ� END **/


    /** ���� ���� - ��Ÿ ���� START **/
    /** ���� ���� - ��Ÿ ���� END **/
}
