using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
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
    class LoginAction
    {
        /// <summary>
        /// 用户登录action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void login(HioldRequest request, HttpListenerResponse response)
        {
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
                    ui.password = "[masked]";
                    if (Server.userCookies.TryGetValue(request.sessionid, out UserInfo uis))
                    {
                        Server.userCookies[request.sessionid] = uis;
                    }
                    else
                    {
                        Server.userCookies.Add(request.sessionid, ui);
                    }
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
        /// 测试
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
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
