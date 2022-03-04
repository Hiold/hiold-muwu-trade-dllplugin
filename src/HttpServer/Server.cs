using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;

namespace HioldModServer
{

    public class Server
    {

        //用户会话信息
        public static Dictionary<string, UserInfo> userCookies = new Dictionary<string, UserInfo>();
        //用户Token信息
        public static Dictionary<string, string> userToken = new Dictionary<string, string>();
        public static void RunServer(int port)
        {


            //在新线程中开启任务防止阻塞主线程
            new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    try
                    {
                        HttpListener listener = new HttpListener();
                        listener.Prefixes.Add(string.Format("http://*:{0}/", port)); //添加需要监听的url范围

                        listener.Start();
                        new Thread(new ThreadStart(delegate
                        {
                            while (true)
                            {
                                try
                                {
                                    //listener.GetContext();会阻塞函数执行
                                    HttpListenerContext context = listener.GetContext();
                                    //开启并行处理线程
                                    Task.Run(() =>
                                    {
                                        //LogUtils.Loger("当前Thread：" + Thread.CurrentThread.ManagedThreadId);
                                        try
                                        {
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
                                                //首次请求也带cookie
                                                request.Cookies.Add(ce);
                                                hioldRequest.sessionid = ce.Value;
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
                                            try
                                            {
                                                HioldMod.HttpServer.router.MainRouter.DispacherRouter(hioldRequest, response);
                                            }
                                            catch (Exception e)
                                            {
                                                LogUtils.Loger("api handler error");
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            LogUtils.Loger(e.StackTrace);
                                        }
                                    });
                                }
                                catch (Exception e)
                                {
                                    LogUtils.Loger(e.StackTrace);
                                }
                            }
                        })).Start();
                        LogUtils.Loger("Hi_oldMod：服务器重启完成，监听正常");
                        break;
                    }
                    catch (Exception e)
                    {
                        LogUtils.Loger(e.Message);
                        LogUtils.Loger("Hi_oldMod：检测到端口被占用，服务器正在无缝重启，等待重启完成");
                        Thread.Sleep(1000);
                    }
                }
            })).Start();




















































            //new Thread(new ThreadStart(delegate
            //{
            //    while (true)
            //    {

            //        try
            //        {
            //            //在新线程中开启任务防止阻塞主线程
            //            new Thread(new ThreadStart(delegate
            //            {

            //                try
            //                {
            //                    HttpListener listener = new HttpListener();
            //                    listener.Prefixes.Add(string.Format("http://*:{0}/", port)); //添加需要监听的url范围
            //                    while (true)
            //                    {
            //                        listener.Start(); //开始监听端口，接收客户端请求
            //                                          //阻塞主函数至接收到一个客户端请求为止
            //                        HttpListenerContext context = listener.GetContext();
            //                        HttpListenerRequest request = context.Request;
            //                        HttpListenerResponse response = context.Response;
            //                        //尝试获取用户是cookie
            //                        //组装自定义请求对象
            //                        HioldRequest hioldRequest = new HioldRequest();
            //                        hioldRequest.request = request;

            //                        string sessionId = null;
            //                        for (int i = 0; i < request.Cookies.Count; i++)
            //                        {
            //                            Cookie ck = request.Cookies[i];
            //                            if (ck.Name.Equals("SESSION_ID") && !ck.Expired)
            //                            {
            //                                sessionId = ck.Value;
            //                            }
            //                        }
            //                        //如果Session为空为用户生成Session
            //                        if (sessionId == null)
            //                        {
            //                            Cookie ce = new Cookie();
            //                            ce.Name = "SESSION_ID";
            //                            ce.Value = ServerUtils.GetRandomString(32);
            //                            ce.Expires = DateTime.Now.AddDays(1);
            //                            ce.Path = "/";
            //                            response.Cookies.Add(ce);
            //                            hioldRequest.sessionid = ce.Value;
            //                        }
            //                        else
            //                        {
            //                            hioldRequest.sessionid = sessionId;
            //                            if (userCookies.TryGetValue(sessionId, out UserInfo ui))
            //                            {
            //                                hioldRequest.user = ui;
            //                            }
            //                        }
            //                        //使用分发器处理请求
            //                        HioldMod.HttpServer.router.MainRouter.DispacherRouter(hioldRequest, response);
            //                    }
            //                }
            //                catch (Exception e)
            //                {
            //                    Log.Error(e.StackTrace);
            //                }
            //            })).Start();
            //            //服务器启动完毕
            //            LogUtils.Loger("交易系统API启动成功");

            //        }
            //        catch (Exception e)
            //        {
            //            LogUtils.Loger(e.Message);
            //            LogUtils.Loger("Hi_oldMod：检测到端口被占用，服务器正在无缝重启，等待重启完成");
            //            Thread.Sleep(1000);
            //        }
            //    }
            //})).Start();
        }
    }
}
