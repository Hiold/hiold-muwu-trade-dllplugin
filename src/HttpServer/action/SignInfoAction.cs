using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.Commons;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.database;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class SignInfoAction
    {
        public static void postSignInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("date", out string date);
                queryRequest.TryGetValue("day", out string day);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("itemicon", out string itemicon);
                queryRequest.TryGetValue("itemtint", out string itemtint);
                queryRequest.TryGetValue("itemchinese", out string itemchinese);
                queryRequest.TryGetValue("quality", out string quality);
                queryRequest.TryGetValue("count", out string count);
                if (!string.IsNullOrEmpty(date))
                {
                    List<SignInfo> resultInfos = SignInfoService.ValidateDate(date);
                    if (resultInfos != null && resultInfos.Count > 0)
                    {
                        ResponseUtils.ResponseFail(response, "已存在该日期的配置，不能重复添加");
                        return;
                    }
                }
                else
                {
                    List<SignInfo> resultInfos = SignInfoService.ValidateCircle(day);
                    if (resultInfos != null && resultInfos.Count > 0)
                    {
                        ResponseUtils.ResponseFail(response, "已存在该日期的配置，不能重复添加");
                        return;
                    }
                }


                SignInfo sinfo = new SignInfo()
                {
                    type = type,
                    day = day,
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    extinfo6 = "",
                    status = "1",
                    count = count,
                    itemname = itemname,
                    itemicon = itemicon,
                    itemtint = itemtint,
                    itemchinese = itemchinese,
                    quality = quality,

                };
                //根据参数赋值date
                if (!string.IsNullOrEmpty(date))
                {
                    sinfo.date = DateTime.Parse(date);
                }

                //添加新的每日奖励
                SignInfoService.addSignInfo(sinfo);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.AddSignInfo,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("添加新签到")
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


        public static void updateSignInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("date", out string date);
                queryRequest.TryGetValue("day", out string day);
                queryRequest.TryGetValue("type", out string type);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("itemicon", out string itemicon);
                queryRequest.TryGetValue("itemtint", out string itemtint);
                queryRequest.TryGetValue("itemchinese", out string itemchinese);
                queryRequest.TryGetValue("quality", out string quality);
                queryRequest.TryGetValue("count", out string count);


                SignInfo target = SignInfoService.getSignInfoByid(id);
                if (target != null)
                {
                    target.type = type;
                    target.day = day;
                    target.itemname = itemname;
                    target.itemicon = itemicon;
                    target.itemtint = itemtint;
                    target.itemchinese = itemchinese;
                    target.quality = quality;
                    target.count = count;
                    //根据参数赋值date
                    if (!string.IsNullOrEmpty(date))
                    {
                        target.date = DateTime.Parse(date);
                    }
                }

                SignInfoService.UpdateSignInfo(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.editSignInfo,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("修改了签到")
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


        public static void deleteSignInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);

                SignInfo target = SignInfoService.getSignInfoByid(id);
                if (target != null)
                {
                    target.status = "0";
                }

                SignInfoService.UpdateSignInfo(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.deleteSignInfo,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("删除了签到")
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

        public static void getSignInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                ResponseUtils.ResponseSuccessWithData(response, SignInfoService.getSignInfos());
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        /// <summary>
        /// 执行签到
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void doSignInfo(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            string postData = ServerUtils.getPostData(request.request);
            Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
            queryRequest.TryGetValue("id", out string id);
            SignInfo targetSignInfo = SignInfoService.getSignInfoByid(id);
            int dateInt = (int)DateTime.Now.DayOfWeek;
            if (targetSignInfo.date.ToString("yyyy-MM-dd").Equals("0001-01-01"))
            {
                if (dateInt != int.Parse(targetSignInfo.day))
                {
                    ResponseUtils.ResponseFail(response, "日期异常无法签到");
                    return;
                }
            }
            else
            {
                if (!targetSignInfo.date.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    ResponseUtils.ResponseFail(response, "日期异常无法签到");
                    return;
                }
            }
            Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, targetSignInfo.date.ToString("yyyy-MM-dd"), targetSignInfo.day);
            if (countResult>0)
            {
                ResponseUtils.ResponseFail(response, "不能重复签到");
                return;
            }

            List<AwardInfo> awards = AwardInfoService.getAwardInfos(targetSignInfo.id + "", AwardInfoTypeConfig.SIGNINFO);
            if (awards == null || awards.Count <= 0)
            {
                ResponseUtils.ResponseFail(response, "此奖池中没有奖品，抽奖失败");
                return;
            }

            //计算完毕发放礼品
            string awardinfo = AwardDeliverTools.DeliverAward(request.user, awards);

            //记录日志数据
            ActionLogService.addLog(new ActionLog()
            {
                actTime = DateTime.Now,
                actType = LogType.doSignInfo,
                atcPlayerEntityId = request.user.gameentityid,
                extinfo1 = id,
                extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(awards),
                extinfo4 = targetSignInfo.date.ToString("yyyy-MM-dd"),
                extinfo5 = targetSignInfo.day,
                desc = "签到获得奖品：" + awardinfo
            });
            ResponseUtils.ResponseSuccessWithData(response, awards);
        }


        /// <summary>
        /// 执行签到
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void doReSignInfo(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            string postData = ServerUtils.getPostData(request.request);
            Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
            queryRequest.TryGetValue("id", out string id);
            SignInfo targetSignInfo = SignInfoService.getSignInfoByid(id);
            if (targetSignInfo.type.Equals("-1"))
            {
                ResponseUtils.ResponseFail(response, "这天不允许补签");
                return;
            }

            Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, targetSignInfo.date.ToString("yyyy-MM-dd"), targetSignInfo.day);
            if (countResult > 0)
            {
                ResponseUtils.ResponseFail(response, "不能重复补签");
                return;
            }

            double count = double.Parse(targetSignInfo.count);
            //积分
            if (targetSignInfo.type.Equals("1"))
            {
                if (!DataBase.MoneyEditor(request.user, DataBase.MoneyType.Money, DataBase.EditType.Sub, count))
                {
                    ResponseUtils.ResponseFail(response, "积分不足");
                    return;
                }
            }
            //点券
            if (targetSignInfo.type.Equals("2"))
            {
                if (!DataBase.MoneyEditor(request.user, DataBase.MoneyType.Credit, DataBase.EditType.Sub, count))
                {
                    ResponseUtils.ResponseFail(response, "点券不足");
                    return;
                }
            }
            //游戏内物品
            if (targetSignInfo.type.Equals("3"))
            {
                int quality = 0;
                int.TryParse(targetSignInfo.quality, out quality);
                UserStorage us = UserStorageService.selectAvaliableItem(request.user.gameentityid + "", targetSignInfo.itemname, quality + "", "1", targetSignInfo.count);
                if (us == null)
                {
                    ResponseUtils.ResponseFail(response, "没有足够的物品抽奖");
                    return;
                }
                //更新库存
                us.storageCount -= int.Parse(targetSignInfo.count);
                if (us.storageCount <= 0)
                {
                    us.itemUsedChenal = UserStorageUsedChanel.RESIGNED;
                    us.itemStatus = UserStorageStatus.RESIGNED;
                }
                UserStorageService.UpdateUserStorage(us);
            }
            //特殊物品
            if (targetSignInfo.type.Equals("4"))
            {
                int quality = 0;
                int.TryParse(targetSignInfo.quality, out quality);
                UserStorage us = UserStorageService.selectAvaliableItem(request.user.gameentityid + "", targetSignInfo.itemname, quality + "", "2", targetSignInfo.count);
                if (us == null)
                {
                    ResponseUtils.ResponseFail(response, "没有足够的物品抽奖");
                    return;
                }
                //更新库存
                us.storageCount -= int.Parse(targetSignInfo.count);
                if (us.storageCount <= 0)
                {
                    us.itemUsedChenal = UserStorageUsedChanel.RESIGNED;
                    us.itemStatus = UserStorageStatus.RESIGNED;
                }
                UserStorageService.UpdateUserStorage(us);
            }



            List<AwardInfo> awards = AwardInfoService.getAwardInfos(targetSignInfo.id + "", AwardInfoTypeConfig.SIGNINFO);
            if (awards == null || awards.Count <= 0)
            {
                ResponseUtils.ResponseFail(response, "此奖池中没有奖品，抽奖失败");
                return;
            }

            //计算完毕发放礼品
            string awardinfo = AwardDeliverTools.DeliverAward(request.user, awards);

            //记录日志数据
            ActionLogService.addLog(new ActionLog()
            {
                actTime = DateTime.Now,
                actType = LogType.doReSignInfo,
                atcPlayerEntityId = request.user.gameentityid,
                extinfo1 = id,
                extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(awards),
                extinfo4 = targetSignInfo.date.ToString("yyyy-MM-dd"),
                extinfo5 = targetSignInfo.day,
                desc = "补签获得奖品：" + awardinfo
            });
            ResponseUtils.ResponseSuccessWithData(response, awards);
        }


        public static void getAvailableSignInfo(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            string postData = ServerUtils.getPostData(request.request);
            Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
            queryRequest.TryGetValue("id", out string id);
            //查询可用签到数据，建立返回集合
            List<SignInfo> avalibleSignInfo = SignInfoService.getSignInfosAvalible();
            SignInfoAvalible[] returnData = new SignInfoAvalible[7];
            //处理当前周签到出具
            foreach (SignInfo temp in avalibleSignInfo)
            {
                if (temp.day.Equals("0"))
                {
                    Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, temp.date.ToString("yyyy-MM-dd"), temp.day);
                    List<AwardInfo> awards = AwardInfoService.getAwardInfos(temp.id + "", AwardInfoTypeConfig.SIGNINFO);
                    returnData[0] = new SignInfoAvalible()
                    {
                        info = temp,
                        awards = awards,
                        signed= countResult
                    };
                }
                if (temp.day.Equals("1"))
                {
                    Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, temp.date.ToString("yyyy-MM-dd"), temp.day);
                    List<AwardInfo> awards = AwardInfoService.getAwardInfos(temp.id + "", AwardInfoTypeConfig.SIGNINFO);
                    returnData[1] = new SignInfoAvalible()
                    {
                        info = temp,
                        awards = awards,
                        signed=countResult,
                    };
                }
                if (temp.day.Equals("2"))
                {
                    Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, temp.date.ToString("yyyy-MM-dd"), temp.day);
                    List<AwardInfo> awards = AwardInfoService.getAwardInfos(temp.id + "", AwardInfoTypeConfig.SIGNINFO);
                    returnData[2] = new SignInfoAvalible()
                    {
                        info = temp,
                        awards = awards,
                        signed=countResult,
                    };
                }
                if (temp.day.Equals("3"))
                {
                    Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, temp.date.ToString("yyyy-MM-dd"), temp.day);
                    List<AwardInfo> awards = AwardInfoService.getAwardInfos(temp.id + "", AwardInfoTypeConfig.SIGNINFO);
                    returnData[3] = new SignInfoAvalible()
                    {
                        info = temp,
                        awards = awards,
                        signed=countResult,
                    };
                }
                if (temp.day.Equals("4"))
                {
                    Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, temp.date.ToString("yyyy-MM-dd"), temp.day);
                    List<AwardInfo> awards = AwardInfoService.getAwardInfos(temp.id + "", AwardInfoTypeConfig.SIGNINFO);
                    returnData[4] = new SignInfoAvalible()
                    {
                        info = temp,
                        awards = awards,
                        signed=countResult,
                    };
                }
                if (temp.day.Equals("5"))
                {
                    Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, temp.date.ToString("yyyy-MM-dd"), temp.day);
                    List<AwardInfo> awards = AwardInfoService.getAwardInfos(temp.id + "", AwardInfoTypeConfig.SIGNINFO);
                    returnData[5] = new SignInfoAvalible()
                    {
                        info = temp,
                        awards = awards,
                        signed=countResult,
                    };
                }
                if (temp.day.Equals("6"))
                {
                    Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, temp.date.ToString("yyyy-MM-dd"), temp.day);
                    List<AwardInfo> awards = AwardInfoService.getAwardInfos(temp.id + "", AwardInfoTypeConfig.SIGNINFO);
                    returnData[6] = new SignInfoAvalible()
                    {
                        info = temp,
                        awards = awards,
                        signed=countResult,
                    };
                }
            }
            ResponseUtils.ResponseSuccessWithData(response, returnData);
        }


        public static void getSignLog(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            string postData = ServerUtils.getPostData(request.request);
            Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
            queryRequest.TryGetValue("id", out string id);
            SignInfo targetSignInfo = SignInfoService.getSignInfoByid(id);

            Int64 countResult = ActionLogService.QuerySignInfoCount(request.user.gameentityid, targetSignInfo.date.ToString("yyyy-MM-dd"), targetSignInfo.day);
            
            ResponseUtils.ResponseSuccessWithData(response, countResult);
        }

        class SignInfoAvalible
        {
            public SignInfo info { get; set; }
            public List<AwardInfo> awards { get; set; }
            public Int64 signed { get; set; }
        }
    }
}
