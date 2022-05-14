using HioldMod.HttpServer;
using HioldMod.src;
using HioldMod.src.HttpServer.router;
using QQ_BOTPlugin.bot.model;
using QQ_BOTPlugin.bot.model.message;
using QQ_BOTPlugin.bot.websocket;
using QQ_BOTPlugin.bot.websocket.bean;
using SimpleJson2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HioldMod.HioldMod;

namespace QQ_BOTPlugin.bot
{
    public class BOT
    {
        //public static string host = "http://dx.s1.hiold.net:9991";
        //public static string key = "a1010000";
        //public static int qq = 14021367;
        //public static string token = null;
        //public static int qunNumber = 542051191;

        public static string host = "http://localhost:8998";
        public static int qq = 0;
        public static string token = "";
        public static int qunNumber = 0;
        public static string EnableLottery = "";
        public static string password = "";
        public static bool isAlive = false;
        //心跳数据
        public static HeartBeat heartBeat;

        //是否配置完成


        //定时获取消息队列
        //public static System.Threading.Timer Onlinetimer = new System.Threading.Timer(new TimerCallback(HandleMessage), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        //超过一分钟没有心跳数据收到，暂停功能
        public static System.Threading.Timer Onlinetimer = new System.Threading.Timer(new TimerCallback((object c) => { isAlive = false; }), null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));


        /// <summary>
        /// 初始化bot
        /// </summary>
        public static void initBot()
        {
            Console.WriteLine("正在初始化");
            //分析载入插件后的接口数据
            AttributeAnalysis.AnalysisStart();
            //加载配置文件
            loadConfig();
            //初始化qqbot
            //token = Adaptor.GetToken(key);
            //Adaptor.Bind(token, qq);
            //MessageCount mc = Adaptor.GetMessageCount(token);
            //注册监听事件

            string tokenPath = API.AssemblyPath + @"plugins\Robot\session.token";
            string devicePath = API.AssemblyPath + @"plugins\Robot\device.json";

            if (!System.IO.File.Exists(tokenPath) || !System.IO.File.Exists(devicePath))
            {
                LogUtils.Loger("QQBOT配置文件不完整，无法启动请完成配置");
                CMD.sbConsole.Append("QQBOT配置文件不完整，无法启动请完成配置");
                return;
            }

            if (qunNumber == 0)
            {
                LogUtils.Loger("QQBOT未配置群号，无法开启，请配置群号");
                CMD.sbConsole.Append("QQBOT未配置群号，无法开启，请配置群号");
                return;
            }

            TradeSysEvents.RegOnSellEvent(Events.OnSell);
            //TradeSysEvents.RegSellOutEvent(Events.SellOut);
            TradeSysEvents.RegLotteryEvent(Events.Lottery);
            //运行qq
            CMD.RunQQMCL();
            //初始化websocket
            WebSocketHelper.InitWebSocket();
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        public static void loadConfig()
        {
            string jsonContent = "";
            string path = string.Format("{0}/plugins/bot.json", HioldMod.HioldMod.API.AssemblyPath);
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    return;
                }
                // 读取文本文件
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    // ReadLine()一行一行的循环读取
                    //当然可以直接ReadToEnd()读到最后
                    while ((line = sr.ReadLine()) != null)
                    {
                        jsonContent += line;
                    }
                }
                Dictionary<string, string> jsonMap = new Dictionary<string, string>();
                jsonMap = SimpleJson2.SimpleJson2.DeserializeObject<Dictionary<string, string>>(jsonContent);
                jsonMap.TryGetValue("host", out host);
                jsonMap.TryGetValue("qq", out string qqStr);
                jsonMap.TryGetValue("qunNumber", out string qunNumberStr);
                jsonMap.TryGetValue("lottery", out EnableLottery);
                jsonMap.TryGetValue("password", out password);
                if (!string.IsNullOrEmpty(qqStr))
                {
                    int.TryParse(qqStr, out qq);
                }
                if (!string.IsNullOrEmpty(qunNumberStr))
                {
                    int.TryParse(qunNumberStr, out qunNumber);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        //    public static void HandleMessage(object obj)
        //    {
        //        try
        //        {
        //            Message msg = Adaptor.FetchMessage(token, 10);
        //            //消息体不为空则开始处理
        //            if (msg.data != null && msg.data.Count > 0)
        //            {
        //                foreach (object meessage in msg.data)
        //                {
        //                    //处理群消息
        //                    if (meessage.GetType().IsAssignableFrom(typeof(GroupMessage)))
        //                    {
        //                        MessageDispacher.HandleGrouopMessage(meessage);
        //                    }
        //                    //处理私聊
        //                    if (meessage.GetType().IsAssignableFrom(typeof(TempMessage)))
        //                    {
        //                        MessageDispacher.HandleTempMessage(meessage);
        //                    }

        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
    }
}
