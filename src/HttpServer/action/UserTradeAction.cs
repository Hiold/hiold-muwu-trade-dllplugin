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
using static HioldMod.src.HttpServer.database.DataBase;

namespace HioldMod.src.HttpServer.action
{
    class UserTradeAction
    {
        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void buyItem(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                buy _buy = new buy();
                _buy = (buy)SimpleJson2.SimpleJson2.DeserializeObject(postData, _buy.GetType());
                //List<UserInfo> resultList = UserService.userLogin(_info.username, ServerUtils.md5(_info.password));
                //ResponseUtils.ResponseSuccessWithData(response, ui);
                //根据id获取
                if (request.user != null)
                {
                    //查询用户请求购买的物品
                    TradeManageItem item = ShopTradeService.getShopItemById(int.Parse(_buy.id))[0];

                    //限购检查
                    if (item.xgdatelimit.Equals("2"))
                    {
                        if (item.dateStart > DateTime.Now)
                        {
                            ResponseUtils.ResponseFail(response, "此物品暂时不能购买，开售时间：" + item.dateStart.ToString("yyyy-MM-dd HH:mm:ss"));
                            return;
                        }

                        if (item.dateEnd < DateTime.Now)
                        {
                            ResponseUtils.ResponseFail(response, "此物品不能购买，售卖结束时间：" + item.dateEnd.ToString("yyyy-MM-dd HH:mm:ss"));
                            return;
                        }
                    }
                    //检查登记限制
                    if (item.xglevel.Equals("2") || item.xglevel.Equals("3"))
                    {
                        if (int.TryParse(item.xglevelset, out int levelSet) && int.TryParse(request.user.level, out int userLevel))
                        {
                            if (item.xglevel.Equals("2") && levelSet > userLevel)
                            {
                                ResponseUtils.ResponseFail(response, "此物品需要达到" + levelSet + "级才能购买！您的游戏角色目前等级为" + userLevel);
                                return;
                            }
                            if (item.xglevel.Equals("3") && levelSet < userLevel)
                            {
                                ResponseUtils.ResponseFail(response, "此物品超过" + levelSet + "级无法购买！您的游戏角色目前等级为" + userLevel);
                                return;
                            }
                        }
                        else
                        {
                            ResponseUtils.ResponseFail(response, "此物品限购，无法获取您的等级信息");
                            return;
                        }
                    }

                    //首先计算总价格
                    double priceAll;
                    double priceBefore;
                    if (int.TryParse(_buy.count, out int intCount))
                    {
                        priceAll = item.price * intCount;
                        priceBefore = priceAll;
                    }
                    else
                    {
                        ResponseUtils.ResponseFail(response, "购买数量异常，请检查数量");
                        return;
                    }

                    //检查请求购买物品的库存数量
                    if (item.stock < intCount)
                    {
                        //库存量不足
                        ResponseUtils.ResponseFail(response, "此物品已售罄，无法购买");
                        return;
                    }
                    //进行限购检查
                    string tdStart = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                    string tdEnd = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                    Int64 tdCount = ActionLogService.QueryItemLogCount(request.user.gameentityid, item.id + "", LogType.BuyItem, tdStart, tdEnd);
                    Int64 allCount = ActionLogService.QueryItemLogCount(request.user.gameentityid, item.id + "", LogType.BuyItem, null, null);
                    if (int.TryParse(item.xgdayset, out int intxgdayset))
                    {
                        if (tdCount + intCount > intxgdayset)
                        {
                            //库存量不足
                            ResponseUtils.ResponseFail(response, "此物品每日限购" + intxgdayset + "，已达到限购额，无法继续购买");
                            return;
                        }
                    }

                    if (int.TryParse(item.xgallset, out int intxgallset))
                    {
                        if (allCount + intCount > intxgallset)
                        {
                            //库存量不足
                            ResponseUtils.ResponseFail(response, "此物品总限购" + intxgallset + "，已达到限购额，无法继续购买");
                            return;
                        }
                    }





                    //查看当前物品是否有折扣
                    if (item.discount < 10)
                    {
                        priceAll = priceAll * (item.discount / 10D);
                    }


                    //如果玩家优惠率不为0，计算折后总价
                    if (request.user.vipdiscount != 0)
                    {
                        priceAll = priceAll * ((100D - request.user.vipdiscount) / 100D);
                    }

                    //检查用户是否使用了优惠券
                    if (_buy.couid != null && !_buy.couid.Equals(""))
                    {
                        if (int.TryParse(_buy.couid, out int intCouid))
                        {
                            List<UserStorage> couTicket = UserStorageService.selectPlayerStorage(intCouid + "");
                            if (couTicket.Count > 0)
                            {
                                UserStorage couInfo = couTicket[0];
                                if (couInfo.coudatelimit.Equals("2"))
                                {
                                    if (item.couDateStart > DateTime.Now)
                                    {
                                        ResponseUtils.ResponseFail(response, "无法使用此优惠券购买，有效期开始时间：" + item.couDateStart.ToString("yyyy-MM-dd HH:mm:ss"));
                                        return;
                                    }

                                    if (item.couDateEnd < DateTime.Now)
                                    {
                                        ResponseUtils.ResponseFail(response, "无法使用此优惠券购买，有效期截止：" + item.couDateEnd.ToString("yyyy-MM-dd HH:mm:ss"));
                                        return;
                                    }
                                }



                                if (couInfo.couCurrType.Equals("积分折扣"))
                                {
                                    //货币类型错误
                                    if (item.currency.Equals("2"))
                                    {
                                        ResponseUtils.ResponseFail(response, "无法使用这个优惠券，此优惠券为钻石优惠券");
                                        return;
                                    }

                                    if (int.TryParse(couInfo.couCond, out int intCouCond))
                                    {
                                        //未达到折扣标准
                                        if (priceBefore < intCouCond)
                                        {
                                            ResponseUtils.ResponseFail(response, "无法使用这个优惠券，此优惠券需要购买总价大于" + intCouCond + "才能使用");
                                            return;
                                        }
                                        if (double.TryParse(couInfo.couPrice, out double intCouPrice))
                                        {
                                            priceAll = priceAll * (intCouPrice / 10D);
                                            //执行扣除优惠券
                                            couInfo.storageCount--;
                                            UserStorageService.UpdateUserStorage(couInfo);
                                        }

                                    }
                                    else
                                    {
                                        ResponseUtils.ResponseFail(response, "优惠券配置异常，请联系管理员或者不使用优惠券");
                                        return;
                                    }
                                }

                                if (couInfo.couCurrType.Equals("积分满减"))
                                {
                                    if (item.currency.Equals("2"))
                                    {
                                        ResponseUtils.ResponseFail(response, "无法使用这个优惠券，此优惠券为钻石优惠券");
                                        return;
                                    }
                                    if (int.TryParse(couInfo.couCond, out int intCouCond))
                                    {
                                        //未达到折扣标准
                                        if (priceBefore < intCouCond)
                                        {
                                            ResponseUtils.ResponseFail(response, "无法使用这个优惠券，此优惠券需要购买总价大于" + intCouCond + "才能使用");
                                            return;
                                        }
                                        if (double.TryParse(couInfo.couPrice, out double intCouPrice))
                                        {
                                            priceAll -= intCouPrice;
                                            //执行扣除优惠券
                                            couInfo.storageCount--;
                                            UserStorageService.UpdateUserStorage(couInfo);
                                        }

                                    }
                                    else
                                    {
                                        ResponseUtils.ResponseFail(response, "优惠券配置异常，请联系管理员或者不使用优惠券");
                                        return;
                                    }
                                }

                                if (couInfo.couCurrType.Equals("钻石折扣"))
                                {
                                    if (item.currency.Equals("1"))
                                    {
                                        ResponseUtils.ResponseFail(response, "无法使用这个优惠券，此优惠券为积分优惠券");
                                        return;
                                    }
                                    if (int.TryParse(couInfo.couCond, out int intCouCond))
                                    {
                                        //未达到折扣标准
                                        if (priceBefore < intCouCond)
                                        {
                                            ResponseUtils.ResponseFail(response, "无法使用这个优惠券，此优惠券需要购买总价大于" + intCouCond + "才能使用");
                                            return;
                                        }
                                        if (double.TryParse(couInfo.couPrice, out double intCouPrice))
                                        {
                                            priceAll = priceAll * (intCouPrice / 10D);
                                            //执行扣除优惠券
                                            couInfo.storageCount--;
                                            UserStorageService.UpdateUserStorage(couInfo);
                                        }

                                    }
                                    else
                                    {
                                        ResponseUtils.ResponseFail(response, "优惠券配置异常，请联系管理员或者不使用优惠券");
                                        return;
                                    }
                                }

                                if (couInfo.couCurrType.Equals("钻石满减"))
                                {
                                    if (item.currency.Equals("1"))
                                    {
                                        ResponseUtils.ResponseFail(response, "无法使用这个优惠券，此优惠券为积分优惠券");
                                        return;
                                    }
                                    if (int.TryParse(couInfo.couCond, out int intCouCond))
                                    {
                                        //未达到折扣标准
                                        if (priceBefore < intCouCond)
                                        {
                                            ResponseUtils.ResponseFail(response, "无法使用这个优惠券，此优惠券需要购买总价大于" + intCouCond + "才能使用");
                                            return;
                                        }
                                        if (double.TryParse(couInfo.couPrice, out double intCouPrice))
                                        {
                                            priceAll -= intCouPrice;
                                            //执行扣除优惠券
                                            couInfo.storageCount--;
                                            UserStorageService.UpdateUserStorage(couInfo);
                                        }

                                    }
                                    else
                                    {
                                        ResponseUtils.ResponseFail(response, "优惠券配置异常，请联系管理员或者不使用优惠券");
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    //执行扣款
                    //积分
                    if (item.currency == "1")
                    {
                        if (!database.DataBase.MoneyEditor(request.user, MoneyType.Money, EditType.Sub, priceAll))
                        {
                            ResponseUtils.ResponseFail(response, "积分不足。购买失败！");
                            return;
                        }
                    }
                    //点券
                    if (item.currency == "2")
                    {
                        if (!database.DataBase.MoneyEditor(request.user, MoneyType.Credit, EditType.Sub, priceAll))
                        {
                            ResponseUtils.ResponseFail(response, "点券不足。购买失败！");
                            return;
                        }
                    }
                    UserStorage userStorate = new UserStorage()
                    {
                        //id
                        itemtype = item.itemtype,
                        name = item.name,
                        translate = item.translate,
                        itemicon = item.itemicon,
                        itemtint = item.itemtint,
                        quality = item.quality,
                        num = item.num,
                        class1 = item.class1,
                        class2 = item.class2,
                        classmod = item.classmod,
                        desc = item.desc,
                        couCurrType = item.couCurrType,
                        couPrice = item.couPrice,
                        couCond = item.couCond,
                        coudatelimit = item.coudatelimit,
                        couDateStart = item.couDateStart,
                        couDateEnd = item.couDateEnd,
                        count = item.count,
                        currency = item.currency,
                        price = item.price,
                        discount = item.discount,
                        prefer = item.prefer,
                        selltype = item.selltype,
                        hot = item.hot,
                        hotset = item.hotset,
                        show = item.show,
                        stock = item.stock,
                        collect = item.collect,
                        selloutcount = item.selloutcount,
                        follow = item.follow,
                        xglevel = item.xglevel,
                        xglevelset = item.xglevelset,
                        xgday = item.xgday,
                        xgdayset = item.xgdayset,
                        xgall = item.xgall,
                        xgallset = item.xgallset,
                        xgdatelimit = item.xgdatelimit,
                        dateStart = item.dateStart,
                        dateEnd = item.dateEnd,
                        collected = item.collected,
                        postTime = item.postTime,
                        deleteTime = item.deleteTime,
                        //非继承属性
                        username = request.user.name,
                        platformid = request.user.platformid,
                        gameentityid = request.user.gameentityid,
                        collectTime = DateTime.Now,
                        storageCount = int.Parse(_buy.count),
                        itemGetChenal = UserStorageGetChanel.SHOP_BUY,
                        itemStatus = UserStorageStatus.NORMAL_STORAGED,
                        //拓展属性
                        extinfo1 = "",
                        extinfo2 = "",
                        extinfo3 = "",
                        extinfo4 = "",
                        extinfo5 = "",
                        itemdata = "",
                    };



                    //添加数据到数据库
                    UserStorageService.addUserStorage(userStorate);
                    //更新数据库存
                    item.stock -= intCount;
                    int selled = 0;
                    int.TryParse(item.selloutcount, out selled);
                    item.selloutcount = (selled + intCount) + "";
                    ShopTradeService.updateShopItem(item);
                    //更新交易信息数据
                    UserService.UpdateAmount(request.user, UserInfoCountType.BUY_COUNT, intCount);
                    UserService.UpdateAmount(request.user, UserInfoCountType.BUY_MONEY, priceAll);

                    //记录用户购买数据
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.BuyItem,
                        atcPlayerEntityId = request.user.gameentityid,
                        extinfo1 = _buy.id,
                        extinfo2 = _buy.couid,
                        extinfo3 = _buy.count,
                        extinfo4 = priceAll + ""
                    });
                    ResponseUtils.ResponseSuccess(response);
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "没登录，请登录后再试");
                    return;
                }

            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }



        /// <summary>
        /// 出售物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void sellOutItem(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                int count = 0;
                if (queryRequest.TryGetValue("count", out string countStr))
                {
                    count = int.Parse(countStr);
                }

                double price = 0;
                if (queryRequest.TryGetValue("price", out string priceStr))
                {
                    price = double.Parse(priceStr);
                }
                queryRequest.TryGetValue("id", out string id);
                UserStorage item = UserStorageService.selectUserStorageByid(id);
                //检查物品属性
                if (!request.user.gameentityid.Equals(item.gameentityid))
                {
                    ResponseUtils.ResponseFail(response, "非个人物品，出售失败");
                    return;
                }
                if (item.storageCount < count || count <= 0)
                {
                    ResponseUtils.ResponseFail(response, "出售失败，数量异常");
                    return;
                }
                if (item.storageCount == count)
                {
                    item.storageCount = 0;
                    item.itemStatus = UserStorageStatus.USERSELLED;
                }
                else
                {
                    item.storageCount -= count;
                }
                //更新库存数据
                UserStorageService.UpdateUserStorage(item);
                //将物品保存到trade信息中
                UserTrade userTrade = new UserTrade()
                {
                    //id
                    itemtype = item.itemtype,
                    name = item.name,
                    translate = item.translate,
                    itemicon = item.itemicon,
                    itemtint = item.itemtint,
                    quality = item.quality,
                    num = item.num,
                    class1 = item.class1,
                    class2 = item.class2,
                    classmod = item.classmod,
                    desc = item.desc,
                    couCurrType = item.couCurrType,
                    couPrice = item.couPrice,
                    couCond = item.couCond,
                    coudatelimit = item.coudatelimit,
                    couDateStart = item.couDateStart,
                    couDateEnd = item.couDateEnd,
                    count = item.count,
                    currency = item.currency,
                    price = price,
                    discount = item.discount,
                    prefer = item.prefer,
                    selltype = item.selltype,
                    hot = item.hot,
                    hotset = item.hotset,
                    show = item.show,
                    collect = item.collect,
                    selloutcount = item.selloutcount,
                    follow = item.follow,
                    xglevel = item.xglevel,
                    xglevelset = item.xglevelset,
                    xgday = item.xgday,
                    xgdayset = item.xgdayset,
                    xgall = item.xgall,
                    xgallset = item.xgallset,
                    xgdatelimit = item.xgdatelimit,
                    dateStart = item.dateStart,
                    dateEnd = item.dateEnd,
                    collected = item.collected,
                    postTime = item.postTime,
                    deleteTime = item.deleteTime,
                    //非继承属性
                    username = request.user.name,
                    platformid = request.user.platformid,
                    gameentityid = request.user.gameentityid,
                    itemStatus = UserTradeConfig.NORMAL_ON_TRADE,
                    forSellTime = DateTime.Now,
                    //拓展属性
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    itemdata = "",
                    //重新定义内容属性
                    stock = count,
                };
                //记录用户购买数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.SellItem,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = item.id + "",
                    extinfo2 = count + "",
                });
                UserTradeService.addUserTrade(userTrade);
                ResponseUtils.ResponseSuccessWithData(response, "出售成功!");
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }



        /// <summary>
        /// 出售物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void TackBackItem(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));

                queryRequest.TryGetValue("id", out string id);
                UserTrade item = UserTradeService.selectUserTradeByid(id);
                if (item == null)
                {
                    ResponseUtils.ResponseFail(response, "未找到对应物品，取回失败");
                    return;
                }
                //检查物品属性
                if (!request.user.gameentityid.Equals(item.gameentityid))
                {
                    ResponseUtils.ResponseFail(response, "非个人物品，取回失败");
                    return;
                }
                //更新库存数据
                item.itemStatus = UserTradeConfig.TAKC_BACK;
                UserTradeService.UpdateUserTrade(item);


                //将物品保存到trade信息中
                UserStorage userStorate = new UserStorage()
                {
                    //id
                    itemtype = item.itemtype,
                    name = item.name,
                    translate = item.translate,
                    itemicon = item.itemicon,
                    itemtint = item.itemtint,
                    quality = item.quality,
                    num = item.num,
                    class1 = item.class1,
                    class2 = item.class2,
                    classmod = item.classmod,
                    desc = item.desc,
                    couCurrType = item.couCurrType,
                    couPrice = item.couPrice,
                    couCond = item.couCond,
                    coudatelimit = item.coudatelimit,
                    couDateStart = item.couDateStart,
                    couDateEnd = item.couDateEnd,
                    count = item.count,
                    currency = item.currency,
                    price = item.price,
                    discount = item.discount,
                    prefer = item.prefer,
                    selltype = item.selltype,
                    hot = item.hot,
                    hotset = item.hotset,
                    show = item.show,
                    stock = item.stock,
                    collect = item.collect,
                    selloutcount = item.selloutcount,
                    follow = item.follow,
                    xglevel = item.xglevel,
                    xglevelset = item.xglevelset,
                    xgday = item.xgday,
                    xgdayset = item.xgdayset,
                    xgall = item.xgall,
                    xgallset = item.xgallset,
                    xgdatelimit = item.xgdatelimit,
                    dateStart = item.dateStart,
                    dateEnd = item.dateEnd,
                    collected = item.collected,
                    postTime = item.postTime,
                    deleteTime = item.deleteTime,
                    //非继承属性
                    username = request.user.name,
                    platformid = request.user.platformid,
                    gameentityid = request.user.gameentityid,
                    collectTime = DateTime.Now,
                    storageCount = item.stock,
                    itemGetChenal = UserStorageGetChanel.TACK_BACK,
                    itemStatus = UserStorageStatus.NORMAL_STORAGED,
                    //拓展属性
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    itemdata = "",
                };
                //记录用户购买数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.TackBack,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = item.id + "",
                });
                UserStorageService.addUserStorage(userStorate);
                ResponseUtils.ResponseSuccessWithData(response, "下架成功!");
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }



        public class info
        {

        }

        public class buy
        {
            public string id { get; set; }
            public string count { get; set; }
            public string couid { get; set; }
        }
    }
}
