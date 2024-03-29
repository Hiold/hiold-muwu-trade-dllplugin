﻿using HioldMod.src.HttpServer.action;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.HttpServer.router
{
    class MainRouter
    {
        public static void DispacherRouter(HioldRequest request, HttpListenerResponse response)
        {
            //Console.WriteLine(request.RawUrl);
            string url = "";
            if (request.request.RawUrl.Contains("?"))
            {
                url = request.request.RawUrl.Substring(0, request.request.RawUrl.IndexOf("?"));
            }
            else
            {
                url = request.request.RawUrl;
            }

            //请求静态资源
            if (!url.StartsWith("/api/"))
            {
                Task.Run(() =>
                {
                    GameItemAction.getStaticSource(request, response);
                });
            }
            else if (url.StartsWith("/api/image"))
            {
                if (Filters.IsServerReady(request, response))
                    GameItemAction.getImage(request, response);
            }
            //动态获取图片
            else if (url.StartsWith("/api/getimagetint"))
            {
                if (Filters.IsServerReady(request, response))
                    GameItemAction.getImageTint(request, response);
            }
            else
            {
                //请求接口
                //获取接口信息
                if (AttributeAnalysis.routers.TryGetValue(url, out AttributeAnalysis.RouterInfo router))
                {

                    //检查用户是否登录
                    if (router.IsServerReady)
                    {
                        if (!Filters.IsServerReady(request, response))
                        {
                            return;
                        }
                    }
                    //检查用户是否登录
                    if (router.IsUserLogin)
                    {
                        if (!Filters.UserLoginFilter(request, response))
                        {
                            return;
                        }
                    }
                    //检查是否管理员
                    if (router.IsAdmin)
                    {
                        if (!Filters.isAdminFilter(request, response))
                        {
                            return;
                        }
                    }
                    //
                    long start = DateTime.Now.Ticks / 10000;
                    long timeStempStart = DateTime.Now.Ticks;
                    //Console.WriteLine(router.action.FullName);
                    //动态调用
                    object[] args = new object[] { request, response };
                    router.method.Invoke(router.action, args);
                    long end = DateTime.Now.Ticks / 10000;
                    string log = string.Format("{0} - - [{1}] \"{2} {3} {4}\" {5} {6}", request.request.UserHostAddress, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), request.request.HttpMethod, url, request.request.ProtocolVersion, response.StatusCode, (end - start));
                    //Console.WriteLine(log);
                }
                else
                {
                    //没有找到对应接口，返回404
                    response.StatusCode = 404;
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                }

            }

            //LogUtils.Loger(url);
            //登录
            //if (url.Equals("/api/login"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        LoginAction.login(request, response);
            //}
            // else if
            ////调试
            //(url.Equals("/api/debug"))
            // {
            //     if (Filters.IsServerReady(request, response))
            //         LoginAction.debug(request, response);
            // }
            //获取当前用户
            //else if (url.Equals("/api/getCurrentUser"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        LoginAction.getCurrentUser(request, response);
            //}
            //node登录
            //else if (url.Equals("/api/ncodeLogin"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        LoginAction.ncodeLogin(request, response);
            //}
            //更新用户信息
            //else if (url.Equals("/api/updateUserInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        UserInfoAction.updateUserInfo(request, response);
            //}

            //else if (url.Equals("/api/getSystemItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        GameItemAction.getSystemItem(request, response);
            //}
            //else if (url.Equals("/api/getSystemItemByPage"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        GameItemAction.getSystemItemByPage(request, response);
            //}
            //else if (url.Equals("/api/steamAuth"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        AuthAction.steamAuth(request, response);
            //}
            //else if (url.Equals("/api/verification"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        AuthAction.Verification(request, response);
            //}
            //else if (url.Equals("/api/getTranslation"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        TranslationAction.getTranslation(request, response);
            //}
            //else if (url.StartsWith("/api/image"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        GameItemAction.getImage(request, response);
            //}
            //else if (url.StartsWith("/api/iconImage"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        GameItemAction.getImageIcon(request, response);
            //}
            //else if (url.StartsWith("/api/addShopItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.isAdminFilter(request, response))
            //            TradeManageAction.addShopItem(request, response);
            //}
            //else if (url.StartsWith("/api/queryShopItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        TradeManageAction.queryShopItem(request, response);
            //}
            //else if (url.StartsWith("/api/updateShopItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.isAdminFilter(request, response))
            //            TradeManageAction.updateShopItem(request, response);
            //}
            //else if (url.StartsWith("/api/deleteShopItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.isAdminFilter(request, response))
            //            TradeManageAction.deleteShopItem(request, response);
            //}
            //else if (url.StartsWith("/api/uploadFile"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.isAdminFilter(request, response))
            //            FileUpload.uploadFile(request, response);
            //}
            //else if (url.StartsWith("/api/getIconFile"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        FileUpload.getIconFile(request, response);
            //}
            //else if (url.StartsWith("/api/buyItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserTradeAction.buyItem(request, response);
            //}
            ////获取优惠券  校验登录
            //else if (url.StartsWith("/api/getdisCountTicket"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.getdisCountTicket(request, response);

            //}
            ////获取限购数据
            //else if (url.StartsWith("/api/getItemBuyLimit"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.getItemBuyLimit(request, response);
            //}
            //更新收藏数据
            //else if (url.StartsWith("/api/updateCollect"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.updateCollect(request, response);
            //}
            ////查询用户库存
            //else if (url.StartsWith("/api/getPlayerStorage"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.getPlayerStorage(request, response);

            //}
            ////发放物品
            //else if (url.StartsWith("/api/dispachItemToGame"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.dispachItemToGame(request, response);

            //}
            //删除物品
            //else if (url.StartsWith("/api/deleteItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.deleteItem(request, response);

            //}
            ////出售物品
            //else if (url.StartsWith("/api/sellOutItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserTradeAction.sellOutItem(request, response);
            //}
            ////下架物品
            //else if (url.StartsWith("/api/TackBackItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserTradeAction.TackBackItem(request, response);

            //}
            ////查询玩家在售
            //else if (url.StartsWith("/api/getPlayerOnSell"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        UserInfoAction.getPlayerOnSell(request, response);
            //}
            //发布求购
            //else if (url.StartsWith("/api/postRequire"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserRequireAction.postRequire(request, response);

            //}
            ////查询求购
            //else if (url.StartsWith("/api/getUserRequire"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        UserRequireAction.getUserRequire(request, response);
            //}
            ////查询店铺信息
            //else if (url.StartsWith("/api/getUserShopList"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        UserInfoAction.getUserShopList(request, response);
            //}
            //查询用户信息
            //else if (url.StartsWith("/api/getUserInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.getUserInfo(request, response);
            //}
            //取消求购
            //else if (url.StartsWith("/api/cancelRequire"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserRequireAction.cancelRequire(request, response);
            //}
            ////购买玩家交易物品
            //else if (url.StartsWith("/api/buyTradeItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserTradeAction.buyTradeItem(request, response);
            //}
            ////供货
            //else if (url.StartsWith("/api/supplyItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserTradeAction.supplyItem(request, response);
            //}
            //供货
            //else if (url.StartsWith("/api/loadContainerListAround"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            GameChunckContainerAction.loadContainerListAround(request, response);
            //}
            ////获取容器内物品
            //else if (url.StartsWith("/api/getContainerItems"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            GameChunckContainerAction.getContainerItems(request, response);
            //}
            //提取物品到交易系统
            //else if (url.StartsWith("/api/TakeItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            GameChunckContainerAction.TakeItem(request, response);
            //}
            ////获取日志
            //else if (url.StartsWith("/api/getLogs"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.getLogs(request, response);

            //}
            ////获取已收藏的物品
            //else if (url.StartsWith("/api/getCollectItems"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.getCollectItems(request, response);
            //}
            //添加新红包
            //else if (url.Equals("/api/postDailyAward"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                DailyAwardAction.postDailyAward(request, response);
            //}
            ////修改红包
            //else if (url.Equals("/api/updateDailyAward"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                DailyAwardAction.updateDailyAward(request, response);
            //}
            ////删除红包
            //else if (url.Equals("/api/deleteDailyAward"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                DailyAwardAction.deleteDailyAward(request, response);
            //}
            ////获取红包
            //else if (url.Equals("/api/getDailyAward"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            DailyAwardAction.getDailyAward(request, response);
            //}

            ////添加新奖品
            //else if (url.Equals("/api/postAwardInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                AwardInfoAction.postAwardInfo(request, response);
            //}
            ////修改奖品
            //else if (url.Equals("/api/updateAwardInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                AwardInfoAction.updateAwardInfo(request, response);
            //}
            ////删除奖品
            //else if (url.Equals("/api/deleteAwardInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                AwardInfoAction.deleteAwardInfo(request, response);
            //}
            ////获取奖品
            //else if (url.Equals("/api/getAwardInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            AwardInfoAction.getAwardInfo(request, response);
            //}
            //领取红包
            //else if (url.Equals("/api/pullDailyAward"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            DailyAwardAction.pullDailyAward(request, response);
            //}
            ////查询红包领取日志
            //else if (url.Equals("/api/getDailyAwardLog"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            DailyAwardAction.getDailyAwardLog(request, response);
            //}

            ////添加新活动任务
            //else if (url.Equals("/api/postProgression"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ProgressionTAction.postProgressionT(request, response);
            //}
            ////修改活动任务
            //else if (url.Equals("/api/updateProgression"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ProgressionTAction.updateProgressionT(request, response);
            //}
            ////删除活动任务
            //else if (url.Equals("/api/deleteProgression"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ProgressionTAction.deleteProgressionT(request, response);
            //}
            ////获取活动任务
            //else if (url.Equals("/api/getProgression"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ProgressionTAction.getProgressionT(request, response);
            //}
            ////获取活动进展
            //else if (url.Equals("/api/getProgressionProgress"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ProgressionTAction.getProgressionProgress(request, response);
            //}
            ////获取活动任务奖品
            //else if (url.Equals("/api/getPregressionAward"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ProgressionTAction.getPregressionAward(request, response);
            //}
            ////获取领取情况
            //else if (url.Equals("/api/getPregressionPull"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ProgressionTAction.getPregressionPull(request, response);
            //}
            ////添加新抽奖
            //else if (url.Equals("/api/postLottery"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                LotteryAction.postLottery(request, response);
            //}
            ////修改抽奖
            //else if (url.Equals("/api/updateLottery"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                LotteryAction.updateLottery(request, response);
            //}
            ////删除抽奖
            //else if (url.Equals("/api/deleteLottery"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                LotteryAction.deleteLottery(request, response);
            //}
            ////查询抽奖
            //else if (url.Equals("/api/getLottery"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            LotteryAction.getLottery(request, response);
            //}
            ////抽奖
            //else if (url.Equals("/api/doLottery"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            LotteryAction.doLottery(request, response);
            //}

            ////添加新签到
            //else if (url.Equals("/api/postSignInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                SignInfoAction.postSignInfo(request, response);
            //}
            ////修改签到
            //else if (url.Equals("/api/updateSignInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                SignInfoAction.updateSignInfo(request, response);
            //}
            ////删除签到
            //else if (url.Equals("/api/deleteSignInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                SignInfoAction.deleteSignInfo(request, response);
            //}
            ////查询签到
            //else if (url.Equals("/api/getSignInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SignInfoAction.getSignInfo(request, response);
            //}
            ////查询签到
            //else if (url.Equals("/api/getAvailableSignInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SignInfoAction.getAvailableSignInfo(request, response);
            //}
            ////签到
            //else if (url.Equals("/api/doSignInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SignInfoAction.doSignInfo(request, response);
            //}
            ////补签
            //else if (url.Equals("/api/doReSignInfo"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SignInfoAction.doReSignInfo(request, response);
            //}
            ////获取签到情况
            //else if (url.Equals("/api/getSignLog"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SignInfoAction.getSignLog(request, response);
            //}
            ////查询用户信息分页
            //else if (url.Equals("/api/getUserByPage"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.getUserByPage(request, response);
            //}
            ////更新用户信息
            //else if (url.Equals("/api/updateUserInfoParam"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.updateUserInfoParam(request, response);
            //}
            ////库存查询
            //else if (url.Equals("/api/getStorageByPage"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.getStorageByPage(request, response);
            //}
            ////库存修改
            //else if (url.Equals("/api/updateUserStorageParam"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.updateUserStorageParam(request, response);
            //}
            ////交易查询
            //else if (url.Equals("/api/getTradeByPage"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.getTradeByPage(request, response);
            //}
            ////交易修改
            //else if (url.Equals("/api/updateTradeeParam"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.updateTradeeParam(request, response);
            //}
            ////查询操作记录
            //else if (url.Equals("/api/getActionByPage"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.getActionByPage(request, response);
            //}
            ////获取游戏事件记录
            //else if (url.Equals("/api/getGameActionByPage"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.getGameActionByPage(request, response);
            //}
            ////获取活跃用户
            //else if (url.Equals("/api/getDailyUser"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.getDailyUser(request, response);
            //}
            ////获取交易信息
            //else if (url.Equals("/api/getDailyTrade"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.getDailyTrade(request, response);
            //}
            ////获取出售物品信息getDailySell
            //else if (url.Equals("/api/getDailySell"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ManageAction.getDailySell(request, response);
            //}
            ////点赞
            //else if (url.Equals("/api/like"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.like(request, response);
            //}
            ////获取点赞信息
            //else if (url.Equals("/api/likecount"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            UserInfoAction.likecount(request, response);
            //}
            ////添加物品转换
            //else if (url.Equals("/api/postExchange"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ItemExchangeAction.postExchange(request, response);
            //}
            ////修改物品转换
            //else if (url.Equals("/api/updateExchange"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ItemExchangeAction.updateExchange(request, response);
            //}
            ////删除物品转换
            //else if (url.Equals("/api/deleteExchange"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ItemExchangeAction.deleteExchange(request, response);
            //}
            ////查询物品转换
            //else if (url.Equals("/api/getExchange"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ItemExchangeAction.getExchange(request, response);
            //}
            ////获取有效物品
            //else if (url.Equals("/api/getAvalivleItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ItemExchangeAction.getAvalivleItem(request, response);
            //}
            ////获取有效物品
            //else if (url.Equals("/api/craftItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ItemExchangeAction.craftItem(request, response);
            //}
            ////制作物品
            //else if (url.Equals("/api/craftItem"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ItemExchangeAction.craftItem(request, response);
            //}
            ////获取正在制作的列表
            //else if (url.Equals("/api/getCrafting"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ItemExchangeAction.getCrafting(request, response);
            //}
            ////获取制作产出物品
            //else if (url.Equals("/api/getCraftAward"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ItemExchangeAction.getCraftAward(request, response);
            //}
            ////查询抽奖次数
            //else if (url.Equals("/api/QueryLotteryCount"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            LotteryAction.QueryLotteryCount(request, response);
            //}
            ////查询交易限制
            //else if (url.Equals("/api/QueryItemLimitConfig"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            ItemLimitConfigAction.QueryItemLimitConfig(request, response);
            //}
            ////添加交易限制
            //else if (url.Equals("/api/postItemLimitConfig"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ItemLimitConfigAction.postItemLimitConfig(request, response);
            //}
            ////更新交易限制
            //else if (url.Equals("/api/updateItemLimitConfig"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ItemLimitConfigAction.updateItemLimitConfig(request, response);
            //}
            ////删除交易限制
            //else if (url.Equals("/api/deleteItemLimitConfig"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            if (Filters.isAdminFilter(request, response))
            //                ItemLimitConfigAction.updateItemLimitConfig(request, response);
            //}
            ////水果机获取点数
            //else if (url.Equals("/api/GetSGJPoint"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SlotMachineAction.GetSGJPoint(request, response);
            //}
            ////水果机充值点数
            //else if (url.Equals("/api/ChargeSGJPoint"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SlotMachineAction.ChargeSGJPoint(request, response);
            //}
            ////水果机提取
            //else if (url.Equals("/api/ReleaseSGJPoint"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SlotMachineAction.ReleaseSGJPoint(request, response);
            //}
            ////开始roll
            //else if (url.Equals("/api/SGJRolling"))
            //{
            //    if (Filters.IsServerReady(request, response))
            //        if (Filters.UserLoginFilter(request, response))
            //            SlotMachineAction.SGJRolling(request, response);
            //}
        }
    }
}
