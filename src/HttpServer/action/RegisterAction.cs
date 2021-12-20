using HioldMod.src.HttpServer.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class RegisterAction
    {
        public static void register(HttpListenerRequest request, HttpListenerResponse response)
        {
            ResponseUtils.setResponseJSON(response);
            string responseString = "{\"aaa\":\"1\"}";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            //对客户端输出相应信息.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            //关闭输出流，释放相应资源
            output.Close();
        }
    }
}
