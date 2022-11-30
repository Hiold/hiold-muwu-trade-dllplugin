using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.service
{
    public class UserService
    {
        /// <summary>
        /// 获取玩家店铺信息
        /// </summary>
        /// <param name="name">用户名/店铺名</param>
        /// <param name="orderby">排序</param>
        /// <returns></returns>
        public static List<UserInfo> getUserShopList(string name, string sorttype, int pageIndex, int pageSize)
        {
            int totalCount = 0;
            string sortStr = "";
            //排序处理
            if (sorttype != null)
            {
                if (sorttype.Equals("默认排序"))
                {
                    sortStr = "";
                }
                if (sorttype.Equals("等级高到低"))
                {
                    sortStr = " order by level desc";
                }
                if (sorttype.Equals("积分高到低"))
                {
                    sortStr = " order by money desc";
                }
                if (sorttype.Equals("获赞高到低"))
                {
                    sortStr = " order by likecount desc";
                }
                if (sorttype.Equals("销售额高到低"))
                {
                    sortStr = " order by trade_money desc";
                }
            }
            return DataBase.db.Queryable<UserInfo>().Where(string.Format(" (name like '%{0}%' or shopname like '%{0}%') and type!='1' " + sortStr, name)).ToPageList(pageIndex, pageSize, ref totalCount);
        }


        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserBySteamid(string steamid)
        {
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("gameentityid = '{0}' ", steamid)).ToList();
        }

        public static UserInfo getUserByQQ(string qq)
        {
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("qq = '{0}' ", qq)).First;
        }

    }
}
