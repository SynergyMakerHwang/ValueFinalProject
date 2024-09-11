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

public class TCPClient : MonoBehaviour
{
    [Header("����� ������ ���� ����")]
    TcpClient client;
    NetworkStream stream;
    [SerializeField] bool isConnected = false;
    [SerializeField] float waitTime = 0.1f;
    Process ps;
    int blockNum = 16;

    //GET Param
    string requestGetBlock = "@GET,X0,10";

    //SET Param


    /****������ ���� ����****/
    


    private void Awake()
    {
        //TCP Server ����        
        string fullPath = Path.GetFullPath("03_TCP")+ "\\TCP_Server";        
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


    
    /***********************A-Section START *****************************/
    /***********************A-Section END *****************************/

    /***********************B-Section START *****************************/
    /***********************B-Section END *****************************/



    /***********************C-Section START *****************************/
    /***********************C-Section END *****************************/

    /***********************D-Section START *****************************/
    /***********************D-Section END *****************************/

    /***********************E-Section START *****************************/
    /***********************E-Section END *****************************/

   
    
    
    /***********************Z-Section START *****************************/ 

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
                communicateEvent();

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



    public void communicateEvent()
    {
        if (isConnected)
        {
            StartCoroutine(ScanPlc());
        }
        else
        {
            print("���� ������ �����Դϴ�.");
        }
    }

    //PLC ������ �������� ��ĵ�ؿ���

    IEnumerator ScanPlc()
    {

        yield return new WaitUntil(() => isConnected);
        if (isConnected)
        {
            Task task = RequestScanAsync(requestGetBlock);

            yield return new WaitForSeconds(waitTime);
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
                    }


                    if (point != null)
                    {
                       
                        // SET
                        string reWrite = "";
                        reWrite = WriteTCPDeviceBlock(msg);
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




    private int[][] TransTCPtoDeviceBlock(string returnData)
    {
        int blockSize = 16;



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


