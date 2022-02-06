﻿using HioldMod.HttpServer;
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
    class UserTradeAction
    {
        /// <summary>
        /// 用户登录获取用户折扣券
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getdisCountTicket(HioldRequest request, HttpListenerResponse response)
        {
            try
            {


                string postData = ServerUtils.getPostData(request.request);
                //Dictionary<string, string> param = ServerUtils.GetParam(request);
                info _info = new info();
                _info = (info)SimpleJson2.SimpleJson2.DeserializeObject(postData, _info.GetType());
                //List<UserInfo> resultList = UserService.userLogin(_info.username, ServerUtils.md5(_info.password));

                //ResponseUtils.ResponseSuccessWithData(response, ui);

            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }



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
                                            priceAll = priceAll * ((10D - intCouPrice) / 10D);
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
                                            priceAll = priceAll * ((10D - intCouPrice) / 10D);
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
                                            priceAll = priceAll * ((10D - intCouPrice) / 10D);
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
                                            priceAll = priceAll * ((10D - intCouPrice) / 10D);
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
                        //拓展属性
                        extinfo1 = "",
                        extinfo2 = "",
                        extinfo3 = "",
                        extinfo4 = "",
                        extinfo5 = "",
                    };



                    //添加数据到数据库
                    UserStorageService.addUserStorage(userStorate);
                    //更新数据库存
                    item.stock -= intCount;
                    ShopTradeService.updateShopItem(item);
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
