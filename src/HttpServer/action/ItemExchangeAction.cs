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
    class ItemExchangeAction
    {
        public static void postExchange(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("count", out string count);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("itemquality", out string itemquality);
                queryRequest.TryGetValue("itemchinese", out string itemchinese);
                queryRequest.TryGetValue("itemicon", out string itemicon);
                queryRequest.TryGetValue("itemtint", out string itemtint);
                queryRequest.TryGetValue("command", out string command);
                queryRequest.TryGetValue("couCurrType", out string couCurrType);
                queryRequest.TryGetValue("couPrice", out string couPrice);
                queryRequest.TryGetValue("couCond", out string couCond);
                queryRequest.TryGetValue("coudatelimit", out string coudatelimit);
                queryRequest.TryGetValue("couDateStart", out string couDateStart);
                queryRequest.TryGetValue("couDateEnd", out string couDateEnd);
                queryRequest.TryGetValue("desc", out string desc);
                queryRequest.TryGetValue("crafttime", out string crafttime);


                ItemExchangeService.addItemExchange(new ItemExchange()
                {
                    type = type,
                    count = count,
                    itemname = itemname,
                    itemquality = itemquality,
                    itemchinese = itemchinese,
                    itemicon = itemicon,
                    itemtint = itemtint,
                    command = command,
                    couCurrType = couCurrType,
                    couPrice = couPrice,
                    couCond = couCond,
                    coudatelimit = coudatelimit,
                    couDateStart = couDateStart,
                    couDateEnd = couDateEnd,
                    desc = desc,
                    crafttime = crafttime,
                    status = "1",
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    extinfo6 = "",
                });
                ResponseUtils.ResponseSuccess(response);

            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        public static void updateExchange(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("count", out string count);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("itemquality", out string itemquality);
                queryRequest.TryGetValue("itemchinese", out string itemchinese);
                queryRequest.TryGetValue("itemicon", out string itemicon);
                queryRequest.TryGetValue("itemtint", out string itemtint);
                queryRequest.TryGetValue("command", out string command);
                queryRequest.TryGetValue("couCurrType", out string couCurrType);
                queryRequest.TryGetValue("couPrice", out string couPrice);
                queryRequest.TryGetValue("couCond", out string couCond);
                queryRequest.TryGetValue("coudatelimit", out string coudatelimit);
                queryRequest.TryGetValue("couDateStart", out string couDateStart);
                queryRequest.TryGetValue("couDateEnd", out string couDateEnd);
                queryRequest.TryGetValue("desc", out string desc);
                queryRequest.TryGetValue("crafttime", out string crafttime);

                ItemExchange ie = ItemExchangeService.getItemExchangeByid(id);

                ie.type = type;
                ie.count = count;
                ie.itemname = itemname;
                ie.itemquality = itemquality;
                ie.itemchinese = itemchinese;
                ie.itemicon = itemchinese;
                ie.itemtint = itemtint;
                ie.command = command;
                ie.couCurrType = couCurrType;
                ie.couPrice = couPrice;
                ie.couCond = couCond;
                ie.coudatelimit = coudatelimit;
                ie.couDateStart = couDateStart;
                ie.couDateEnd = couDateEnd;
                ie.desc = desc;
                ie.crafttime = crafttime;

                ItemExchangeService.UpdateItemExchange(ie);

                ResponseUtils.ResponseSuccess(response);

            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }


        public static void deleteExchange(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                ItemExchangeService.deleteItemExchanges(id);
                ResponseUtils.ResponseSuccess(response);

            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        public static void getExchange(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("type", out string type);
                List<ItemExchange> ls = ItemExchangeService.getItemExchangeByType(type);
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                foreach (ItemExchange ie in ls)
                {
                    Dictionary<string, object> tmp = new Dictionary<string, object>();
                    tmp.Add("data", ie);
                    tmp.Add("award", AwardInfoService.getAwardInfos(ie.id + "", AwardInfoTypeConfig.ITEM_EXCHANGE));
                    result.Add(tmp);
                }
                ResponseUtils.ResponseSuccessWithData(response, result);
            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }
}
