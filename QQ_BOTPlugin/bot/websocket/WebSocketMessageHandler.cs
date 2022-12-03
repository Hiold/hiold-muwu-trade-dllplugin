using HioldMod.HttpServer;
using HioldMod.src.HttpServer.bean;
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
                string qq = message.sender.user_id + "";
                if (BOT.bindUser.TryGetValue(qq, out string steamid))
                {

                }
                else
                {
                    BOT.bindUser[qq] = steamid;
                    BOT.saveBindSteam();
                    MessagePostUtils.PostGroupMessage(BOT.qunNumber, "签到失败！你还没有绑定steam账号，请前往交易系统-交易中心-编辑资料 中进行绑定");
                }
            }

        }
    }
}