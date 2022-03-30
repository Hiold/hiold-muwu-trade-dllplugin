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
    class PlayerGameEventService
    {
        /// <summary>DataBase.gameeventdb
        /// 插入游戏内物品数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void addLog(PlayerGameEvent log)
        {
            DataBase.gameeventdb.Insertable<PlayerGameEvent>(log).ExecuteCommand();
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
                List<PlayerGameEvent> ls = DataBase.gameeventdb.Queryable<PlayerGameEvent>().Where(string.Format("atcPlayerEntityId='{0}' order by actTime desc", userid)).ToPageList(pageIndex, pageSize, ref totalCount);
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
                dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select sum(extinfo3) cnt from PlayerGameEvent t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' ", id, itemid, logtype)).Select();
            }
            else
            {
                dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select sum(extinfo3) cnt from PlayerGameEvent t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", id, itemid, logtype, startTime, endTime)).Select();
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
        public static List<PlayerGameEvent> QueryDailyAwardPull(string userid, string hbid)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<PlayerGameEvent> ls = DataBase.gameeventdb.Queryable<PlayerGameEvent>().Where(string.Format("atcPlayerEntityId='{0}' and extinfo1={1} and actType={2} order by actTime desc", userid, hbid,LogType.pullGetDailyAward)).ToList();
            return ls;
        }


        /// <summary>
        /// 查询用户操作日志
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, object> QueryEventParam(string steamid, string eosid, string username, string actType, int pageIndex, int pageSize)
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
            List<PlayerGameEvent> ls = DataBase.gameeventdb.Queryable<PlayerGameEvent>().Where(" 1=1 " + whereStr + " order by actTime desc ").ToPageList(pageIndex, pageSize, ref totalCount);
            foreach (PlayerGameEvent al in ls)
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
    }
}
