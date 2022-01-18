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
            return DataBase.db.Queryable<UserInfo>().Where(s => s.name.Equals(username) && s.password.Equals(ServerUtils.md5(password))).ToList(); ;
        }

        /// <summary>
        /// 根据ID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserById(string id)
        {
            return DataBase.db.Queryable<UserInfo>().Where(s => s.id.Equals(id)).ToList(); ;
        }

        /// <summary>
        /// 根据EOS获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserByEOS(string eos)
        {
            return DataBase.db.Queryable<UserInfo>().Where(s => s.platformid.Equals(eos)).ToList(); ;
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<UserInfo> getUserBySteamid(string steamid)
        {
            return DataBase.db.Queryable<UserInfo>().Where(s => s.gameentityid.Equals(steamid)).ToList(); ;
        }

    }
}
