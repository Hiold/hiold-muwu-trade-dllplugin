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
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    extinfo6 = ""
                });

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.AddDailyAward,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("添加新红包")
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


        public static void updateDailyAward(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("timestart", out string timestart);
                queryRequest.TryGetValue("timeend", out string timeend);
                queryRequest.TryGetValue("startdate", out string startdate);
                queryRequest.TryGetValue("enddate", out string enddate);
                queryRequest.TryGetValue("desc", out string desc);
                queryRequest.TryGetValue("type", out string type);

                DailyAward target = DailyAwardService.getDailyAwardByid(id);
                if (target != null)
                {
                    target.startdate = startdate;
                    target.enddate = enddate;
                    target.timestart = timestart;
                    target.timeend = timeend;
                    target.desc = desc;
                    target.type = type;
                }

                DailyAwardService.UpdateDailyAward(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.editDailyAward,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("修改了红包")
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


        public static void deleteDailyAward(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);

                DailyAward target = DailyAwardService.getDailyAwardByid(id);
                if (target != null)
                {
                    target.status = "0";
                }

                DailyAwardService.UpdateDailyAward(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.deleteDailyAward,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("删除了红包")
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


        public static void pullDailyAward(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                //获取红包信息
                DailyAward target = DailyAwardService.getDailyAwardByid(id);
                if (target != null)
                {
                    //判断红包是否可领取
                    string _start = "";
                    string _end = "";
                    if (string.IsNullOrEmpty(target.startdate))
                    {
                        _start += "1970-01-01";
                    }
                    else
                    {
                        _start += target.startdate;
                    }
                    if (string.IsNullOrEmpty(target.enddate))
                    {
                        _end += "9999-01-01";
                    }
                    else
                    {
                        _end += target.enddate;
                    }
                    if (string.IsNullOrEmpty(target.timestart))
                    {
                        _start += " 00:00:00";
                    }
                    else
                    {
                        _start += " " + target.timestart;
                    }
                    if (string.IsNullOrEmpty(target.timeend))
                    {
                        _end += " 23:59:59";
                    }
                    else
                    {
                        _end += " " + target.timeend;
                    }
                    DateTime sDate = DateTime.ParseExact(_start, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime eDate = DateTime.ParseExact(_end, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture);
                    DateTime now = DateTime.Now;
                    bool hbAvaliable = false;
                    if (now.Ticks > sDate.Ticks && now.Ticks < eDate.Ticks)
                    {
                        //this.hbClass = "btn b2";
                        //this.hbNotice = "抢红包";
                        hbAvaliable = true;
                    }
                    else if (now.Ticks < sDate.Ticks)
                    {
                        //this.hbClass = "btn b1";
                        //this.hbNotice = "未开始";
                        ResponseUtils.ResponseFail(response, "未开始，无法领取");
                        return;
                    }
                    else if (now.Ticks > eDate.Ticks)
                    {
                        //this.hbClass = "btn b4";
                        //this.hbNotice = "已过期";
                        ResponseUtils.ResponseFail(response, "已过期，无法领取");
                        return;
                    }

                    List<ActionLog> logs = ActionLogService.QueryDailyAwardPull(request.user.gameentityid, id);
                    //
                    if (target.type.Equals("1"))
                    {
                        //
                        foreach (ActionLog lg in logs)
                        {
                            if (lg.actTime.Ticks > DateTime.Now.Date.Ticks)
                            {
                                ResponseUtils.ResponseFail(response, "今天已领取，不能重复领取！");
                                return;
                            }
                        }
                    }
                    //单次领取服务
                    else if (target.type.Equals("2"))
                    {
                        if (logs != null && logs.Count > 0)
                        {
                            ResponseUtils.ResponseFail(response, "这个红包只能领取一次！");
                            return;
                        }
                    }


                    //可以领取红包
                    if (hbAvaliable)
                    {
                        //获取奖励信息
                        List<AwardInfo> awards = AwardInfoService.getAwardInfos(target.id + "",AwardInfoTypeConfig.DAILY_AWARD);
                        if (awards != null && awards.Count > 0)
                        {
                            //不同类型奖品分发方式不同
                            string awardinfo = AwardDeliverTools.DeliverAward(request.user, awards);
                            //LogUtils.Loger("物品发放成功");

                            //记录日志数据
                            ActionLogService.addLog(new ActionLog()
                            {
                                actTime = DateTime.Now,
                                actType = LogType.pullGetDailyAward,
                                atcPlayerEntityId = request.user.gameentityid,
                                extinfo1 = id,
                                extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                                extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(awards),
                                desc = "领取了红包 （" + target.desc + "） 奖品：" + awardinfo
                            });
                            ResponseUtils.ResponseSuccess(response);
                            return;
                        }
                        else
                        {
                            ResponseUtils.ResponseFail(response, "这个红包没有奖品，无法领取");
                            return;
                        }

                    }

                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        public static void getDailyAwardLog(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                ResponseUtils.ResponseSuccessWithData(response, ActionLogService.QueryDailyAwardPull(request.user.gameentityid, id));
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public static void getDailyAward(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                ResponseUtils.ResponseSuccessWithData(response, DailyAwardService.getDailyAwards());
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
