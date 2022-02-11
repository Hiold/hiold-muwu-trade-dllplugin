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
        public static Dictionary<string, object> queryShopItem(string itemname, int pageIndex, int pageSize, string mainType, string gruopsStrs)
        {
            int totalCount = 0;
            string groupStr = "";
            string mainTypeStr = "";
            //包含group信息
            if (gruopsStrs != null && gruopsStrs.Length > 0)
            {
                groupStr += " and (";
                string[] groups = gruopsStrs.Split('/');
                for (int i = 0; i < groups.Length; i++)
                {
                    if (i == 0)
                    {
                        groupStr += string.Format(" class1 like '%{0}%' or class2 like '%{0}%'", groups[i]);
                    }
                    else
                    {
                        groupStr += string.Format(" or class1 like '%{0}%' or class2 like '%{0}%'", groups[i]);
                    }
                }
                groupStr += ") ";
            }
            //包含大分类信息
            if (mainType != null && mainType.Length > 0)
            {
                if (mainType.Equals("活动折扣"))
                {
                    mainTypeStr += " and discount<10 ";
                }
                if (mainType.Equals("特殊商品"))
                {
                    mainTypeStr += " and itemtype='2' ";
                }
                if (mainType.Equals("积分商城"))
                {
                    mainTypeStr += " and currency='1' ";
                }
                if (mainType.Equals("钻石商城"))
                {
                    mainTypeStr += " and currency='2' ";
                }
            }


            List<TradeManageItem> ls = DataBase.db.Queryable<TradeManageItem>().Where(string.Format("(name like '%{0}%' or translate like '%{0}%') and deleteTime ='0001-01-01 00:00:00'" + groupStr + mainTypeStr, itemname)).ToPageList(pageIndex, pageSize, ref totalCount);
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
        /// 根据用户ID获取玩家折扣券数据
        /// </summary>
        /// <param name="itemname">物品名</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <returns></returns>
        public static List<TradeManageItem> getUserDiscountTicket(int userid)
        {
            List<TradeManageItem> result = DataBase.db.Queryable<TradeManageItem>().Where(string.Format("id='{0}' and deleteTime is not null", userid)).ToList();
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
