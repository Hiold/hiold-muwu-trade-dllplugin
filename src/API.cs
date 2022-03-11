using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using HioldMod.src.UserTools;
using HioldMod.src.HttpServer.database;
using HioldMod.src.HttpServer.service;
using HioldMod.src.HttpServer.bean;
using HioldMod.HttpServer.common;
using ServerTools;
using ConfigTools;
using static ConfigTools.LoadMainConfig;
using HarmonyLib;
using Pathfinding;
using HioldMod.src.Reflection;
using HioldMod.HttpServer;
using NaiwaziServerKitInterface;
using HioldMod.src.Commons;
using System.Threading;

namespace HioldMod
{
    public class HioldMod
    {
        public class API : IModApi
        {
            //当前dll运行路径
            public static string ConfigPath = string.Format("{0}/config/", getModDir());
            public static string AssemblyPath = string.Format("{0}\\", getModDir());
            //运行在服务器
            public static bool isOnServer = false;
            public static bool isDebug = true;
            //已启用NaiwaziBot
            public static bool isNaiwaziBot = false;
            //已启用serverkit
            public static bool isServerKit = false;
            //正在快速重启
            public static bool isFastRestarting = false;
            //指令隐藏
            private static ChatHider chatHider = null;


            /// <summary>
            /// 初始化mod
            /// </summary>
            /// <param name="_modInstance">A20新增形参</param>
            public void InitMod(Mod _modInstance)
            {
                isOnServer = true;
                //检查日志文件是否存在
                //检查文件夹
                if (!Directory.Exists(string.Format("{0}/Logs/", API.ConfigPath)))
                {
                    Directory.CreateDirectory(string.Format("{0}/Logs/", API.ConfigPath));
                }

                //注入hook
                RunTimePatch.PatchAll();

                //注册事件
                ModEvents.GameStartDone.RegisterHandler(GameStartDone);
                ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
                ModEvents.ChatMessage.RegisterHandler(ChatMessage);
                ModEvents.GameUpdate.RegisterHandler(GameUpdate);
                ModEvents.EntityKilled.RegisterHandler(KillEntityHandler.EntityKilledHandler);
            }

            /// <summary>
            /// 游戏服务器初始化完成触发事件
            /// </summary>
            private static void GameStartDone()
            {
                //naiwazi适配部分
                NaiwaziPointHelper.InitNaiwaziBotPointHelper();
                //翻译转换部分
                LocalizationUtils.InitReverseTranslate();
                isServerKit = CommonInterface.IsServerKitEnabled();
                if (isServerKit)
                {
                    LogUtils.Loger("检测到ServerKit启用适配");

                    /*
                     *	ChatHider
                     */
                    chatHider = new ChatHider();

                    //If you seperate the gateway and gameserver, that means they have different IPs, so you need to set it manually.
                    //chatHider.Ip = "x.x.x.x";

                    chatHider.SetChatHider("/xmm");

                    int ret = ChatInterface.ChatWatcher_Register(OnChatMessage, "HioldMuwu");
                    if (ret != 0)
                    {
                        LogUtils.Loger("Register chat watcher failed, err: " + ret.ToString());
                    }

                    ret = FastRestartNoticeInterface.Notice_Register(OnFastRestartPrepared, "HioldMuwu");
                    if (ret != 0)
                    {
                        LogUtils.Loger("Register fast-restarting notice failed, err: " + ret.ToString());
                    }
                }



                //反射获取已上锁Tile
                bool isReflected = LockedEntity.doReflection();
                LogUtils.Loger("反射获取数据情况: " + isReflected);

                //检查文件夹
                if (!Directory.Exists(API.ConfigPath))
                {
                    Directory.CreateDirectory(API.ConfigPath);
                }
                //加载配置文件
                LoadMainConfig.Load();

                LogUtils.Loger("Host:" + MainConfig.Host + "  Port" + MainConfig.Port);
                DataBase.InitDataBase();
                int port = 26911;
                if (MainConfig.Port.Equals("auto"))
                {
                    port = GamePrefs.GetInt(EnumGamePrefs.ServerPort) + 11;
                }
                else
                {
                    int.TryParse(MainConfig.Port, out port);
                }

                HioldModServer.Server.RunServer(port);
                //定时器发送心跳数据
                LogUtils.Loger("正在初始化用户基础信息更新定时器");
                System.Threading.Timer Onlinetimer = new System.Threading.Timer(new TimerCallback(HartBeatHandler.HandlePlayerHartbeat), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
                LogUtils.Loger("Init执行完毕");
            }

            /// <summary>
            /// 原生聊天信息处理
            /// </summary>
            /// <param name="_cInfo"></param>
            /// <param name="_type"></param>
            /// <param name="_senderId"></param>
            /// <param name="_msg"></param>
            /// <param name="_mainName"></param>
            /// <param name="_localizeMain"></param>
            /// <param name="_recipientEntityIds"></param>
            /// <returns></returns>
            public bool ChatMessage(ClientInfo _cInfo, EChatType _type, int _senderId, string _msg, string _mainName,
                 bool _localizeMain, List<int> _recipientEntityIds)
            {

                //监听[/sa]命令
                if (!string.IsNullOrEmpty(_msg) && _msg.EqualsCaseInsensitive("/shop"))
                {
                    LogUtils.Loger("正在执行shop");
                    if (_cInfo != null)
                    {
                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("xui open HioldshopWindows", true));
                    }
                    else
                    {
                        Log.Error("ChatHookExample: Argument _cInfo null on message: {0}", _msg);
                    }
                    return false;
                }
                return true;
            }

            /// <summary>
            /// serverkit处理的聊天信息
            /// </summary>
            /// <param name="_cInfo"></param>
            /// <param name="_msg"></param>
            /// <param name="_type"></param>
            /// <param name=""></param>
            private static void OnChatMessage(ClientInfo _cInfo, string _msg, /*uint _timeStamp,*/ EChatType _type)
            {
                //监听[/sa]命令
                if (!string.IsNullOrEmpty(_msg) && _msg.EqualsCaseInsensitive("/shop"))
                {
                    LogUtils.Loger("正在执行shop");
                    if (_cInfo != null)
                    {
                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConsoleCmdClient>().Setup("xui open HioldshopWindows", true));
                    }
                    else
                    {
                        Log.Error("ChatHookExample: Argument _cInfo null on message: {0}", _msg);
                    }
                }
            }

            private static void OnFastRestartPrepared()
            {
                LogUtils.Loger("收到通知,正在快速重启,暂停所有接口服务");
                isFastRestarting = true;
            }

            /// <summary>
            /// 执行update主线程任务
            /// </summary>
            private static void GameUpdate()
            {
                DeliverItemTools.TryDeliverItem();
                DeliverItemTools.TryExecuteCommand();
                DeliverItemTools.TryDeleverItemWithData();

                //处理调度任务
                //if (chatInstance != null)
                //{
                //    chatInstance.PullMessages(OnChatMessage);
                //}
            }


            /// <summary>
            /// 获取当前mod运行路径
            /// </summary>
            /// <returns>路径</returns>
            public static string getModDir()
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return System.IO.Path.GetDirectoryName(path);
            }

            /// <summary>
            /// 用户登录系自动注册
            /// </summary>
            /// <param name="_cInfo"></param>
            /// <param name="_respawnReason"></param>
            /// <param name="_pos"></param>
            private static void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)
            {
                if (_respawnReason == RespawnType.EnterMultiplayer || _respawnReason == RespawnType.JoinMultiplayer)
                {
                    //string regParam = "{\"steamid\":\"" + _cInfo.PlatformId.PlatformIdentifierString + "\",\"name\":\"" + _cInfo.playerName + "\"}";
                    try
                    {
                        string pwd = ServerUtils.GetRandomPwd(6);
                        //根据SteamID获取已注册用户信息
                        List<UserInfo> exists = UserService.getUserBySteamid(_cInfo.PlatformId.ReadablePlatformUserIdentifier);

                        if (exists == null || exists.Count <= 0)
                        {

                            int id = UserService.userRegister(new UserInfo()
                            {
                                created_at = DateTime.Now,
                                name = _cInfo.playerName,
                                gameentityid = _cInfo.PlatformId.ReadablePlatformUserIdentifier,
                                platformid = _cInfo.CrossplatformId.ReadablePlatformUserIdentifier,
                                money = 0,
                                credit = 0,
                                status = 1,
                                password = ServerUtils.md5(pwd),
                                qq = "",
                                email = "",
                                avatar = _cInfo.PlatformId.ReadablePlatformUserIdentifier + ".png",
                                sign = "",
                                extinfo1 = "",
                                extinfo2 = "",
                                extinfo3 = "",
                                extinfo4 = "",
                                extinfo5 = "",
                                extinfo6 = "",
                                trade_count = "0",
                                store_count = "0",
                                require_count = "0",
                                type = "0",
                                level = 0,
                                online_time = "0",
                                zombie_kills = "0",
                                player_kills = "0",
                                total_crafted = "0",
                                vipdiscount = 0,
                                creditcharge = 0,
                                creditcost = 0,
                                moneycharge = 0,
                                moneycost = 0,
                                signdays = 0,
                                likecount = 0,
                                trade_money = 0,
                                require_money = 0,
                                buy_count = "0",
                                shopname = _cInfo.playerName + "的小店",
                                ncode = "",
                            });
                            if (id >= 0)
                            {
                                _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[00FF00]" + "您的初始密码为：" + pwd, "[87CEFA]交易系统", false, null));
                            }
                            else
                            {
                                _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[00FF00]" + "注册失败，请联系管理员", "[87CEFA]交易系统", false, null));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[FF0000]注册失败:" + "系统内部错误,请联系管理员", "[87CEFA]交易系统", false, null));
                    }
                }
            }
        }
    }
}