using HioldMod.HttpServer;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using SqlSugar;
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
        public static Dictionary<string, object> queryShopItem(UserInfo userInfo, string itemname, int pageIndex, int pageSize, string mainType, string gruopsStrs, string sorttype)
        {
            int totalCount = 0;
            string groupStr = "";
            string mainTypeStr = "";
            string sortStr = "";
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
            //排序处理
            if (sorttype != null)
            {
                if (sorttype.Equals("默认排序"))
                {
                    sortStr = "";
                }
                if (sorttype.Equals("销量优先"))
                {
                    sortStr = " order by selloutcount desc";
                }
                if (sorttype.Equals("收藏最多"))
                {
                    sortStr = " order by collect desc";
                }
                if (sorttype.Equals("价格低到高"))
                {
                    sortStr = " order by price asc";
                }
                if (sorttype.Equals("价格高到低"))
                {
                    sortStr = " order by price desc";
                }
            }


            List<TradeManageItem> ls = DataBase.db.Queryable<TradeManageItem>().Where(string.Format("(name like '%{0}%' or translate like '%{0}%') and deleteTime ='0001-01-01 00:00:00'" + groupStr + mainTypeStr + sortStr, itemname)).ToPageList(pageIndex, pageSize, ref totalCount);
            //逐一查询收藏数据
            if (userInfo != null)
            {
                foreach (TradeManageItem item in ls)
                {
                    List<UserConfig> cfgs = UserConfigService.QueryConfig(userInfo.gameentityid, ConfigType.Collect, item.id + "");
                    if (cfgs != null && cfgs.Count > 0)
                    {
                        UserConfig cfg = cfgs[0];
                        item.collected = cfg.available;
                    }
                }
            }


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
        /// 根据ID获取系统商店售卖物品
        /// </summary>
        /// <param name="itemname">物品名</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">数量</param>
        /// <returns></returns>
        public static List<TradeManageItem> getShopItemByIdWithCollect(int id, UserInfo userInfo)
        {
            List<TradeManageItem> result = DataBase.db.Queryable<TradeManageItem>().Where(string.Format("id='{0}' and deleteTime is not null", id)).ToList();
            //逐一查询收藏数据
            if (userInfo != null)
            {
                foreach (TradeManageItem item in result)
                {
                    List<UserConfig> cfgs = UserConfigService.QueryConfig(userInfo.gameentityid, ConfigType.Collect, item.id + "");
                    if (cfgs != null && cfgs.Count > 0)
                    {
                        UserConfig cfg = cfgs[0];
                        item.collected = cfg.available;
                    }
                }
            }
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

        /// <summary>
        /// 增加collect
        /// </summary>
        /// <param name="id"></param>
        public static void updateCollectAdd(string id)
        {
            foreach (TradeManageItem ti in getShopItemById(int.Parse(id)))
            {

                //字典
                var dt = new Dictionary<string, object>();
                dt.Add("id", id);
                dt.Add("collect", ti.collect + 1);
                var t66 = DataBase.db.Updateable(dt).AS("shopitem").WhereColumns("id").ExecuteCommand();
            }
        }


        /// <summary>
        /// 增加collect
        /// </summary>
        /// <param name="id"></param>
        public static void updateCollectSub(string id)
        {
            foreach (TradeManageItem ti in getShopItemById(int.Parse(id)))
            {

                //字典
                var dt = new Dictionary<string, object>();
                dt.Add("id", id);
                dt.Add("collect", ti.collect - 1);
                var t66 = DataBase.db.Updateable(dt).AS("shopitem").WhereColumns("id").ExecuteCommand();
            }
        }
    }
}
