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
using HioldMod.src.HttpServer.router;
using HioldMod.src.Plugins;
using System.Diagnostics;
using System.Threading.Tasks;

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
                //查杀已存在的进程
                KillJavaProcess("taskkill -F -IM 16076_main.exe");

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


            public static void KillJavaProcess(string command)
            {
                //Console.WriteLine("请输⼊要执⾏的命令:");
                Process p = new Process();
                //设置要启动的应⽤程序
                p.StartInfo.FileName = "cmd.exe";
                //是否使⽤操作系统shell启动
                p.StartInfo.UseShellExecute = false;
                // 接受来⾃调⽤程序的输⼊信息
                p.StartInfo.RedirectStandardInput = true;
                //输出信息
                p.StartInfo.RedirectStandardOutput = true;
                // 输出错误
                p.StartInfo.RedirectStandardError = true;
                //不显⽰程序窗⼝
                p.StartInfo.CreateNoWindow = true;
                //启动程序
                p.Start();


                new Task(() =>
                {
                    while (!p.StandardOutput.EndOfStream)
                    {
                        string line = p.StandardOutput.ReadLine();
                        Console.WriteLine(line);
                    }

                }).Start();

                new Task(() =>
                {
                    while (!p.StandardError.EndOfStream)
                    {
                        string line = p.StandardError.ReadLine();
                        Console.WriteLine(line);
                    }
                }).Start();

                //向cmd窗⼝发送输⼊信息
                p.StandardInput.WriteLine(command);
                p.StandardInput.AutoFlush = true;
                //获取输出信息
                p.Close();
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

                    chatHider.SetChatHider("/pmreg");

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

                LogUtils.Loger("Host:" + MainConfig.Host + "  Port" + MainConfig.Port);
                DataBase.InitDataBase();

                //加载配置文件
                LoadMainConfig.Load();

                int port = 26911;
                if (MainConfig.Port.Equals("auto"))
                {
                    port = GamePrefs.GetInt(EnumGamePrefs.ServerPort) + 11;
                }
                else
                {
                    int.TryParse(MainConfig.Port, out port);
                }
                //
                LogUtils.Loger("开始初始化Action");
                AttributeAnalysis.AnalysisStart();
                //开启http服务器
                HioldModServer.Server.RunServer(port);
                //定时器发送心跳数据
                LogUtils.Loger("正在初始化用户基础信息更新定时器");
                System.Threading.Timer Onlinetimer = new System.Threading.Timer(new TimerCallback(HartBeatHandler.HandlePlayerHartbeat), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
                LogUtils.Loger("正在初始化游戏记录入库定时器");
                System.Threading.Timer Onlinetimer2 = new System.Threading.Timer(new TimerCallback(KillEntityHandler.AddEventLog), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
                //开始加载插件
                PluginsLoader.LoadAllPlugins();

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

                //监听[/pmreg]命令
                if (!string.IsNullOrEmpty(_msg) && _msg.StartsWith("/pmreg"))
                {
                    string[] command = _msg.Split(' ');
                    //命令参数不正确
                    if (command.Length < 2)
                    {
                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[ff0000]注册失败,格式错误,正确格式为/pmreg 密码", "[87CEFA]交易系统", false, null));
                        return false;
                    }
                    if (_cInfo != null)
                    {
                        //Log.Out("响应玩家的拍卖请求 {0}", _cInfo.playerId);
                        //HandleCommand.handleRegUser(_cInfo, command[1]);
                        string newpw = command[1];
                        List<UserInfo> us = UserService.getUserBySteamid(_cInfo.PlatformId.ReadablePlatformUserIdentifier);
                        if (us != null)
                        {
                            UserInfo ui = us[0];
                            ui.password = ServerUtils.md5(newpw);
                            UserService.UpdateUserInfo(ui);
                            _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[00FF00]密码修改成功", "[87CEFA]交易系统", false, null));
                        }
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

                //监听[/pmreg]命令
                if (!string.IsNullOrEmpty(_msg) && _msg.StartsWith("/pmreg"))
                {
                    string[] command = _msg.Split(' ');
                    //命令参数不正确
                    if (command.Length < 2)
                    {
                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[ff0000]注册失败,格式错误,正确格式为/pmreg 密码", "[87CEFA]交易系统", false, null));
                    }
                    if (_cInfo != null)
                    {
                        //Log.Out("响应玩家的拍卖请求 {0}", _cInfo.playerId);
                        //HandleCommand.handleRegUser(_cInfo, command[1]);
                        string newpw = command[1];
                        List<UserInfo> us = UserService.getUserBySteamid(_cInfo.PlatformId.ReadablePlatformUserIdentifier);
                        if (us != null)
                        {
                            UserInfo ui = us[0];
                            ui.password = ServerUtils.md5(newpw);
                            UserService.UpdateUserInfo(ui);
                            _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[00FF00]密码修改成功", "[87CEFA]交易系统", false, null));
                        }
                    }
                    else
                    {
                        Log.Error("ChatHookExample: Argument _cInfo null on message: {0}", _msg);
                    }
                }


                //监听[/pmreg]命令
                if (!string.IsNullOrEmpty(_msg) && _msg.StartsWith("/rmtest"))
                {
                    string[] command = _msg.Split(' ');
                    //命令参数不正确
                    if (command.Length < 2)
                    {
                        _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[ff0000]注册失败,格式错误,正确格式为/pmreg 密码", "[87CEFA]交易系统", false, null));
                    }
                    if (_cInfo != null)
                    {
                        string newpw = command[1];
                        int ta = int.Parse(newpw);
                        EntityAlive target = (EntityAlive)GameManager.Instance.World.GetEntity(_cInfo.entityId);
                        NetPackagePlayerStats nps = NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(target);
                        Traverse.Create(nps).Field("holdingItemIndex").SetValue((byte)ta);
                        Traverse.Create(nps).Field("holdingItemStack").SetValue(ItemStack.Empty.Clone());
                        _cInfo.SendPackage(nps);
                        BlockValue bv = Block.GetBlockValue("terrStone");
                        Vector3i pos = new Vector3i(target.position);
                        GameManager.Instance.World.SetBlockRPC(pos, bv);
                    }
                    else
                    {
                        Log.Error("ChatHookExample: Argument _cInfo null on message: {0}", _msg);
                    }
                }


                //监听[/pmreg]命令
                if (!string.IsNullOrEmpty(_msg) && _msg.StartsWith("/setb"))
                {

                    EntityAlive target = (EntityAlive)GameManager.Instance.World.GetEntity(_cInfo.entityId);
                    BlockValue bv = Block.GetBlockValue("terrStone");
                    Vector3i pos = new Vector3i(target.position);
                    GameManager.Instance.World.SetBlockRPC(pos, bv);

                }

                //监听[/pmreg]命令
                if (!string.IsNullOrEmpty(_msg) && _msg.StartsWith("/rmb"))
                {

                    EntityAlive target = (EntityAlive)GameManager.Instance.World.GetEntity(_cInfo.entityId);
                    BlockValue bv = Block.GetBlockValue("air");
                    Vector3i pos = new Vector3i(target.position);
                    GameManager.Instance.World.SetBlockRPC(pos, bv);

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
                        else
                        {
                            UserInfo ui = exists[0];
                            ui.name = _cInfo.playerName;
                            ui.gameentityid = _cInfo.PlatformId.ReadablePlatformUserIdentifier;
                            ui.platformid = _cInfo.CrossplatformId.ReadablePlatformUserIdentifier;
                            UserService.UpdateUserInfo(ui);
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