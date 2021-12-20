using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using HioldMod.src.UserTools;
using HioldMod.src.HttpServer.database;

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
        }

        /// <summary>
        /// 游戏服务器初始化完成触发事件
        /// </summary>
        private static void GameStartDone()
        {
            isOnServer = true;
            DataBase.InitTable();
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
    }
}