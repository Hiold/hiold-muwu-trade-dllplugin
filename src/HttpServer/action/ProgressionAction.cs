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
                    status = "1"
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
                queryRequest.TryGetValue("type", out string type);
                ResponseUtils.ResponseSuccessWithData(response, ProgressionTService.getProgressionTs(type));
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        public static void getProgressionProgress(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("ttype", out string ttype);
                queryRequest.TryGetValue("ptype", out string ptype);
                ResponseUtils.ResponseSuccessWithData(response, ProgressionTService.getProgressionProgress(int.Parse(ttype), int.Parse(ptype), request.user.gameentityid));
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        /// <summary>
        /// 获取玩家是否已领取奖励
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void getPregressionPull(HioldRequest request, HttpListenerResponse response)
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
                    long count = 0;
                    if (target.type == HttpServer.bean.ProgressionType.DAILY)
                    {
                        //当天日期
                        string[] daypair = ServerUtils.getDayOfToday();
                        count = ActionLogService.QueryProgresionCount(request.user.gameentityid, target.id + "", LogType.pullGetProgression, daypair[0], daypair[1]);
                    }
                    else if (target.type == HttpServer.bean.ProgressionType.WEEK)
                    {
                        //本周日期
                        string[] weekpair = ServerUtils.getDayOfThisWeek();
                        count = ActionLogService.QueryProgresionCount(request.user.gameentityid, target.id + "", LogType.pullGetProgression, weekpair[0], weekpair[1]);
                    }
                    else if (target.type == HttpServer.bean.ProgressionType.MAIN)
                    {
                        count = ActionLogService.QueryProgresionCount(request.user.gameentityid, target.id + "", LogType.pullGetProgression, null, null);
                    }
                    ResponseUtils.ResponseSuccessWithData(response, count);
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "没有找到对应的活动任务");
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public static void getPregressionAward(HioldRequest request, HttpListenerResponse response)
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
                    long process = ProgressionTService.getProgressionProgress(target.progressionType, target.type, request.user.gameentityid);
                    if (process >= double.Parse(target.value))
                    {
                        //校验是否已领取过
                        long count = 0;
                        if (target.type == HttpServer.bean.ProgressionType.DAILY)
                        {
                            //当天日期
                            string[] daypair = ServerUtils.getDayOfToday();
                            count = ActionLogService.QueryProgresionCount(request.user.gameentityid, target.id + "", LogType.pullGetProgression, daypair[0], daypair[1]);
                        }
                        else if (target.type == HttpServer.bean.ProgressionType.WEEK)
                        {
                            //本周日期
                            string[] weekpair = ServerUtils.getDayOfThisWeek();
                            count = ActionLogService.QueryProgresionCount(request.user.gameentityid, target.id + "", LogType.pullGetProgression, weekpair[0], weekpair[1]);
                        }
                        else if (target.type == HttpServer.bean.ProgressionType.MAIN)
                        {
                            count = ActionLogService.QueryProgresionCount(request.user.gameentityid, target.id + "", LogType.pullGetProgression, null, null);
                        }


                        if (count>0)
                        {
                            ResponseUtils.ResponseFail(response, "已领取过，无法重复领取");
                            return;
                        }


                        List<AwardInfo> awards = AwardInfoService.getAwardInfos(target.id + "", AwardInfoTypeConfig.PROGRESSION);
                        //获取奖励信息
                        if (awards != null && awards.Count > 0)
                        {
                            //不同类型奖品分发方式不同
                            string awardinfo = AwardDeliverTools.DeliverAward(request.user, awards);
                            //LogUtils.Loger("物品发放成功");

                            //记录日志数据
                            ActionLogService.addLog(new ActionLog()
                            {
                                actTime = DateTime.Now,
                                actType = LogType.pullGetProgression,
                                atcPlayerEntityId = request.user.gameentityid,
                                extinfo1 = id,
                                extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                                extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(awards),
                                desc = "完成了活动任务 （" + target.desc + "） 奖品：" + awardinfo
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
                    else
                    {
                        ResponseUtils.ResponseFail(response, "任务尚未完成");
                    }
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "没有找到对应的活动任务");
                }
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
