using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.common;
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
    class TradeManage
    {
        /// <summary>
        /// 翻译action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void addShopItem(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                Dictionary<string, string> param = ServerUtils.GetParam(request);
                string couCurrType = param["couCurrType"];
                string couPrice = param["couPrice"];
                string couCond = param["couCond"];
                string xgDateStart = param["xgDateStart"];
                string xgDateEnd = param["xgDateEnd"];
                string xgAll = param["xgAll"];
                string couDateStart = param["couDateStart"];
                string couDateEnd = param["couDateEnd"];
                string fallow = param["fallow"];
                string level = param["level"];
                string levelset = param["levelset"];
                string hotset = param["hotset"];
                string hot = param["hot"];
                string sellType = param["sellType"];
                string desc = param["desc"];
                string prefer = param["prefer"];
                string discount = param["discount"];
                string itemType = param["itemType"];
                string currency = param["currency"];
                string price = param["price"];
                string itemName = param["itemName"];
                string itemnum = param["itemnum"];
                string quality = param["quality"];
                string itemname = param["itemname"];
                string itemGroup = param["itemGroup"];
                string itemIcon = param["itemIcon"];
                string itemTint = param["itemTint"];

            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }

        }
    }
}
