using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class UserTradeService
    {
        /// <summary>
        /// 向数据库中插入玩家库存数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void addUserTrade(UserTrade storage)
        {
            DataBase.db.Insertable<UserTrade>(storage).ExecuteCommand();
        }


        /// <summary>
        /// 根据id获取库存数据
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <returns></returns>
        public static UserTrade selectUserTradeByid(string id)
        {
            return DataBase.db.Queryable<UserTrade>().Where(string.Format("id = '{0}' ", id)).First();
        }


        /// <summary>
        /// 根据用户id获取玩家库存
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <param name="itemtype">物品类型</param>
        /// <returns></returns>
        public static Dictionary<string, object> selectPlayersOnSell(string playerid, string itemname, int pageIndex, int pageSize, string mainType, string gruopsStrs)
        {
            int totalCount = 0;
            string groupStr = "";
            string mainTypeStr = "";
            string sortStr = "";
            //包含group信息
            if (gruopsStrs != null && gruopsStrs.Length > 0)
            {
                groupStr += " and (";
                string[] groups = gruopsStrs.Split('/');
                for (int i = 0; i < groups.Length; i++)
                {
                    if (i == 0)
                    {
                        groupStr += string.Format(" class1 like '%{0}%' or class2 like '%{0}%'", groups[i]);
                    }
                    else
                    {
                        groupStr += string.Format(" or class1 like '%{0}%' or class2 like '%{0}%'", groups[i]);
                    }
                }
                groupStr += ") ";
            }
            //包含大分类信息
            if (mainType != null && mainType.Length > 0)
            {
                if (mainType.Equals("普通物品"))
                {
                    mainTypeStr += " and itemtype='1' ";
                }
                if (mainType.Equals("特殊物品"))
                {
                    mainTypeStr += " and itemtype='2' ";
                }
                if (mainType.Equals("积分商城"))
                {
                    mainTypeStr += " and currency='1' ";
                }
                if (mainType.Equals("钻石商城"))
                {
                    mainTypeStr += " and currency='2' ";
                }
            }
            //判断是否要加上用户判断
            string isStr = "";
            if (!string.IsNullOrEmpty(playerid))
            {
                isStr += " and gameentityid = '" + playerid + "'";
            }

            List<UserTrade> ls = DataBase.db.Queryable<UserTrade>().Where(string.Format(" stock > 0 and (name like '%{1}%' or translate like '%{1}%') and deleteTime ='0001-01-01 00:00:00' and itemStatus='1' " + isStr + groupStr + mainTypeStr, playerid, itemname)).ToPageList(pageIndex, pageSize, ref totalCount);
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("data", ls);
            result.Add("count", totalCount);
            return result;
        }




        /// <summary>
        /// 根据用户id获取物品库存
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public static List<UserTrade> selectPlayerStorage(string id)
        {
            return DataBase.db.Queryable<UserTrade>().Where(string.Format("id = '{0}' ", id)).ToList();
        }

        /// <summary>
        /// 更新物品信息
        /// </summary>
        /// <param name="item">物品</param>
        public static void UpdateUserTrade(UserTrade storage)
        {
            DataBase.db.Updateable<UserTrade>(storage).ExecuteCommand();
        }


        /// <summary>
        /// 根据用户id获取玩家库存
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <param name="itemtype">物品类型</param>
        /// <returns></returns>
        public static Dictionary<string, object> selectTradeParam(string steamid, string eosid, string username, string itemname, string mainType, string gruopsStrs,string status, int pageIndex, int pageSize)
        {
            int totalCount = 0;
            string whereStr = "";
            //包含group信息
            if (gruopsStrs != null && gruopsStrs.Length > 0)
            {
                whereStr += " and (";
                string[] groups = gruopsStrs.Split('/');
                for (int i = 0; i < groups.Length; i++)
                {
                    if (i == 0)
                    {
                        whereStr += string.Format(" class1 like '%{0}%' or class2 like '%{0}%'", groups[i]);
                    }
                    else
                    {
                        whereStr += string.Format(" or class1 like '%{0}%' or class2 like '%{0}%'", groups[i]);
                    }
                }
                whereStr += ") ";
            }
            //包含大分类信息
            if (mainType != null && mainType.Length > 0)
            {
                if (mainType.Equals("普通物品"))
                {
                    whereStr += " and itemtype='1' ";
                }
                if (mainType.Equals("特殊物品"))
                {
                    whereStr += " and itemtype='2' ";
                }
                if (mainType.Equals("积分商城"))
                {
                    whereStr += " and currency='1' ";
                }
                if (mainType.Equals("钻石商城"))
                {
                    whereStr += " and currency='2' ";
                }
            }
            //判断是否要加上用户判断
            if (!string.IsNullOrEmpty(steamid))
            {
                whereStr += " and gameentityid = '" + steamid + "' ";
            }
            if (!string.IsNullOrEmpty(eosid))
            {
                whereStr += " and platformid = '" + eosid + "' ";
            }
            if (!string.IsNullOrEmpty(itemname)){
                whereStr += string.Format(" and (name like '%{0}%' or translate like '%{0}%') ", itemname);
            }
            if (!string.IsNullOrEmpty(status))
            {
                whereStr += string.Format(" and itemStatus='{0}' ", status);
            }
            if (!string.IsNullOrEmpty(username))
            {
                whereStr += string.Format(" and username like '%{0}%' ", username);
            }

            List<UserTrade> ls = DataBase.db.Queryable<UserTrade>().Where(string.Format(" 1 = 1 " + whereStr)).ToPageList(pageIndex, pageSize, ref totalCount);
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("data", ls);
            result.Add("count", totalCount);
            return result;
        }
        public static void UpdateParam(Dictionary<string, object> dt)
        {
            var t66 = DataBase.db.Updateable(dt).AS("usertrade").WhereColumns("id").ExecuteCommand();
        }
    }
}
