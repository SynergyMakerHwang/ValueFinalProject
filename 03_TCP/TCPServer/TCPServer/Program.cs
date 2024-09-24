using ActUtlType64Lib;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static TCPServer;
using static TCPServer.MxComponent;


class TCPServer
{


    // PLC - TCP Server - Unity
    // PLC로 데이터 송수신
    // 클라이언트(Unitiy)로 데이터 송수신


    //TCP Sercer : TCP Listener 객체 사용
    //TCP Client : TCP Client 객체 사용

    //@Response
    //data
    //MX Componet 객체 생성
    static int blockNum = 10;
    static int blockSize = 16;
    static MxComponent mxComponent = new MxComponent();
    public static void Main()
    {

        try
        {


            mxComponent.Init();

            //종료 이벤트
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            TcpListener listener = new TcpListener(IPAddress.Any, 7000); //TCP/IP port는 7000
            listener.Start();


            Console.WriteLine("TCP SEVER START");

            TcpClient client;
            NetworkStream stream;


            while (true)
            {
                //1.TCP Client의 요청을 받아들인다.
                client = listener.AcceptTcpClient();


                //2.TCP Client 객체에 NetwordStream을 받아온다.
                stream = client.GetStream();

                byte[] buffer = new byte[1024];

                int nByte;
                string returnMsg = "";
                //3. 데이터 수신
                while ((nByte = stream.Read(buffer, 0, buffer.Length)) > 0)
                {

                    //4.데이터 인코딩(Byte[] -> UTF8)
                    string responseMsg = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine("Respone :  " + responseMsg);
                    responseMsg = responseMsg.Trim();
                    if (responseMsg.IndexOf("@") == 0)
                    {
                        responseMsg = responseMsg.Substring(1);
                    }
                    Console.WriteLine("Respone :  " + responseMsg);
                    buffer = new byte[1024];

                    returnMsg = "";
                    if (responseMsg.Contains("Disconnect"))
                    {
                        returnMsg = mxComponent.DisConnect();

                    }
                    else if (responseMsg.Contains("Connect"))
                    {

                        returnMsg = mxComponent.Connect();

                    }
                    else if (responseMsg.Contains("SET") || responseMsg.Contains("GET"))
                    {
                        string[] responseArr = responseMsg.Split("@");
                        //string[] response
                        foreach (string str in responseArr)
                        {
                            if (str.Contains("SETDevice"))
                            {
                                mxComponent.setDevice(responseMsg);
                            }
                            else if (str.Contains("SET"))
                            {
                                //SET,Y0,1,2                         
                                mxComponent.WriteDeviceBlockTCPServer(str);

                                /**SET한 값의 블럭값 가져오기**/
                                str.Replace("SET", "GET");
                                string[] tmp_str = str.Split(",");
                                returnMsg = mxComponent.ReadDeviceBlockTCPServer(tmp_str[0] + "," + tmp_str[1] + "," + blockNum);
                            }
                            else if (str.Contains("GET"))
                            {
                                returnMsg = mxComponent.ReadDeviceBlockTCPServer(str);
                            }
                        }

                    }



                    //5. 데이터 송신

                    if (returnMsg != "")
                    {
                        buffer = Encoding.UTF8.GetBytes(returnMsg);
                    }
                    stream.Write(buffer, 0, buffer.Length);



                    if (responseMsg.Contains("quit"))
                    {
                        Console.WriteLine("서버를 종료합니다.");
                        break;
                    }

                    buffer = new byte[1024];

                }


            }
            stream.Close();
            client.Close();

        }
        catch (Exception e)
        {
            e.ToString();

        }

    }

    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        //throw new NotImplementedException();
        mxComponent.DisConnect();


    }


    //Unity MxComponet Method
    //- ActUtlType64
    // + enum status
    //+ Init()
    //+ Connect()
    //+ Disconnect()
    //+ ReadDeviceBlok
    //-WriteDeviceBlock

    public class MxComponent
    {
        ActUtlType64 mxComponent;
        string log;

        int[] devices;
        public Status status = Status.DISCONNECTED;

        public MxComponent()
        {
            mxComponent = new ActUtlType64();
        }
        public void Init()
        {
            this.mxComponent.ActLogicalStationNumber = 0;
        }

        public void WriteLog(string msg)
        {
            log = $"{DateTime.Now}:{msg}";
            Console.WriteLine(msg);
        }
        public enum Status
        {
            CONNECTED,
            DISCONNECTED
        }

        public string Connect()
        {
            string result = "";
            int ret = mxComponent.Open();
            if (ret == 0)
            {
                status = Status.CONNECTED;
                result = "PLC 연결 성공하였습니다";
                Console.WriteLine(result);
            }
            else
            {
                result = "PLC 연결에 실패하였습니다.";
                Console.WriteLine(result);
            }

            return result;

        }

        public string DisConnect()
        {
            int ret = -1;
            string resultMsg = "";
            if (status == Status.CONNECTED)
            {
                ret = mxComponent.Close();

                if (ret == 0)
                {
                    resultMsg = "PLC 연결 해제에 성공하였습니다.";
                    Console.WriteLine(resultMsg);
                }
                else
                {
                    resultMsg = "PLC 연결 해제에 실패하였습니다.";
                    Console.WriteLine(resultMsg);
                }

            }
            else
            {
                resultMsg = "이미 PLC 연결 해제 되었습니다.";
                Console.WriteLine(resultMsg);
            }
            return resultMsg;
        }

        public string ReadDeviceBlockTCPServer(string requestMsg)
        {

            string returnMsg;
            string[] requestData = requestMsg.Split(",");
            string deviceName = requestData[1];
            //Console.WriteLine(requestData.Length);
            if (requestData.Length > 2)
            {
                blockNum = int.Parse(requestData[2]);
            }

            int[] values = new int[blockNum];
            int ret = mxComponent.ReadDeviceBlock(deviceName, values.Length, out values[0]);

            if (ret == 0)
            {
                returnMsg = deviceName;

                foreach (int value in values)
                {

                    returnMsg += "," + value;
                }

            }
            else
            {

                returnMsg = "ERROR ReadDeviceBlock" + ret;

            }



            Console.WriteLine(">" + returnMsg);
            return returnMsg;
        }



        public void WriteDeviceBlockTCPServer(string dataFromClient)
        {
            string[] requestData = dataFromClient.Split(",");
            string deviceName = requestData[1];
            int blockCnt = requestData.Length - 2;

            int[] data = new int[blockCnt];
            for (int i = 0; i < blockCnt; i++)
            {
                data[i] = int.Parse(requestData[i + 2]);
            }

            mxComponent.WriteDeviceBlock(deviceName, blockCnt, ref data[0]);

        }

        public string setDevice(string requestMsg)
        {
            string returnMsg = "";
            if (requestMsg.Contains(","))
            {
                string[] requestDataArr = requestMsg.Split(",");
                if (requestDataArr.Length > 2)
                {

                    string deviceName = requestDataArr[1];
                    string requestData = requestDataArr[2];

                    int ret = mxComponent.SetDevice(deviceName, int.Parse(requestData));
                    if (ret == 0)
                    {
                        returnMsg = "SETDevice Success - " + deviceName + ":" + requestData;
                    }
                    else
                    {
                        returnMsg = "ERROR SETDevice - " + ret;

                    }

                }

            }

            Console.WriteLine(returnMsg);
            return returnMsg;
        }

    }

}


