using HioldMod.HttpServer;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.service;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;

namespace HioldMod.src.HttpServer.database
{
    public class DataBase
    {
        //业务数据库路径
        private static string maindbPath;
        //日志数据库路径
        private static string logdbPath;
        //游戏进程日志数据库路径
        private static string gameeventdbPath;
        //数据库客户端

        //调试使用的数据库文件存储路径
        public static string relatedPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
        public static string debugDbfilePath = @"C:\Users\Administrator\Source\Repos\hiold-muwu-trade-dllplugin\db\";
        public static SqlSugarClient db = null;
        public static SqlSugarClient logdb = null;
        public static SqlSugarClient gameeventdb = null;

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public static void InitDataBase()
        {

            ////创建数据库链接
            if (HioldMod.API.isOnServer)
            {
                //在服务器端运行获取服务器路径
                string modDBDir = string.Format("{0}database/", HioldMod.API.AssemblyPath);
                //判断是否存在路径
                if (!Directory.Exists(modDBDir))
                {
                    Directory.CreateDirectory(modDBDir);
                }
                maindbPath = string.Format("DataSource={0}TradeManageDB.db", modDBDir);
                logdbPath = string.Format("DataSource={0}Log.db", modDBDir);
                gameeventdbPath = string.Format("DataSource={0}GameEventLog.db", modDBDir);

            }
            else
            {
                //加载固定路径
                try
                {
                    debugDbfilePath = relatedPath.Substring(0, relatedPath.IndexOf("Repos\\")) + "Repos\\hiold-muwu-trade-dllplugin\\db\\";
                }
                catch (Exception)
                {
                    debugDbfilePath = relatedPath.Substring(0, relatedPath.IndexOf("repos\\")) + "repos\\hiold-muwu-trade-dllplugin\\db\\";
                }

                if (!Directory.Exists(debugDbfilePath))
                {
                    Directory.CreateDirectory(debugDbfilePath);
                }
                maindbPath = string.Format("DataSource={0}TradeManageDB.db", debugDbfilePath);
                logdbPath = string.Format("DataSource={0}Log.db", debugDbfilePath);
                gameeventdbPath = string.Format("DataSource={0}GameEventLog.db", debugDbfilePath);
            }

            ////创建litedb实例
            //litedb = new LiteDatabase(@dbFilePath);
            //string connectionString = string.Format("DataSource={0}TradeManageDB.db", @dbFilePath);
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = maindbPath,
                DbType = SqlSugar.DbType.Sqlite,
                IsAutoCloseConnection = true,//自动释放
            });
            //初始化logdb 日志存储数据库
            logdb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = logdbPath,
                DbType = SqlSugar.DbType.Sqlite,
                IsAutoCloseConnection = true,//自动释放
            });

            //初始化游戏进程日志存储数据库
            gameeventdb = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = gameeventdbPath,
                DbType = SqlSugar.DbType.Sqlite,
                IsAutoCloseConnection = true,//自动释放
            });

            Console.WriteLine("开始同步表结构");
            //初始化表
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(TradeManageItem));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(UserInfo));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(UserStorage));
            logdb.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(ActionLog));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(UserConfig));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(UserTrade));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(UserRequire));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(ProgressionT));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(ItemExchange));
            //红包与奖励配置
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(DailyAward));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(AwardInfo));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(Lottery));
            db.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(SignInfo));
            //游戏进程相关数据库表
            gameeventdb.CodeFirst.SetStringDefaultLength(512).InitTables(typeof(PlayerGameEvent));
        }

        /// <summary>
        /// 修改玩家积分、点券数量
        /// </summary>
        /// <param name="info">玩家信息</param>
        /// <param name="mt">货币类型</param>
        /// <param name="et">编辑类型</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static bool MoneyEditor(UserInfo info, MoneyType mt, EditType et, double count)
        {
            List<UserInfo> targetUsers = UserService.getUserById(info.id + "");
            if (targetUsers != null && targetUsers.Count > 0)
            {
                UserInfo _target = targetUsers[0];
                //积分
                if (mt == MoneyType.Money)
                {
                    if (et == EditType.Add)
                    {
                        //运行在服务器 并启用了baiwazibot
                        if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                        {
                            int result = add(info, count);
                            _target.money += result;
                        }
                        else
                        {
                            _target.money += count;
                        }

                        UserService.UpdateUserInfo(_target);
                        return true;
                    }
                    if (et == EditType.Sub)
                    {
                        //积分不足
                        if (_target.money < count)
                        {
                            return false;
                        }

                        //运行在服务器 并启用了baiwazibot
                        if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                        {
                            int result = sub(info, count);
                            _target.money -= result;
                        }
                        else
                        {
                            _target.money -= count;
                        }

                        UserService.UpdateUserInfo(_target);
                        return true;
                    }
                    if (et == EditType.Set)
                    {

                        //运行在服务器 并启用了baiwazibot
                        if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                        {
                            set(info, count);
                            _target.money = count;
                        }
                        else
                        {
                            _target.money = count;
                        }


                        UserService.UpdateUserInfo(_target);
                        return true;
                    }
                }

                //点券
                else if (mt == MoneyType.Credit)
                {
                    if (et == EditType.Add)
                    {
                        _target.credit += count;
                        UserService.UpdateUserInfo(_target);
                        return true;
                    }
                    if (et == EditType.Sub)
                    {
                        //积分不足
                        if (_target.credit < count)
                        {
                            return false;
                        }
                        _target.credit -= count;
                        UserService.UpdateUserInfo(_target);
                        return true;
                    }
                    if (et == EditType.Set)
                    {
                        _target.credit = count;
                        UserService.UpdateUserInfo(_target);
                        return true;
                    }
                }
            }

            return false;
        }

        private static int add(UserInfo info, double c)
        {
            NaiwaziServerKitInterface.NaiwaziPointHelper.AddPoint("EOS_" + info.platformid, (int)c, out int result);
            return result;
        }
        private static int sub(UserInfo info, double c)
        {
            NaiwaziServerKitInterface.NaiwaziPointHelper.SubPoint("EOS_" + info.platformid, (int)c, out int result);
            return result;
        }
        private static void set(UserInfo info, double c)
        {
            NaiwaziServerKitInterface.NaiwaziPointHelper.SetPoint("EOS_" + info.platformid, (int)c);
        }

        public enum MoneyType
        {
            Money, Credit
        }

        public enum EditType
        {
            Add, Sub, Set
        }
    }
}
