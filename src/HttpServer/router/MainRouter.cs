using HioldMod.src.HttpServer.action;
using HioldMod.src.HttpServer.common;
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
            if (url.Equals("/api/login"))
            {
                LoginAction.login(request, response);
            }
            //登录
            if (url.Equals("/api/debug"))
            {
                LoginAction.debug(request, response);
            }
            else if (url.Equals("/api/getSystemItem"))
            {
                GameItemAction.getSystemItem(request, response);
            }
            else if (url.Equals("/api/getTranslation"))
            {
                TranslationAction.getTranslation(request, response);
            }
            else if (url.StartsWith("/api/image"))
            {
                GameItemAction.getImage(request, response);
            }
            else if (url.StartsWith("/api/iconImage"))
            {
                GameItemAction.getImageIcon(request, response);
            }
            else if (url.StartsWith("/api/addShopItem"))
            {
                TradeManageAction.addShopItem(request, response);
            }
            else if (url.StartsWith("/api/queryShopItem"))
            {
                TradeManageAction.queryShopItem(request, response);
            }
            else if (url.StartsWith("/api/updateShopItem"))
            {
                TradeManageAction.updateShopItem(request, response);
            }
            else if (url.StartsWith("/api/deleteShopItem"))
            {
                TradeManageAction.deleteShopItem(request, response);
            }
            else if (url.StartsWith("/api/uploadFile"))
            {
                FileUpload.uploadFile(request, response);
            }
            else if (url.StartsWith("/api/getIconFile"))
            {
                FileUpload.getIconFile(request, response);
            }
            //没有匹配的router 返回404
            else
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("404-ERROR");
                //对客户端输出相应信息.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                //关闭输出流，释放相应资源
                output.Close();
            }
        }
    }
}
