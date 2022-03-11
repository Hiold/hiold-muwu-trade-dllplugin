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
        public static int MAIN = 1;
        //每日任务
        public static int DAILY = 2;
        //每周任务
        public static int WEEK = 3;

    }
    public static class ProgressionPType
    {
        public static int ZOMBIE_KILL = 1;
        public static int ANIMAL_KILL = 2;
        public static int LIKE = 3;
        public static int ONLINE_TIME = 4;
        public static int TRADE_COUNT = 5;
        public static int TRADE_AMOUNT = 6;
        public static int REQUIRE_COUNT = 7;
        public static int REQUIRE_AMOUNT = 8;
        public static int SUPPLY_COUNT = 9;
        public static int SUPPLY_AMOUNT = 10;
        public static int DAILY_SIGN = 11;
        public static int LEVEL = 12;
        public static int CRAFTED = 13;

    }
}
