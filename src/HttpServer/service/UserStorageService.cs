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
        public static void addShopItem(UserStorage storage)
        {
            DataBase.db.Insertable<UserStorage>(storage).ExecuteCommand();
        }
    }
}
