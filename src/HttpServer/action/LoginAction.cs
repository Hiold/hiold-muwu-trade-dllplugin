using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
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
        public static void login(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                Dictionary<string, string> param = ServerUtils.GetParam(request);
                //Console.WriteLine(param["username"]);
                //Console.WriteLine(param["password"]);
                //查询用户信息
                List<UserInfo> resultList = UserService.userLogin(param["username"], param["password"]);
                if (resultList != null && resultList.Count > 0)
                {
                    ResponseUtils.ResponseSuccess(response);
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "账号或密码错误");
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }
}
