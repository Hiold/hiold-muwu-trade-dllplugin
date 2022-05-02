using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("userstorage")]
    public class UserStorage : TradeManageItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_username" })]
        public string username { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_entityid" })]
        public string gameentityid { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_platformid" })]
        public string platformid { get; set; }

        public int storageCount { get; set; }

        //获取时间
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_collecttime" })]
        public DateTime collectTime { get; set; }

        //分发时间
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_obtaintime" })]
        public DateTime obtainTime { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "us_index_itemgetchanel" })]
        public int itemGetChenal { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "us_index_itemusedchanel" })]
        public int itemUsedChenal { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "us_index_itemstatus" })]
        public int itemStatus { get; set; }

        [SugarColumn(Length = 2048, DefaultValue = "", IsNullable = true)]
        public string itemdata { get; set; }

        public string extinfo1 { get; set; }
        public string extinfo2 { get; set; }
        public string extinfo3 { get; set; }
        public string extinfo4 { get; set; }
        public string extinfo5 { get; set; }

    }


    //物品状态
    public class UserStorageStatus
    {
        //正常在库
        public static int NORMAL_STORAGED = 1;
        //已发放到游戏内（部分）
        public static int DISPACHED_APART = 1;
        //已发放到游戏内
        public static int DISPACHED = 2;
        //用户自行删除
        public static int USERDELETED = 3;
        //用户自行卖出
        public static int USERSELLED = 4;
        //用户供货消耗
        public static int SUPPLYED = 5;
        //用户抽奖消耗
        public static int LOTERYED = 6;
        //补签消耗
        public static int RESIGNED = 7;
        //补签消耗
        public static int ADMINDELETE = 8;
    }

    //物品获取途径
    public class UserStorageGetChanel
    {
        //从系统中获得
        public static int SHOP_BUY = 1;
        //下架物品
        public static int TACK_BACK = 2;
        //从系统中获得
        public static int TRADE_BUY = 3;
        //从供货获得
        public static int SUPPLY = 4;
        //从游戏中获得
        public static int GAME_STORAGE = 5;
        //签到
        public static int SIGN = 6;
        //补签
        public static int RESIGN = 7;
        //抽奖
        public static int LOTTERY = 8;
        //任务
        public static int PROGRESSION = 9;
        //红包
        public static int DAILYAWARD = 10;
        //自动堆叠
        public static int STACK = 11;
        //自动堆叠
        public static int CRAFT = 12;
    }

    //物品最终用途
    public class UserStorageUsedChanel
    {
        //已发放到游戏内
        public static int OBTAIN_TO_GAME = 1;
        //供货
        public static int SUPPLY_TO_OTHERS = 2;
        //抽奖消耗
        public static int LOTTERYED = 3;
        //补签消耗
        public static int RESIGNED = 4;
    }
}
