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
    [Header("����� ������ ���� ����")]
    TcpClient client;
    NetworkStream stream;
    [SerializeField] bool isConnected = false;
    [SerializeField] float waitTime = 0.1f;
    [SerializeField] int loadCnt = 10; // ��Ʈ ���� ����
    Process ps;
    int blockNum = 10; //������ ����
    int blockSize = 16; // ���� ����

    [Header("[B] ��ô ����")]
    [SerializeField] SubWeightSensor washerWeightSensor;
    [SerializeField] SubLocationSensor washerLocationSensor;


    [Header("[C] ��ǳ���� ����")]
    [SerializeField] Dryer dryerOpenSensor;


    [Header("[Z] AGV ����")]
    [SerializeField] AGVParkingSensor agvWasherParkingSensor;
    [SerializeField] AGVParkingSensor agvDryerParkingSensor;
    [SerializeField] AGVParkingSensor agvCuttingParkingSensor;
        



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

        //��Ʈ ���� ���� - PLC ����
        StartCoroutine(setDevice("@SETDevice,D0,"+ loadCnt));


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
        if (point[0][0] == 1)
        {
            SubConveyor.Instance.SpawnTottPLC();
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

        //(�߰�)��ô ���� - �κ��� ���� - get
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
            Dryer.Instance.DryerOpenClosePLC();
        }


        //��ǳ���� ���� - ���� Ŭ���� �߻� (Y51)
        if (point[5][1] == 1)
        {
            Dryer.Instance.DryerOpenClosePLC();
        }

        // ��ǳ���� ���� 
        if (point[5][2] == 1)
        {
            Dryer.Instance.RunDryerPLC();
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


        return requestMsg;
    }


    /***********************C-Section END *****************************/

    /***********************D-Section START *****************************/
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
        print("dl;fjal;ksdjflskjd("+ agvParkingSensor + ")");

        //���ܰ��� - AGV ��������  (X40)
        agvParkingSensor = (agvCuttingParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X40," + agvParkingSensor;


        //�������� - AGV ��������  (X50)
        agvParkingSensor = (agvDryerParkingSensor.isAgvParking == true) ? 1 : 0;
        requestMsg += "@SETDevice,X50," + agvParkingSensor;


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


                    int[][] point = null;                    
                    if (msg != null && msg != "")
                    {
                        point = TransTCPtoDeviceBlock(msg);
                        // ******************** GET *************************
                        //(B)��ô����
                        excuteWasherProcess(point);
                        
                        //(C)��ǳ��������
                        //excuteDryerProcess(point);
                    }


                    //��ǳ���� ����
                    
                    if (point != null)
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



                        print(reWrite);
                        
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
        client.Close();
        stream.Close();
        ps.Close();
        isConnected = false;
    }

    /***********************Z-Section END *****************************/
}


