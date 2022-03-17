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
    class LotteryAction
    {
        public static void postLottery(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("desc", out string desc);
                queryRequest.TryGetValue("limit", out string limit);
                queryRequest.TryGetValue("one", out string one);
                queryRequest.TryGetValue("ten", out string ten);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("itemicon", out string itemicon);
                queryRequest.TryGetValue("itemtint", out string itemtint);
                queryRequest.TryGetValue("itemchinese", out string itemchinese);
                queryRequest.TryGetValue("quality", out string quality);

                //添加新的每日奖励
                LotteryService.addLottery(new Lottery()
                {
                    type = type,
                    desc = desc,
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    extinfo6 = "",
                    status = "1",
                    itemname = itemname,
                    itemicon = itemicon,
                    itemtint = itemtint,
                    limit = limit,
                    one = one,
                    ten = ten,
                    itemchinese = itemchinese,
                    quality = quality,

                });

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.AddLottery,
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


        public static void updateLottery(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("desc", out string desc);
                queryRequest.TryGetValue("limit", out string limit);
                queryRequest.TryGetValue("one", out string one);
                queryRequest.TryGetValue("ten", out string ten);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("itemicon", out string itemicon);
                queryRequest.TryGetValue("itemtint", out string itemtint);
                queryRequest.TryGetValue("itemchinese", out string itemchinese);
                queryRequest.TryGetValue("quality", out string quality);

                Lottery target = LotteryService.getLotteryByid(id);
                if (target != null)
                {
                    target.type = type;
                    target.desc = desc;
                    target.limit = limit;
                    target.one = one;
                    target.ten = ten;
                    target.itemname = itemname;
                    target.itemicon = itemicon;
                    target.itemtint = itemtint;
                    target.itemchinese = itemchinese;
                    target.quality = quality;
                }

                LotteryService.UpdateLottery(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.editLottery,
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


        public static void deleteLottery(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);

                Lottery target = LotteryService.getLotteryByid(id);
                if (target != null)
                {
                    target.status = "0";
                }

                LotteryService.UpdateLottery(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.deleteLottery,
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

        public static void getLottery(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                ResponseUtils.ResponseSuccessWithData(response, LotteryService.getLotterys());
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
