using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;

namespace HioldMod
{

    public class Server
    {

        //用户会话信息
        public static Dictionary<string, UserInfo> userCookies = new Dictionary<string, UserInfo>();
        public static void RunServer(int port)
        {

            //在新线程中开启任务防止阻塞主线程
            new Thread(new ThreadStart(delegate
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add(string.Format("http://*:{0}/", port)); //添加需要监听的url范围
                while (true)
                {
                    listener.Start(); //开始监听端口，接收客户端请求
                    //阻塞主函数至接收到一个客户端请求为止
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    //尝试获取用户是cookie
                    //组装自定义请求对象
                    HioldRequest hioldRequest = new HioldRequest();
                    hioldRequest.request = request;

                    string sessionId = null;
                    for (int i = 0; i < request.Cookies.Count; i++)
                    {
                        Cookie ck = request.Cookies[i];
                        if (ck.Name.Equals("SESSION_ID") && !ck.Expired)
                        {
                            sessionId = ck.Value;
                        }
                    }
                    //如果Session为空为用户生成Session
                    if (sessionId == null)
                    {
                        Cookie ce = new Cookie();
                        ce.Name = "SESSION_ID";
                        ce.Value = ServerUtils.GetRandomString(32);
                        ce.Expires = DateTime.Now.AddDays(1);
                        ce.Path = "/";
                        response.Cookies.Add(ce);
                    }
                    else
                    {
                        hioldRequest.sessionid = sessionId;
                        if (userCookies.TryGetValue(sessionId, out UserInfo ui))
                        {
                            hioldRequest.user = ui;
                        }
                    }
                    //使用分发器处理请求
                    HioldMod.HttpServer.router.MainRouter.DispacherRouter(hioldRequest, response);
                }
                listener.Stop(); //关闭HttpListener
            })).Start();
        }
    }
}
