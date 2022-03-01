using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class UserService
    {
        /// <summary>
        /// 注册用户 web
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="entityid">id</param>
        /// <returns></returns>
        public static int userRegister(string username, string password, string entityid)
        {
            UserInfo user = new UserInfo()
            {
                name = username,
                password = ServerUtils.md5(password),
                gameentityid = entityid
            };
            var nv = DataBase.db.Insertable<UserInfo>(user).ExecuteCommand();
            return nv;
        }

        /// <summary>
        /// 注册用户 system
        /// </summary>
        /// <param name="user">用户bean</param>
        /// <returns></returns>
        public static int userRegister(UserInfo user)
        {
            var nv = DataBase.db.Insertable<UserInfo>(user).ExecuteCommand();
            return nv;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static List<UserInfo> userLogin(string username, string password)
        {
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("(name = '{0}' and `password` = '{1}') or (gameentityid = '{0}' and `password` = '{1}') ", username, password)).ToList();
        }

        /// <summary>
        /// 根据ID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserById(string id)
        {
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("id = '{0}' ", id)).ToList(); ;
        }

        /// <summary>
        /// 根据EOS获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserByEOS(string eos)
        {
            if (string.IsNullOrWhiteSpace(eos))
            {
                return null;
            }

            List<UserInfo> result = DataBase.db.Queryable<UserInfo>().Where(string.Format("platformid = '{0}' ", eos)).ToList();
            foreach (UserInfo ui in result)
            {
                ui.password = "[masked]";
            }
            return result;
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserBySteamid(string steamid)
        {
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("gameentityid = '{0}' ", steamid)).ToList(); ;
        }

        public static void UpdateUserInfo(UserInfo user)
        {
            DataBase.db.Updateable<UserInfo>(user).ExecuteCommand();
        }

        public static void UpdateAmount(UserInfo info, int actType, double actCount)
        {
            List<UserInfo> targetUsers = UserService.getUserById(info.id + "");
            if (targetUsers != null && targetUsers.Count > 0)
            {
                UserInfo _target = targetUsers[0];
                //buy
                if (actType == UserInfoCountType.BUY_COUNT)
                {
                    if (int.TryParse(_target.buy_count, out int buycount))
                    {
                        _target.buy_count = (buycount + actCount) + "";
                        UserService.UpdateUserInfo(_target);
                    }
                }
                if (actType == UserInfoCountType.BUY_MONEY)
                {
                    _target.moneycost += actCount;
                    UserService.UpdateUserInfo(_target);
                }

                //trade
                if (actType == UserInfoCountType.TRADE_COUNT)
                {
                    if (int.TryParse(_target.trade_count, out int buycount))
                    {
                        _target.trade_count = (buycount + actCount) + "";
                        UserService.UpdateUserInfo(_target);
                    }
                }
                if (actType == UserInfoCountType.TRADE_MONEY)
                {
                    _target.trade_money += actCount;
                    UserService.UpdateUserInfo(_target);
                }
                //require
                //
                if (actType == UserInfoCountType.REQUIRE_COUNT)
                {
                    if (int.TryParse(_target.require_count, out int buycount))
                    {
                        _target.require_count = (buycount + actCount) + "";
                        UserService.UpdateUserInfo(_target);
                    }
                }
                if (actType == UserInfoCountType.REQUIRE_MONEY)
                {
                    _target.require_money += actCount;
                    UserService.UpdateUserInfo(_target);
                }

            }
        }

        /// <summary>
        /// 获取玩家店铺信息
        /// </summary>
        /// <param name="name">用户名/店铺名</param>
        /// <param name="orderby">排序</param>
        /// <returns></returns>
        public static List<UserInfo> getUserShopList(string name, string sorttype)
        {
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
            return DataBase.db.Queryable<UserInfo>().Where(string.Format(" (name like '%{0}%' or shopname like '%{0}%') and type!='1' " + sortStr, name)).ToList(); ;
        }
    }
}
