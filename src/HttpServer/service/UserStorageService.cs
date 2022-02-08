using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class UserStorageService
    {
        /// <summary>
        /// 向数据库中插入玩家库存数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void addUserStorage(UserStorage storage)
        {
            DataBase.db.Insertable<UserStorage>(storage).ExecuteCommand();
        }

        /// <summary>
        /// 根据用户id获取优惠券
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <returns></returns>
        public static List<UserStorage> selectPlayersCou(string playerid)
        {
            return DataBase.db.Queryable<UserStorage>().Where(string.Format("gameentityid = '{0}' and storageCount > 0 and itemtype='2' ", playerid)).ToList();
        }

        /// <summary>
        /// 根据用户id获取物品库存
        /// </summary>
        /// <param name="id">物品id</param>
        /// <returns></returns>
        public static List<UserStorage> selectPlayerStorage(string id)
        {
            return DataBase.db.Queryable<UserStorage>().Where(string.Format("id = '{0}' ", id)).ToList();
        }

        /// <summary>
        /// 更新物品信息
        /// </summary>
        /// <param name="item">物品</param>
        public static void UpdateUserStorage(UserStorage storage)
        {
            DataBase.db.Updateable<UserStorage>(storage).ExecuteCommand();
        }
    }
}
