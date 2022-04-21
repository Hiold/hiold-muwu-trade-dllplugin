using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class UserConfigService
    {
        /// <summary>
        /// 插入配置
        /// </summary>
        /// <param name="item">物品</param>
        public static void addConfig(UserConfig config)
        {
            DataBase.db.Insertable<UserConfig>(config).ExecuteCommand();
        }

        /// <summary>
        /// 更新配置
        /// </summary>
        /// <param name="item">物品</param>
        public static void updateConfig(UserConfig config)
        {
            DataBase.db.Updateable<UserConfig>(config).ExecuteCommand();
        }


        /// <summary>
        /// 获取玩家收藏信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static List<UserConfig> QueryConfig(string gameentityid, string type, string id)
        {
            return DataBase.db.Queryable<UserConfig>().Where(string.Format("gameentityid = '{0}' and configType = '{1}' and configValue = '{2}'", gameentityid, type, id)).ToList();
        }


        /// <summary>
        /// 获取点数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static UserConfig QuerySGJPoint(string gameentityid)
        {
            return DataBase.db.Queryable<UserConfig>().Where(string.Format("gameentityid = '{0}' and configType = '{1}' ", gameentityid, ConfigType.Sgj_Point)).First();
        }


        /// <summary>
        /// 获取玩家所有收藏信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static List<UserConfig> QueryUserCollect(string gameentityid, string type)
        {
            return DataBase.db.Queryable<UserConfig>().Where(string.Format("gameentityid = '{0}' and configType = '{1}' and available='1'", gameentityid, type)).ToList();
        }

        public static Dictionary<string, object> QueryItemLimitConfig(string itemname, string page, string limit)
        {
            int count = 0;
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<UserConfig> list = DataBase.db.Queryable<UserConfig>().Where(string.Format(" configType = '{0}' and name like '%{1}%' ", ConfigType.Item_Limit, itemname)).ToPageList(int.Parse(page), int.Parse(limit), ref count);
            result.Add("count", count);
            result.Add("data", list);
            return result;
        }

        public static UserConfig getUserConfigById(string id)
        {
            return DataBase.db.Queryable<UserConfig>().Where(string.Format(" id = '{0}' ", id)).First();
        }

        public static List<UserConfig> getUserConfigByItemName(string name)
        {
            return DataBase.db.Queryable<UserConfig>().Where(string.Format(" name = '{0}' ", name)).ToList();
        }

    }
}
