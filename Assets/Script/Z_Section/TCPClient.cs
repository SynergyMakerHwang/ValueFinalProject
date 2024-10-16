using System.IO;
using System.Net.Sockets;
using System;
using UnityEngine;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;
using static UnityEngine.InputSystem.Controls.AxisControl;
using UnityEditor.Hardware;
using UnityEngine.UI;

public class TCPClient : MonoBehaviour
{
    [Header("연결과 데이터 전송 관련")]
    TcpClient client;
    NetworkStream stream;
    [SerializeField] bool isConnected = false;
    [SerializeField] float waitTime = 0.1f;
    [SerializeField] int loadCnt = 1; // 도트 적재 수량
    Process ps;
    int blockNum = 10; //블럭들의 수량
    int blockSize = 16; // 블럭의 수량

    [Header("[B] 세척 공정")]
    [SerializeField] SubWeightSensor washerWeightSensor;
    [SerializeField] SubLocationSensor washerLocationSensor;
    bool washer_isHaveTott = false; //토트 발생
    int washer_TottIndex = 0; //토트작업순서
    


    [Header("[C] 열풍건조 공정")]
    [SerializeField] Dryer dryerOpenSensor;
    int dryer_TottIndex = 0; //토트작업순서


    [Header("[C] 절단건조 공정")]
    int cutting_TottIndex = 0; //토트작업순서

    [Header("[D] 포장 공정")]
    int packing_TottIndex = 0; //토트작업순서


    [Header("[E] 적재 공정")]


    [Header("[Z] AGV 관련")]
    [SerializeField] AGVParkingSensor agvWasherParkingSensor;
    [SerializeField] AGVParkingSensor agvDryerParkingSensor;
    [SerializeField] AGVParkingSensor agvCuttingParkingSensor;
    [SerializeField] AGVParkingSensor agvCuttingLoadingParkingSensor;
    [SerializeField] AGVParkingSensor agvPackingParkingSensor;
    [SerializeField] AGVParkingSensor agvunLoadingParkingSensor;

    int currentTottIndex = 0;


    //GET Param
    string requestGetBlock = "@GET,Y0,10";


    //SET Param block
    //(예시) @SET,Y0,1

    //SET Param Device
    //(예시) @SETDevice,D0,1


    private void Awake()
    {
        //TCP Server 연결        
        string fullPath = Path.GetFullPath("03_TCP")+ "\\TCPServer\\TCPServer\\bin\\Debug\\net8.0";       
        ps = new Process();
        ps.StartInfo = new ProcessStartInfo("TCPServer.exe");
        ps.StartInfo.WorkingDirectory = fullPath;
        ps.StartInfo.CreateNoWindow = true;
       
        ps.Start(); 
    }



    private void Start()
    {
        // 로컬호스트: 로컬 컴퓨터의 디폴트 IP
        try
        {
            client = new TcpClient("127.0.0.1", 7000);

            stream = client.GetStream();
            
            //MX Componet 연결 및 연동 요청
            requestConnect();
        }
        catch (Exception e)
        {
            print(e.ToString());
            print("서버를 먼저 켜주세요.");
        }
    }

    public void OnDisConnectTCPSever() {

        Request("Disconnect");
        Request("quit");
    }


    // 제어판 - 실행 이벤트
    public void OnProcessStartBtnClk() {



        //PLC 설정 ( 도트 수량 & PLC 전원 ON)
        StartCoroutine(setDevice("@SETDevice,D0," + loadCnt + "@SETDevice,X0,1"));

        //로봇팔 동작 수량
        AGV_RobotArmController.instance.TottCnt = loadCnt;

        UserInterfaceManager.instance.transUserMonitoringMode();
        //공정 시작
        StartCoroutine(AGVManager.Instance.moveProcessStartPostion());
        
        //전체 공정 PLC <-> Unity 연동
        StartCoroutine(ScanPlc());
    }





    /***********************A-Section START *****************************/


    /***********************A-Section END *****************************/


    /***********************B-Section START *****************************/

    //세척 공정
    private void excuteWasherProcess(int[][] point)
    {
        //세척공정 - 도트 발생 (Y0)
        if (point[0][0] == 1 && !washer_isHaveTott)
        {   
            SubConveyor.Instance.SpawnTottPLC();
            washer_isHaveTott = true;

        }

        //세척 공정 - 펌핑모터 (Y30)
        if (point[3][0] == 1)
        {
            StartCoroutine(MainConveyor.instance.WaterFlowPLC());
        }
               

        //세척 공정 - subConvayor (Y31)
        if (point[3][1] == 1)
        {
            SubConveyor.Instance.SubConveyorOnPLC();
        }
        else
        {
            SubConveyor.Instance.SubConveyorOffPLC();
        }

        //세척 공정 - mainConvayor(Y32)
        if (point[3][2] == 1)
        {
            MainConveyor.instance.MainConveyorOnPLC();
        }
        else {
            MainConveyor.instance.MainConveyorOffPLC();
        }

        //세척 공정 - 로봇팔 동작 - get
        if (point[3][3] == 1 && washer_TottIndex < loadCnt)
        {
            washer_TottIndex++;
            //하역 동작            
            AGV_RobotArmController.instance.excuteCycleEvent("washer_loading", washer_TottIndex);
           
        }

        //세척 공정 완료 ( Y34)
        if (point[3][4] == 1)
        {
            StartCoroutine(AGVManager.Instance.moveProcessEndPostion("30"));
            washer_TottIndex = 0;
            //모니터링 - 세척 공정 완료            
            UserInterfaceManager.instance.btnOnChangeColorText("30", "Done");
        }

    }

    //세척 공정
    private string requestWasherProcess()
    {

        string requestMsg = "";

      
        //세척공정 - 도트 정위치센서 (X31)
        int tottSensor = (washerLocationSensor.RightLocationSensorPLC == true) ? 1 : 0;       
        requestMsg += "@SETDevice,X31," + tottSensor;
      

        //세척공정 - 무게센서  (X32)
        int weightSensor = (washerWeightSensor.WeightSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X32," + weightSensor;

        //(추가)세척공정 - 하역 로봇팔 동작 완료 sensor (X33)
        int robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X33," + robotACTEndSensor;        

        if (robotACTEndSensor == 1)
        {
            washer_isHaveTott = false;
        }
        

        return requestMsg;
    }





    /***********************B-Section END *****************************/



    /***********************C-Section START *****************************/

    //열풍건조 공정 - GET
    private void excuteDryerProcess(int[][] point)
    {
        //열풍건조 공정 - 도어 오픈 발생 (Y50)
        if (point[5][0] == 1)
        {

            Dryer.Instance.DryerOpenPLC();
        }

        // 열풍건조 공정 - 상역 AGV+로봇팔 (Y51)        
        if (point[5][1] == 1 && dryer_TottIndex < loadCnt)
        {
            dryer_TottIndex++;
            //상역 동작            
            AGV_RobotArmController.instance.excuteCycleEvent("dryer_unLoading", dryer_TottIndex);
            
        }        

        //열풍건조 공정 - 도어 클로즈 발생 (Y52)
        if (point[5][2] == 1)
        {
          Dryer.Instance.DryerClosePLC();
          dryer_TottIndex = 0;
        }

        // 열풍건조 공정 - 건조기 동작 (Y53)
        if (point[5][3] == 1)
        {
            Dryer.Instance.RunDryerPLC();
        }

        // 열풍건조 공정 - 하역 AGV+로봇팔 (Y54)
        if (point[5][4] == 1 && dryer_TottIndex < loadCnt)
        {
            dryer_TottIndex++;
            //하역 동작            
            AGV_RobotArmController.instance.excuteCycleEvent("dryer_loading", dryer_TottIndex);
            
        }
            

        // 열풍건조 공정  - 완료 (Y55)
        if (point[5][5] == 1)
        {
          
            dryer_TottIndex = 0;
            StartCoroutine(AGVManager.Instance.moveProcessEndPostion("50"));
          
            //모니터링 - 건조 공정 완료            
            UserInterfaceManager.instance.btnOnChangeColorText("50", "Done");
        }



    }

    //열풍건조 공정 - SET
    private string requestDryerProcess()
    {

        string requestMsg = "";


        //열풍건조 공정  - 도어 오픈 센서 (X51)
        int openSensor = (dryerOpenSensor.IsOpened == true) ? 1 : 0;
        requestMsg += "@SETDevice,X51," + openSensor;


        //열풍건조 공정  - 도어 클로즈 센서 (X53)
        openSensor = (dryerOpenSensor.IsOpened == false) ? 1 : 0;
        requestMsg += "@SETDevice,X53," + openSensor;



        //열풍건조 공정  - 로봇팔 상/하역 완료 센서 (X52)        
        int robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X52," + robotACTEndSensor;

        return requestMsg;
    }





    //절단건조 공정 - GET
    private void excuteCuttingProcess(int[][] point)
    {
        //절단건조 공정 - 메인 컨베이어 모터 발생 (Y40)
        if (point[4][0] == 1)
        {
            CutterConveyor.Instance.MaingGoPLC();
        }
        else {
            CutterConveyor.Instance.MainStopPLC();

        }


        //절단건조 공정 - 서브 컨베이어 모터 발생 (Y41)
        if (point[4][1] == 1)
        {
            CutterConveyor.Instance.SubGoPLC();
        }
        else {
            CutterConveyor.Instance.SubStopPLC();
        }


        // 절단건조 공정 - 상역 AGV+로봇팔 (Y42)        
        if (point[4][2] == 1 && cutting_TottIndex < loadCnt)
        {
            cutting_TottIndex++;
            //상역 동작            
            AGV_RobotArmController.instance.excuteCycleEvent("cutting_unLoading", cutting_TottIndex);

        }

        // 절단건조 공정 - 블레이드CYL(단동) (Y43)        
        if (point[4][3] == 1)        
        {
            CuttingMachine.Instance.BladeGoBackPLC();            
        }


        // 절단건조 공정 - 하역 공정으로 AGV이동 통신 (Y44)        
        if (point[4][4] == 1)
        {
            StartCoroutine(AGVManager.Instance.moveProcessEndPostion("41"));
            cutting_TottIndex = 0;
        }



        // 절단건조 공정 - 상역 AGV+로봇팔 (Y45)        
        if (point[4][5] == 1 && cutting_TottIndex < loadCnt)
        {
            cutting_TottIndex++;
            //상역 동작            
            AGV_RobotArmController.instance.excuteCycleEvent("cutting_loading", cutting_TottIndex);

        }




        // 절단건조 공정  - 완료 (Y46)
        if (point[4][6] == 1)
        {
            StartCoroutine(AGVManager.Instance.moveProcessEndPostion("40"));
            dryer_TottIndex = 0;
            //모니터링 - 절단 공정 완료            
            UserInterfaceManager.instance.btnOnChangeColorText("40", "Done");
        }



    }

    //절단 공정 - SET
    private string requestCuttingProcess()
    {

        string requestMsg = "";


        //절단공정  - 도트감지 센서 (X41)
        int sensor = (TottSensor.Instance.IsTottSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X41," + sensor;


        //절단공정  - 로봇팔 상/하역 완료 센서 (X42)        
        int robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X42," + robotACTEndSensor;



        //절단공정  - 전진 블레이드 LS (X43)
        sensor = (CuttingMachine.Instance.BladeLsPLC == false) ? 1 : 0;
        requestMsg += "@SETDevice,X43," + sensor;


        //절단공정  - 후진 블레이드 LS (X44)
        sensor = (CuttingMachine.Instance.BladeLsPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X44," + sensor;


        //절단공정  -  AGV 하역 위치 도착 (X45)
        sensor = (agvCuttingLoadingParkingSensor.isAgvParking == true) ? 1 : 0;        
        requestMsg += "@SETDevice,X45," + sensor;


        //절단공정  - 토트 정위치 센서 (X46)
        sensor = (CuttingMachine.Instance.RightTottSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X46," + sensor;


        //절단공정  - 로봇팔 상/하역 완료 센서 (X47)        
        robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X47," + robotACTEndSensor;

        return requestMsg;
    }


    /***********************C-Section END *****************************/

    /***********************D-Section START *****************************/


    //포장건조 공정 - GET
    private void excutePackingProcess(int[][] point)
    {
        //포장 공정 - 상역 컨베이어 모터 (Y70)
        if (point[7][0] == 1)
        {
            D1.instance.DConveyorOnPLC();
        }

        //포장 공정 - 카튼 발생(Y1)
        if (point[0][1] == 1)
        {
            D2.instance.SpawnBoxPLC();            
        }


        // 포장 공정 - 상역 AGV+로봇팔 (Y71)        
        if (point[7][1] == 1 && packing_TottIndex < loadCnt)
        {
            packing_TottIndex++;
            //상역 동작            
            AGV_RobotArmController.instance.excuteCycleEvent("packing_unLoading", packing_TottIndex);

        }


        //포장 공정 - 제함 로봇팔 동작 통신 (Y72)
        if (point[7][2] == 1)
        {
            D2.instance.RobotGoPLC();
        }


        //포장 공정 - 상부 테이핑 컨베이어 모터1 (Y73)
        if (point[7][3] == 1)
        {
            D2.instance.WrapConveyorOnPLC();
        }


        //포장 공정 - 상부 테이핑 컨베이어 모터2 (Y74)
        if (point[7][4] == 1)
        {
            D2.instance.WrapConeyorOffPLC();
        }


        //포장 공정 - Hitting 모터(정) (Y75)
        if (point[7][5] == 1)
        {
            D2.instance.HittingGoPLC();
        }


        // 포장 공정  - 완료 (Y76)
        if (point[7][6] == 1)
        {
            D2.instance.HittingBackPLC();
        }
    

    }




    //포장 공정 - SET
    private string requestPackingProcess()
    {

        string requestMsg = "";


        //포장 공정  - 카튼 발생 감지 센서 (X71)
        int sensor = (RightBoxSensor.instacne.RightBoxSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X71," + sensor;


        //포장 공정  - 토트 감지 센서(도라에몽박스) (X72)        
        sensor = (도라에몽상자.instance.IsTotePLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X72," + sensor;



        //포장 공정  - 상역 로봇팔 동작완료 sensor(X73)   
        int robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X73," + robotACTEndSensor;



        //포장 공정  - 카튼 인케이싱 정위치 센서 (X74)
        sensor = (DLocationSensor.Instance.DRightSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X74," + sensor;


        //포장 공정  -  AGV 하역 위치 도착 (X75)
        sensor = (DWeightSensor.instance.DWeightSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X75," + sensor;


        return requestMsg;
    }




    /***********************D-Section END *****************************/

    /***********************E-Section START *****************************/
    /***********************E-Section END *****************************/




    /***********************Z-Section START *****************************/

    //AGV 도착센서
    private string requestAgvParkingSensor()
    {

        string requestMsg = "";

        //세척공정 - AGV 도착센서  (X30)
        int agvParkingSensor = (agvWasherParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X30," + agvParkingSensor;
        if (agvParkingSensor == 1) {
            currentTottIndex = 0;
        }
        //모티터링 - 세척 공정 시작
        if (agvParkingSensor == 1 && AGV_RobotArmController.instance.IsProcessCycleEndAction == false) { 
            UserInterfaceManager.instance.btnOnChangeColorText("30", "ON");
        }



        //절단공정 - AGV 도착센서  (X40)
        agvParkingSensor = (agvCuttingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X40," + agvParkingSensor;    //절단공정 - AGV 도착센서  (X40)
                                                               //모티터링 - 절단 공정 시작
        if (agvParkingSensor == 1)
        {
            UserInterfaceManager.instance.btnOnChangeColorText("40", "ON");
        }

        //절단공정 - AGV 하역 도착센서  (X45) 
        agvParkingSensor = (agvCuttingLoadingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X45," + agvParkingSensor; //절단공정 - AGV 로딩 도착센서  (X45)

       




        //건조공정 - AGV 도착센서  (X50)
        agvParkingSensor = (agvDryerParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X50," + agvParkingSensor;
        //모티터링 - 세척 공정 시작
        if (agvParkingSensor == 1)
        {   
            UserInterfaceManager.instance.btnOnChangeColorText("50", "ON");
        }


        //포장공정 - AGV 도착센서  (X70)
        agvParkingSensor = (agvPackingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X70," + agvParkingSensor;
        //모티터링 - 포장 공정 시작
        if (agvParkingSensor == 1)
        {
            UserInterfaceManager.instance.btnOnChangeColorText("70", "ON");
        }
        
        //적재공정 - AGV 도착센서  (X80)
        agvParkingSensor = (agvunLoadingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X80," + agvParkingSensor;
        //모티터링 - 세척 공정 시작
        if (agvParkingSensor == 1)
        {
            UserInterfaceManager.instance.btnOnChangeColorText("80", "ON");
        }






        return requestMsg;


      

}

/***********************Z-Section END *****************************/


/*****************TCP 통신 관련 *********************/
//mx component 연결 요청 
public void requestConnect()
    {
        if (!isConnected && client != null && stream != null)
        {
            string retrunMsg = "";

            retrunMsg = Request("Connect");

            if (retrunMsg.Contains("성공"))
            {
                isConnected = true;               

            }
        }
        else if (client == null && stream == null)
        {

            client = new TcpClient("127.0.0.1", 7000);

            stream = client.GetStream();
        }
        else
        {
            print("이미 연결된 상태입니다.");
        }
    }

       

    //PLC 데이터 블럭단위로 스캔 및 제어 하기

    IEnumerator ScanPlc()
    {
        print("ScanPlc start ");
        yield return new WaitUntil(() => isConnected);
        if (isConnected)
        {
            Task task = RequestScanAsync(requestGetBlock);

            yield return new WaitForSeconds(waitTime);
        }
        else {
            print("연결 해제된 상태입니다.");
        }

    }


    IEnumerator setDevice(string requsetMsg)
    {

        yield return new WaitUntil(() => isConnected);
        if (isConnected)
        {
            Task task = RequestSetDeviceAsynOnce(requsetMsg);

            yield return new WaitForSeconds(waitTime);
        }
        else
        {
            print("연결 해제된 상태입니다.");
        }

    }



    private async Task RequestScanAsync(string requestMsg)
    {
        string returnMsg = "";
        
        if (requestMsg == "")
        {
            returnMsg = "서버 요청값을 입력해주세요.";
            print(returnMsg);
        }
        else
        {
            while (true)
            {
                try
                {

                    // Connect -> data
                    byte[] buffer = Encoding.UTF8.GetBytes(requestMsg);


                    // NetworkStream에 데이터 쓰기
                    await stream.WriteAsync(buffer, 0, buffer.Length);

                    // 데이터 수신(i.g GET,Y0,5)
                    byte[] buffer2 = new byte[1024];
                    int nBytes = await stream.ReadAsync(buffer2, 0, buffer2.Length);
                    string msg = Encoding.UTF8.GetString(buffer2, 0, nBytes);



                    string[] responseArr = null;
                    if (msg != null && msg != "")
                    {
                        msg = msg.Trim();
                        if (msg.IndexOf("@") == 0)
                        {
                            msg = msg.Substring(1);
                        }

                        responseArr = msg.Split("@");
                        //string[] response
                        foreach (string str in responseArr)
                        {
                            int[][] point = null;
                            point = TransTCPtoDeviceBlock(str);

                            // ******************** GET *************************
                            //(B)세척공정
                            excuteWasherProcess(point);

                            //(C)열풍건조공정
                            excuteDryerProcess(point);

                            //(C)절단건조공정
                            excuteCuttingProcess(point);

                            //(D)포장공정
                           excutePackingProcess(point);
                         
                            

                        }
                       
                    }


                    //열풍건조 공정
                    
                    if (responseArr.Length>0)
                    {

                        // ******************** SET *************************
                        string reWrite = "";
                        //reWrite = WriteTCPDeviceBlock(msg);

                        //(Z)AGV - 공정별 도착센서
                        reWrite += requestAgvParkingSensor();

                        //(B)세척공정 
                        reWrite += requestWasherProcess();

                        //(C)열풍공정 
                        reWrite += requestDryerProcess();

                        //(C)절단공정 
                        reWrite += requestCuttingProcess();

                        //(D)포장공정 
                        reWrite += requestPackingProcess();



                        //print(reWrite);

                        if (reWrite != "")
                        {
                            
                            buffer = new byte[1024];
                            buffer = Encoding.UTF8.GetBytes(reWrite);
                            // NetworkStream에 데이터 쓰기
                            await stream.WriteAsync(buffer, 0, buffer.Length);
                            buffer = new byte[1024];

                        }


                    }


                    
                    if (!isConnected) break;
                }
                catch (Exception e)
                {
                    print(e.ToString());
                }
            }
        }

    }


    private async Task RequestSetDeviceAsynOnce(string requestMsg)
    {
        string returnMsg = "";

        if (requestMsg == "")
        {
            returnMsg = "서버 요청값을 입력해주세요.";
            print(returnMsg);
        }
        else
        {   try
                {

                    // Connect -> data
                    byte[] buffer = Encoding.UTF8.GetBytes(requestMsg);


                    // NetworkStream에 데이터 쓰기
                    await stream.WriteAsync(buffer, 0, buffer.Length);

                    byte[] buffer2 = new byte[1024];
                    int nBytes = await stream.ReadAsync(buffer2, 0, buffer2.Length);
                    string resultMsg = Encoding.UTF8.GetString(buffer2, 0, nBytes);
                    print(resultMsg);

                   
                }
                catch (Exception e)
                {
                    print(e.ToString());
                }
           
        }

    }



    private int[][] TransTCPtoDeviceBlock(string returnData)
    {
        
        returnData = returnData.Substring(returnData.IndexOf(",") + 1);        
        string[] returnArr = returnData.Split(",");

        if (returnArr.Length > 0)
        {
            int[][] information = new int[returnArr.Length][];
            int i = 0;
            foreach (var value in returnArr)
            {
                string binary = Convert.ToString(int.Parse(value), 2);
                information[i] = ConverStringToIntArray(binary, blockSize);
                i++;
            }

            return information;
        }
        else
        {
            return null;
        }

    }


    private int[] ConverStringToIntArray(string binary, int blockSize)
    {
        //실습1 , string  - > int형 배열        
        //1.Reverse
        string newStr = new string(binary.Reverse().ToArray());

        //2.0추가 
        for (int i = 0; i < blockSize - binary.Length; i++)
        {
            newStr += "0";
        }

        //3.Stirng -> int형 배열  
        char[] strings = newStr.ToCharArray();

        int[] devicePoints = Array.ConvertAll(strings, c => (int)Char.GetNumericValue(c));

        return devicePoints;
    }



    public string WriteTCPDeviceBlock(string responeMsg)
    {
        string result = "";

        try
        {

            string[] dataFrom = responeMsg.Split(',');

            string deviceName = dataFrom[0];

            int[] data = new int[blockNum];
            data[0] = int.Parse(dataFrom[1]);
            

            /*** SET 값 설정 (**GET과 같이 쓰여지기 때문에 "@"추가함)***/
            result = "@SET," + deviceName;
            foreach (var item in data)
            {
                result += "," + item;
            }


        }
        catch (Exception e)
        {
            print(e.ToString());
            result = "";

        }


        return result;



    }

    private string Request(string order)
    {
        string result = "";
        try
        {
            byte[] buffer = Encoding.UTF8.GetBytes(order);

            // NetworkStream에 데이터 쓰기
            stream.Write(buffer, 0, buffer.Length);

            // 데이터 수신(i.g GET,Y0,5)
            byte[] buffer2 = new byte[1024];
            int nBytes = stream.Read(buffer2, 0, buffer2.Length);
            string msg = Encoding.UTF8.GetString(buffer2, 0, nBytes);
            print(msg);
            result = msg;
        }
        catch (Exception e)
        {
            print(e.ToString());
        }

        return result;
    }

    private void OnDestroy()
    {
        OnDisConnectTCPSever();
        client.Close();
        stream.Close();
        ps.Close();
        isConnected = false;
        
    }

    /***********************Z-Section END *****************************/
}


