using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    /// <summary>
    /// 交易管理模块
    /// </summary>
    class TradeManageAction
    {
        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void addShopItem(HttpListenerRequest request, HttpListenerResponse response)
        {
            //获取参数并进行强制类型转换
            try
            {
                string postData = ServerUtils.getPostData(request);
                addRequestBean addRequest = new addRequestBean();
                addRequest = (addRequestBean)SimpleJson2.SimpleJson2.DeserializeObject(postData, addRequest.GetType());
                string couCurrType = addRequest.couCurrType;
                string couPrice = addRequest.couPrice;
                string couCond = addRequest.couCond;
                string xgDateStart = addRequest.xgDate[0];
                string xgDateEnd = addRequest.xgDate[1];
                string xgAll = addRequest.xgAll;
                string couDateStart = addRequest.couDate[0];
                string couDateEnd = addRequest.couDate[1];
                string fallow = addRequest.fallow;
                string level = addRequest.level;
                string levelset = addRequest.levelset;
                string hotset = addRequest.hotset;
                string hot = addRequest.hot;
                string sellType = addRequest.sellType;
                string desc = addRequest.desc;
                string prefer = addRequest.prefer;
                double discount = addRequest.discount;
                string itemType = addRequest.itemType;
                string currency = addRequest.currency;
                string price = addRequest.price;
                string itemName = addRequest.itemName;
                string itemnum = addRequest.itemnum;
                int quality = addRequest.quality;
                string itemname = addRequest.itemName;
                string itemGroup = addRequest.itemGroup;
                string itemIcon = addRequest.itemIcon;
                string itemTint = addRequest.itemTint;
                string xgCount = addRequest.xgCount;
                string stock = addRequest.stock;

                //类型转换

                //游戏内物品
                TradeManageItem shopItem = new TradeManageItem()
                {
                    itemtype = itemType,
                    name = itemName,
                    itemIcon = itemIcon,
                    itemTint = itemTint,
                    quality = quality,
                    num = int.Parse(itemnum),
                    currency = currency,
                    price = double.Parse(price),
                    discount = discount,
                    prefer = double.Parse(prefer),
                    desc = desc,
                    class1 = itemGroup,
                    class2 = itemGroup,
                    classMod = itemType,
                    hot = hot,
                    hotSet = int.Parse(hotset),
                    show = "1",
                    stock = int.Parse(stock),
                    xgLevel = level,
                    xgLevelset = levelset,
                    xgCount = xgCount,
                    follow = fallow,
                    xgAll = xgAll,
                    dateStart = DateTime.Parse(xgDateStart),
                    dateEnd = DateTime.Parse(xgDateEnd),
                    couCurrType = couCurrType,
                    couCond = couCond,
                    couPrice = couPrice,
                    count = itemnum,
                    couDateStart = DateTime.Parse(couDateStart),
                    couDateEnd = DateTime.Parse(couDateEnd),
                    postTime = DateTime.Now,
                    sell = "0",
                    collect = 0,
                    collected = "0"
                };


                ShopTradeService.addShopItem(shopItem);
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }

        }





        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void updateShopItem(HttpListenerRequest request, HttpListenerResponse response)
        {
            //获取参数并进行强制类型转换
            try
            {
                string postData = ServerUtils.getPostData(request);
                addRequestBean addRequest = new addRequestBean();
                addRequest = (addRequestBean)SimpleJson2.SimpleJson2.DeserializeObject(postData, addRequest.GetType());
                string couCurrType = addRequest.couCurrType;
                string couPrice = addRequest.couPrice;
                string couCond = addRequest.couCond;
                string xgDateStart = addRequest.xgDate[0];
                string xgDateEnd = addRequest.xgDate[1];
                string xgAll = addRequest.xgAll;
                string couDateStart = addRequest.couDate[0];
                string couDateEnd = addRequest.couDate[1];
                string fallow = addRequest.fallow;
                string level = addRequest.level;
                string levelset = addRequest.levelset;
                string hotset = addRequest.hotset;
                string hot = addRequest.hot;
                string sellType = addRequest.sellType;
                string desc = addRequest.desc;
                string prefer = addRequest.prefer;
                double discount = addRequest.discount;
                string itemType = addRequest.itemType;
                string currency = addRequest.currency;
                string price = addRequest.price;
                string itemName = addRequest.itemName;
                string itemnum = addRequest.itemnum;
                int quality = addRequest.quality;
                string itemname = addRequest.itemName;
                string itemGroup = addRequest.itemGroup;
                string itemIcon = addRequest.itemIcon;
                string itemTint = addRequest.itemTint;
                string xgCount = addRequest.xgCount;
                string stock = addRequest.stock;
                int id = addRequest.id;
                //类型转换

                List<TradeManageItem> olditem = ShopTradeService.getShopItemById(addRequest.id);

                if (olditem == null || olditem.Count <= 0)
                {
                    ResponseUtils.ResponseFail(response, "更新失败，未查询到原数据");
                    return;
                }

                //游戏内物品
                TradeManageItem shopItem = olditem[0];

                shopItem.id = id;
                shopItem.itemtype = itemType;
                shopItem.name = itemName;
                shopItem.itemIcon = itemIcon;
                shopItem.itemTint = itemTint;
                shopItem.quality = quality;
                shopItem.num = int.Parse(itemnum);
                shopItem.currency = currency;
                shopItem.price = double.Parse(price);
                shopItem.discount = discount;
                shopItem.prefer = double.Parse(prefer);
                shopItem.desc = desc;
                shopItem.class1 = itemGroup;
                shopItem.class2 = itemGroup;
                shopItem.classMod = itemType;
                shopItem.hot = hot;
                shopItem.hotSet = int.Parse(hotset);
                shopItem.stock = int.Parse(stock);
                shopItem.xgLevel = level;
                shopItem.xgLevelset = levelset;
                shopItem.xgCount = xgCount;
                shopItem.follow = fallow;
                shopItem.xgAll = xgAll;
                shopItem.dateStart = DateTime.Parse(xgDateStart);
                shopItem.dateEnd = DateTime.Parse(xgDateEnd);
                shopItem.couCurrType = couCurrType;
                shopItem.couCond = couCond;
                shopItem.couPrice = couPrice;
                shopItem.count = itemnum;
                shopItem.couDateStart = DateTime.Parse(couDateStart);
                shopItem.couDateEnd = DateTime.Parse(couDateEnd);
                shopItem.postTime = DateTime.Now;

                ShopTradeService.updateShopItem(shopItem);
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }

        }


        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void deleteShopItem(HttpListenerRequest request, HttpListenerResponse response)
        {
            //获取参数并进行强制类型转换
            try
            {
                string postData = ServerUtils.getPostData(request);
                addRequestBean addRequest = new addRequestBean();
                addRequest = (addRequestBean)SimpleJson2.SimpleJson2.DeserializeObject(postData, addRequest.GetType());
                int id = addRequest.id;
                //类型转换

                List<TradeManageItem> olditem = ShopTradeService.getShopItemById(addRequest.id);

                if (olditem == null || olditem.Count <= 0)
                {
                    ResponseUtils.ResponseFail(response, "更新失败，未查询到原数据");
                    return;
                }

                //游戏内物品
                TradeManageItem shopItem = olditem[0];
                shopItem.id = id;
                shopItem.deleteTime = DateTime.Now;

                ShopTradeService.updateShopItem(shopItem);
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }

        }





        /// <summary>
        /// 获取ShopItem
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void queryShopItem(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                string itemname = "";
                int pageIndex = 1;
                int pageSize = 10;
                queryRequest.TryGetValue("itemname", out itemname);
                if (queryRequest.TryGetValue("pageIndex", out string pageIndexStr))
                {
                    pageIndex = int.Parse(pageIndexStr);
                }
                if (queryRequest.TryGetValue("pageSize", out string pageSizeStr))
                {
                    pageSize = int.Parse(pageSizeStr);
                }
                Dictionary<string, object> result = ShopTradeService.queryShopItem(itemname, pageIndex, pageSize);
                ResponseUtils.ResponseSuccessWithData(response, result);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.ToString());
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }






    public class addRequestBean
    {
        public string xgCount { get; set; }
        public string stock { get; set; }
        public string[] xgDate { get; set; }
        public string[] couDate { get; set; }
        public string couCurrType { get; set; }
        public string couPrice { get; set; }
        public string couCond { get; set; }
        public string xgDateStart { get; set; }
        public string xgDateEnd { get; set; }
        public string xgAll { get; set; }
        public string couDateStart { get; set; }
        public string couDateEnd { get; set; }
        public string fallow { get; set; }
        public string level { get; set; }
        public string levelset { get; set; }
        public string hotset { get; set; }
        public string hot { get; set; }
        public string sellType { get; set; }
        public string desc { get; set; }
        public string prefer { get; set; }
        public double discount { get; set; }
        public string itemType { get; set; }
        public string currency { get; set; }
        public string price { get; set; }
        public string itemName { get; set; }
        public string translate { get; set; }
        public string itemnum { get; set; }
        public int quality { get; set; }
        public string itemGroup { get; set; }
        public string itemIcon { get; set; }
        public string itemTint { get; set; }
        public int id { get; set; }
    }


}
