using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.service;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
        public static string debugDbfilePath = "E:/database/";

        public static LiteDatabase litedb;

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void InitTable()
        {
            ////创建数据库链接
            //if (API.isOnServer)
            //{
            //    //在服务器端运行获取服务器路径
            //    string modDBDir = string.Format("{0}database/", API.AssemblyPath);
            //    //判断是否存在路径
            //    if (!Directory.Exists(modDBDir))
            //    {
            //        Directory.CreateDirectory(modDBDir);
            //    }
            //    dbFilePath = string.Format("{0}database.db", modDBDir);

            //}
            //else
            //{
            //    if (!Directory.Exists(debugDbfilePath))
            //    {
            //        Directory.CreateDirectory(debugDbfilePath);
            //    }
            //    dbFilePath = string.Format("{0}database.db", debugDbfilePath);
            //}

            ////创建litedb实例
            //litedb = new LiteDatabase(@dbFilePath);
            const string connectionString = "URI=file:SqliteTest.db";
            IDbConnection dbcon = new SQLiteConnection(connectionString);
            dbcon.Open();
            IDbCommand dbcmd = dbcon.CreateCommand();
            // requires a table to be created named employee
            // with columns firstname and lastname
            // such as,
            //        CREATE TABLE employee (
            //           firstname nvarchar(32),
            //           lastname nvarchar(32));
            const string sql =
               "SELECT firstname, lastname " +
               "FROM employee";
            dbcmd.CommandText = sql;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                string firstName = reader.GetString(0);
                string lastName = reader.GetString(1);
                Console.WriteLine("Name: {0} {1}",
                    firstName, lastName);
            }
            // clean up
            reader.Dispose();
            dbcmd.Dispose();
            dbcon.Close();
        }
    }
}
