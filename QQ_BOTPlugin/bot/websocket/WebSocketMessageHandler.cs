using HioldMod.HttpServer;
using HioldMod.src.Commons;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using HioldMod.src.HttpServer.service;
using QQ_BOTPlugin.bot.websocket.bean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.websocket
{
    public class WebSocketMessageHandler
    {
        public static Dictionary<string, DateTime> check = new Dictionary<string, DateTime>();

        public static void HandleMessage(string msg)
        {
            CMD.sbConsole.AppendLine("收到消息：" + msg);
            //心跳数据
            if (msg.Contains("\"meta_event_type\":\"heartbeat\""))
            {
                HeartBeat heartBeat = SimpleJson2.SimpleJson2.DeserializeObject<HeartBeat>(msg);
                BOT.isAlive = true;
                BOT.heartBeat = heartBeat;
            }
            //群消息
            if (msg.Contains("\"message_type\":\"group\""))
            {
                CMD.sbConsole.AppendLine("群消息：" + msg);
                GroupMessage groupMessage = SimpleJson2.SimpleJson2.DeserializeObject<GroupMessage>(msg);
                HandleGrouopMessage(groupMessage);
            }
        }


        public static void HandleGrouopMessage(GroupMessage message)
        {

            //判断是否处理该群消息
            if (BOT.qunNumber == null || message.group_id != BOT.qunNumber)
            {
                CMD.sbConsole.AppendLine("来自群" + message.group_id + "的群消息，当前配置" + BOT.qunNumber + "不处理");
                return;
            }

            //发送聊天数据
            if (BOT.chat)
            {
                UserInfo info = QQ_BOTPlugin.bot.service.UserService.getUserByQQ(message.sender.user_id + "");
                BOT.BroadCast(message.sender.nickname + "：" + message.message);
            }


            //校验完成继续处理
            string command = message.message;
            CMD.sbConsole.AppendLine("响应指令：" + command);
            //循环处理完毕
            //Adaptor.PostGroupMessage(BOT.qunNumber, BOT.token, "复读机:" + command);

            if (command.Equals("在线玩家"))
            {
                string content = "";
                if (ConnectionManager.Instance.Clients.List.Count > 0)
                {
                    content += "在线玩家" + ConnectionManager.Instance.Clients.List.Count + "人：\n\r";
                    foreach (var s in ConnectionManager.Instance.Clients.List)
                    {
                        //心跳数据
                        List<UserInfo> us = UserService.getUserBySteamid(s.PlatformId.ReadablePlatformUserIdentifier);
                        if (us != null && us.Count > 0)
                        {
                            content += us[0].name + "\r\n";
                        }
                    }
                }
                else
                {
                    content = "没有在线玩家！";
                }
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, content);
            }

            if (command.Equals("土豪榜"))
            {
                string content = "土豪榜：\r\n";
                List<UserInfo> us = UserService.getUserShopList("", "积分高到低", 1, 10);
                for (int i = 0; i < us.Count; i++)
                {
                    UserInfo ui = us[i];
                    content += (i + 1) + "." + ui.name + " （积分：" + ui.money + "）\r\n";
                }
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, content);
            }

            if (command.Equals("掌柜榜"))
            {
                string content = "掌柜榜：\r\n";
                List<UserInfo> us = UserService.getUserShopList("", "销售额高到低", 1, 10);
                for (int i = 0; i < us.Count; i++)
                {
                    UserInfo ui = us[i];
                    content += (i + 1) + "." + ui.name + " （售卖：" + ui.trade_money + "）\r\n";
                }
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, content);
            }

            if (command.Equals("等级榜"))
            {
                string content = "等级榜：\r\n";
                List<UserInfo> us = UserService.getUserShopList("", "等级高到低", 1, 10);
                for (int i = 0; i < us.Count; i++)
                {
                    UserInfo ui = us[i];
                    content += (i + 1) + "." + ui.name + " （等级：" + ui.level + "）\r\n";
                }
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, content);
            }

            if (command.Equals("点赞榜"))
            {
                string content = "点赞榜：\r\n";
                List<UserInfo> us = UserService.getUserShopList("", "获赞高到低", 1, 10);
                for (int i = 0; i < us.Count; i++)
                {
                    UserInfo ui = us[i];
                    content += (i + 1) + "." + ui.name + " （获赞：" + ui.likecount + "）\r\n";
                }
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, content);
            }


            if (command.StartsWith("签到"))
            {
                //
                UserInfo info = QQ_BOTPlugin.bot.service.UserService.getUserByQQ(message.sender.user_id + "");
                if (info != null)
                {
                    string tms = DateTime.Now.ToString("yyyy-MM-dd");
                    long lotTimes = QQ_BOTPlugin.bot.service.UserService.QuerySignInCount(info.gameentityid, tms + " 00:00:00", tms + " 23:59:59");
                    if (lotTimes > 0)
                    {
                        MessagePostUtils.PostGroupMessage(BOT.qunNumber, "今天已签到！请明天再来！");
                    }
                    else
                    {
                        DataBase.MoneyEditor(info, DataBase.MoneyType.Money, DataBase.EditType.Add, BOT.qdCount);
                        MessagePostUtils.PostGroupMessage(BOT.qunNumber, "签到成功！\r\n获得积分：" + BOT.qdCount);
                        DataBase.logdb.Insertable<ActionLog>(new ActionLog
                        {
                            actTime = DateTime.Now,
                            actType = 50,
                            atcPlayerEntityId = info.gameentityid,
                            extinfo1 = "",
                            desc = string.Format("群签到")
                        }).ExecuteCommand();
                    }
                }
                else
                {
                    //BOT.saveBindSteam();
                    MessagePostUtils.PostGroupMessage(BOT.qunNumber, "签到失败！你还没有绑定游戏账号，请前往交易系统-交易中心-编辑资料 中进行绑定");
                }
            }

            //抽奖
            if (command.Equals("抽奖"))
            {
                StringBuilder stringBuilder = new StringBuilder("抽奖用法如下：\r\n");
                stringBuilder.Append("发送[奖池]查看所有奖池奖励及id\r\n");
                stringBuilder.Append("发送[十连 id]十发对应id奖池抽奖\r\n");
                stringBuilder.Append("发送[单抽 id]一发对应id奖池抽奖\r\n");
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, stringBuilder.ToString());
            }

            //奖池 
            if (command.Equals("奖池"))
            {
                List<Lottery> lotteries = QQ_BOTPlugin.bot.service.UserService.getLotterys();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Lottery lottery in lotteries)
                {
                    List<AwardInfo> awards = QQ_BOTPlugin.bot.service.UserService.getAwardInfos(lottery.id + "", AwardInfoTypeConfig.LOTTERY);
                    stringBuilder.Append(string.Format("奖池id:{0} 名称:{1} 单抽消耗:{2}x{4} 十连消耗{3}x{5} 奖品列表\r\n", lottery.id, lottery.desc, getLotItemName(lottery), getLotItemName(lottery), lottery.one, lottery.ten));
                    foreach (AwardInfo awardInfo in awards)
                    {
                        stringBuilder.Append(string.Format(getAwardName(awardInfo)) + "\r\n");
                    }
                    stringBuilder.Append("-------------------------------\r\n");
                }
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, stringBuilder.ToString());
            }

            //单抽 
            if (command.StartsWith("单抽"))
            {
                UserInfo info = QQ_BOTPlugin.bot.service.UserService.getUserByQQ(message.sender.user_id + "");
                if (info == null)
                {
                    //BOT.saveBindSteam();
                    MessagePostUtils.PostGroupMessage(BOT.qunNumber, "你还没有绑定游戏账号，请前往交易系统-交易中心-编辑资料 中进行绑定");
                }
                string id = command.Replace("单抽", "").Replace(" ", "");
                string msg = doLottery(info, id, "1");
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, msg);
            }

            //十连 
            if (command.StartsWith("十连"))
            {
                UserInfo info = QQ_BOTPlugin.bot.service.UserService.getUserByQQ(message.sender.user_id + "");
                if (info == null)
                {
                    //BOT.saveBindSteam();
                    MessagePostUtils.PostGroupMessage(BOT.qunNumber, "你还没有绑定游戏账号，请前往交易系统-交易中心-编辑资料 中进行绑定");
                }
                string id = command.Replace("十连", "").Replace(" ", "");
                string msg = doLottery(info, id, "10");
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, msg);
            }

            //打卡
            if (command.StartsWith("打卡"))
            {
                UserInfo info = QQ_BOTPlugin.bot.service.UserService.getUserByQQ(message.sender.user_id + "");
                if (info == null)
                {
                    //BOT.saveBindSteam();
                    MessagePostUtils.PostGroupMessage(BOT.qunNumber, "你还没有绑定游戏账号，请前往交易系统-交易中心-编辑资料 中进行绑定");
                }

                if (check.TryGetValue(message.sender.user_id + "", out DateTime lastcheck))
                {
                    if (lastcheck.AddMinutes(30) > DateTime.Now)
                    {
                        MessagePostUtils.PostGroupMessage(BOT.qunNumber, "你已打卡！请30分钟后再试");
                    }
                    else
                    {
                        check[message.sender.user_id + ""] = DateTime.Now;
                        DataBase.MoneyEditor(info, DataBase.MoneyType.Money, DataBase.EditType.Add, BOT.qdCount);
                        MessagePostUtils.PostGroupMessage(BOT.qunNumber, "打卡成功！\r\n获得积分：" + BOT.qdCount);
                    }
                }
                else
                {
                    check.Add(message.sender.user_id + "", DateTime.Now);
                    DataBase.MoneyEditor(info, DataBase.MoneyType.Money, DataBase.EditType.Add, BOT.qdCount);
                    MessagePostUtils.PostGroupMessage(BOT.qunNumber, "打卡成功！\r\n获得积分：" + BOT.qdCount);
                }


            }

        }
        public static string getLotItemName(Lottery lottery)
        {
            if (lottery.type.Equals("1"))
            {
                return "积分";
            }
            if (lottery.type.Equals("2"))
            {
                return "点券";
            }
            if (lottery.type.Equals("3"))
            {
                return lottery.itemchinese;
            }
            if (lottery.type.Equals("4"))
            {
                return lottery.itemchinese;
            }
            return "";
        }

        public static string getAwardName(AwardInfo award)
        {
            if (award.type.Equals("1"))
            {
                return award.itemchinese + "x" + award.count;
            }
            if (award.type.Equals("2"))
            {
                return award.itemchinese + "x" + award.count;
            }
            if (award.type.Equals("3"))
            {
                return "指令";
            }
            if (award.type.Equals("4"))
            {
                return "积分x" + award.count;

            }
            if (award.type.Equals("5"))
            {
                return "点券x" + award.count;
            }
            return "";
        }


        public static string doLottery(UserInfo info, string id, string count)
        {
            //获取参数
            int lotteryTimes = 0;
            Lottery target = QQ_BOTPlugin.bot.service.UserService.getLotteryByid(id);
            List<AwardInfo> awards = QQ_BOTPlugin.bot.service.UserService.getAwardInfos(target.id + "", AwardInfoTypeConfig.LOTTERY);
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
                return "抽奖次数异常";
            }

            if (awards == null || awards.Count <= 0)
            {
                return "此奖池中没有奖品，抽奖失败";
            }

            if (target != null)
            {
                //检查抽奖限制
                if (!string.IsNullOrEmpty(target.limit))
                {
                    string tms = DateTime.Now.ToString("yyyy-MM-dd");
                    int.TryParse(target.limit, out int intlimit);
                    long lotTimes = QQ_BOTPlugin.bot.service.UserService.QueryLotteryCount(info.gameentityid, id, LogType.doLottery, tms + " 00:00:00", tms + " 23:59:59");
                    if (intlimit != -1 && lotTimes >= intlimit)
                    {
                        return "今日抽奖次数已用完";
                    }
                    if (intlimit != -1 && lotTimes + int.Parse(count) > intlimit)
                    {
                        return "可用抽奖次数不足，剩余" + (intlimit - lotTimes) + "次";
                    }
                }



                //积分
                if (target.type.Equals("1"))
                {
                    if (!DataBase.MoneyEditor(info, DataBase.MoneyType.Money, DataBase.EditType.Sub, lotteryTimes))
                    {
                        return "积分不足";
                    }
                }
                //点券
                if (target.type.Equals("2"))
                {
                    if (!DataBase.MoneyEditor(info, DataBase.MoneyType.Credit, DataBase.EditType.Sub, lotteryTimes))
                    {
                        return "点券不足";
                    }
                }
                //游戏内物品
                if (target.type.Equals("3"))
                {
                    int quality = 0;
                    int.TryParse(target.quality, out quality);
                    UserStorage us = QQ_BOTPlugin.bot.service.UserService.selectAvaliableItem(info.gameentityid + "", target.itemname, quality + "", "1", lotteryTimes + "");
                    if (us == null)
                    {
                        return "没有足够的物品抽奖";
                    }
                    //更新库存
                    us.storageCount -= lotteryTimes;
                    if (us.storageCount <= 0)
                    {
                        us.itemUsedChenal = UserStorageUsedChanel.LOTTERYED;
                        us.itemStatus = UserStorageStatus.LOTERYED;
                    }
                    QQ_BOTPlugin.bot.service.UserService.UpdateUserStorage(us);
                }
                //特殊物品
                if (target.type.Equals("4"))
                {
                    int quality = 0;
                    int.TryParse(target.quality, out quality);
                    UserStorage us = QQ_BOTPlugin.bot.service.UserService.selectAvaliableItem(info.gameentityid + "", target.itemname, quality + "", "2", lotteryTimes + "");
                    if (us == null)
                    {
                        return "没有足够的物品抽奖";
                    }
                    //更新库存
                    us.storageCount -= lotteryTimes;
                    if (us.storageCount <= 0)
                    {
                        us.itemUsedChenal = UserStorageUsedChanel.LOTTERYED;
                        us.itemStatus = UserStorageStatus.LOTERYED;
                    }
                    QQ_BOTPlugin.bot.service.UserService.UpdateUserStorage(us);
                }
                //积分物品扣除完毕
                int allChance = 0;
                List<int> sfs = new List<int>();
                for (int af = 0; af < awards.Count; af++)
                {
                    AwardInfo ainfo = awards[af];
                    int.TryParse(ainfo.chance, out int cs);
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
                string awardinfo = AwardDeliverTools.DeliverAward(info, resultAward, UserStorageGetChanel.LOTTERY);

                //记录日志数据
                QQ_BOTPlugin.bot.service.UserService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.doLottery,
                    atcPlayerEntityId = info.gameentityid,
                    extinfo1 = id,
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject("id:" + id + "count:" + count),
                    extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(resultAward),
                    extinfo4 = count,
                    desc = "抽奖获得 （" + target.desc + "） 奖品：" + awardinfo
                });
                StringBuilder stringBuilder = new StringBuilder(string.Format("恭喜玩家[{0}]抽奖获得奖励", info.name));
                foreach (AwardInfo awardInfo in resultAward)
                {
                    stringBuilder.Append(string.Format(getAwardName(awardInfo)) + "\r\n");
                }
                return stringBuilder.ToString();
            }
            else
            {
                return "未找到此奖池";
            }
            return "";
        }


    }
}