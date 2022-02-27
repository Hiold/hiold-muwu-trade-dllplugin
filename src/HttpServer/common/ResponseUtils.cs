using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.common
{
    class ResponseUtils
    {
        public static void setResponseJSON(HttpListenerResponse response)
        {
            response.ContentType = "application/json;charset=utf-8";
            response.Headers.Set("Server", "hiold-Server");
        }

        public static void ResponseSuccess(HttpListenerResponse response)
        {
            ResponseUtils.setResponseJSON(response);
            //设置返回数据
            ResponseBase rbase = new ResponseBase()
            {
                respCode = "1",
                respMsg = "success",
            };
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(SimpleJson2.SimpleJson2.SerializeObject(rbase));
            //对客户端输出相应信息.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            //关闭输出流，释放相应资源
            output.Close();
        }

        public static void ResponseSuccessWithData(HttpListenerResponse response, Object data)
        {
            ResponseUtils.setResponseJSON(response);
            //设置返回数据
            ResponseBase rbase = new ResponseBase()
            {
                respCode = "1",
                respMsg = "success",
                data = data
            };

            //序列化返回对象
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(SimpleJson2.SimpleJson2.SerializeObject(rbase));
            //对客户端输出相应信息.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            //关闭输出流，释放相应资源


            output.Close();
        }

        public static void ResponseFail(HttpListenerResponse response, string msg)
        {
            ResponseUtils.setResponseJSON(response);
            //设置返回数据
            ResponseBase rbase = new ResponseBase()
            {
                respCode = "0",
                respMsg = msg,
            };
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(SimpleJson2.SimpleJson2.SerializeObject(rbase));
            //对客户端输出相应信息.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            //关闭输出流，释放相应资源
            output.Close();
        }
    }

    class ResponseBase
    {
        public string respCode { get; set; }
        public string respMsg { get; set; }
        public Object data { get; set; }
        public string ext { get; set; }
    }
}
