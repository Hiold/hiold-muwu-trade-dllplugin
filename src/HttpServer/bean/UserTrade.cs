using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("usertrade")]
    class UserTrade : TradeManageItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ut_index_username" })]
        public string username { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ut_index_entityid" })]
        public string gameentityid { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ut_index_platformid" })]
        public string platformid { get; set; }

        //上架时间
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_forselltime" })]
        public DateTime forSellTime { get; set; }
        //售完时间
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_selledtime" })]
        public DateTime selledTime { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "ut_index_itemstatus" })]
        public int itemStatus { get; set; }

        [SugarColumn(Length = 2048, DefaultValue = "", IsNullable = true)]
        public string itemdata { get; set; }

        public string extinfo1 { get; set; }
        public string extinfo2 { get; set; }
        public string extinfo3 { get; set; }
        public string extinfo4 { get; set; }
        public string extinfo5 { get; set; }
    }

    public class UserTradeConfig
    {
        //正常在售
        public static int NORMAL_ON_TRADE = 1;
        //已售出
        public static int SELLED = 2;
        //已下架取回
        public static int TAKC_BACK = 3;
    }
}
