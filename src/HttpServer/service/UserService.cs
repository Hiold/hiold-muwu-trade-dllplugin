using HioldMod.HttpServer;
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
            //LogUtils.Loger("进入查询");
            if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                return HandleUserListQueryMoney(DataBase.db.Queryable<UserInfo>().Where(string.Format("(name = '{0}' and `password` = '{1}') or (gameentityid = '{0}' and `password` = '{1}') ", username, password)).ToList());
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("(name = '{0}' and `password` = '{1}') or (gameentityid = '{0}' and `password` = '{1}') ", username, password)).ToList();
        }

        /// <summary>
        /// 根据ID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserById(string id)
        {
            if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                return HandleUserListQueryMoney(DataBase.db.Queryable<UserInfo>().Where(string.Format("id = '{0}' ", id)).ToList());
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("id = '{0}' ", id)).ToList();
        }

        /// <summary>
        /// 根据ID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserByNcode(string ncode)
        {
            if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                return HandleUserListQueryMoney(DataBase.db.Queryable<UserInfo>().Where(string.Format("ncode = '{0}' ", ncode)).ToList());
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("ncode = '{0}' ", ncode)).ToList();
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
            if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                return HandleUserListQueryMoney(result);
            return result;
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserBySteamid(string steamid)
        {
            if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                return HandleUserListQueryMoney(DataBase.db.Queryable<UserInfo>().Where(string.Format("gameentityid = '{0}' ", steamid)).ToList());
            return DataBase.db.Queryable<UserInfo>().Where(string.Format("gameentityid = '{0}' ", steamid)).ToList();
        }
        /// <summary>
        /// 通过姓名获取用户
        /// </summary>
        /// <param name="steamid"></param>
        /// <returns></returns>
        public static List<UserInfo> getUserByName(string name)
        {
            if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                return HandleUserListQueryMoney(DataBase.db.Queryable<UserInfo>().Where(string.Format(" (name like '%{0}%' or shopname like '%{0}%') ", name)).ToList());
            return DataBase.db.Queryable<UserInfo>().Where(string.Format(" (name like '%{0}%' or shopname like '%{0}%') ", name)).ToList();
        }

        /// <summary>
        /// 获取管理员用户
        /// </summary>
        /// <param name="steamid"></param>
        /// <returns></returns>
        public static UserInfo getAdmin(string name)
        {
            return DataBase.db.Queryable<UserInfo>().Where(string.Format(" name='{0}' ", name)).First();
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
            if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                return HandleUserListQueryMoney(DataBase.db.Queryable<UserInfo>().Where(string.Format(" (name like '%{0}%' or shopname like '%{0}%') and type!='1' " + sortStr, name)).ToPageList(pageIndex, pageSize, ref totalCount));
            return DataBase.db.Queryable<UserInfo>().Where(string.Format(" (name like '%{0}%' or shopname like '%{0}%') and type!='1' " + sortStr, name)).ToPageList(pageIndex, pageSize, ref totalCount);
        }

        /// <summary>
        /// 获取玩家店铺信息
        /// </summary>
        /// <param name="name">用户名/店铺名</param>
        /// <param name="orderby">排序</param>
        /// <returns></returns>
        public static Dictionary<string, object> getUserByPage(string sorttype, string name, string steamid, string eosid, int page, int limit)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            int count = 0;
            string whereStr = "";
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

                if (!string.IsNullOrEmpty(name))
                {
                    whereStr += string.Format("and (name like '%{0}%' or shopname like '%{0}%') ", name);
                }

                if (!string.IsNullOrEmpty(steamid))
                {
                    whereStr += string.Format(" and gameentityid='{0}' ", steamid);
                }

                if (!string.IsNullOrEmpty(eosid))
                {
                    whereStr += string.Format(" and platformid='{0}' ", eosid);
                }
            }
            if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
            {
                result.Add("data", HandleUserListQueryMoney(DataBase.db.Queryable<UserInfo>().Where(string.Format("type!='1' " + whereStr + sortStr)).ToPageList(page, limit, ref count)));
                result.Add("totalCount", count);
                return result;
            }
            else
            {
                result.Add("data", DataBase.db.Queryable<UserInfo>().Where(string.Format("type!='1' " + whereStr + sortStr)).ToPageList(page, limit, ref count));
                result.Add("totalCount", count);
                return result;
            }
        }

        public static List<UserInfo> HandleUserListQueryMoney(List<UserInfo> source)
        {
            try
            {
                foreach (UserInfo ui in source)
                {
                    if (HioldMod.API.isOnServer && HioldMod.API.isNaiwaziBot)
                    {
                        int result = NaiwaziServerKitInterface.NaiwaziPointHelper.GetPoint("EOS_" + ui.platformid);
                        if (result >= 0)
                        {
                            ui.money = result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger("没有找到NaiwaziBot依赖");
            }
            return source;
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static void UpdateUserInfoParam(Dictionary<string, object> dt)
        {
            var t66 = DataBase.db.Updateable(dt).AS("userinfo").WhereColumns("id").ExecuteCommand();
        }
    }
}
