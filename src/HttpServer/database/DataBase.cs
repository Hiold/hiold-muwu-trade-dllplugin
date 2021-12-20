using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.service;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.database
{
    public class DataBase
    {
        //数据库文件路径
        private static string dbFilePath;
        //数据库客户端

        //调试使用的数据库文件存储路径
        public static string debugDbfilePath = "H:/SteamLibrary/steamapps/common/7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin/database/";

        public static LiteDatabase litedb;

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void InitTable()
        {
            //创建数据库链接
            if (API.isOnServer)
            {
                //在服务器端运行获取服务器路径
                string modDBDir = string.Format("{0}database/", API.AssemblyPath);
                //判断是否存在路径
                if (!Directory.Exists(modDBDir))
                {
                    Directory.CreateDirectory(modDBDir);
                }
                dbFilePath = string.Format("{0}database.db", modDBDir);

            }
            else
            {
                if (!Directory.Exists(debugDbfilePath))
                {
                    Directory.CreateDirectory(debugDbfilePath);
                }
                dbFilePath = string.Format("{0}database.db", debugDbfilePath);
            }

            //创建litedb实例
            litedb = new LiteDatabase(@dbFilePath);



            


            UserInfo user = new UserInfo()
            {
                name = "海鸥",
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                password = ServerUtils.md5("a10100"),
                gameentityid = "test"
            };
            var col = litedb.GetCollection<UserInfo>("UserInfo");
            col.Insert(user);
        }
    }
}
