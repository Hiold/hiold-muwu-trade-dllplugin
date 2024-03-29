﻿using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class UserRequireService
    {
        /// <summary>
        /// 向数据库中插入玩家求购数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void addUserRequire(UserRequire storage)
        {
            DataBase.db.Insertable<UserRequire>(storage).ExecuteCommand();
        }


        /// <summary>
        /// 根据id获取求购数据
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        public static UserRequire selectUserRequireByid(string id)
        {
            return DataBase.db.Queryable<UserRequire>().Where(string.Format("id = '{0}' ", id)).First();
        }


        /// <summary>
        /// 根据用户id获取求购数据
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <returns></returns>
        public static List<UserRequire> selectUserRequiresByUserid(string playerid, string gruopsStrs, string name, string sort)
        {
            string groupStr = "";
            string sortStr = "";
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.Equals("价格高到低"))
                {
                    sortStr += "order by Price desc";
                }
                if (sort.Equals("价格低到高"))
                {
                    sortStr += "order by Price asc";
                }
            }
            if (gruopsStrs != null && gruopsStrs.Length > 0)
            {
                groupStr += " and (";
                string[] groups = gruopsStrs.Split('/');
                for (int i = 0; i < groups.Length; i++)
                {
                    if (i == 0)
                    {
                        groupStr += string.Format(" Itemgroups like '%{0}%' ", groups[i]);
                    }
                    else
                    {
                        groupStr += string.Format(" or Itemgroups like '%{0}%' ", groups[i]);
                    }
                }
                groupStr += ") ";
            }

            if (string.IsNullOrEmpty(playerid))
            {
                return DataBase.db.Queryable<UserRequire>().Where(string.Format("Status = '{0}' and Itemchinese like '%{1}%' " + groupStr + sortStr, UserRequireConfig.NORMAL_REQUIRE, name)).ToList();
            }
            else
            {
                return DataBase.db.Queryable<UserRequire>().Where(string.Format("Status = '{0}' and gameentityid='{1}' and Itemchinese like '%{2}%' " + groupStr + sortStr, UserRequireConfig.NORMAL_REQUIRE, playerid, name)).ToList();
            }
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

            List<UserRequire> ls = DataBase.db.Queryable<UserRequire>().Where(string.Format("gameentityid = '{0}' and stock > 0 and (name like '%{1}%' or translate like '%{1}%') and deleteTime ='0001-01-01 00:00:00' and itemStatus='1' " + groupStr + mainTypeStr, playerid, itemname)).ToPageList(pageIndex, pageSize, ref totalCount);
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
        public static List<UserRequire> selectPlayerRequire(string id)
        {
            return DataBase.db.Queryable<UserRequire>().Where(string.Format("id = '{0}' ", id)).ToList();
        }

        /// <summary>
        /// 更新物品信息
        /// </summary>
        /// <param name="item">物品</param>
        public static void UpdateUserRequire(UserRequire storage)
        {
            DataBase.db.Updateable<UserRequire>(storage).ExecuteCommand();
        }
    }
}
