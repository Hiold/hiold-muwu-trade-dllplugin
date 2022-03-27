using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class ManageAction
    {
        public static void getUserByPage(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("sorttype", out string sorttype);
                queryRequest.TryGetValue("name", out string name);
                queryRequest.TryGetValue("steamid", out string steamid);
                queryRequest.TryGetValue("eosid", out string eosid);
                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);
                ResponseUtils.ResponseSuccessWithData(response, UserService.getUserByPage(sorttype, name, steamid, eosid, int.Parse(page), int.Parse(limit)));
            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }
}
