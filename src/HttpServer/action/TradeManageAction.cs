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
        public static void addShopItem(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数并进行强制类型转换
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                addRequestBean addRequest = new addRequestBean();
                addRequest = (addRequestBean)SimpleJson2.SimpleJson2.DeserializeObject(postData, addRequest.GetType());

                //类型转换

                //游戏内物品
                TradeManageItem shopItem = new TradeManageItem()
                {
                    itemtype = addRequest.itemType,
                    name = addRequest.itemName,
                    itemicon = addRequest.itemIcon,
                    itemtint = addRequest.itemTint,
                    quality = addRequest.quality,
                    num = int.Parse(addRequest.itemnum),
                    currency = addRequest.currency,
                    price = double.Parse(addRequest.price),
                    discount = addRequest.discount,
                    prefer = double.Parse(addRequest.prefer),
                    desc = addRequest.desc,
                    class1 = addRequest.itemGroup,
                    class2 = addRequest.itemGroup,
                    classmod = addRequest.itemType,
                    hot = addRequest.hot,
                    hotset = int.Parse(addRequest.hotset),
                    show = "1",
                    stock = int.Parse(addRequest.stock),
                    xglevel = addRequest.xglevel,
                    xglevelset = addRequest.xglevelset,
                    follow = addRequest.fallow,
                    xgall = addRequest.xgall,
                    xgallset = addRequest.xgallset,
                    xgdatelimit = addRequest.xgdatelimit,
                    dateStart = DateTime.Parse(addRequest.xgDate[0]),
                    dateEnd = DateTime.Parse(addRequest.xgDate[1]),
                    couCurrType = addRequest.couCurrType,
                    couCond = addRequest.couCond,
                    couPrice = addRequest.couPrice,
                    count = addRequest.itemnum,
                    coudatelimit = addRequest.coudatelimit,
                    couDateStart = DateTime.Parse(addRequest.couDate[0]),
                    couDateEnd = DateTime.Parse(addRequest.couDate[1]),
                    postTime = DateTime.Now,
                    selltype = int.Parse(addRequest.sellType),
                    selloutcount = "0",
                    collect = 0,
                    collected = "0",
                    translate = addRequest.translate,
                    xgday = addRequest.xgday,
                    xgdayset = addRequest.xgdayset,
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
        public static void updateShopItem(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数并进行强制类型转换
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                addRequestBean addRequest = new addRequestBean();
                addRequest = (addRequestBean)SimpleJson2.SimpleJson2.DeserializeObject(postData, addRequest.GetType());
                //类型转换
                List<TradeManageItem> olditem = ShopTradeService.getShopItemById(addRequest.id);

                if (olditem == null || olditem.Count <= 0)
                {
                    ResponseUtils.ResponseFail(response, "更新失败，未查询到原数据");
                    return;
                }

                //游戏内物品
                TradeManageItem shopItem = olditem[0];

                //Console.WriteLine("热卖:" + hot);

                shopItem.id = addRequest.id;
                shopItem.itemtype = addRequest.itemType;
                shopItem.name = addRequest.itemName;
                shopItem.itemicon = addRequest.itemIcon;
                shopItem.itemtint = addRequest.itemTint;
                shopItem.quality = addRequest.quality;
                shopItem.num = int.Parse(addRequest.itemnum);
                shopItem.currency = addRequest.currency;
                shopItem.price = double.Parse(addRequest.price);
                shopItem.discount = addRequest.discount;
                shopItem.prefer = double.Parse(addRequest.prefer);
                shopItem.desc = addRequest.desc;
                shopItem.class1 = addRequest.itemGroup;
                shopItem.class2 = addRequest.itemGroup;
                shopItem.classmod = addRequest.itemType;
                shopItem.hot = addRequest.hot;
                shopItem.hotset = int.Parse(addRequest.hotset);
                shopItem.stock = int.Parse(addRequest.stock);
                shopItem.xglevel = addRequest.xglevel;
                shopItem.xglevelset = addRequest.xglevelset;
                shopItem.follow = addRequest.fallow;
                shopItem.xgall = addRequest.xgall;
                shopItem.xgallset = addRequest.xgallset;
                shopItem.dateStart = DateTime.Parse(addRequest.xgDate[0]);
                shopItem.dateEnd = DateTime.Parse(addRequest.xgDate[1]);
                shopItem.couCurrType = addRequest.couCurrType;
                shopItem.couCond = addRequest.couCond;
                shopItem.couPrice = addRequest.couPrice;
                shopItem.count = addRequest.itemnum;
                shopItem.couDateStart = DateTime.Parse(addRequest.couDate[0]);
                shopItem.couDateEnd = DateTime.Parse(addRequest.couDate[1]);
                shopItem.xgday = addRequest.xgday;
                shopItem.xgdayset = addRequest.xgdayset;
                shopItem.xgdatelimit = addRequest.xgdatelimit;
                shopItem.coudatelimit = addRequest.coudatelimit;
                shopItem.postTime = DateTime.Now;
                shopItem.translate = addRequest.translate;

                ShopTradeService.updateShopItem(shopItem);
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.ToString());
                ResponseUtils.ResponseFail(response, "参数异常");
            }

        }


        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void deleteShopItem(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数并进行强制类型转换
            try
            {
                string postData = ServerUtils.getPostData(request.request);
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
        public static void queryShopItem(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
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

                queryRequest.TryGetValue("class1", out string class1);
                queryRequest.TryGetValue("class2", out string class2);

                Dictionary<string, object> result = ShopTradeService.queryShopItem(itemname, pageIndex, pageSize, class1 ,class2);
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
        public string xgdatelimit { get; set; }
        public string coudatelimit { get; set; }
        public string[] xgDate { get; set; }
        public string[] couDate { get; set; }
        public string couCurrType { get; set; }
        public string couPrice { get; set; }
        public string couCond { get; set; }
        public string fallow { get; set; }
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
        //登记限购
        public string xglevel { get; set; }
        public string xglevelset { get; set; }
        //每日限购
        public string xgday { get; set; }
        public string xgdayset { get; set; }
        //总限购
        public string xgall { get; set; }
        public string xgallset { get; set; }
    }


}
