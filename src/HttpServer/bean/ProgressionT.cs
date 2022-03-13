using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("progression")]
    public class ProgressionT
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        public int type { get; set; }
        public int progressionType { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string desc { get; set; }
        public string status { get; set; }
        public string extinfo1 { get; set; }
        public string extinfo2 { get; set; }
        public string extinfo3 { get; set; }
        public string extinfo4 { get; set; }
        public string extinfo5 { get; set; }
        public string extinfo6 { get; set; }
    }
    public static class ProgressionType
    {
        //主线任务
        public const int MAIN = 1;
        //每日任务
        public const int DAILY = 2;
        //每周任务
        public const int WEEK = 3;

    }
    public static class ProgressionPType
    {
        public const int ZOMBIE_KILL = 1;
        public const int ANIMAL_KILL = 2;
        public const int LIKE = 3;
        public const int ONLINE_TIME = 4;
        public const int TRADE_COUNT = 5;
        public const int TRADE_AMOUNT = 6;
        public const int REQUIRE_COUNT = 7;
        public const int REQUIRE_AMOUNT = 8;
        public const int SUPPLY_COUNT = 9;
        public const int SUPPLY_AMOUNT = 10;
        public const int DAILY_SIGN = 11;
        public const int LEVEL = 12;
        public const int CRAFTED = 13;

    }
}
