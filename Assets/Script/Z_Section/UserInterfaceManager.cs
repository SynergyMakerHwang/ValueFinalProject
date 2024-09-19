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

        /** 공정 설정 페이지 첫페이지 이외에 비활성화 **/
        stepFirstPanel.SetActive(true);
        stepSecondPanel.SetActive(false);
        stepEtcPanel.SetActive(false);
        
    }

    //사용자 설정 페이지로 전환
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


    /** 공정 설정 - 1단계 START **/

    /** 공정 설정 - 1단계 END **/


    /** 공정 설정 - 2단계  START **/
    /** 공정 설정 - 2단계 END **/


    /** 공정 설정 - 기타 설정 START **/
    /** 공정 설정 - 기타 설정 END **/
}
