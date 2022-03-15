using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class AwardInfoService
    {
        /// <summary>
        /// 添加新的红包
        /// </summary>
        /// <param name="da"></param>
        public static void addAwardInfo(AwardInfo da)
        {
            DataBase.db.Insertable<AwardInfo>(da).ExecuteCommand();
        }


        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static AwardInfo getAwardInfoByid(string id)
        {
            return DataBase.db.Queryable<AwardInfo>().Where(string.Format("id = '{0}' ", id)).First();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<AwardInfo> getAwardInfos(string containerid, string funcid)
        {
            return DataBase.db.Queryable<AwardInfo>().Where(string.Format("status = '1' and containerid = {0} and funcid={1}", containerid, funcid)).ToList();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static void deleteAwardInfos(string id)
        {
            //字典
            var dt = new Dictionary<string, object>();
            dt.Add("id", id);
            dt.Add("status", "0");
            var t66 = DataBase.db.Updateable(dt).AS("AwardInfo").WhereColumns("id").ExecuteCommand();
        }

        public static void UpdateAwardInfo(AwardInfo da)
        {
            DataBase.db.Updateable<AwardInfo>(da).ExecuteCommand();
        }

    }
}
