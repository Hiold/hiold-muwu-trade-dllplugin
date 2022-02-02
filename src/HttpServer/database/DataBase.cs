using HioldMod.src.HttpServer.bean;
using SqlSugar;
using System.IO;

namespace HioldMod.src.HttpServer.database
{
    public class DataBase
    {
        //数据库文件路径
        private static string dbFilePath;
        //数据库客户端

        //调试使用的数据库文件存储路径
        public static string debugDbfilePath = "D:/database/";
        public static SqlSugarClient db = null;

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void InitDataBase()
        {
            ////创建数据库链接
            if (API.isOnServer)
            {
                //在服务器端运行获取服务器路径
                string modDBDir = string.Format("{0}database/", API.AssemblyPath);
                //判断是否存在路径
                if (!Directory.Exists(modDBDir))
                {
                    Directory.CreateDirectory(modDBDir);
                }
                dbFilePath = string.Format("DataSource={0}TradeManageDB.db", modDBDir);

            }
            else
            {
                if (!Directory.Exists(debugDbfilePath))
                {
                    Directory.CreateDirectory(debugDbfilePath);
                }
                dbFilePath = string.Format("DataSource={0}TradeManageDB.db", debugDbfilePath);
            }

            ////创建litedb实例
            //litedb = new LiteDatabase(@dbFilePath);
            //string connectionString = string.Format("DataSource={0}TradeManageDB.db", @dbFilePath);
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = dbFilePath,
                DbType = SqlSugar.DbType.Sqlite,
                IsAutoCloseConnection = true,//自动释放
            });

            //初始化表
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(TradeManageItem));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(UserInfo));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(UserStorage));
        }
    }
}
