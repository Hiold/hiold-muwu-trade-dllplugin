using HioldMod.HttpServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.websocket
{
    public class WebSocketMessageHandler
    {
        public static void HandleMessage(string msg)
        {
            LogUtils.Loger(msg);
            //心跳数据
            if (msg.Contains("\"meta_event_type\":\"heartbeat\""))
            {

            }
            //群消息
            if (msg.Contains("\"message_type\":\"group\""))
            {

            }
        }
    }
}
