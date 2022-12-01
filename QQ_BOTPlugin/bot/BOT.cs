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
        //签到qdCount
        public static int qdCount = 0;
        //同步chat
        public static bool chat = false;

        public static Dictionary<string, string> bindUser = new Dictionary<string, string>();

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
            //
            loadBindSteam();
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
            //注册聊天同步方法
            TradeSysEvents.RegChatEvent(Events.OnChat);
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


        /// <summary>
        /// 加载配置文件
        /// </summary>
        public static void loadBindSteam()
        {
            LogUtils.Loger("加载绑定用户");
            string path = string.Format("{0}/plugins/steam.json", HioldMod.HioldMod.API.AssemblyPath);
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    return;
                }
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] tmp = line.Split("-");
                        bindUser[tmp[0]] = tmp[1];
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void saveBindSteam()
        {
            LogUtils.Loger("保存绑定用户");
            string path = string.Format("{0}/plugins/steam.json", HioldMod.HioldMod.API.AssemblyPath);
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                fi.Delete();
            }
            fi.Create();

            // 读取文本文件
            using (StreamWriter sr = new StreamWriter(path))
            {
                foreach (KeyValuePair<string, string> kvp in bindUser)
                {
                    sr.WriteLine(kvp.Key + "-" + kvp.Value);
                }

            }
        }


        public static void loadWYSConfig()
        {
            string jsonContent = "";
            string path = string.Format("{0}/plugins/wys.json", HioldMod.HioldMod.API.AssemblyPath);
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
                jsonMap.TryGetValue("qdCount", out string qdCountStr);
                jsonMap.TryGetValue("chat", out string chatStr);
                int.TryParse(qdCountStr, out qdCount);
                bool.TryParse(chatStr, out chat);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }


        /// <summary>
        /// 获取玩家ClientInfo数据
        /// </summary>
        /// <param name="_playerId"></param>
        /// <returns></returns>
        public static ClientInfo GetClientInfoByEOSorSteamid(string _playerId)
        {
            ClientInfoCollection cic = ConnectionManager.Instance.Clients;
            //Log.Out("[HioldMod]服务器中总玩家数为:" + _persistentPlayerList.Players.Count);
            foreach (ClientInfo clientInfo in cic.List)
            {
                //Log.Out("[HioldMod]Identify=" + pls.Key.CombinedString);
                if (clientInfo.PlatformId.ReadablePlatformUserIdentifier.Equals(_playerId) || clientInfo.CrossplatformId.ReadablePlatformUserIdentifier.Equals(_playerId))
                {
                    return clientInfo;
                }
            }
            return null;
        }

        public static void sendToPlayer(string steamid, string msg)
        {
            ClientInfo _cInfo = GetClientInfoByEOSorSteamid(steamid);
            if (_cInfo != null)
            {
                _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, msg, "[87CEFA]群聊天消息", false, null));
            }
        }
    }
}
