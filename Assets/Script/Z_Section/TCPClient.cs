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

public class TCPClient : MonoBehaviour
{
    [Header("연결과 데이터 전송 관련")]
    TcpClient client;
    NetworkStream stream;
    [SerializeField] bool isConnected = false;
    [SerializeField] float waitTime = 0.1f;
    [SerializeField] int loadCnt = 10; // 도트 적재 수량
    Process ps;
    int blockNum = 10; //블럭들의 수량
    int blockSize = 16; // 블럭의 수량

    [Header("[B] 세척 공정")]
    [SerializeField] SubWeightSensor washerWeightSensor;
    [SerializeField] SubLocationSensor washerLocationSensor;


    [Header("[C] 열풍건조 공정")]
    [SerializeField] Dryer dryerOpenSensor;


    [Header("[Z] AGV 관련")]
    [SerializeField] AGVParkingSensor agvWasherParkingSensor;
    [SerializeField] AGVParkingSensor agvDryerParkingSensor;
    [SerializeField] AGVParkingSensor agvCuttingParkingSensor;
        



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

        //도트 적재 수량 - PLC 설정
        StartCoroutine(setDevice("@SETDevice,D0,"+ loadCnt));


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
        if (point[0][0] == 1)
        {
            SubConveyor.Instance.SpawnTottPLC();
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

        //(추가)세척 공정 - 로봇팔 동작 - get
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
            Dryer.Instance.DryerOpenClosePLC();
        }


        //열풍건조 공정 - 도어 클로즈 발생 (Y51)
        if (point[5][1] == 1)
        {
            Dryer.Instance.DryerOpenClosePLC();
        }

        // 열풍건조 공정 
        if (point[5][2] == 1)
        {
            Dryer.Instance.RunDryerPLC();
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


        return requestMsg;
    }


    /***********************C-Section END *****************************/

    /***********************D-Section START *****************************/
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
        print("dl;fjal;ksdjflskjd("+ agvParkingSensor + ")");

        //절단공정 - AGV 도착센서  (X40)
        agvParkingSensor = (agvCuttingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X40," + agvParkingSensor;


        //건조공정 - AGV 도착센서  (X50)
        agvParkingSensor = (agvDryerParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X50," + agvParkingSensor;


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


                    int[][] point = null;                    
                    if (msg != null && msg != "")
                    {
                        point = TransTCPtoDeviceBlock(msg);
                        // ******************** GET *************************
                        //(B)세척공정
                        excuteWasherProcess(point);
                        
                        //(C)열풍건조공정
                        //excuteDryerProcess(point);
                    }


                    //열풍건조 공정
                    
                    if (point != null)
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



                        print(reWrite);
                        
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
        client.Close();
        stream.Close();
        ps.Close();
        isConnected = false;
    }

    /***********************Z-Section END *****************************/
}


