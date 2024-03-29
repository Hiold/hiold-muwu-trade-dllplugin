﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("actionlog")]
    public class ActionLog
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
        public static int BuyUserTrade = 5;
        //供货
        public static int SupplyItem = 6;
        //图片上传
        public static int imageUpload = 7;
        //从游戏内容器提取物品到交易提醒
        public static int takeItemToTradeSys = 8;
        //添加系统售卖物品
        public static int addShopItem = 9;
        //更新了系统售卖物品信息
        public static int updateShopItem = 10;
        //删除系统售卖物品
        public static int deleteShopItem = 11;
        //库存提取到游戏中
        public static int dispachToGame = 12;
        //玩家自主删除物品 无补偿
        public static int deleteItem = 13;
        //添加收藏物品
        public static int collect = 14;
        //取消收藏物品
        public static int discollect = 15;
        //更新玩家基础信息
        public static int updateUserinfo = 16;
        //取消求购
        public static int DeleteRequire = 17;
        //物品被购买
        public static int ItemSellOuted = 18;
        //被供货
        public static int SuppliedItem = 19;
        //添加红包
        public static int AddDailyAward = 20;
        //修改红包
        public static int editDailyAward = 21;
        //删除红包
        public static int deleteDailyAward = 22;

        //添加奖品
        public static int AddAwardInfo = 23;
        //修改奖品
        public static int editAwardInfo = 24;
        //删除奖品
        public static int deleteAwardInfo = 25;

        //领取红包
        public static int pullGetDailyAward = 26;

        //添加阶段任务
        public static int AddProgression = 27;
        //修改阶段任务
        public static int editProgression = 28;
        //删除阶段任务
        public static int deleteProgression = 29;
        //签到
        public static int dailySign = 30;
        //领取阶段任务奖励
        public static int pullGetProgression = 31;

        //添加抽奖
        public static int AddLottery = 32;
        //修改抽奖
        public static int editLottery = 33;
        //删除抽奖
        public static int deleteLottery = 34;
        //抽奖
        public static int doLottery = 35;

        //添加签到
        public static int AddSignInfo = 36;
        //修改签到
        public static int editSignInfo = 37;
        //删除签到
        public static int deleteSignInfo = 38;
        //签到
        public static int doSignInfo = 39;
        //补签
        public static int doReSignInfo = 40;
        //点赞
        public static int Like = 41;
        //物品制作
        public static int CraftItem = 42;

        //添加奖品
        public static int AddItemLimitConfig = 43;
        //修改奖品
        public static int editItemLimitConfig = 44;
        //删除奖品
        public static int deleteItemLimitConfig = 45;

        //水果机相关
        public static int slotMachineChargePoint = 46;
        public static int slotMachineReleasePoint = 47;
        public static int slotMachineRollingNotHit = 48;
        public static int slotMachineRollingHit = 49;

    }
}
