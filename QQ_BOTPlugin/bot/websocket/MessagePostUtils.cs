using QQ_BOTPlugin.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.websocket
{
    public static class MessagePostUtils
    {
        public static void PostGroupMessage(int qunNumber, string msg)
        {
            Dictionary<string, string> req = new Dictionary<string, string>();
            req.Add("group_id", qunNumber + "");
            req.Add("message", msg);
            //发送请求
            //string response = HttpUtils.HttpPost("http://127.0.0.1:8998/send_group_msg", SimpleJson2.SimpleJson2.SerializeObject(req));
            string response = HttpUtils.HttpGet("http://127.0.0.1:8998/send_group_msg?group_id=" + qunNumber + "&message=" + msg);
            CMD.sbConsole.AppendLine(response);
        }


        public static void PostTempMessage(int qunNumber, int qq, string msg)
        {
            Dictionary<string, string> req = new Dictionary<string, string>();
            req.Add("message_type", "group");
            req.Add("user_id", qq + "");
            req.Add("group_id", qunNumber + "");
            req.Add("message", msg);
            //发送请求
            string response = HttpUtils.HttpPost("http://127.0.0.1:8998/send_msg", SimpleJson2.SimpleJson2.SerializeObject(req));
            CMD.sbConsole.AppendLine(response);
        }
    }
}
