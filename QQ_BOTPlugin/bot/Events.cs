using HioldMod.src.HttpServer.bean;
using QQ_BOTPlugin.bot.websocket;
using QQ_BOTPlugin.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot
{
    public class Events
    {
        public static void OnSell(UserInfo ui, UserTrade ut)
        {
            CMD.sbConsole.AppendLine("触发上架事件群号为:"+BOT.qunNumber);
            string content = string.Format("{0} 上架了{1}个{2}，单价{3}", ui.name, ut.stock, RegexUtils.ReplaceBBCode(ut.translate), ut.price);
            MessagePostUtils.PostGroupMessage(BOT.qunNumber, content);
        }

        public static void SellOut(UserInfo buyer, UserInfo seller, UserStorage us, double priceAll)
        {
            if (seller != null && !string.IsNullOrEmpty(seller.qq))
            {
                if (int.TryParse(seller.qq, out int qqnumber))
                {
                    string content = string.Format("{0} 购买了{1}个你出售的{2}，获得积分{3}", buyer.name, us.storageCount, RegexUtils.ReplaceBBCode(us.translate), priceAll);
                    Adaptor.PostTempMessage(BOT.qunNumber, qqnumber, BOT.token, content);
                }
            }
        }

        public static void Lottery(Lottery lo, UserInfo ui, List<AwardInfo> awards)
        {
            CMD.sbConsole.AppendLine("触发抽奖广播");
            //启用了抽奖广播，广播信息
            if (BOT.EnableLottery.Equals("True"))
            {
                StringBuilder awardinfo = new StringBuilder("");
                foreach (AwardInfo ai in awards)
                {
                    //游戏内物品
                    if (ai.type.Equals("1"))
                    {
                        awardinfo.Append(ai.itemchinese + "x" + ai.count + "\r\n");
                    }
                    //特殊物品
                    if (ai.type.Equals("2"))
                    {
                        awardinfo.Append(ai.itemchinese + "x" + ai.count + "\r\n");
                    }
                    //指令
                    if (ai.type.Equals("3"))
                    {
                        awardinfo.Append("一条神秘指令\r\n");
                    }
                    //积分
                    if (ai.type.Equals("4"))
                    {
                        awardinfo.Append(ai.count + "积分\r\n");
                    }
                    //点券
                    if (ai.type.Equals("5"))
                    {
                        awardinfo.Append(ai.count + "点券\r\n");
                    }
                }
                string content = string.Format("玩家 [{0}] 通过抽奖获得奖励\r\n", ui.name) + awardinfo.ToString();
                MessagePostUtils.PostGroupMessage(BOT.qunNumber, content);
            }
        }
    }
}
