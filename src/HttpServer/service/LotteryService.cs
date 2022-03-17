using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class LotteryService
    {
        /// <summary>
        /// 添加新的抽奖
        /// </summary>
        /// <param name="da"></param>
        public static void addLottery(Lottery da)
        {
            DataBase.db.Insertable<Lottery>(da).ExecuteCommand();
        }


        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static Lottery getLotteryByid(string id)
        {
            return DataBase.db.Queryable<Lottery>().Where(string.Format("id = '{0}' ", id)).First();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<Lottery> getLotterys()
        {
            return DataBase.db.Queryable<Lottery>().Where(string.Format("status = '1'")).ToList();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static void deleteLotterys(string id)
        {
            //字典
            var dt = new Dictionary<string, object>();
            dt.Add("id", id);
            dt.Add("status", "0");
            var t66 = DataBase.db.Updateable(dt).AS("Lottery").WhereColumns("id").ExecuteCommand();
        }

        public static void UpdateLottery(Lottery da)
        {
            DataBase.db.Updateable<Lottery>(da).ExecuteCommand();
        }

    }
}
