using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using HioldMod.HttpServer;

namespace QQ_BOTPlugin.bot.websocket
{
    public class WebSocketHelper
    {
        public static ClientWebSocket client;
        public static bool isSocketOk = false;
        public static void InitWebSocket()
        {
            new Task(() =>
            {
                while (true)
                {
                    if (!isSocketOk)
                    {
                        try
                        {
                            client = new ClientWebSocket();
                            client.ConnectAsync(new Uri("ws://127.0.0.1:8999/event/"), CancellationToken.None).Wait();
                            isSocketOk = true;
                            //client.ConnectAsync(new Uri("ws://localhost:4567/ws/"), CancellationToken.None).Wait();
                            //监听正常
                            CMD.sbConsole.AppendLine("已开启监听");
                            StartReceiving(client);
                        }
                        catch (Exception e)
                        {
                            CMD.sbConsole.AppendLine("监听启动异常：" + e.Message);
                        }
                    }
                    Thread.Sleep(10000);
                }

            }).Start();

        }
        static async void StartReceiving(ClientWebSocket client)
        {
            while (true)
            {
                try
                {
                    var array = new byte[4096];
                    var result = await client.ReceiveAsync(new ArraySegment<byte>(array), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string msg = Encoding.UTF8.GetString(array, 0, result.Count);
                        WebSocketMessageHandler.HandleMessage(msg);
                    }
                }
                catch (Exception e)
                {
                    CMD.sbConsole.AppendLine("消息处理异常：" + e.Message);
                }
            }
        }

    }
}
