using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class ShopTradeService
    {
        /// <summary>
        /// 插入游戏内物品数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void addShopItem(SpecialItem item)
        {
            var col = DataBase.litedb.GetCollection<SpecialItem>("ShopItem");
            col.Insert(item);
        }

        //public static List<SpecialItem> queryShopItem(string itemname, int page, int limit)
        //{

        //}
    }
}
