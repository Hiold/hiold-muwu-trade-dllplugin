using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class ActionLogService
    {
        /// <summary>
        /// 插入游戏内物品数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void addLog(ActionLog log)
        {
            DataBase.logdb.Insertable<ActionLog>(log).ExecuteCommand();
        }

        /// <summary>
        /// 查询用户操作日志
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, object> QueryLogs(string userid, string type, int pageIndex, int pageSize)
        {
            int totalCount = 0;
            if (type.Equals("all"))
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                List<ActionLog> ls = DataBase.logdb.Queryable<ActionLog>().Where(string.Format("atcPlayerEntityId='{0}' order by actTime desc", userid)).ToPageList(pageIndex, pageSize, ref totalCount);
                result.Add("data", ls);
                result.Add("count", totalCount);
                return result;
            }
            else
            {
                return null;
            }
        }


        public static Int64 QueryItemLogCount(string id, string itemid, int logtype, string startTime, string endTime)
        {
            DataRow[] dt = null;
            if (startTime == null && endTime == null)
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo3) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' ", id, itemid, logtype)).Select();
            }
            else
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo3) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", id, itemid, logtype, startTime, endTime)).Select();
            }
            //Console.WriteLine(dt);
            foreach (DataRow row in dt)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        return (Int64)data;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }


        /// <summary>
        /// 查询用户操作日志
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ActionLog> QueryDailyAwardPull(string userid, string hbid)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<ActionLog> ls = DataBase.logdb.Queryable<ActionLog>().Where(string.Format("atcPlayerEntityId='{0}' and extinfo1={1} and actType={2} order by actTime desc", userid, hbid, LogType.pullGetDailyAward)).ToList();
            return ls;
        }

        public static Int64 QueryProgresionCount(string id, string itemid, int logtype, string startTime, string endTime)
        {
            DataRow[] dt = null;
            if (startTime == null && endTime == null)
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' ", id, itemid, logtype)).Select();
            }
            else
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", id, itemid, logtype, startTime, endTime)).Select();
            }
            //Console.WriteLine(dt);
            foreach (DataRow row in dt)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        return (Int64)data;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 查询签到
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="hbid"></param>
        /// <returns></returns>
        public static Int64 QuerySignInfoCount(string userid, string date, string day)
        {
            DataRow[] dt = null;
            //循环签到奖励
            if (date == null || date.Equals("0001-01-01"))
            {
                string[] dataPair = ServerUtils.getDayOfThisWeek();
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo5='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", userid, day, LogType.doSignInfo, dataPair[0], dataPair[1])).Select();
                if (dt != null)
                {
                    foreach (DataRow row in dt)
                    {
                        foreach (object data in row.ItemArray)
                        {
                            try
                            {
                                if ((Int64)data > 0) { return (Int64)data; }

                            }
                            catch (Exception)
                            {
                                return 0;
                            }
                        }
                    }
                }
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo5='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", userid, day, LogType.doReSignInfo, dataPair[0], dataPair[1])).Select();
                if (dt != null)
                {
                    foreach (DataRow row in dt)
                    {
                        foreach (object data in row.ItemArray)
                        {
                            try
                            {
                                if ((Int64)data > 0) { return (Int64)data; }
                            }
                            catch (Exception)
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            //指定日期签到奖励
            else
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo4='{1}' and t.actType='{2}' ", userid, date, LogType.doSignInfo)).Select();
                if (dt != null)
                {
                    foreach (DataRow row in dt)
                    {
                        foreach (object data in row.ItemArray)
                        {
                            try
                            {
                                if ((Int64)data > 0) { return (Int64)data; }
                            }
                            catch (Exception)
                            {
                                return 0;
                            }
                        }
                    }
                }
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo4='{1}' and t.actType='{2}' ", userid, date, LogType.doReSignInfo)).Select();
                if (dt != null)
                {
                    foreach (DataRow row in dt)
                    {
                        foreach (object data in row.ItemArray)
                        {
                            try
                            {
                                if ((Int64)data > 0) { return (Int64)data; }
                            }
                            catch (Exception)
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            //Console.WriteLine(dt);
            return 0;
        }
    }
}
