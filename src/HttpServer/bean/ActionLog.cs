using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("userinfo")]
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

        public string extinfo1 { get; set; }
        public string extinfo2 { get; set; }
        public string extinfo3 { get; set; }
        public string extinfo4 { get; set; }
        public string extinfo5 { get; set; }
        public string extinfo6 { get; set; }
        public string extinfo7 { get; set; }
        public string extinfo8 { get; set; }
        public string extinfo9 { get; set; }
        public string extinfo10 { get; set; }
    }

    public class LogType
    {
        public static int PlayerLogin = 0;
        public static int BuyItem = 1;
    }
}
