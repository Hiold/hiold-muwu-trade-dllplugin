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
        public static bool userRegister(string username, string password, string entityid)
        {
            UserInfo user = new UserInfo()
            {
                name = username,
                password = ServerUtils.md5(password),
                gameentityid = entityid
            };
            var col = DataBase.litedb.GetCollection<UserInfo>("UserInfo");
            col.Insert(user);
            return false;
        }

        public static List<UserInfo> userLogin(string username, string password)
        {
            var col = DataBase.litedb.GetCollection<UserInfo>("UserInfo");
            var results = col.Find(s => s.name.Equals(username) && s.password.Equals(ServerUtils.md5(password))).ToList();
            return results;
        }

    }
}
