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

        /// <summary>
        /// 执行抽奖
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void doLottery(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            string postData = ServerUtils.getPostData(request.request);
            Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
            queryRequest.TryGetValue("id", out string id);
            queryRequest.TryGetValue("count", out string count);
            int lotteryTimes = 0;
            Lottery target = LotteryService.getLotteryByid(id);
            List<AwardInfo> awards = AwardInfoService.getAwardInfos(target.id + "", AwardInfoTypeConfig.LOTTERY);
            //检查抽奖次数


            if (count.Equals("1"))
            {
                lotteryTimes = int.Parse(target.one);
            }
            else if (count.Equals("10"))
            {
                lotteryTimes = int.Parse(target.ten);
            }
            else
            {
                ResponseUtils.ResponseFail(response, "抽奖次数异常");
                return;
            }

            if (awards == null || awards.Count <= 0)
            {
                ResponseUtils.ResponseFail(response, "此奖池中没有奖品，抽奖失败");
                return;
            }

            if (target != null)
            {
                //检查抽奖限制
                if (!string.IsNullOrEmpty(target.limit))
                {
                    string tms = DateTime.Now.ToString("yyyy-MM-dd");
                    int.TryParse(target.limit, out int intlimit);
                    long lotTimes = ActionLogService.QueryLotteryCount(request.user.gameentityid, id, LogType.doLottery, tms + " 00:00:00", tms + " 23:59:59");
                    if (intlimit != -1 && lotTimes >= intlimit)
                    {
                        ResponseUtils.ResponseFail(response, "今日抽奖次数已用完");
                        return;
                    }
                    if (intlimit != -1 && lotTimes + int.Parse(count) > intlimit)
                    {
                        ResponseUtils.ResponseFail(response, "可用抽奖次数不足，剩余" + (intlimit - lotTimes) + "次");
                        return;
                    }
                }



                //积分
                if (target.type.Equals("1"))
                {
                    if (!DataBase.MoneyEditor(request.user, DataBase.MoneyType.Money, DataBase.EditType.Sub, lotteryTimes))
                    {
                        ResponseUtils.ResponseFail(response, "积分不足");
                        return;
                    }
                }
                //点券
                if (target.type.Equals("2"))
                {
                    if (!DataBase.MoneyEditor(request.user, DataBase.MoneyType.Credit, DataBase.EditType.Sub, lotteryTimes))
                    {
                        ResponseUtils.ResponseFail(response, "点券不足");
                        return;
                    }
                }
                //游戏内物品
                if (target.type.Equals("3"))
                {
                    int quality = 0;
                    int.TryParse(target.quality, out quality);
                    UserStorage us = UserStorageService.selectAvaliableItem(request.user.gameentityid + "", target.itemname, quality + "", "1", lotteryTimes + "");
                    if (us == null)
                    {
                        ResponseUtils.ResponseFail(response, "没有足够的物品抽奖");
                        return;
                    }
                    //更新库存
                    us.storageCount -= lotteryTimes;
                    if (us.storageCount <= 0)
                    {
                        us.itemUsedChenal = UserStorageUsedChanel.LOTTERYED;
                        us.itemStatus = UserStorageStatus.LOTERYED;
                    }
                    UserStorageService.UpdateUserStorage(us);
                }
                //特殊物品
                if (target.type.Equals("4"))
                {
                    int quality = 0;
                    int.TryParse(target.quality, out quality);
                    UserStorage us = UserStorageService.selectAvaliableItem(request.user.gameentityid + "", target.itemname, quality + "", "2", lotteryTimes + "");
                    if (us == null)
                    {
                        ResponseUtils.ResponseFail(response, "没有足够的物品抽奖");
                        return;
                    }
                    //更新库存
                    us.storageCount -= lotteryTimes;
                    if (us.storageCount <= 0)
                    {
                        us.itemUsedChenal = UserStorageUsedChanel.LOTTERYED;
                        us.itemStatus = UserStorageStatus.LOTERYED;
                    }
                    UserStorageService.UpdateUserStorage(us);
                }
                //积分物品扣除完毕
                int allChance = 0;
                List<int> sfs = new List<int>();
                for (int af = 0; af < awards.Count; af++)
                {
                    AwardInfo info = awards[af];
                    int.TryParse(info.chance, out int cs);
                    for (int ai = 0; ai < cs; ai++)
                    {
                        sfs.Add(af);
                    }
                    allChance += cs;
                }
                byte[] b = new byte[4];
                new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
                Random r = new Random(BitConverter.ToInt32(b, 0));
                List<AwardInfo> resultAward = new List<AwardInfo>();
                for (int bs = 0; bs < int.Parse(count); bs++)
                {
                    resultAward.Add(awards[sfs[r.Next(0, allChance)]]);
                }
                //计算完毕发放礼品
                string awardinfo = AwardDeliverTools.DeliverAward(request.user, resultAward, UserStorageGetChanel.LOTTERY);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.doLottery,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = id,
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(resultAward),
                    extinfo4 = count,
                    desc = "抽奖获得 （" + target.desc + "） 奖品：" + awardinfo
                });
                ResponseUtils.ResponseSuccessWithData(response, resultAward);
            }
            else
            {
                ResponseUtils.ResponseFail(response, "未找到此奖池");
                return;
            }
        }

        public static void QueryLotteryCount(HioldRequest request, HttpListenerResponse response)
        {
            //获取参数
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                string tms = DateTime.Now.ToString("yyyy-MM-dd");
                ResponseUtils.ResponseSuccessWithData(response, ActionLogService.QueryLotteryCount(request.user.gameentityid, id, LogType.doLottery, tms + " 00:00:00", tms + " 23:59:59"));
            }
            catch (Exception)
            {
                ResponseUtils.ResponseFail(response, "请求异常");
                return;
            }

        }

    }
}
