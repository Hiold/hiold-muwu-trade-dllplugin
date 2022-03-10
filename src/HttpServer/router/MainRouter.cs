using HioldMod.src.HttpServer.action;
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
            //LogUtils.Loger(url);
            //登录
            if (url.Equals("/api/login"))
            {
                if (Filters.IsServerReady(request, response))
                    LoginAction.login(request, response);
            }
            else if
           //调试
           (url.Equals("/api/debug"))
            {
                if (Filters.IsServerReady(request, response))
                    LoginAction.debug(request, response);
            }
            //获取当前用户
            else if (url.Equals("/api/getCurrentUser"))
            {
                if (Filters.IsServerReady(request, response))
                    LoginAction.getCurrentUser(request, response);
            }
            //node登录
            else if (url.Equals("/api/ncodeLogin"))
            {
                if (Filters.IsServerReady(request, response))
                    LoginAction.ncodeLogin(request, response);
            }
            //更新用户信息
            else if (url.Equals("/api/updateUserInfo"))
            {
                if (Filters.IsServerReady(request, response))
                    UserInfoAction.updateUserInfo(request, response);
            }

            else if (url.Equals("/api/getSystemItem"))
            {
                if (Filters.IsServerReady(request, response))
                    GameItemAction.getSystemItem(request, response);
            }
            else if (url.Equals("/api/getSystemItemByPage"))
            {
                if (Filters.IsServerReady(request, response))
                    GameItemAction.getSystemItemByPage(request, response);
            }
            else if (url.Equals("/api/steamAuth"))
            {
                if (Filters.IsServerReady(request, response))
                    AuthAction.steamAuth(request, response);
            }
            else if (url.Equals("/api/verification"))
            {
                if (Filters.IsServerReady(request, response))
                    AuthAction.Verification(request, response);
            }
            else if (url.Equals("/api/Verification"))
            {
                if (Filters.IsServerReady(request, response))
                    TranslationAction.getTranslation(request, response);
            }
            else if (url.StartsWith("/api/image"))
            {
                if (Filters.IsServerReady(request, response))
                    GameItemAction.getImage(request, response);
            }
            else if (url.StartsWith("/api/iconImage"))
            {
                if (Filters.IsServerReady(request, response))
                    GameItemAction.getImageIcon(request, response);
            }
            else if (url.StartsWith("/api/addShopItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.isAdminFilter(request, response))
                        TradeManageAction.addShopItem(request, response);
            }
            else if (url.StartsWith("/api/queryShopItem"))
            {
                if (Filters.IsServerReady(request, response))
                    TradeManageAction.queryShopItem(request, response);
            }
            else if (url.StartsWith("/api/updateShopItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.isAdminFilter(request, response))
                        TradeManageAction.updateShopItem(request, response);
            }
            else if (url.StartsWith("/api/deleteShopItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.isAdminFilter(request, response))
                        TradeManageAction.deleteShopItem(request, response);
            }
            else if (url.StartsWith("/api/uploadFile"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.isAdminFilter(request, response))
                        FileUpload.uploadFile(request, response);
            }
            else if (url.StartsWith("/api/getIconFile"))
            {
                if (Filters.IsServerReady(request, response))
                    FileUpload.getIconFile(request, response);
            }
            else if (url.StartsWith("/api/buyItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserTradeAction.buyItem(request, response);
            }
            //获取优惠券  校验登录
            else if (url.StartsWith("/api/getdisCountTicket"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.getdisCountTicket(request, response);

            }
            //获取限购数据
            else if (url.StartsWith("/api/getItemBuyLimit"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.getItemBuyLimit(request, response);
            }
            //更新收藏数据
            else if (url.StartsWith("/api/updateCollect"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.updateCollect(request, response);
            }
            //查询用户库存
            else if (url.StartsWith("/api/getPlayerStorage"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.getPlayerStorage(request, response);

            }
            //发放物品
            else if (url.StartsWith("/api/dispachItemToGame"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.dispachItemToGame(request, response);

            }
            //删除物品
            else if (url.StartsWith("/api/deleteItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.deleteItem(request, response);

            }
            //出售物品
            else if (url.StartsWith("/api/sellOutItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserTradeAction.sellOutItem(request, response);
            }
            //下架物品
            else if (url.StartsWith("/api/TackBackItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserTradeAction.TackBackItem(request, response);

            }
            //查询玩家在售
            else if (url.StartsWith("/api/getPlayerOnSell"))
            {
                if (Filters.IsServerReady(request, response))
                    UserInfoAction.getPlayerOnSell(request, response);
            }
            //发布求购
            else if (url.StartsWith("/api/postRequire"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserRequireAction.postRequire(request, response);

            }
            //查询求购
            else if (url.StartsWith("/api/getUserRequire"))
            {
                if (Filters.IsServerReady(request, response))
                    UserRequireAction.getUserRequire(request, response);
            }
            //查询店铺信息
            else if (url.StartsWith("/api/getUserShopList"))
            {
                if (Filters.IsServerReady(request, response))
                    UserInfoAction.getUserShopList(request, response);
            }
            //查询用户信息
            else if (url.StartsWith("/api/getUserInfo"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.getUserInfo(request, response);
            }
            //取消求购
            else if (url.StartsWith("/api/cancelRequire"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserRequireAction.cancelRequire(request, response);
            }
            //购买玩家交易物品
            else if (url.StartsWith("/api/buyTradeItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserTradeAction.buyTradeItem(request, response);
            }
            //供货
            else if (url.StartsWith("/api/supplyItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserTradeAction.supplyItem(request, response);
            }
            //供货
            else if (url.StartsWith("/api/loadContainerListAround"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        GameChunckContainerAction.loadContainerListAround(request, response);
            }
            //获取容器内物品
            else if (url.StartsWith("/api/getContainerItems"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        GameChunckContainerAction.getContainerItems(request, response);
            }
            //提取物品到交易系统
            else if (url.StartsWith("/api/TakeItem"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        GameChunckContainerAction.TakeItem(request, response);
            }
            //获取日志
            else if (url.StartsWith("/api/getLogs"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.getLogs(request, response);

            }
            //获取已收藏的物品
            else if (url.StartsWith("/api/getCollectItems"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        UserInfoAction.getCollectItems(request, response);
            }
            //添加新红包
            else if (url.Equals("/api/postDailyAward"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        if (Filters.isAdminFilter(request, response))
                            DailyAwardAction.postDailyAward(request, response);
            }
            //修改红包
            else if (url.Equals("/api/updateDailyAward"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        if (Filters.isAdminFilter(request, response))
                            DailyAwardAction.updateDailyAward(request, response);
            }
            //删除红包
            else if (url.Equals("/api/deleteDailyAward"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        if (Filters.isAdminFilter(request, response))
                            DailyAwardAction.deleteDailyAward(request, response);
            }
            //获取红包
            else if (url.Equals("/api/getDailyAward"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        DailyAwardAction.getDailyAward(request, response);
            }

            //添加新奖品
            else if (url.Equals("/api/postAwardInfo"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        if (Filters.isAdminFilter(request, response))
                            AwardInfoAction.postAwardInfo(request, response);
            }
            //修改奖品
            else if (url.Equals("/api/updateAwardInfo"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        if (Filters.isAdminFilter(request, response))
                            AwardInfoAction.updateAwardInfo(request, response);
            }
            //删除奖品
            else if (url.Equals("/api/deleteAwardInfo"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        if (Filters.isAdminFilter(request, response))
                            AwardInfoAction.deleteAwardInfo(request, response);
            }
            //获取奖品
            else if (url.Equals("/api/getAwardInfo"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        AwardInfoAction.getAwardInfo(request, response);
            }
            //领取红包
            else if (url.Equals("/api/pullDailyAward"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        DailyAwardAction.pullDailyAward(request, response);
            }
            //查询红包领取日志
            else if (url.Equals("/api/getDailyAwardLog"))
            {
                if (Filters.IsServerReady(request, response))
                    if (Filters.UserLoginFilter(request, response))
                        DailyAwardAction.getDailyAwardLog(request, response);
            }

            //没有匹配的router 返回404
            else
            {
                GameItemAction.getStaticSource(request, response);
            }
        }
    }
}
