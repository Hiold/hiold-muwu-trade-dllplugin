using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class UserInfoAction
    {
        /// <summary>
        /// 用户登录action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getdisCountTicket(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //string postData = ServerUtils.getPostData(request.request);
                ////Dictionary<string, string> param = ServerUtils.GetParam(request);
                //info _info = new info();
                //_info = (info)SimpleJson2.SimpleJson2.DeserializeObject(postData, _info.GetType());
                List<UserStorage> cous = UserStorageService.selectPlayersCou(request.user.gameentityid);
                //List<UserInfo> resultList = UserService.userLogin(_info.username, ServerUtils.md5(_info.password));

                ResponseUtils.ResponseSuccessWithData(response, cous);

            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void debug(HttpListenerRequest request, HttpListenerResponse response)
        {
            FileStream fs = new FileStream("D:/test.txt", FileMode.OpenOrCreate);
            byte[] data = Encoding.UTF8.GetBytes(SimpleJson2.SimpleJson2.SerializeObject(request.Headers));
            fs.Write(data, 0, data.Length);


            fs.Flush();
            fs.Close();
            response.StatusCode = 200;
            response.Close();
        }


        public class info
        {
            public string userid { get; set; }
        }
    }
}
