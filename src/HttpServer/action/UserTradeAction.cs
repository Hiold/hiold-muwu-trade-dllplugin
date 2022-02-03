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
                    TradeManageItem item = ShopTradeService.getShopItemById(int.Parse(_buy.id))[0];
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
                        extinfo1="",
                        extinfo2 = "",
                        extinfo3 = "",
                        extinfo4 = "",
                        extinfo5 = "",
                    };

                    

                    //添加数据到数据库
                    UserStorageService.addUserStorage(userStorate);
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
        }
    }
}
