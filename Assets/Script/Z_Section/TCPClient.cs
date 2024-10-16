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
    [Header("����� ������ ���� ����")]
    TcpClient client;
    NetworkStream stream;
    [SerializeField] bool isConnected = false;
    [SerializeField] float waitTime = 0.1f;
    [SerializeField] int loadCnt = 1; // ��Ʈ ���� ����
    Process ps;
    int blockNum = 10; //������ ����
    int blockSize = 16; // ���� ����

    [Header("[B] ��ô ����")]
    [SerializeField] SubWeightSensor washerWeightSensor;
    [SerializeField] SubLocationSensor washerLocationSensor;
    bool washer_isHaveTott = false; //��Ʈ �߻�
    int washer_TottIndex = 0; //��Ʈ�۾�����
    


    [Header("[C] ��ǳ���� ����")]
    [SerializeField] Dryer dryerOpenSensor;
    int dryer_TottIndex = 0; //��Ʈ�۾�����


    [Header("[C] ���ܰ��� ����")]
    int cutting_TottIndex = 0; //��Ʈ�۾�����

    [Header("[D] ���� ����")]
    int packing_TottIndex = 0; //��Ʈ�۾�����


    [Header("[E] ���� ����")]


    [Header("[Z] AGV ����")]
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
    //(����) @SET,Y0,1

    //SET Param Device
    //(����) @SETDevice,D0,1


    private void Awake()
    {
        //TCP Server ����        
        string fullPath = Path.GetFullPath("03_TCP")+ "\\TCPServer\\TCPServer\\bin\\Debug\\net8.0";       
        ps = new Process();
        ps.StartInfo = new ProcessStartInfo("TCPServer.exe");
        ps.StartInfo.WorkingDirectory = fullPath;
        ps.StartInfo.CreateNoWindow = true;
       
        ps.Start(); 
    }



    private void Start()
    {
        // ����ȣ��Ʈ: ���� ��ǻ���� ����Ʈ IP
        try
        {
            client = new TcpClient("127.0.0.1", 7000);

            stream = client.GetStream();
            
            //MX Componet ���� �� ���� ��û
            requestConnect();
        }
        catch (Exception e)
        {
            print(e.ToString());
            print("������ ���� ���ּ���.");
        }
    }

    public void OnDisConnectTCPSever() {

        Request("Disconnect");
        Request("quit");
    }


    // ������ - ���� �̺�Ʈ
    public void OnProcessStartBtnClk() {



        //PLC ���� ( ��Ʈ ���� & PLC ���� ON)
        StartCoroutine(setDevice("@SETDevice,D0," + loadCnt + "@SETDevice,X0,1"));

        //�κ��� ���� ����
        AGV_RobotArmController.instance.TottCnt = loadCnt;

        UserInterfaceManager.instance.transUserMonitoringMode();
        //���� ����
        StartCoroutine(AGVManager.Instance.moveProcessStartPostion());
        
        //��ü ���� PLC <-> Unity ����
        StartCoroutine(ScanPlc());
    }





    /***********************A-Section START *****************************/


    /***********************A-Section END *****************************/


    /***********************B-Section START *****************************/

    //��ô ����
    private void excuteWasherProcess(int[][] point)
    {
        //��ô���� - ��Ʈ �߻� (Y0)
        if (point[0][0] == 1 && !washer_isHaveTott)
        {   
            SubConveyor.Instance.SpawnTottPLC();
            washer_isHaveTott = true;

        }

        //��ô ���� - ���θ��� (Y30)
        if (point[3][0] == 1)
        {
            StartCoroutine(MainConveyor.instance.WaterFlowPLC());
        }
               

        //��ô ���� - subConvayor (Y31)
        if (point[3][1] == 1)
        {
            SubConveyor.Instance.SubConveyorOnPLC();
        }
        else
        {
            SubConveyor.Instance.SubConveyorOffPLC();
        }

        //��ô ���� - mainConvayor(Y32)
        if (point[3][2] == 1)
        {
            MainConveyor.instance.MainConveyorOnPLC();
        }
        else {
            MainConveyor.instance.MainConveyorOffPLC();
        }

        //��ô ���� - �κ��� ���� - get
        if (point[3][3] == 1 && washer_TottIndex < loadCnt)
        {
            washer_TottIndex++;
            //�Ͽ� ����            
            AGV_RobotArmController.instance.excuteCycleEvent("washer_loading", washer_TottIndex);
           
        }

        //��ô ���� �Ϸ� ( Y34)
        if (point[3][4] == 1)
        {
            StartCoroutine(AGVManager.Instance.moveProcessEndPostion("30"));
            washer_TottIndex = 0;
            //����͸� - ��ô ���� �Ϸ�            
            UserInterfaceManager.instance.btnOnChangeColorText("30", "Done");
        }

    }

    //��ô ����
    private string requestWasherProcess()
    {

        string requestMsg = "";

      
        //��ô���� - ��Ʈ ����ġ���� (X31)
        int tottSensor = (washerLocationSensor.RightLocationSensorPLC == true) ? 1 : 0;       
        requestMsg += "@SETDevice,X31," + tottSensor;
      

        //��ô���� - ���Լ���  (X32)
        int weightSensor = (washerWeightSensor.WeightSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X32," + weightSensor;

        //(�߰�)��ô���� - �Ͽ� �κ��� ���� �Ϸ� sensor (X33)
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

    //��ǳ���� ���� - GET
    private void excuteDryerProcess(int[][] point)
    {
        //��ǳ���� ���� - ���� ���� �߻� (Y50)
        if (point[5][0] == 1)
        {

            Dryer.Instance.DryerOpenPLC();
        }

        // ��ǳ���� ���� - �� AGV+�κ��� (Y51)        
        if (point[5][1] == 1 && dryer_TottIndex < loadCnt)
        {
            dryer_TottIndex++;
            //�� ����            
            AGV_RobotArmController.instance.excuteCycleEvent("dryer_unLoading", dryer_TottIndex);
            
        }        

        //��ǳ���� ���� - ���� Ŭ���� �߻� (Y52)
        if (point[5][2] == 1)
        {
          Dryer.Instance.DryerClosePLC();
          dryer_TottIndex = 0;
        }

        // ��ǳ���� ���� - ������ ���� (Y53)
        if (point[5][3] == 1)
        {
            Dryer.Instance.RunDryerPLC();
        }

        // ��ǳ���� ���� - �Ͽ� AGV+�κ��� (Y54)
        if (point[5][4] == 1 && dryer_TottIndex < loadCnt)
        {
            dryer_TottIndex++;
            //�Ͽ� ����            
            AGV_RobotArmController.instance.excuteCycleEvent("dryer_loading", dryer_TottIndex);
            
        }
            

        // ��ǳ���� ����  - �Ϸ� (Y55)
        if (point[5][5] == 1)
        {
          
            dryer_TottIndex = 0;
            StartCoroutine(AGVManager.Instance.moveProcessEndPostion("50"));
          
            //����͸� - ���� ���� �Ϸ�            
            UserInterfaceManager.instance.btnOnChangeColorText("50", "Done");
        }



    }

    //��ǳ���� ���� - SET
    private string requestDryerProcess()
    {

        string requestMsg = "";


        //��ǳ���� ����  - ���� ���� ���� (X51)
        int openSensor = (dryerOpenSensor.IsOpened == true) ? 1 : 0;
        requestMsg += "@SETDevice,X51," + openSensor;


        //��ǳ���� ����  - ���� Ŭ���� ���� (X53)
        openSensor = (dryerOpenSensor.IsOpened == false) ? 1 : 0;
        requestMsg += "@SETDevice,X53," + openSensor;



        //��ǳ���� ����  - �κ��� ��/�Ͽ� �Ϸ� ���� (X52)        
        int robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X52," + robotACTEndSensor;

        return requestMsg;
    }





    //���ܰ��� ���� - GET
    private void excuteCuttingProcess(int[][] point)
    {
        //���ܰ��� ���� - ���� �����̾� ���� �߻� (Y40)
        if (point[4][0] == 1)
        {
            CutterConveyor.Instance.MaingGoPLC();
        }
        else {
            CutterConveyor.Instance.MainStopPLC();

        }


        //���ܰ��� ���� - ���� �����̾� ���� �߻� (Y41)
        if (point[4][1] == 1)
        {
            CutterConveyor.Instance.SubGoPLC();
        }
        else {
            CutterConveyor.Instance.SubStopPLC();
        }


        // ���ܰ��� ���� - �� AGV+�κ��� (Y42)        
        if (point[4][2] == 1 && cutting_TottIndex < loadCnt)
        {
            cutting_TottIndex++;
            //�� ����            
            AGV_RobotArmController.instance.excuteCycleEvent("cutting_unLoading", cutting_TottIndex);

        }

        // ���ܰ��� ���� - ���̵�CYL(�ܵ�) (Y43)        
        if (point[4][3] == 1)        
        {
            CuttingMachine.Instance.BladeGoBackPLC();            
        }


        // ���ܰ��� ���� - �Ͽ� �������� AGV�̵� ��� (Y44)        
        if (point[4][4] == 1)
        {
            StartCoroutine(AGVManager.Instance.moveProcessEndPostion("41"));
            cutting_TottIndex = 0;
        }



        // ���ܰ��� ���� - �� AGV+�κ��� (Y45)        
        if (point[4][5] == 1 && cutting_TottIndex < loadCnt)
        {
            cutting_TottIndex++;
            //�� ����            
            AGV_RobotArmController.instance.excuteCycleEvent("cutting_loading", cutting_TottIndex);

        }




        // ���ܰ��� ����  - �Ϸ� (Y46)
        if (point[4][6] == 1)
        {
            StartCoroutine(AGVManager.Instance.moveProcessEndPostion("40"));
            dryer_TottIndex = 0;
            //����͸� - ���� ���� �Ϸ�            
            UserInterfaceManager.instance.btnOnChangeColorText("40", "Done");
        }



    }

    //���� ���� - SET
    private string requestCuttingProcess()
    {

        string requestMsg = "";


        //���ܰ���  - ��Ʈ���� ���� (X41)
        int sensor = (TottSensor.Instance.IsTottSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X41," + sensor;


        //���ܰ���  - �κ��� ��/�Ͽ� �Ϸ� ���� (X42)        
        int robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X42," + robotACTEndSensor;



        //���ܰ���  - ���� ���̵� LS (X43)
        sensor = (CuttingMachine.Instance.BladeLsPLC == false) ? 1 : 0;
        requestMsg += "@SETDevice,X43," + sensor;


        //���ܰ���  - ���� ���̵� LS (X44)
        sensor = (CuttingMachine.Instance.BladeLsPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X44," + sensor;


        //���ܰ���  -  AGV �Ͽ� ��ġ ���� (X45)
        sensor = (agvCuttingLoadingParkingSensor.isAgvParking == true) ? 1 : 0;        
        requestMsg += "@SETDevice,X45," + sensor;


        //���ܰ���  - ��Ʈ ����ġ ���� (X46)
        sensor = (CuttingMachine.Instance.RightTottSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X46," + sensor;


        //���ܰ���  - �κ��� ��/�Ͽ� �Ϸ� ���� (X47)        
        robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X47," + robotACTEndSensor;

        return requestMsg;
    }


    /***********************C-Section END *****************************/

    /***********************D-Section START *****************************/


    //������� ���� - GET
    private void excutePackingProcess(int[][] point)
    {
        //���� ���� - �� �����̾� ���� (Y70)
        if (point[7][0] == 1)
        {
            D1.instance.DConveyorOnPLC();
        }

        //���� ���� - īư �߻�(Y1)
        if (point[0][1] == 1)
        {
            D2.instance.SpawnBoxPLC();            
        }


        // ���� ���� - �� AGV+�κ��� (Y71)        
        if (point[7][1] == 1 && packing_TottIndex < loadCnt)
        {
            packing_TottIndex++;
            //�� ����            
            AGV_RobotArmController.instance.excuteCycleEvent("packing_unLoading", packing_TottIndex);

        }


        //���� ���� - ���� �κ��� ���� ��� (Y72)
        if (point[7][2] == 1)
        {
            D2.instance.RobotGoPLC();
        }


        //���� ���� - ��� ������ �����̾� ����1 (Y73)
        if (point[7][3] == 1)
        {
            D2.instance.WrapConveyorOnPLC();
        }


        //���� ���� - ��� ������ �����̾� ����2 (Y74)
        if (point[7][4] == 1)
        {
            D2.instance.WrapConeyorOffPLC();
        }


        //���� ���� - Hitting ����(��) (Y75)
        if (point[7][5] == 1)
        {
            D2.instance.HittingGoPLC();
        }


        // ���� ����  - �Ϸ� (Y76)
        if (point[7][6] == 1)
        {
            D2.instance.HittingBackPLC();
        }
    

    }




    //���� ���� - SET
    private string requestPackingProcess()
    {

        string requestMsg = "";


        //���� ����  - īư �߻� ���� ���� (X71)
        int sensor = (RightBoxSensor.instacne.RightBoxSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X71," + sensor;


        //���� ����  - ��Ʈ ���� ����(���󿡸��ڽ�) (X72)        
        sensor = (���󿡸�����.instance.IsTotePLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X72," + sensor;



        //���� ����  - �� �κ��� ���ۿϷ� sensor(X73)   
        int robotACTEndSensor = (AGV_RobotArmController.instance.IsProcessCycleEndAction == true) ? 1 : 0;
        requestMsg += "@SETDevice,X73," + robotACTEndSensor;



        //���� ����  - īư �����̽� ����ġ ���� (X74)
        sensor = (DLocationSensor.Instance.DRightSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X74," + sensor;


        //���� ����  -  AGV �Ͽ� ��ġ ���� (X75)
        sensor = (DWeightSensor.instance.DWeightSensorPLC == true) ? 1 : 0;
        requestMsg += "@SETDevice,X75," + sensor;


        return requestMsg;
    }




    /***********************D-Section END *****************************/

    /***********************E-Section START *****************************/
    /***********************E-Section END *****************************/




    /***********************Z-Section START *****************************/

    //AGV ��������
    private string requestAgvParkingSensor()
    {

        string requestMsg = "";

        //��ô���� - AGV ��������  (X30)
        int agvParkingSensor = (agvWasherParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X30," + agvParkingSensor;
        if (agvParkingSensor == 1) {
            currentTottIndex = 0;
        }
        //��Ƽ�͸� - ��ô ���� ����
        if (agvParkingSensor == 1 && AGV_RobotArmController.instance.IsProcessCycleEndAction == false) { 
            UserInterfaceManager.instance.btnOnChangeColorText("30", "ON");
        }



        //���ܰ��� - AGV ��������  (X40)
        agvParkingSensor = (agvCuttingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X40," + agvParkingSensor;    //���ܰ��� - AGV ��������  (X40)
                                                               //��Ƽ�͸� - ���� ���� ����
        if (agvParkingSensor == 1)
        {
            UserInterfaceManager.instance.btnOnChangeColorText("40", "ON");
        }

        //���ܰ��� - AGV �Ͽ� ��������  (X45) 
        agvParkingSensor = (agvCuttingLoadingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X45," + agvParkingSensor; //���ܰ��� - AGV �ε� ��������  (X45)

       




        //�������� - AGV ��������  (X50)
        agvParkingSensor = (agvDryerParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X50," + agvParkingSensor;
        //��Ƽ�͸� - ��ô ���� ����
        if (agvParkingSensor == 1)
        {   
            UserInterfaceManager.instance.btnOnChangeColorText("50", "ON");
        }


        //������� - AGV ��������  (X70)
        agvParkingSensor = (agvPackingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X70," + agvParkingSensor;
        //��Ƽ�͸� - ���� ���� ����
        if (agvParkingSensor == 1)
        {
            UserInterfaceManager.instance.btnOnChangeColorText("70", "ON");
        }
        
        //������� - AGV ��������  (X80)
        agvParkingSensor = (agvunLoadingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X80," + agvParkingSensor;
        //��Ƽ�͸� - ��ô ���� ����
        if (agvParkingSensor == 1)
        {
            UserInterfaceManager.instance.btnOnChangeColorText("80", "ON");
        }






        return requestMsg;


      

}

/***********************Z-Section END *****************************/


/*****************TCP ��� ���� *********************/
//mx component ���� ��û 
public void requestConnect()
    {
        if (!isConnected && client != null && stream != null)
        {
            string retrunMsg = "";

            retrunMsg = Request("Connect");

            if (retrunMsg.Contains("����"))
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
            print("�̹� ����� �����Դϴ�.");
        }
    }

       

    //PLC ������ �������� ��ĵ �� ���� �ϱ�

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
            print("���� ������ �����Դϴ�.");
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
            print("���� ������ �����Դϴ�.");
        }

    }



    private async Task RequestScanAsync(string requestMsg)
    {
        string returnMsg = "";
        
        if (requestMsg == "")
        {
            returnMsg = "���� ��û���� �Է����ּ���.";
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


                    // NetworkStream�� ������ ����
                    await stream.WriteAsync(buffer, 0, buffer.Length);

                    // ������ ����(i.g GET,Y0,5)
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
                            //(B)��ô����
                            excuteWasherProcess(point);

                            //(C)��ǳ��������
                            excuteDryerProcess(point);

                            //(C)���ܰ�������
                            excuteCuttingProcess(point);

                            //(D)�������
                           excutePackingProcess(point);
                         
                            

                        }
                       
                    }


                    //��ǳ���� ����
                    
                    if (responseArr.Length>0)
                    {

                        // ******************** SET *************************
                        string reWrite = "";
                        //reWrite = WriteTCPDeviceBlock(msg);

                        //(Z)AGV - ������ ��������
                        reWrite += requestAgvParkingSensor();

                        //(B)��ô���� 
                        reWrite += requestWasherProcess();

                        //(C)��ǳ���� 
                        reWrite += requestDryerProcess();

                        //(C)���ܰ��� 
                        reWrite += requestCuttingProcess();

                        //(D)������� 
                        reWrite += requestPackingProcess();



                        //print(reWrite);

                        if (reWrite != "")
                        {
                            
                            buffer = new byte[1024];
                            buffer = Encoding.UTF8.GetBytes(reWrite);
                            // NetworkStream�� ������ ����
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
            returnMsg = "���� ��û���� �Է����ּ���.";
            print(returnMsg);
        }
        else
        {   try
                {

                    // Connect -> data
                    byte[] buffer = Encoding.UTF8.GetBytes(requestMsg);


                    // NetworkStream�� ������ ����
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
        //�ǽ�1 , string  - > int�� �迭        
        //1.Reverse
        string newStr = new string(binary.Reverse().ToArray());

        //2.0�߰� 
        for (int i = 0; i < blockSize - binary.Length; i++)
        {
            newStr += "0";
        }

        //3.Stirng -> int�� �迭  
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
            

            /*** SET �� ���� (**GET�� ���� �������� ������ "@"�߰���)***/
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

            // NetworkStream�� ������ ����
            stream.Write(buffer, 0, buffer.Length);

            // ������ ����(i.g GET,Y0,5)
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


