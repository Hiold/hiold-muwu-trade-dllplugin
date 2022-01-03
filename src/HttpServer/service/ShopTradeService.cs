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
        public static void addShopItem(TradeManageItem item)
        {
            DataBase.db.Insertable<TradeManageItem>(item).ExecuteCommand();
        }

        /// <summary>
        /// 查询系统商店售卖物品
        /// </summary>
        /// <param name="itemname">物品名</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <returns></returns>
        public static Dictionary<string, object> queryShopItem(string itemname, int pageIndex, int pageSize)
        {
            int totalCount = 0;
            List<TradeManageItem> ls = DataBase.db.Queryable<TradeManageItem>().Where(string.Format("(name like '%{0}%' or translate like '%{0}%') and deleteTime is not null", itemname)).ToPageList(pageIndex, pageSize, ref totalCount);
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("data", ls);
            result.Add("count", totalCount);
            return result;
        }



        /// <summary>
        /// 根据ID获取系统商店售卖物品
        /// </summary>
        /// <param name="itemname">物品名</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <returns></returns>
        public static List<TradeManageItem> getShopItemById(int id)
        {
            List<TradeManageItem> result = DataBase.db.Queryable<TradeManageItem>().Where(string.Format("id='{0}' and deleteTime is not null", id)).ToList();
            return result;
        }



        /// <summary>
        /// 更新游戏内物品数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void updateShopItem(TradeManageItem item)
        {
            DataBase.db.Updateable(item).ExecuteCommand(); ;
        }
    }
}
