using HioldMod.HttpServer;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.router
{
    public class Filters
    {
        /// <summary>
        /// 校验用户登录情况
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool UserLoginFilter(HioldRequest request, HttpListenerResponse response)
        {
            //Console.WriteLine(request.user);
            if (request.user == null)
            {
                LogUtils.Loger(request.sessionid);
                ResponseUtils.ResponseFail(response, "未登录，无法进行操作");
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsServerReady(HioldRequest request, HttpListenerResponse response)
        {
            if (HioldMod.API.isFastRestarting == true)
            {
                LogUtils.Loger(request.sessionid);
                ResponseUtils.ResponseFail(response, "服务器正在快速重启，请耐心等待重启完毕");
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// 校验用户登录情况
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public static bool isAdminFilter(HioldRequest request, HttpListenerResponse response)
        {
            //Console.WriteLine(request.user);
            if (request.user == null || !request.user.type.Equals("1"))
            {
                LogUtils.Loger(request.sessionid);
                ResponseUtils.ResponseFail(response, "无权使用此接口");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
