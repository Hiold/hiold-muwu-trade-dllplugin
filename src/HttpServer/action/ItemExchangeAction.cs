using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.Commons;
using HioldMod.src.HttpServer.attributes;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.database;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    [ActionAttribute]
    class ItemExchangeAction
    {
        /// <summary>
        /// 新增转换
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/postExchange")]
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

        /// <summary>
        /// 更新转换
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/updateExchange")]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/deleteExchange")]
        public static void deleteExchange(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                Console.WriteLine(id);
                ItemExchangeService.deleteItemExchanges(id);
                ResponseUtils.ResponseSuccess(response);

            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 获取转换
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/getExchange")]
        public static void getExchange(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("searchType", out string searchType);
                queryRequest.TryGetValue("awardType", out string awardType);
                queryRequest.TryGetValue("name", out string name);
                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);
                UserInfo ui = UserService.getUserById(request.user.id + "")[0];
                if (!string.IsNullOrEmpty(awardType) && awardType.Equals("4"))
                {
                    name = "";
                }
                if (!string.IsNullOrEmpty(awardType) && awardType.Equals("5"))
                {
                    name = "";
                }
                List<ItemExchange> ls = ItemExchangeService.getItemExchangeByType(searchType, awardType, type, name, page, limit);
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                foreach (ItemExchange ie in ls)
                {
                    Dictionary<string, object> tmp = new Dictionary<string, object>();
                    tmp.Add("data", ie);
                    List<AwardInfo> ais = AwardInfoService.getAwardInfos(ie.id + "", AwardInfoTypeConfig.ITEM_EXCHANGE);
                    foreach (AwardInfo ai in ais)
                    {
                        if (ai.type.Equals("1") || ai.type.Equals("2"))
                        {
                            if (string.IsNullOrEmpty(ai.itemquality))
                            {
                                ai.itemquality = "0";
                            }
                            UserStorage usT = UserStorageService.selectAvaliableItem(request.user.gameentityid, ai.itemname, ai.itemquality, ai.type, "0");
                            if (usT != null)
                            {
                                ai.extinfo6 = usT.storageCount + "";
                            }
                            else
                            {
                                ai.extinfo6 = "0";
                            }
                        }
                        else if (ai.type.Equals("4"))
                        {
                            ai.extinfo6 = ui.money + "";

                        }
                        else if (ai.type.Equals("5"))
                        {
                            ai.extinfo6 = ui.credit + "";

                        }

                    }
                    tmp.Add("award", ais);
                    result.Add(tmp);
                }
                ResponseUtils.ResponseSuccessWithData(response, result);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 获取有效物品
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/getAvalivleItem")]
        public static void getAvalivleItem(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("quality", out string quality);
                queryRequest.TryGetValue("itemtype", out string itemtype);
                queryRequest.TryGetValue("count", out string count);
                ResponseUtils.ResponseSuccessWithData(response, UserStorageService.selectAvaliableItem(request.user.gameentityid, itemname, quality, itemtype, count));
            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 制作物品
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/craftItem")]
        public static void craftItem(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("count", out string count);
                int intCount = int.Parse(count);
                ItemExchange ie = ItemExchangeService.getItemExchangeByid(id);
                if (!ie.status.Equals("1"))
                {
                    ResponseUtils.ResponseFail(response, "无法合成该物品，请联系管理员");
                    return;
                }
                if (!ie.type.Equals("1") && !ie.type.Equals("3"))
                {
                    ResponseUtils.ResponseFail(response, "无法合成该物品，请联系管理员");
                    return;
                }
                //查询物品列表
                List<AwardInfo> infos = AwardInfoService.getAwardInfos(ie.id + "", AwardInfoTypeConfig.ITEM_EXCHANGE);
                if (infos == null || infos.Count <= 0)
                {
                    ResponseUtils.ResponseFail(response, "这个物品没有配置合成表，请联系管理员");
                    return;
                }
                List<UserStorage> uss = new List<UserStorage>();
                UserInfo ui = UserService.getUserById(request.user.id + "")[0];
                foreach (AwardInfo info in infos)
                {
                    if (info.type.Equals("1") || info.type.Equals("2"))
                    {
                        if (string.IsNullOrEmpty(info.itemquality))
                        {
                            info.itemquality = "0";
                        }
                        UserStorage us = UserStorageService.selectAvaliableItem(request.user.gameentityid, info.itemname, info.itemquality, info.type, (int.Parse(info.count) * intCount) + "");
                        if (us == null)
                        {
                            ResponseUtils.ResponseFail(response, "物品（" + info.itemchinese + "）数量不足，无法合成");
                            return;
                        }
                        else
                        {
                            us.storageCount -= (int.Parse(info.count) * intCount);
                            uss.Add(us);

                        }
                    }
                    else if (info.type.Equals("4"))
                    {
                        if (ui.money < int.Parse(info.count))
                        {
                            ResponseUtils.ResponseFail(response, "积分数量不足，无法合成");
                            return;
                        }
                    }
                    else if (info.type.Equals("5"))
                    {
                        if (ui.credit < int.Parse(info.count))
                        {
                            ResponseUtils.ResponseFail(response, "点券数量不足，无法合成");
                            return;
                        }
                    }
                }
                //
                foreach (AwardInfo info in infos)
                {
                    if (info.type.Equals("4"))
                    {
                        bool ok = DataBase.MoneyEditor(ui, DataBase.MoneyType.Money, DataBase.EditType.Sub, double.Parse(info.count));
                        if (!ok)
                        {
                            ResponseUtils.ResponseFail(response, "积分数量不足，合成失败");
                            return;
                        }
                    }
                    else if (info.type.Equals("5"))
                    {
                        ui.credit -= double.Parse(info.count);
                    }
                }

                //执行更新库存
                foreach (UserStorage avaStorage in uss)
                {
                    UserStorageService.UpdateUserStorage(avaStorage);
                }


                //记录用户购买数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.CraftItem,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(ie),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(infos),
                    extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(uss),
                    extinfo4 = count,
                    extinfo5 = ie.crafttime,
                    extinfo6 = "0",
                    desc = string.Format("制作{0}个{1}", count, ie.itemchinese)
                });

                ResponseUtils.ResponseSuccess(response);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 获取制作中物品
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/getCrafting")]
        public static void getCrafting(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                ResponseUtils.ResponseSuccessWithData(response, ActionLogService.getCrafting(request.user.gameentityid, "0"));
            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 领取物品
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/getCraftAward")]
        public static void getCraftAward(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                ActionLog al = ActionLogService.getActionLogById(id);
                ItemExchange ie = (ItemExchange)SimpleJson2.SimpleJson2.DeserializeObject(al.extinfo1, typeof(ItemExchange));
                List<AwardInfo> infos = new List<AwardInfo>();
                //计算能完成时间
                DateTime craftTime = al.actTime;
                craftTime = craftTime.AddSeconds(int.Parse(al.extinfo5) * int.Parse(al.extinfo4));
                if (craftTime > DateTime.Now)
                {
                    ResponseUtils.ResponseFail(response, "尚未完成制作");
                    return;
                }
                string itemtype = "";
                if (ie.type.Equals("1") || ie.type.Equals("2"))
                {
                    itemtype = "1";
                }
                else if (ie.type.Equals("3") || ie.type.Equals("4"))
                {
                    itemtype = "2";
                }
                infos.Add(new AwardInfo()
                {
                    type = itemtype,
                    containerid = 0,
                    count = ie.count,
                    itemname = ie.itemname,
                    itemquality = ie.itemquality,
                    itemchinese = ie.itemchinese,
                    itemicon = ie.itemicon,
                    itemtint = ie.itemtint,
                    command = ie.command,
                    couCond = ie.couCond,
                    couCurrType = ie.couCurrType,
                    couPrice = ie.couPrice,
                    coudatelimit = ie.coudatelimit,
                    couDateEnd = ie.couDateEnd,
                    couDateStart = ie.couDateStart,
                    status = "1",
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    extinfo6 = "",
                    funcid = 0,
                    chance = "0"
                });
                //计算完毕发放礼品
                string awardinfo = AwardDeliverTools.DeliverAward(request.user, infos, UserStorageGetChanel.CRAFT);
                //更新数据
                var dt = new Dictionary<string, object>();
                dt.Add("id", id);
                dt.Add("extinfo6", "1");
                dt.Add("extinfo7", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                ActionLogService.UpdateParam(dt);

                ResponseUtils.ResponseSuccessWithData(response, infos);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }
}
