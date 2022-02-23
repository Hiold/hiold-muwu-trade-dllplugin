using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("actionlog")]
    class ActionLog
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "act_date" }, IsNullable = true)]
        public DateTime actTime { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "act_type" }, IsNullable = true)]
        public int actType { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "act_playerid" }, IsNullable = true)]
        public string atcPlayerEntityId { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "act_extinfo1" }, IsNullable = true)]
        public string extinfo1 { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "act_extinfo2" }, IsNullable = true)]
        public string extinfo2 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string extinfo3 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string extinfo4 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string extinfo5 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string extinfo6 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string extinfo7 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string extinfo8 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string extinfo9 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string extinfo10 { get; set; }
    }

    public class LogType
    {
        //登录
        public static int PlayerLogin = 0;
        //购买物品
        public static int BuyItem = 1;
        //出售物品
        public static int SellItem = 2;
        //取回
        public static int TackBack = 3;
        //上架求购
        public static int PostRequire = 4;
        //购买玩家交易物品
        public static int BuyUserTrade = 4;

    }
}
