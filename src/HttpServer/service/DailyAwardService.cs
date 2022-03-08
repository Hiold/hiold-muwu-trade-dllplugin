using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class DailyAwardService
    {
        /// <summary>
        /// 添加新的红包
        /// </summary>
        /// <param name="da"></param>
        public static void addDailyAward(DailyAward da)
        {
            DataBase.db.Insertable<DailyAward>(da).ExecuteCommand();
        }


        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static DailyAward getDailyAwardByid(string id)
        {
            return DataBase.db.Queryable<DailyAward>().Where(string.Format("id = '{0}' ", id)).First();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<DailyAward> getDailyAwards()
        {
            return DataBase.db.Queryable<DailyAward>().Where(string.Format("status = '1' ")).ToList();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static void deleteDailyAwards(string id)
        {
            //字典
            var dt = new Dictionary<string, object>();
            dt.Add("id", id);
            dt.Add("status", "0");
            var t66 = DataBase.db.Updateable(dt).AS("dailyaward").WhereColumns("id").ExecuteCommand();
        }

        public static void UpdateDailyAward(DailyAward da)
        {
            DataBase.db.Updateable<DailyAward>(da).ExecuteCommand();
        }

    }
}
