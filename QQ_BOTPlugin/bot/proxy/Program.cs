using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using static ConfigTools.LoadMainConfig;
using System.Threading.Tasks;

namespace TcpProxy
{
    /// <summary>
    /// by 路过秋天
    /// http://www.cnblogs.com/cyq1162
    /// </summary>
    public class ProxyInterface
    {

        public static void startProxy()
        {
            int port = 26911;
            if (MainConfig.Port.Equals("auto"))
            {
                port = GamePrefs.GetInt(EnumGamePrefs.ServerPort) + 12;
            }
            else
            {
                int.TryParse(MainConfig.Port, out port);
            }
            new Task(() =>
            {
                Listen(port);
            }).Start();
        }

        public static void Listen(int port)
        {
            Write("准备监听端口:" + port);
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
            foreach (System.Net.IPAddress ip in ips)
            {
                Write(ip.ToString());
            }
            System.Net.IPAddress ipp = System.Net.IPAddress.Parse("0.0.0.0");
            TcpListener tcplistener = new TcpListener(ipp, port);
            //tcplistener.ExclusiveAddressUse = false;
            tcplistener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //tcplistener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.,ProtocolType.IP);
            try
            {
                tcplistener.Start();
            }
            catch (Exception err)
            {
                Write(err.Message);
                Write("该端口已被占用,请更换端口号!!!");
                //ReListen(tcplistener);
                return;
            }


            Write("成功监听端口:" + port);
            //侦听端口号 
            Socket socket;
            while (true)
            {
                socket = tcplistener.AcceptSocket();
                //并获取传送和接收数据的Scoket实例 
                Proxy proxy = new Proxy(socket);
                //Proxy类实例化 
                Thread thread = new Thread(new ThreadStart(proxy.Run));
                //创建线程 
                thread.Start();
                System.Threading.Thread.Sleep(200);
                //启动线程 
            }

        }

        private static void Write(string msg)
        {
            // return;
            System.Console.WriteLine(msg);
        }

        static void ReListen(TcpListener listener)
        {
            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }
            Write("请输入监听端口号:");
            string newPort = Console.ReadLine();
            int port;
            if (int.TryParse(newPort, out port))
            {
                Listen(port);
            }
            else
            {
                ReListen(listener);
            }
        }
    }
}
