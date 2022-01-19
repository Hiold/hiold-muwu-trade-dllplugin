using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using HioldMod.src.UserTools;
using HioldMod.src.HttpServer.database;
using HioldMod.src.HttpServer.service;
using HioldMod.src.HttpServer.bean;
using HioldMod.HttpServer.common;

namespace HioldMod
{
    public class API : IModApi
    {
        //当前dll运行路径
        public static string AssemblyPath = string.Format("{0}\\", getModDir());
        public static bool isOnServer = false;
        public static bool isDebug = true;

        /// <summary>
        /// 初始化mod
        /// </summary>
        /// <param name="_modInstance">A20新增形参</param>
        public void InitMod(Mod _modInstance)
        {
            //Assembly assembly1 = Assembly.Load(string.Format(@"{0}System.Web.dll", AssemblyPath));
            //Assembly assembly2 = Assembly.Load(string.Format(@"{0}LiteDB.dll", AssemblyPath));
            //Log.Warning(assembly1.Location);
            //Log.Warning(assembly2.Location);
            //注册事件
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerSpawnedInWorld);
        }

        /// <summary>
        /// 游戏服务器初始化完成触发事件
        /// </summary>
        private static void GameStartDone()
        {
            isOnServer = true;
            DataBase.InitDataBase();
            Server.RunServer(GamePrefs.GetInt(EnumGamePrefs.ServerPort) + 11);
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
            return Path.GetDirectoryName(path);
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
                            avatar = "",
                            sign = "",
                            extinfo1 = "",
                            extinfo2 = "",
                            extinfo3 = "",
                            extinfo4 = "",
                            extinfo5 = "",
                            extinfo6 = "",
                            trade_count = "",
                            store_count = "",
                            require_count = "",
                            type = "0",
                            level = "",
                            online_time = "",
                            zombie_kills = "",
                            player_kills = "",
                            total_crafted = "",
                            vipdiscount = 0,
                            creditcharge = 0,
                            creditcost = 0,
                            moneycharge = 0,
                            moneycost = 0,
                            signdays = 0,
                            likecount = 0,
                            shopname = _cInfo.playerName + "的小店",
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