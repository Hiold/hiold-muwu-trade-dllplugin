using HioldMod.src.HttpServer.bean;
using QQ_BOTPlugin.bot.model.api;
using QQ_BOTPlugin.bot.model.message;
using QQ_BOTPlugin.bot.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot
{
    class MessageDispacher
    {
        public static void HandleGrouopMessage(object message)
        {
            GroupMessage msg = (GroupMessage)message;

            //判断是否处理该群消息
            if (msg.sender.group.id != BOT.qunNumber)
            {
                return;
            }
            //校验完成继续处理
            if (msg.messageChain != null && msg.messageChain.Count > 0)
            {
                string command = null;
                foreach (object msgObj in msg.messageChain)
                {
                    //跳过Source
                    if (msgObj.GetType().IsAssignableFrom(typeof(Source)))
                    {
                        continue;
                    }
                    //拼接Plain类型字符串
                    if (msgObj.GetType().IsAssignableFrom(typeof(Plain)))
                    {
                        Plain msgPlain = (Plain)msgObj;
                        command += msgPlain.text;
                    }
                }
                //循环处理完毕
                Console.WriteLine(command);
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
                    Adaptor.PostGroupMessage(BOT.qunNumber, BOT.token,  content);
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
                    Adaptor.PostGroupMessage(BOT.qunNumber, BOT.token, content);
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
                    Adaptor.PostGroupMessage(BOT.qunNumber, BOT.token, content);
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
                    Adaptor.PostGroupMessage(BOT.qunNumber, BOT.token, content);
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
                    Adaptor.PostGroupMessage(BOT.qunNumber, BOT.token, content);
                }

            }
        }

        public static void HandleTempMessage(object message)
        {
            TempMessage msg = (TempMessage)message;
            if (msg.messageChain != null && msg.messageChain.Count > 0)
            {

            }
        }
    }
}
