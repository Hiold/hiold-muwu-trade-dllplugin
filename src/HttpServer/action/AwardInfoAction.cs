﻿using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.attributes;
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
    [ActionAttribute]
    class AwardInfoAction
    {
        /// <summary>
        /// 新增奖励
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/postAwardInfo")]
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
                //特殊属性
                queryRequest.TryGetValue("couCurrType", out string couCurrType);
                queryRequest.TryGetValue("couPrice", out string couPrice);
                queryRequest.TryGetValue("couCond", out string couCond);
                queryRequest.TryGetValue("coudatelimit", out string coudatelimit);
                queryRequest.TryGetValue("couDateStart", out string couDateStart);
                queryRequest.TryGetValue("couDateEnd", out string couDateEnd);
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
                    couCond = couCond,
                    couCurrType = couCurrType,
                    couPrice = couPrice,
                    coudatelimit = coudatelimit,
                    couDateEnd = couDateEnd,
                    couDateStart = couDateStart,
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

        /// <summary>
        /// 更新奖励
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/updateAwardInfo")]
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
                //特殊属性
                queryRequest.TryGetValue("couCurrType", out string couCurrType);
                queryRequest.TryGetValue("couPrice", out string couPrice);
                queryRequest.TryGetValue("couCond", out string couCond);
                queryRequest.TryGetValue("coudatelimit", out string coudatelimit);
                queryRequest.TryGetValue("couDateStart", out string couDateStart);
                queryRequest.TryGetValue("couDateEnd", out string couDateEnd);
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
                    target.couCurrType = couCurrType;
                    target.couCond = couCond;
                    target.couPrice = couPrice;
                    target.coudatelimit = coudatelimit;
                    target.couDateStart = couDateStart;
                    target.couDateEnd = couDateEnd;
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

        /// <summary>
        /// 删除奖励
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/deleteAwardInfo")]
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

        /// <summary>
        /// 获取奖励
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/getAwardInfo")]
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
