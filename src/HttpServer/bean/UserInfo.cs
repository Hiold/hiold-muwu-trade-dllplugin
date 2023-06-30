using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("userinfo")]
    public class UserInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_usercreateat" }, IsNullable = true)]
        public DateTime created_at { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime updated_at { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime deleted_at { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "index_username" })]
        public string name { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_entityid" })]
        public string gameentityid { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_platformid" })]
        public string platformid { get; set; }

        public double money { get; set; }

        public double credit { get; set; }

        public double status { get; set; }

        public string password { get; set; }

        public string qq { get; set; }

        public string email { get; set; }

        public string avatar { get; set; }

        public string sign { get; set; }

        public string extinfo1 { get; set; }

        public string extinfo2 { get; set; }

        public string extinfo3 { get; set; }

        public string extinfo4 { get; set; }

        public string extinfo5 { get; set; }

        public string extinfo6 { get; set; }

        public string trade_count { get; set; }
        public double trade_money { get; set; }

        public string store_count { get; set; }
        public string buy_count { get; set; }

        public string require_count { get; set; }
        public double require_money { get; set; }

        public string type { get; set; }

        public int level { get; set; }

        public string online_time { get; set; }

        public string zombie_kills { get; set; }

        public string player_kills { get; set; }

        public string total_crafted { get; set; }

        public double vipdiscount { get; set; }

        public double creditcharge { get; set; }

        public double creditcost { get; set; }

        public double moneycharge { get; set; }

        public double moneycost { get; set; }

        public double signdays { get; set; }

        public double likecount { get; set; }

        public string shopname { get; set; }

        [SugarColumn(IsNullable = true)]
        public string ncode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string token { get; set; }
    }

    public static class UserInfoCountType
    {
        public static int TRADE_COUNT = 1;
        public static int BUY_COUNT = 2;
        public static int REQUIRE_COUNT = 3;
        public static int TRADE_MONEY = 4;
        public static int BUY_MONEY = 5;
        public static int REQUIRE_MONEY = 5;
    }
    public static class UserInfoCountAct
    {
        public static int ADD = 1;
        public static int SUB = 2;
    }
}
