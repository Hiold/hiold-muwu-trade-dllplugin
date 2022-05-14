using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TcpProxy
{
    /// <summary>
    /// by 路过秋天
    /// http://www.cnblogs.com/cyq1162
    /// </summary>
    public class Proxy
    {
        Socket clientSocket;//接收和返回
        Byte[] read = null;//存储来自客户端请求数据包
        Byte[] recvBytes = null;//存储中转请求返回的数据
        bool isQQProxyTest = false;
        public Proxy(Socket socket)
        {
            clientSocket = socket;
            recvBytes = new Byte[1024 * 1024];
            clientSocket.ReceiveBufferSize = recvBytes.Length;
            clientSocket.SendBufferSize = recvBytes.Length;
            clientSocket.SendTimeout = 10000;
            clientSocket.ReceiveTimeout = 10000;
        }
        public void Run()
        {
            #region 获取客户端请求数据
            string clientmessage = "";
            read = new byte[clientSocket.Available];
            int bytes = ReadMessage(read, ref clientSocket, ref clientmessage);
            if (bytes == 0)
            {
                Write("读取不到数据!");
                CloseSocket(clientSocket);
                return;
            }
            int port = 80;
            string url = GetUrl(clientmessage, ref port);
            Write("获取到URL为:" + url);
            if (url == "")
            {
                CloseSocket(clientSocket);
                return;
            }
            #endregion

            try
            {
                #region IP解析获取
                IPHostEntry IPHost = Dns.GetHostEntry(url);
                Write("-----------------------------请求开始---------------------------");
                Write("http://" + IPHost.HostName);
                string[] aliases = IPHost.Aliases;
                IPAddress[] address = IPHost.AddressList;//解析出要访问的服务器地址
                Write("IP地址：" + address[0]);
                #endregion


                #region 创建中转Socket及建立连接
                IPEndPoint ipEndpoint = new IPEndPoint(address[0], port);
                Socket IPsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //创建连接Web服务器端的Socket对象
                IPsocket.Connect(ipEndpoint);
                //Socket连Web接服务器
                if (IPsocket.Connected)
                {
                    Write("Socket 正确连接！");
                }
                #endregion
                #region 发送中转请求
                IPsocket.Send(read, 0);
                Write("发送字节：" + read.Length);
                #endregion
                #region 接收中转请求数据
                int length = 0, count = 0;
                do
                {
                    length = IPsocket.Receive(recvBytes, count, IPsocket.Available, 0);
                    count = count + length;
                    Write("正在接收数据..." + length);
                    System.Threading.Thread.Sleep(100);//关键点,请求太快数据接收不全
                }
                while (IPsocket.Available > 0);
                #endregion
                #region 关闭中转请求Socket
                CloseSocket(IPsocket);
                #endregion

                #region 将中转请求收到的数据发回客户端

                if (isQQProxyTest)
                {
                    Write(System.Text.Encoding.ASCII.GetString(recvBytes, 0, count));
                    byte[] qqOkData = QQokProxyData();
                    clientSocket.Send(qqOkData, 0, qqOkData.Length, 0);
                }
                else
                {
                    clientSocket.Send(recvBytes, 0, count, 0);
                }

                Write("正在返回数据..." + count);
                #endregion
                #region 结束请求,关闭客户端Socket
                CloseSocket(clientSocket);
                recvBytes = null;
                Write("完成,已关闭连接...");
                Write("-----------------------------请求结束---------------------------");
                #endregion
            }
            catch (Exception err)
            {
                System.Console.WriteLine(err.Message + err.Source);
            }

        }
        //从请求头里解析出url和端口号
        private string GetUrl(string clientmessage, ref int port)
        {
            int index1 = clientmessage.IndexOf(' ');
            int index2 = clientmessage.IndexOf(' ', index1 + 1);
            if ((index1 == -1) || (index2 == -1))
            {
                return "";
            }
            string part1 = clientmessage.Substring(index1 + 1, index2 - index1).Trim();
            string url = string.Empty;
            if (!part1.Contains("http://"))
            {
                if (part1.Substring(0, 1) == "/")
                {
                    part1 = "127.0.0.1" + part1;
                }
                part1 = "http://" + part1;
            }
            Uri uri = null;
            try
            {
                uri = new Uri(part1);
            }
            catch
            {
                return "";
            }
            url = uri.Host;
            port = uri.Port;
            if (url == "http.tencent.com" && port == 443)
            {
                isQQProxyTest = true;
            }
            return url;
        }
        //接收客户端的HTTP请求数据
        private int ReadMessage(byte[] readByte, ref Socket s, ref string clientmessage)
        {
            try
            {
                int bytes = s.Receive(readByte, readByte.Length, 0);
                clientmessage = Encoding.ASCII.GetString(readByte);
                return bytes;
            }
            catch
            {
                System.Threading.Thread.Sleep(300);
                return 0;
            }
        }

        private void Write(string msg)
        {
            // return;
            System.Console.WriteLine(msg);
        }
        private void CloseSocket(Socket socket)
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
        private byte[] QQokProxyData()
        {
            string data = "HTTP/1.0 200 Connection established";//  Proxy-agent: CCProxy 2010";
            return System.Text.Encoding.ASCII.GetBytes(data);
        }
    }
}
