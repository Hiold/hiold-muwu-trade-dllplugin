using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.Commons;
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
    class ProgressionTAction
    {
        public static void postProgressionT(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("key", out string key);
                queryRequest.TryGetValue("ProgressionTType", out string ProgressionTType);
                queryRequest.TryGetValue("value", out string value);
                queryRequest.TryGetValue("desc", out string desc);

                //添加新的每日奖励
                ProgressionTService.addProgressionT(new ProgressionT()
                {
                    type = int.Parse(type),
                    progressionType = int.Parse(ProgressionTType),
                    key = key,
                    value = value,
                    desc = desc,
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    extinfo6 = "",
                    status="1"
                });

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.AddProgression,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("添加新活动任务")
                });

                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public static void updateProgressionT(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("key", out string key);
                queryRequest.TryGetValue("ProgressionTType", out string ProgressionTType);
                queryRequest.TryGetValue("value", out string value);
                queryRequest.TryGetValue("desc", out string desc);

                ProgressionT target = ProgressionTService.getProgressionTByid(id);
                if (target != null)
                {
                    target.type = int.Parse(type);
                    target.key = key;
                    target.progressionType = int.Parse(ProgressionTType);
                    target.value = value;
                    target.desc = desc;
                }

                ProgressionTService.UpdateProgressionT(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.editProgression,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("修改了活动任务")
                });
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public static void deleteProgressionT(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);

                ProgressionT target = ProgressionTService.getProgressionTByid(id);
                if (target != null)
                {
                    target.status = "0";
                }

                ProgressionTService.UpdateProgressionT(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.deleteProgression,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("删除了活动任务")
                });
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public static void getProgressionT(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                ResponseUtils.ResponseSuccessWithData(response, ProgressionTService.getProgressionTs());
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
