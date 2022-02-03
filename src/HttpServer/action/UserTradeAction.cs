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
                    UserStorage userStorate = new UserStorage() {
         id 
         itemtype 
         name 
         translate 
         itemicon 
         itemtint 
         quality 
         num 
         class1 
         class2 
         classmod 
         desc 
         couCurrType 
         couPrice 
         couCond 
         coudatelimit 
         couDateStart 
         couDateEnd 
         count 
         currency 
         price 
         discount 
         prefer 
         selltype 
         hot 
         hotset 
         show 
         stock 
         collect 
         selloutcount 
         follow 
         xglevel 
         xglevelset 
         xgday 
         xgdayset 
         xgall 
         xgallset 
         xgdatelimit 
         dateStart 
         dateEnd 
         collected 
         postTime 
         deleteTime 
    };
                    

                    userStorate.name = request.user.name;
                    userStorate.platformid = request.user.platformid;
                    userStorate.gameentityid = request.user.gameentityid;
                    userStorate.collectTime = DateTime.Now;
                    userStorate.storageCount = int.Parse(_buy.count);

                    //添加数据到数据库
                    UserStorageService.addUserStorage(userStorate);
                }
                else
                {
                    ResponseUtils.ResponseFail(response,"没登录，请登录后再试");
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
