using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.service;
using LiteDB;
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
        /// 翻译action
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

                Console.WriteLine(couDateStart);
                Console.WriteLine(DateTime.Parse(xgDateStart));
                //游戏内物品
                SpecialItem shopItem = new SpecialItem()
                {
                    id = ObjectId.NewObjectId(),
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
                    dateStart = DateTime.Parse(xgDateStart),
                    dateEnd = DateTime.Parse(xgDateEnd),
                    couCurrType = couCurrType,
                    couCond = couCond,
                    couPrice = couPrice,
                    count = itemnum,
                    couDateStart = DateTime.Parse(couDateStart),
                    couDateEnd = DateTime.Parse(couDateEnd),
                };

                Console.WriteLine(shopItem);

                ShopTradeService.addShopItem(shopItem);
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
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
        public string itemnum { get; set; }
        public int quality { get; set; }
        public string itemGroup { get; set; }
        public string itemIcon { get; set; }
        public string itemTint { get; set; }

    }


}
