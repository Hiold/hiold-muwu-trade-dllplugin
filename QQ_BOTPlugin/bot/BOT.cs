using QQ_BOTPlugin.bot.model;
using QQ_BOTPlugin.bot.model.message;
using SimpleJson2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot
{
    public class BOT
    {
        public static string host = "http://dx.s1.hiold.net:9991";
        public static string key = "a1010000";
        public static int qq = 14021367;
        public static string token = null;
        public static int qunNumber = 542051191;
        //定时获取消息队列
        public static System.Threading.Timer Onlinetimer = new System.Threading.Timer(new TimerCallback(HandleMessage), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));


        /// <summary>
        /// 初始化bot
        /// </summary>
        public static void initBot()
        {
            token = Adaptor.GetToken(key);
            Adaptor.Bind(token, qq);
            MessageCount mc = Adaptor.GetMessageCount(token);
        }

        public static void HandleMessage(object obj)
        {
            try
            {
                Message msg = Adaptor.FetchMessage(token, 10);
                //消息体不为空则开始处理
                if (msg.data != null && msg.data.Count > 0)
                {
                    foreach (object meessage in msg.data)
                    {
                        //处理群消息
                        if (meessage.GetType().IsAssignableFrom(typeof(GroupMessage)))
                        {
                            MessageDispacher.HandleGrouopMessage(meessage);
                        }
                        //处理私聊
                        if (meessage.GetType().IsAssignableFrom(typeof(TempMessage)))
                        {
                            MessageDispacher.HandleTempMessage(meessage);
                        }

                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
