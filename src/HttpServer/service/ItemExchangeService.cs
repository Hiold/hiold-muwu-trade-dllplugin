using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class ItemExchangeService
    {
        /// <summary>
        /// 添加新的红包
        /// </summary>
        /// <param name="da"></param>
        public static void addItemExchange(ItemExchange da)
        {
            DataBase.db.Insertable<ItemExchange>(da).ExecuteCommand();
        }


        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static ItemExchange getItemExchangeByid(string id)
        {
            return DataBase.db.Queryable<ItemExchange>().Where(string.Format("id = '{0}' ", id)).First();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<ItemExchange> getItemExchanges(string containerid, string funcid)
        {
            return DataBase.db.Queryable<ItemExchange>().Where(string.Format("status = '1' and containerid = {0} and funcid={1}", containerid, funcid)).ToList();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static void deleteItemExchanges(string id)
        {
            //字典
            var dt = new Dictionary<string, object>();
            dt.Add("id", id);
            dt.Add("status", "0");
            var t66 = DataBase.db.Updateable(dt).AS("itemexchange").WhereColumns("id").ExecuteCommand();
        }

        public static void UpdateItemExchange(ItemExchange da)
        {
            DataBase.db.Updateable<ItemExchange>(da).ExecuteCommand();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<ItemExchange> getItemExchangeByType(string searchType, string awardType, string type, string name, string page, string limit)
        {
            if (searchType.Equals("制作物"))
            {
                return DataBase.db.Queryable<ItemExchange>().Where(string.Format("status = '1' and type in ({0}) and itemchinese like '%{1}%' ", type, name)).ToPageList(int.Parse(page), int.Parse(limit));
            }
            else if (searchType.Equals("配方"))
            {
                if (string.IsNullOrEmpty(awardType))
                {
                    return DataBase.db.Queryable<ItemExchange>().Where(string.Format("status = '1' and type in ({0}) and id in (SELECT containerid FROM awardinfo where funcid = '5' and itemchinese like '%{1}%' ) ", type, name)).ToPageList(int.Parse(page), int.Parse(limit));
                }
                else
                {
                    return DataBase.db.Queryable<ItemExchange>().Where(string.Format("status = '1' and type in ({0}) and id in (SELECT containerid FROM awardinfo where funcid = '5' and itemchinese like '%{1}%' and type='{2}' ) ", type, name, awardType)).ToPageList(int.Parse(page), int.Parse(limit));
                }

            }
            else
            {
                return null;
            }

        }

    }
}
