using HioldMod.HttpServer;
using QQ_BOTPlugin.bot.model;
using QQ_BOTPlugin.bot.model.api;
using QQ_BOTPlugin.bot.model.message;
using QQ_BOTPlugin.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot
{
    public class Adaptor
    {
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetToken(string key)
        {
            VerifyRequest req = new VerifyRequest() { verifyKey = key };
            string response = HttpUtils.HttpPost(BOT.host + "/verify", SimpleJson2.SimpleJson2.SerializeObject(req));
            LogUtils.Loger(response);
            VerifyResponse resp = SimpleJson2.SimpleJson2.DeserializeObject<VerifyResponse>(response);
            return resp.session;
        }

        /// <summary>
        /// 绑定会话
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="qq"></param>
        public static void Bind(string sessionKey, int qq)
        {
            BindRequest req = new BindRequest() { qq = qq, sessionKey = sessionKey };
            string response = HttpUtils.HttpPost(BOT.host + "/bind", SimpleJson2.SimpleJson2.SerializeObject(req));
            LogUtils.Loger(response);
            BindResponse resp = SimpleJson2.SimpleJson2.DeserializeObject<BindResponse>(response);
        }

        /// <summary>
        /// 获取消息数量
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static MessageCount GetMessageCount(string sessionKey)
        {
            string response = HttpUtils.HttpGet(BOT.host + "/countMessage?sessionKey=" + sessionKey);
            LogUtils.Loger(response);
            MessageCount resp = SimpleJson2.SimpleJson2.DeserializeObject<MessageCount>(response);
            return resp;
        }

        /// <summary>
        /// 获取队列中消息
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Message FetchMessage(string sessionKey, int count)
        {
            string response = HttpUtils.HttpGet(BOT.host + "/fetchMessage?sessionKey=" + sessionKey + "&count=" + count);
            Message resp = new Message(response);
            return resp;
        }

        public static GroupMessageResponse PostGroupMessage(int qunNumber, string sessionKey, string msg)
        {
            Plain text = new Plain(msg);
            List<object> msgList = new List<object>();
            msgList.Add(text);
            //构建需要发送的数据
            GroupMessageRequest req = new GroupMessageRequest()
            {
                target = qunNumber,
                sessionKey = sessionKey,
                messageChain = msgList,
            };
            //发送请求
            string response = HttpUtils.HttpPost(BOT.host + "/sendGroupMessage", SimpleJson2.SimpleJson2.SerializeObject(req));
            LogUtils.Loger(response);
            GroupMessageResponse resp = SimpleJson2.SimpleJson2.DeserializeObject<GroupMessageResponse>(response);
            return resp;
        }


        public static TempMessageResponse PostTempMessage(int qunNumber, int qq, string sessionKey, string msg)
        {
            Plain text = new Plain(msg);
            List<object> msgList = new List<object>();
            msgList.Add(text);
            //构建需要发送的数据
            TempMessageRequest req = new TempMessageRequest()
            {
                group = qunNumber,
                qq = qq,
                sessionKey = sessionKey,
                messageChain = msgList,
            };
            //发送请求
            string response = HttpUtils.HttpPost(BOT.host + "/sendTempMessage", SimpleJson2.SimpleJson2.SerializeObject(req));
            LogUtils.Loger(response);
            TempMessageResponse resp = SimpleJson2.SimpleJson2.DeserializeObject<TempMessageResponse>(response);
            return resp;
        }

    }
}
