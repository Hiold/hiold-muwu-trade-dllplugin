using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.Commons;
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

                    UserInfo ui = UserService.getUserById(request.user.id + "")[0];

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
                        if (int.TryParse(item.xglevelset, out int levelSet) && ui.level >= 0)
                        {
                            if (item.xglevel.Equals("2") && levelSet > ui.level)
                            {
                                ResponseUtils.ResponseFail(response, "此物品需要达到" + levelSet + "级才能购买！您的游戏角色目前等级为" + ui.level);
                                return;
                            }
                            if (item.xglevel.Equals("3") && levelSet < ui.level)
                            {
                                ResponseUtils.ResponseFail(response, "此物品超过" + levelSet + "级无法购买！您的游戏角色目前等级为" + ui.level);
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
                    if (item.stock != -1 && item.stock < intCount)
                    {
                        //库存量不足
                        ResponseUtils.ResponseFail(response, "此物品已售罄，无法购买");
                        return;
                    }
                    //进行限购检查
                    string tdStart = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                    string tdEnd = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                    Int64 tdCount = ActionLogService.QueryItemLogCount(ui.gameentityid, item.id + "", LogType.BuyItem, tdStart, tdEnd);
                    Int64 allCount = ActionLogService.QueryItemLogCount(ui.gameentityid, item.id + "", LogType.BuyItem, null, null);
                    if ((item.xglevel.Equals("2") || item.xglevel.Equals("3")) && int.TryParse(item.xgdayset, out int intxgdayset))
                    {
                        if (tdCount + intCount > intxgdayset)
                        {
                            //库存量不足
                            ResponseUtils.ResponseFail(response, "此物品每日限购" + intxgdayset + "，已达到限购额，无法继续购买");
                            return;
                        }
                    }

                    if (item.xgall.Equals("2") && int.TryParse(item.xgallset, out int intxgallset))
                    {
                        if (allCount + intCount > intxgallset)
                        {
                            //库存量不足
                            ResponseUtils.ResponseFail(response, "此物品总限购" + intxgallset + "，已达到限购额，无法继续购买");
                            return;
                        }
                    }


                    if (item.selltype == 2)
                    {
                        if (ui.vipdiscount <= 0)
                        {
                            //库存量不足
                            ResponseUtils.ResponseFail(response, "此物品为VIP专属，您不是VIP");
                            return;
                        }
                    }


                    //查看当前物品是否有折扣
                    if (item.discount < 10)
                    {
                        priceAll = priceAll * (item.discount / 10D);
                    }


                    //如果玩家优惠率不为0，计算折后总价
                    if (ui.vipdiscount != 0)
                    {
                        priceAll = priceAll * ((100D - ui.vipdiscount) / 100D);
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
                                if (string.IsNullOrEmpty(couInfo.couCond))
                                {
                                    couInfo.couCond = "0";
                                }
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



                                if (couInfo.couCurrType.Contains("积分折扣"))
                                {
                                    //货币类型错误
                                    if (item.currency.Equals("2"))
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

                                if (couInfo.couCurrType.Contains("积分满减"))
                                {
                                    if (item.currency.Equals("2"))
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

                                if (couInfo.couCurrType.Contains("钻石折扣"))
                                {
                                    if (item.currency.Equals("1"))
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

                                if (couInfo.couCurrType.Contains("钻石满减"))
                                {
                                    if (item.currency.Equals("1"))
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

                                couInfo.storageCount--;
                                UserStorageService.UpdateUserStorage(couInfo);
                            }
                            else
                            {
                                ResponseUtils.ResponseFail(response, "优惠券已耗尽，无法继续使用");
                                return;
                            }
                        }
                    }

                    //执行扣款
                    //积分
                    if (item.currency == "1")
                    {
                        if (!database.DataBase.MoneyEditor(ui, MoneyType.Money, EditType.Sub, priceAll))
                        {
                            ResponseUtils.ResponseFail(response, "积分不足。购买失败！");
                            return;
                        }
                    }
                    //点券
                    if (item.currency == "2")
                    {
                        if (!database.DataBase.MoneyEditor(ui, MoneyType.Credit, EditType.Sub, priceAll))
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
                        username = ui.name,
                        platformid = ui.platformid,
                        gameentityid = ui.gameentityid,
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
                    if (item.stock > 0)
                    {
                        item.stock -= intCount;
                    }
                    int selled = 0;
                    int.TryParse(item.selloutcount, out selled);
                    item.selloutcount = (selled + intCount) + "";
                    ShopTradeService.updateShopItem(item);
                    //更新交易信息数据
                    UserService.UpdateAmount(ui, UserInfoCountType.BUY_COUNT, intCount);
                    UserService.UpdateAmount(ui, UserInfoCountType.BUY_MONEY, priceAll);

                    //记录用户购买数据
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.BuyItem,
                        atcPlayerEntityId = ui.gameentityid,
                        extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(_buy),
                        extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(userStorate),
                        extinfo3 = priceAll + "",
                        desc = string.Format("从系统商店购买{0}个{1}，共消费{2}", _buy.count, item.translate, priceAll)
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
                    itemdata = item.itemdata,
                    //重新定义内容属性
                    stock = count,
                };
                //记录用户购买数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.SellItem,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(item),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(userTrade),
                    extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("上架{0}个{1}，售价{2}", count, item.translate, price),
                });
                UserTradeService.addUserTrade(userTrade);
                ResponseUtils.ResponseSuccessWithData(response, "出售成功!");
                //全服广播
                HioldsCommons.BroadCast("有玩家上架了物品 （" + item.translate + "），有需要请前往交易系统查看");
                return;
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

                //检查物品属性
                if (item.itemStatus!= UserTradeConfig.NORMAL_ON_TRADE)
                {
                    ResponseUtils.ResponseFail(response, "物品状态校验失败，下架失败");
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
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(item),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("下架{0}个{1}", item.count, item.translate),
                });
                UserStorageService.addUserStorage(userStorate);
                ResponseUtils.ResponseSuccessWithData(response, "下架成功!");
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        /// <summary>
        /// 购买玩家交易物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void buyTradeItem(HioldRequest request, HttpListenerResponse response)
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
                    UserTrade ut = UserTradeService.selectUserTradeByid(_buy.id);

                    UserInfo ui = UserService.getUserById(request.user.id + "")[0];

                    //检查请求购买物品的库存数量
                    if (ut.gameentityid.Equals(ui.gameentityid))
                    {
                        //库存量不足
                        ResponseUtils.ResponseFail(response, "禁止自产自销");
                        return;
                    }

                    //检查请求购买物品的库存数量
                    if (ut.itemStatus != UserTradeConfig.NORMAL_ON_TRADE)
                    {
                        //库存量不足
                        ResponseUtils.ResponseFail(response, "该物品无法交易");
                        return;
                    }


                    //首先计算总价格
                    double priceAll;
                    if (int.TryParse(_buy.count, out int intCount))
                    {
                        priceAll = ut.price * intCount;
                    }
                    else
                    {
                        ResponseUtils.ResponseFail(response, "购买数量异常，请检查数量");
                        return;
                    }

                    //检查请求购买物品的库存数量
                    if (ut.stock < intCount)
                    {
                        //库存量不足
                        ResponseUtils.ResponseFail(response, "此物品已售罄，无法购买");
                        return;
                    }


                    UserStorage userStorate = new UserStorage()
                    {
                        //id
                        itemtype = ut.itemtype,
                        name = ut.name,
                        translate = ut.translate,
                        itemicon = ut.itemicon,
                        itemtint = ut.itemtint,
                        quality = ut.quality,
                        num = ut.num,
                        class1 = ut.class1,
                        class2 = ut.class2,
                        classmod = ut.classmod,
                        desc = ut.desc,
                        couCurrType = ut.couCurrType,
                        couPrice = ut.couPrice,
                        couCond = ut.couCond,
                        coudatelimit = ut.coudatelimit,
                        couDateStart = ut.couDateStart,
                        couDateEnd = ut.couDateEnd,
                        count = ut.count,
                        currency = ut.currency,
                        price = ut.price,
                        discount = ut.discount,
                        prefer = ut.prefer,
                        selltype = ut.selltype,
                        hot = ut.hot,
                        hotset = ut.hotset,
                        show = ut.show,
                        stock = ut.stock,
                        collect = ut.collect,
                        selloutcount = ut.selloutcount,
                        follow = ut.follow,
                        xglevel = ut.xglevel,
                        xglevelset = ut.xglevelset,
                        xgday = ut.xgday,
                        xgdayset = ut.xgdayset,
                        xgall = ut.xgall,
                        xgallset = ut.xgallset,
                        xgdatelimit = ut.xgdatelimit,
                        dateStart = ut.dateStart,
                        dateEnd = ut.dateEnd,
                        collected = ut.collected,
                        postTime = ut.postTime,
                        deleteTime = ut.deleteTime,
                        //非继承属性
                        username = request.user.name,
                        platformid = request.user.platformid,
                        gameentityid = request.user.gameentityid,
                        collectTime = DateTime.Now,
                        storageCount = int.Parse(_buy.count),
                        itemGetChenal = UserStorageGetChanel.TRADE_BUY,
                        itemStatus = UserStorageStatus.NORMAL_STORAGED,
                        //拓展属性
                        extinfo1 = ut.extinfo1,
                        extinfo2 = ut.extinfo2,
                        extinfo3 = ut.extinfo3,
                        extinfo4 = ut.extinfo4,
                        extinfo5 = ut.extinfo5,
                        itemdata = ut.itemdata,
                    };

                    //获取出售方信息
                    UserInfo seller = UserService.getUserBySteamid(ut.gameentityid)[0];

                    //购买方扣钱
                    if (!database.DataBase.MoneyEditor(request.user, MoneyType.Money, EditType.Sub, priceAll))
                    {
                        ResponseUtils.ResponseFail(response, "积分不足。购买失败！");
                        return;
                    }

                    //出售方加钱
                    database.DataBase.MoneyEditor(seller, MoneyType.Money, EditType.Add, priceAll);



                    //添加数据到数据库
                    UserStorageService.addUserStorage(userStorate);
                    //更新数据库存
                    ut.stock -= intCount;
                    if (ut.stock <= 0)
                    {
                        ut.itemStatus = UserTradeConfig.SELLED;
                    }
                    UserTradeService.UpdateUserTrade(ut);


                    //更新交易信息数据
                    UserService.UpdateAmount(request.user, UserInfoCountType.BUY_COUNT, intCount);
                    UserService.UpdateAmount(request.user, UserInfoCountType.BUY_MONEY, priceAll);


                    UserService.UpdateAmount(seller, UserInfoCountType.TRADE_COUNT, intCount);
                    UserService.UpdateAmount(seller, UserInfoCountType.TRADE_MONEY, priceAll);

                    //记录日志数据
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.ItemSellOuted,
                        atcPlayerEntityId = seller.gameentityid,
                        extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(ut),
                        extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(userStorate),
                        extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(_buy),
                        extinfo4 = priceAll + "",
                        desc = string.Format("玩家：{0} ，购买{1}个你上架的{2}，获得{3}", request.user.name, userStorate.storageCount, userStorate.translate, priceAll),
                    });


                    //记录日志数据
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.BuyUserTrade,
                        atcPlayerEntityId = request.user.gameentityid,
                        extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(ut),
                        extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(userStorate),
                        extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(_buy),
                        extinfo4 = priceAll + "",
                        desc = string.Format("从玩家交易购买{0}个{1}，消费{2}", userStorate.storageCount, userStorate.translate, priceAll),
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
        /// 供货
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void supplyItem(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));

                queryRequest.TryGetValue("id", out string id);
                UserRequire ur = UserRequireService.selectUserRequireByid(id);


                if (ur == null)
                {
                    ResponseUtils.ResponseFail(response, "未找到对应物品，供货失败");
                    return;
                }
                //检查物品属性
                if (request.user.gameentityid.Equals(ur.gameentityid))
                {
                    ResponseUtils.ResponseFail(response, "不能给自己供货");
                    return;
                }


                UserStorage us = UserStorageService.selectSupplyableItem(request.user.gameentityid, ur.Itemname, ur.Itemquality, ur.Itemcount + "");
                if (us == null)
                {
                    ResponseUtils.ResponseFail(response, "未找到可以供货的物品");
                    return;
                }
                if (us.storageCount < ur.Itemcount)
                {
                    ResponseUtils.ResponseFail(response, "数量不足");
                    return;
                }

                //更新求购数据
                ur.Status = UserRequireConfig.SUPPLYED;
                ur.supplygameentityid = request.user.gameentityid;
                ur.supplyplatformid = request.user.platformid;
                ur.supplyusername = request.user.name;
                ur.Supplytime = DateTime.Now;
                UserRequireService.UpdateUserRequire(ur);

                //更新供货人数据
                us.storageCount -= ur.Itemcount;
                if (us.storageCount <= 0)
                {
                    us.itemUsedChenal = UserStorageUsedChanel.SUPPLY_TO_OTHERS;
                    us.itemStatus = UserStorageStatus.SUPPLYED;
                }
                UserStorageService.UpdateUserStorage(us);
                //供货方加钱
                database.DataBase.MoneyEditor(request.user, MoneyType.Money, EditType.Add, ur.Price);
                UserService.UpdateAmount(request.user, UserInfoCountType.TRADE_COUNT, ur.Itemcount);
                UserService.UpdateAmount(request.user, UserInfoCountType.TRADE_MONEY, ur.Price);

                //更新求购人数据
                //获取求购人信息
                UserInfo supplyer = UserService.getUserBySteamid(ur.gameentityid)[0];
                UserService.UpdateAmount(supplyer, UserInfoCountType.BUY_COUNT, ur.Itemcount);
                UserService.UpdateAmount(supplyer, UserInfoCountType.BUY_MONEY, ur.Price);




                int intquality = 0;
                int.TryParse(ur.Itemquality, out intquality);
                //将物品保存到用户库存信息中
                UserStorage userStorate = new UserStorage()
                {
                    //id
                    itemtype = us.itemtype,
                    name = us.name,
                    translate = us.translate,
                    itemicon = us.itemicon,
                    itemtint = us.itemtint,
                    quality = us.quality,
                    num = us.num,
                    class1 = us.class1,
                    class2 = us.class2,
                    classmod = us.classmod,
                    desc = us.desc,
                    couCurrType = us.couCurrType,
                    couPrice = us.couPrice,
                    couCond = us.couCond,
                    coudatelimit = us.coudatelimit,
                    couDateStart = us.couDateStart,
                    couDateEnd = us.couDateEnd,
                    count = us.count,
                    currency = us.currency,
                    price = us.price,
                    discount = us.discount,
                    prefer = us.prefer,
                    selltype = us.selltype,
                    hot = us.hot,
                    hotset = us.hotset,
                    show = us.show,
                    stock = us.stock,
                    collect = us.collect,
                    selloutcount = us.selloutcount,
                    follow = us.follow,
                    xglevel = us.xglevel,
                    xglevelset = us.xglevelset,
                    xgday = us.xgday,
                    xgdayset = us.xgdayset,
                    xgall = us.xgall,
                    xgallset = us.xgallset,
                    xgdatelimit = us.xgdatelimit,
                    dateStart = us.dateStart,
                    dateEnd = us.dateEnd,
                    collected = us.collected,
                    postTime = us.postTime,
                    deleteTime = us.deleteTime,
                    //非继承属性
                    username = ur.username,
                    platformid = ur.platformid,
                    gameentityid = ur.gameentityid,
                    collectTime = DateTime.Now,
                    storageCount = ur.Itemcount,
                    itemGetChenal = UserStorageGetChanel.TRADE_BUY,
                    itemStatus = UserStorageStatus.NORMAL_STORAGED,
                    //拓展属性
                    extinfo1 = us.extinfo1,
                    extinfo2 = us.extinfo2,
                    extinfo3 = us.extinfo3,
                    extinfo4 = us.extinfo4,
                    extinfo5 = us.extinfo5,
                    itemdata = us.itemdata,
                };

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.SuppliedItem,
                    atcPlayerEntityId = supplyer.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(us),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(ur),
                    extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("玩家：{0} 为你供货{1}个{2}", request.user.name, ur.Itemcount, ur.Itemchinese, ur.Price),
                });

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.SupplyItem,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(us),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(ur),
                    extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    extinfo4 = ur.Price + "",
                    desc = string.Format("为玩家：{0} 供货{1}个{2}，获得{3}", supplyer.name, ur.Itemcount, ur.Itemchinese, ur.Price),
                });

                UserStorageService.addUserStorage(userStorate);
                ResponseUtils.ResponseSuccessWithData(response, "供货成功!");
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
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
