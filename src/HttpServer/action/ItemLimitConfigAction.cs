using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class ItemLimitConfigAction
    {
        public static void QueryItemLimitConfig(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);

            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }
}
