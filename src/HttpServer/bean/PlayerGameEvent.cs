using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("gameeventlog")]
    class PlayerGameEvent
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
        [SugarColumn(IsNullable = true)]
        public string desc { get; set; }
    }
    public static class PlayerGameEventType
    {
        public static int KILL_ZOMBIE = 1;
        public static int KILL_ANIMAL = 2;
        public static int KILL_PLAYER = 3;
        public static int LIKE = 4;
    }
}
