using HioldMod.HttpServer;
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
    class DailyAwardAction
    {
        public static void postDailyAward(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("timestart", out string timestart);
                queryRequest.TryGetValue("timeend", out string timeend);
                queryRequest.TryGetValue("startdate", out string startdate);
                queryRequest.TryGetValue("enddate", out string enddate);
                queryRequest.TryGetValue("desc", out string desc);
                queryRequest.TryGetValue("type", out string type);

                //添加新的每日奖励
                DailyAwardService.addDailyAward(new DailyAward
                {
                    timestart = timestart,
                    timeend = timeend,
                    startdate = startdate,
                    enddate = enddate,
                    status = "1",
                    type = type,
                    desc = desc,
                });

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.AddDailyAward,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("添加了新的红包奖励")
                });

            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }
    }
}
