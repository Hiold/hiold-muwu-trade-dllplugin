using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class ProgressionTService
    {
        /// <summary>
        /// 添加新的红包
        /// </summary>
        /// <param name="da"></param>
        public static void addProgressionT(ProgressionT da)
        {
            DataBase.db.Insertable<ProgressionT>(da).ExecuteCommand();
        }


        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static ProgressionT getProgressionTByid(string id)
        {
            return DataBase.db.Queryable<ProgressionT>().Where(string.Format("id = '{0}' ", id)).First();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<ProgressionT> getProgressionTs()
        {
            return DataBase.db.Queryable<ProgressionT>().Where(string.Format("status = '1' ")).ToList();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static void deleteProgressionTs(string id)
        {
            //字典
            var dt = new Dictionary<string, object>();
            dt.Add("id", id);
            dt.Add("status", "0");
            var t66 = DataBase.db.Updateable(dt).AS("progression").WhereColumns("id").ExecuteCommand();
        }

        public static void UpdateProgressionT(ProgressionT da)
        {
            DataBase.db.Updateable<ProgressionT>(da).ExecuteCommand();
        }

    }
}
