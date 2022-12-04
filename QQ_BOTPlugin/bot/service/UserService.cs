using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Data;
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
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("qq = '{0}' ", qq)).First();
        }


        public static int signInLot = 50;
        /// <summary>
        /// 查询抽奖次数
        /// </summary>
        /// <returns></returns>
        public static Int64 QuerySignInCount(string id, string startTime, string endTime)
        {
            DataRow[] dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' and t.actTime>'{2}' and t.actTime< '{3}' ", id, signInLot, startTime, endTime)).Select();
            //Console.WriteLine(dt);
            foreach (DataRow row in dt)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        return (Int64)data;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        //获取奖池数据
        public static List<Lottery> getLotterys()
        {
            return DataBase.db.Queryable<Lottery>().Where(string.Format("status = '1'")).ToList();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<AwardInfo> getAwardInfos(string containerid, string funcid)
        {
            return DataBase.db.Queryable<AwardInfo>().Where(string.Format("status = '1' and containerid = '{0}' and funcid={1}", containerid, funcid)).ToList();
        }

        public static Lottery getLotteryByid(string id)
        {
            return DataBase.db.Queryable<Lottery>().Where(string.Format("id = '{0}' ", id)).First();
        }


        public static Int64 QueryLotteryCount(string id, string itemid, int logtype, string startTime, string endTime)
        {
            DataRow[] dt = null;
            if (startTime == null && endTime == null)
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' ", id, itemid, logtype)).Select();
            }
            else
            {
                dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", id, itemid, logtype, startTime, endTime)).Select();
            }
            //Console.WriteLine(dt);
            foreach (DataRow row in dt)
            {
                foreach (object data in row.ItemArray)
                {
                    try
                    {
                        return (Int64)data;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// 获取有效物品
        /// </summary>
        /// <param name="playerid">用户id</param>
        /// <returns></returns>
        public static UserStorage selectAvaliableItem(string playerid, string itemname, string quality, string itemtype, string count)
        {
            return DataBase.db.Queryable<UserStorage>().Where(string.Format("gameentityid = '{0}' and storageCount >= {1} and itemStatus='1' and itemtype='{2}' and name='{3}' and cast(quality as int)>={4}", playerid, count, itemtype, itemname, quality)).First();
        }

        /// <summary>
        /// 更新物品信息
        /// </summary>
        /// <param name="item">物品</param>
        public static void UpdateUserStorage(UserStorage storage)
        {
            DataBase.db.Updateable<UserStorage>(storage).ExecuteCommand();
        }

        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="item">物品</param>
        public static void addLog(ActionLog log)
        {
            DataBase.logdb.Insertable<ActionLog>(log).ExecuteCommand();
        }

    }
}
