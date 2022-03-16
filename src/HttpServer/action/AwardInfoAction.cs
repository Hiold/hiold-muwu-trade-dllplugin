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
    class AwardInfoAction
    {
        public static void postAwardInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("containerid", out string containerid);
                queryRequest.TryGetValue("funcid", out string funcid);
                queryRequest.TryGetValue("count", out string count);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("itemquality", out string itemquality);
                queryRequest.TryGetValue("itemchinese", out string itemchinese);
                queryRequest.TryGetValue("itemicon", out string itemicon);
                queryRequest.TryGetValue("itemtint", out string itemtint);
                queryRequest.TryGetValue("command", out string command);
                string chance = "";
                queryRequest.TryGetValue("chance", out chance);


                //添加新的每日奖励
                AwardInfoService.addAwardInfo(new AwardInfo
                {
                    type = type,
                    containerid = int.Parse(containerid),
                    count = count,
                    itemname = itemname,
                    itemquality = itemquality,
                    itemchinese = itemchinese,
                    itemicon = itemicon,
                    itemtint = itemtint,
                    command = command,
                    status = "1",
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    extinfo6 = "",
                    funcid = int.Parse(funcid),
                    chance = chance

                });

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.AddAwardInfo,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("添加新奖品")
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


        public static void updateAwardInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("containerid", out string containerid);
                queryRequest.TryGetValue("funcid", out string funcid);
                queryRequest.TryGetValue("count", out string count);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("itemquality", out string itemquality);
                queryRequest.TryGetValue("itemchinese", out string itemchinese);
                queryRequest.TryGetValue("itemicon", out string itemicon);
                queryRequest.TryGetValue("itemtint", out string itemtint);
                queryRequest.TryGetValue("command", out string command);
                string chance = "";
                queryRequest.TryGetValue("chance", out chance);
                AwardInfo target = AwardInfoService.getAwardInfoByid(id);
                if (target != null)
                {
                    target.type = type;
                    target.containerid = int.Parse(containerid);
                    target.count = count;
                    target.itemname = itemname;
                    target.itemquality = itemquality;
                    target.itemchinese = itemchinese;
                    target.itemicon = itemicon;
                    target.itemtint = itemtint;
                    target.command = command;
                    target.funcid = int.Parse(funcid);
                    target.chance = chance;
                }

                AwardInfoService.UpdateAwardInfo(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.editAwardInfo,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("修改了奖品")
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


        public static void deleteAwardInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);

                AwardInfo target = AwardInfoService.getAwardInfoByid(id);
                if (target != null)
                {
                    target.status = "0";
                }

                AwardInfoService.UpdateAwardInfo(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.deleteAwardInfo,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("删除了奖品")
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


        public static void getAwardInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("containerid", out string containerid);
                queryRequest.TryGetValue("funcid", out string funcid);
                ResponseUtils.ResponseSuccessWithData(response, AwardInfoService.getAwardInfos(containerid, funcid));
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
