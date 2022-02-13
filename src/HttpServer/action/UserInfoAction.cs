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
        /// 用户登录action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getItemBuyLimit(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                limitinfo _limit = new limitinfo();
                _limit = (limitinfo)SimpleJson2.SimpleJson2.DeserializeObject(postData, _limit.GetType());
                //获取当日购买数量
                string tdStart = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                string tdEnd = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                Int64 tdCount = ActionLogService.QueryItemLogCount(request.user.gameentityid, _limit.id, LogType.BuyItem, tdStart, tdEnd);
                Int64 allCount = ActionLogService.QueryItemLogCount(request.user.gameentityid, _limit.id, LogType.BuyItem, null, null);
                limitCountInfo lci = new limitCountInfo()
                {
                    tdCount = tdCount,
                    allCount = allCount,
                };

                ResponseUtils.ResponseSuccessWithData(response, lci);


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



        /// <summary>
        /// 用户登录action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void updateCollect(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("value", out string value);
                //获取配置
                List<UserConfig> cfgs = UserConfigService.QueryConfig(request.user.gameentityid, ConfigType.Collect, id);
                if (cfgs != null && cfgs.Count > 0)
                {
                    UserConfig cfg = cfgs[0];
                    cfg.available = value;
                    cfg.updated_at = DateTime.Now;
                    UserConfigService.updateConfig(cfg);
                    ResponseUtils.ResponseSuccessWithData(response, cfg);
                }
                else
                {
                    UserConfig cfg = new UserConfig()
                    {
                        created_at = DateTime.Now,
                        name = request.user.name,
                        gameentityid = request.user.gameentityid,
                        platformid = request.user.platformid,
                        configType = ConfigType.Collect,
                        configValue = id,
                        available = "1"
                    };
                    UserConfigService.addConfig(cfg);
                    ResponseUtils.ResponseSuccessWithData(response, cfg);
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }


        public class info
        {
            public string userid { get; set; }
        }

        public class limitinfo
        {
            public string id { get; set; }
        }

        public class limitCountInfo
        {
            public Int64 tdCount { get; set; }
            public Int64 allCount { get; set; }
        }
    }
}
