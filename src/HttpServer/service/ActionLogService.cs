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
        /// 插入日志
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

        /// <summary>
        /// 查询日志
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemid"></param>
        /// <param name="logtype"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static Int64 QueryItemLogCount(string id, string itemid, int logtype, string startTime, string endTime)
        {
            DataRow[] dt = null;
            if (startTime == null && endTime == null)
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo5) cnt from actionlog t where t.atcPlayerEntityId='{0}' and extinfo4='{1}' and t.actType='{2}' ", id, itemid, logtype)).Select();
            }
            else
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo5) cnt from actionlog t where t.atcPlayerEntityId='{0}' and extinfo4='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", id, itemid, logtype, startTime, endTime)).Select();
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

        /// <summary>
        /// 查询Progression数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemid"></param>
        /// <param name="logtype"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 查询用户操作日志
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, object> QueryLogsParam(string steamid, string eosid, string username, string actType, int pageIndex, int pageSize)
        {
            string whereStr = "";
            int totalCount = 0;
            if (!string.IsNullOrEmpty(steamid))
            {
                whereStr += string.Format(" and atcPlayerEntityId='{0}' ", steamid);
            }

            if (!string.IsNullOrEmpty(eosid))
            {
                string tmp = "and atcPlayerEntityId in (";
                List<UserInfo> uis = UserService.getUserByEOS(eosid);
                if (uis != null && uis.Count > 0)
                {
                    foreach (UserInfo ui in uis)
                    {
                        tmp += string.Format("'{0}',", ui.gameentityid);
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    tmp += ")";
                    whereStr += tmp;
                }
            }

            if (!string.IsNullOrEmpty(username))
            {
                string tmp = "and atcPlayerEntityId in (";
                List<UserInfo> uis = UserService.getUserByName(username);
                if (uis != null && uis.Count > 0)
                {
                    foreach (UserInfo ui in uis)
                    {
                        tmp += string.Format("'{0}',", ui.gameentityid);
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    tmp += ")";
                    whereStr += tmp;
                }
            }
            if (!string.IsNullOrEmpty(actType))
            {
                whereStr += string.Format(" and actType='{0}' ", actType);
            }
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            List<ActionLog> ls = DataBase.logdb.Queryable<ActionLog>().Where(" 1=1 " + whereStr + " order by actTime desc ").ToPageList(pageIndex, pageSize, ref totalCount);
            foreach (ActionLog al in ls)
            {
                Dictionary<string, object> tmp = new Dictionary<string, object>();
                tmp.Add("action", al);
                tmp.Add("userinfo", UserService.getUserBySteamid(al.atcPlayerEntityId));
                data.Add(tmp);
            }
            result.Add("data", data);
            result.Add("count", totalCount);
            return result;
        }

        /// <summary>
        /// 查询活跃用户数
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemid"></param>
        /// <param name="logtype"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static Int64[] QueryUserActionCount(string startTime, string endTime)
        {
            //提前声明数量
            Int64[] result = new Int64[] { 0, 0 };
            //查询接口访问量
            DataRow[] dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.actTime>'{0}' and t.actTime< '{1}' ", startTime, endTime)).Select();
            //Console.WriteLine(dt);
            foreach (DataRow row in dt)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        result[0] = (Int64)data;
                    }
                    catch (Exception)
                    {
                        result[0] = 0;
                    }
                }
            }

            //查询活跃用户量
            DataRow[] dt2 = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) from (SELECT atcPlayerEntityId FROM actionlog where actTime>'{0}' and actTime< '{1}' group by atcPlayerEntityId) ", startTime, endTime)).Select();
            //Console.WriteLine(dt);
            foreach (DataRow row in dt2)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        result[1] = (Int64)data;
                    }
                    catch (Exception)
                    {
                        result[1] = 0;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 查询交易数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemid"></param>
        /// <param name="logtype"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static Int64[] QueryTradeCount(string startTime, string endTime)
        {
            //提前声明数量
            Int64[] result = new Int64[] { 0, 0 };
            //查询数量
            DataRow[] dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.actType='{0}' and t.actTime>'{1}' and t.actTime< '{2}' ", LogType.BuyUserTrade, startTime, endTime)).Select();
            //Console.WriteLine(dt);
            foreach (DataRow row in dt)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        result[0] = (Int64)data;
                    }
                    catch (Exception)
                    {
                        result[0] = 0;
                    }
                }
            }

            //查询金额
            DataRow[] dt2 = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(t.extinfo4) cnt from actionlog t where t.actType='{0}' and t.actTime>'{1}' and t.actTime< '{2}' ", LogType.BuyUserTrade, startTime, endTime)).Select();
            //Console.WriteLine(dt);
            foreach (DataRow row in dt2)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        result[1] = (Int64)data;
                    }
                    catch (Exception)
                    {
                        result[1] = 0;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 查询出售数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemid"></param>
        /// <param name="logtype"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static Int64[] QuerySellCount(string startTime, string endTime)
        {
            //提前声明数量
            Int64[] result = new Int64[] { 0, 0 };
            //查询数量
            DataRow[] dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.actType='{0}' and t.actTime>'{1}' and t.actTime< '{2}' ", LogType.BuyItem, startTime, endTime)).Select();
            //Console.WriteLine(dt);
            foreach (DataRow row in dt)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        result[0] = (Int64)data;
                    }
                    catch (Exception)
                    {
                        result[0] = 0;
                    }
                }
            }

            //查询金额
            DataRow[] dt2 = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(t.extinfo3) cnt from actionlog t where t.actType='{0}' and t.actTime>'{1}' and t.actTime< '{2}' ", LogType.BuyItem, startTime, endTime)).Select();
            //Console.WriteLine(dt);
            foreach (DataRow row in dt2)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        result[1] = (Int64)data;
                    }
                    catch (Exception)
                    {
                        result[1] = 0;
                    }
                }
            }

            return result;
        }

        public static Int64 QueryLikeCount(string actplayer, string targetplayer, string startTime, string endTime)
        {
            //查询数量
            DataRow[] dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.actType='{0}' and t.actTime>'{1}' and t.actTime< '{2}' and atcPlayerEntityId='{3}' and extinfo1='{4}'", LogType.Like, startTime, endTime, actplayer, targetplayer)).Select();
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
        /// 查询正在制作的物品
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ActionLog> getCrafting(string userid, string status)
        {
            List<ActionLog> ls = DataBase.logdb.Queryable<ActionLog>().Where(string.Format("atcPlayerEntityId='{0}' and actType='{1}' and extinfo6='{2}' order by actTime desc", userid, LogType.CraftItem, status)).ToList();
            return ls;
        }

        /// <summary>
        /// 查询正在制作的物品
        /// </summary>
        /// <param name="if"></param>
        /// <returns></returns>
        public static ActionLog getActionLogById(string id)
        {
            return DataBase.logdb.Queryable<ActionLog>().Where(string.Format("id='{0}' ", id)).First();
        }
        //更新数据
        public static void UpdateParam(Dictionary<string, object> dt)
        {
            var t66 = DataBase.logdb.Updateable(dt).AS("actionlog").WhereColumns("id").ExecuteCommand();
        }



        /// <summary>
        /// 查询抽奖次数
        /// </summary>
        /// <returns></returns>
        public static Int64 QueryLotteryCount(string id, string itemid, int logtype, string startTime, string endTime)
        {
            DataRow[] dt = null;
            if (startTime == null && endTime == null)
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' ", id, itemid, logtype)).Select();
            }
            else
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", id, itemid, logtype, startTime, endTime)).Select();
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

    }
}
