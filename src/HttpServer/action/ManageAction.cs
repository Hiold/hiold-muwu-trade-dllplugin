using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.attributes;
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
    [ActionAttribute]
    class ManageAction
    {
        /// <summary>
        /// 查询玩家
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getUserByPage")]
        public static void getUserByPage(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("sorttype", out string sorttype);
                queryRequest.TryGetValue("name", out string name);
                queryRequest.TryGetValue("steamid", out string steamid);
                queryRequest.TryGetValue("eosid", out string eosid);
                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);
                ResponseUtils.ResponseSuccessWithData(response, UserService.getUserByPage(sorttype, name, steamid, eosid, int.Parse(page), int.Parse(limit)));
            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 更新玩家
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/updateUserInfoParam")]
        public static void updateUserInfoParam(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("data", out string data);

                List<UserInfo> uis = UserService.getUserById(id);
                if (uis != null && uis.Count > 0)
                {
                    UserInfo ui = uis[0];
                    if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot && type.Equals("money"))
                    {
                        database.DataBase.MoneyEditor(ui, database.DataBase.MoneyType.Money, database.DataBase.EditType.Set, double.Parse(data));
                    }

                    var dt = new Dictionary<string, object>();
                    dt.Add("id", id);
                    dt.Add(type, data);
                    UserService.UpdateUserInfoParam(dt);
                }
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getStorageByPage")]
        public static void getStorageByPage(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("steamid", out string steamid);
                queryRequest.TryGetValue("eosid", out string eosid);
                queryRequest.TryGetValue("username", out string username);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("getchanel", out string getchanel);
                queryRequest.TryGetValue("status", out string status);
                queryRequest.TryGetValue("group", out string group);
                queryRequest.TryGetValue("itemtype", out string itemtype);

                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);
                ResponseUtils.ResponseSuccessWithData(response, UserStorageService.selectPlayersStoragePage(steamid, eosid, username, itemname, getchanel, status, itemtype, group, int.Parse(page), int.Parse(limit)));
            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 更新库存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/updateUserStorageParam")]
        public static void updateUserStorageParam(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("data", out string data);
                if (type.Equals("itemStatus"))
                {
                    var dt2 = new Dictionary<string, object>();
                    dt2.Add("id", id);
                    dt2.Add("itemStatus", data);
                    UserStorageService.UpdateParam(dt2);
                }


                var dt = new Dictionary<string, object>();
                dt.Add("id", id);
                dt.Add(type, data);
                UserStorageService.UpdateParam(dt);
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 查询交易
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getTradeByPage")]
        public static void getTradeByPage(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("steamid", out string steamid);
                queryRequest.TryGetValue("eosid", out string eosid);
                queryRequest.TryGetValue("username", out string username);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("status", out string status);
                queryRequest.TryGetValue("group", out string group);
                queryRequest.TryGetValue("itemtype", out string itemtype);

                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);
                ResponseUtils.ResponseSuccessWithData(response, UserTradeService.selectTradeParam(steamid, eosid, username, itemname, itemtype, group, status, int.Parse(page), int.Parse(limit)));
            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 更新交易
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/updateTradeeParam")]
        public static void updateTradeeParam(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("data", out string data);

                var dt = new Dictionary<string, object>();
                dt.Add("id", id);
                dt.Add(type, data);
                UserTradeService.UpdateParam(dt);
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 查询操作记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getActionByPage")]
        public static void getActionByPage(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("steamid", out string steamid);
                queryRequest.TryGetValue("eosid", out string eosid);
                queryRequest.TryGetValue("username", out string username);
                queryRequest.TryGetValue("type", out string type);

                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);
                ResponseUtils.ResponseSuccessWithData(response, ActionLogService.QueryLogsParam(steamid, eosid, username, type, int.Parse(page), int.Parse(limit)));
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 查询游戏事件记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getGameActionByPage")]
        public static void getGameActionByPage(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("steamid", out string steamid);
                queryRequest.TryGetValue("eosid", out string eosid);
                queryRequest.TryGetValue("username", out string username);
                queryRequest.TryGetValue("type", out string type);

                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);
                ResponseUtils.ResponseSuccessWithData(response, PlayerGameEventService.QueryEventParam(steamid, eosid, username, type, int.Parse(page), int.Parse(limit)));
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 查询用户数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getDailyUser")]
        public static void getDailyUser(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                DateTime now = DateTime.Now;
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                for (int idx = 6; idx >= 0; idx--)
                {
                    Dictionary<string, object> tmp = new Dictionary<string, object>();
                    string target = now.AddDays(-idx).ToString("yyyy-MM-dd");
                    Int64[] count = ActionLogService.QueryUserActionCount(target + " 00:00:00", target + " 23:59:59");
                    tmp.Add("date", target);
                    tmp.Add("data", count);
                    result.Add(tmp);
                }
                ResponseUtils.ResponseSuccessWithData(response, result);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 查询交易数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getDailyTrade")]
        public static void getDailyTrade(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                DateTime now = DateTime.Now;
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                for (int idx = 6; idx >= 0; idx--)
                {
                    Dictionary<string, object> tmp = new Dictionary<string, object>();
                    string target = now.AddDays(-idx).ToString("yyyy-MM-dd");
                    Int64[] count = ActionLogService.QueryTradeCount(target + " 00:00:00", target + " 23:59:59");
                    tmp.Add("date", target);
                    tmp.Add("data", count);
                    result.Add(tmp);
                }
                ResponseUtils.ResponseSuccessWithData(response, result);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }

        /// <summary>
        /// 查询交易数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getDailySell")]
        public static void getDailySell(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                DateTime now = DateTime.Now;
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                for (int idx = 6; idx >= 0; idx--)
                {
                    Dictionary<string, object> tmp = new Dictionary<string, object>();
                    string target = now.AddDays(-idx).ToString("yyyy-MM-dd");
                    Int64[] count = ActionLogService.QuerySellCount(target + " 00:00:00", target + " 23:59:59");
                    tmp.Add("date", target);
                    tmp.Add("data", count);
                    result.Add(tmp);
                }
                ResponseUtils.ResponseSuccessWithData(response, result);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }
}
