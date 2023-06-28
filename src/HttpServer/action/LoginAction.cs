using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.attributes;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    [ActionAttribute]
    class LoginAction
    {
        /// <summary>
        /// 用户登录action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, url = "/api/login")]
        public static void login(HioldRequest request, HttpListenerResponse response)
        {
            //LogUtils.Loger("进入naiwazi积分同步");
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                //Dictionary<string, string> param = ServerUtils.GetParam(request);
                LoginRequest loginreq = new LoginRequest();
                loginreq = (LoginRequest)SimpleJson2.SimpleJson2.DeserializeObject(postData, loginreq.GetType());
                List<UserInfo> resultList = UserService.userLogin(loginreq.username, ServerUtils.md5(loginreq.password));
                if (resultList != null && resultList.Count > 0)
                {
                    UserInfo ui = resultList[0];
                    if (ui.status != 1)
                    {
                        ResponseUtils.ResponseFail(response, "账号已被封禁，请联系管理员");
                        return;
                    }
                    ui.password = "[masked]";
                    if (HioldModServer.Server.userCookies.TryGetValue(request.sessionid, out UserInfo uis))
                    {
                        //HioldModServer.Server.userCookies[request.sessionid] = uis;
                        HioldModServer.Server.userCookies.Remove(request.sessionid);
                    }
                    HioldModServer.Server.userCookies.Add(request.sessionid, ui);

                    //记录日志数据
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.PlayerLogin,
                        atcPlayerEntityId = ui.gameentityid,
                        desc = "使用账号密码登录了交易系统，登录ip：" + request.request.RemoteEndPoint.Address
                    });
                    
                    ResponseUtils.ResponseSuccessWithData(response, ui);
                    return;
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "账号或密码错误");
                    return;
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        /// <summary>
        /// 用户登录action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, url = "/api/ncodeLogin")]
        public static void ncodeLogin(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                string ncode = "";
                queryRequest.TryGetValue("ncode", out ncode);

                if (string.IsNullOrEmpty(ncode))
                {
                    ResponseUtils.ResponseFail(response, "Ncode错误，快捷登录失败");
                    return;
                }

                List<UserInfo> uiss = UserService.getUserByNcode(ncode);
                if (uiss == null || uiss.Count <= 0)
                {
                    if (HioldModServer.Server.userToken.TryGetValue(ncode, out string es))
                    {
                        uiss = UserService.getUserBySteamid(es);
                    }
                }


                if (uiss != null && uiss.Count > 0)
                {
                    UserInfo ui = uiss[0];
                    if (ui.status != 1)
                    {
                        ResponseUtils.ResponseFail(response, "账号已被封禁，请联系管理员");
                        return;
                    }
                    ui.password = "[masked]";
                    if (HioldModServer.Server.userCookies.TryGetValue(request.sessionid, out UserInfo uis))
                    {
                        //HioldModServer.Server.userCookies[request.sessionid] = uis;
                        HioldModServer.Server.userCookies.Remove(request.sessionid);
                    }
                    HioldModServer.Server.userCookies.Add(request.sessionid, ui);

                    //记录日志数据
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.PlayerLogin,
                        atcPlayerEntityId = ui.gameentityid,
                        desc = "使用cnode免密登录了交易系统，登录ip：" + request.request.RemoteEndPoint.Address
                    });

                    ResponseUtils.ResponseSuccessWithData(response, ui);
                    return;



                }
                else
                {
                    ResponseUtils.ResponseFail(response, "Ncode错误，快捷登录失败");
                    return;
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, url = "/api/debug")]
        public static void debug(HioldRequest request, HttpListenerResponse response)
        {
            //FileStream fs = new FileStream("D:/test.txt", FileMode.OpenOrCreate);
            //byte[] data = Encoding.UTF8.GetBytes(SimpleJson2.SimpleJson2.SerializeObject(request.request.Headers));
            //fs.Write(data, 0, data.Length);


            //fs.Flush();
            //fs.Close();
            //response.StatusCode = 200;6
            //response.Close();
            Console.WriteLine(request.sessionid);
            Console.WriteLine(request.user.name);
            response.StatusCode = 200;
            response.Close();
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, url = "/api/getCurrentUser")]
        public static void getCurrentUser(HioldRequest request, HttpListenerResponse response)
        {
            if (request.user != null)
            {
                UserInfo ui = UserService.getUserById(request.user.id + "")[0];
                ResponseUtils.ResponseSuccessWithData(response, ui);
                return;
            }
            else
            {
                ResponseUtils.ResponseFail(response, "未登录");
                return;
            }
        }


        public class LoginRequest
        {
            public string username { get; set; }
            public string password { get; set; }
        }
    }
}
