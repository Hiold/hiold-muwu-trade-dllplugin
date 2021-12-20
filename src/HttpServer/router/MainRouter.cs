using HioldMod.src.HttpServer.action;
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
        public static void DispacherRouter(HttpListenerRequest request, HttpListenerResponse response)
        {
            //Console.WriteLine(request.RawUrl);
            string url = "";
            if (request.RawUrl.Contains("?"))
            {
                url = request.RawUrl.Substring(0, request.RawUrl.IndexOf("?"));
            }
            else
            {
                url = request.RawUrl;
            }
            //登录
            if (url.Equals("/api/login")) LoginAction.login(request, response);
        }
    }
}
