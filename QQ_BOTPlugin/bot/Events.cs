using HioldMod.src.HttpServer.bean;
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
            string content = string.Format("{0} 上架了{1}个{2}，单价{3}", ui.name, ut.stock, RegexUtils.ReplaceBBCode(ut.translate), ut.price);
            Adaptor.PostGroupMessage(BOT.qunNumber, BOT.token, content);
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
    }
}
