using HioldMod.src.HttpServer.bean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src
{
    public class TradeSysEvents
    {
        //出售事件
        private static List<Action<UserInfo, UserTrade>> OnSellEvent = new List<Action<UserInfo, UserTrade>>();
        //物品被购买事件
        private static List<Action<UserInfo, UserInfo, UserStorage, double>> SellOutEvent = new List<Action<UserInfo, UserInfo, UserStorage, double>>();


        public static void RegOnSellEvent(Action<UserInfo, UserTrade> action)
        {
            //委托方法加入列表
            OnSellEvent.Add(action);
        }

        public static void RegSellOutEvent(Action<UserInfo, UserInfo, UserStorage, double> action)
        {
            //委托方法加入列表
            SellOutEvent.Add(action);
        }


        /// <summary>
        /// 触发出售
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="us"></param>
        public static void TrigerOnSellEvent(UserInfo ui, UserTrade ut)
        {
            foreach (Action<UserInfo, UserTrade> delegateFunc in OnSellEvent)
            {
                Task.Run(() =>
                {
                    delegateFunc(ui, ut);
                });
            }
        }

        /// <summary>
        /// 触发购买
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="us"></param>
        public static void TrigerSellOutEvent(UserInfo buyer, UserInfo seller, UserStorage ut, double priceAll)
        {
            foreach (Action<UserInfo, UserInfo, UserStorage, double> delegateFunc in SellOutEvent)
            {
                Task.Run(() =>
                {
                    delegateFunc(buyer, seller, ut, priceAll);
                });
            }
        }
    }
}
