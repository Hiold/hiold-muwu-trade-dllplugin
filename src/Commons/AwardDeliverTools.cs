using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using HioldMod.src.HttpServer.service;
using HioldMod.src.UserTools;
using HioldModServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.Commons
{
    public static class AwardDeliverTools
    {
        public static string DeliverAward(UserInfo ui, List<AwardInfo> awards)
        {
            StringBuilder awardinfo = new StringBuilder("");
            foreach (AwardInfo ai in awards)
            {
                //游戏内物品
                if (ai.type.Equals("1"))
                {
                    //获取游戏内物品数据
                    ItemClass currentItem = ItemClass.GetItem(ai.itemname).ItemClass;

                    string groupInfo = "";
                    foreach (string temp in currentItem.Groups)
                    {
                        groupInfo += temp + "|";
                    }
                    if (groupInfo.Length > 0)
                    {
                        groupInfo = groupInfo.Substring(0, groupInfo.Length - 1);
                    }

                    int quality = 0;
                    int.TryParse(ai.itemquality, out quality);
                    UserStorage userStorate = new UserStorage()
                    {
                        //id
                        itemtype = "1",
                        name = ai.itemname,
                        translate = ai.itemchinese,
                        itemicon = ai.itemicon,
                        itemtint = ai.itemtint,
                        quality = quality,
                        num = 1,
                        class1 = groupInfo,
                        class2 = groupInfo,
                        classmod = "0",
                        desc = LocalizationUtils.getDesc(ai.itemname + "Desc"),
                        couCurrType = "0",
                        couPrice = "0",
                        couCond = "0",
                        coudatelimit = "0",
                        couDateStart = DateTime.MinValue,
                        couDateEnd = DateTime.MinValue,
                        count = "1",
                        currency = "0",
                        price = 0,
                        discount = 0,
                        prefer = 0,
                        selltype = 0,
                        hot = "0",
                        hotset = 0,
                        show = "0",
                        stock = 0,
                        collect = 0,
                        selloutcount = "0",
                        follow = "0",
                        xglevel = "0",
                        xglevelset = "0",
                        xgday = "0",
                        xgdayset = "0",
                        xgall = "0",
                        xgallset = "0",
                        xgdatelimit = "0",
                        dateStart = DateTime.MinValue,
                        dateEnd = DateTime.MinValue,
                        collected = "0",
                        postTime = DateTime.MinValue,
                        deleteTime = DateTime.MinValue,
                        //非继承属性
                        username = ui.name,
                        platformid = ui.platformid,
                        gameentityid = ui.gameentityid,
                        collectTime = DateTime.Now,
                        storageCount = int.Parse(ai.count),
                        itemGetChenal = UserStorageGetChanel.SHOP_BUY,
                        itemStatus = UserStorageStatus.NORMAL_STORAGED,
                        //拓展属性
                        extinfo1 = "",
                        extinfo2 = "",
                        extinfo3 = "",
                        extinfo4 = "",
                        extinfo5 = "",
                        itemdata = "",
                    };
                    //添加数据到数据库
                    UserStorageService.addUserStorage(userStorate);
                    awardinfo.Append(ai.itemchinese + "x" + ai.count + "，");
                }
                //特殊物品
                if (ai.type.Equals("2"))
                {
                    UserStorage userStorate = new UserStorage()
                    {
                        //id
                        itemtype = "2",
                        name = ai.itemname,
                        translate = ai.itemchinese,
                        itemicon = ai.itemicon,
                        itemtint = ai.itemtint,
                        quality = 0,
                        num = 1,
                        class1 = "",
                        class2 = "",
                        classmod = "0",
                        desc = "",
                        couCurrType = "0",
                        couPrice = "0",
                        couCond = "0",
                        coudatelimit = "0",
                        couDateStart = DateTime.MinValue,
                        couDateEnd = DateTime.MinValue,
                        count = "1",
                        currency = "0",
                        price = 0,
                        discount = 0,
                        prefer = 0,
                        selltype = 0,
                        hot = "0",
                        hotset = 0,
                        show = "0",
                        stock = 0,
                        collect = 0,
                        selloutcount = "0",
                        follow = "0",
                        xglevel = "0",
                        xglevelset = "0",
                        xgday = "0",
                        xgdayset = "0",
                        xgall = "0",
                        xgallset = "0",
                        xgdatelimit = "0",
                        dateStart = DateTime.MinValue,
                        dateEnd = DateTime.MinValue,
                        collected = "0",
                        postTime = DateTime.MinValue,
                        deleteTime = DateTime.MinValue,
                        //非继承属性
                        username = ui.name,
                        platformid = ui.platformid,
                        gameentityid = ui.gameentityid,
                        collectTime = DateTime.Now,
                        storageCount = int.Parse(ai.count),
                        itemGetChenal = UserStorageGetChanel.SHOP_BUY,
                        itemStatus = UserStorageStatus.NORMAL_STORAGED,
                        //拓展属性
                        extinfo1 = "",
                        extinfo2 = "",
                        extinfo3 = "",
                        extinfo4 = "",
                        extinfo5 = "",
                        itemdata = "",
                    };
                    //添加数据到数据库
                    UserStorageService.addUserStorage(userStorate);
                    awardinfo.Append(ai.itemchinese + "x" + ai.count + "，");
                }
                //指令
                if (ai.type.Equals("3"))
                {
                    string command = ai.command.Replace("{name}", ui.name).Replace("{id}", ui.platformid);
                    DeliverItemTools.commandExecution.Enqueue(command);
                    awardinfo.Append("一条神秘指令，");
                }
                //积分
                if (ai.type.Equals("4"))
                {
                    DataBase.MoneyEditor(ui, DataBase.MoneyType.Money, DataBase.EditType.Add, double.Parse(ai.count));
                    awardinfo.Append(ai.count + "积分，");
                }
                //点券
                if (ai.type.Equals("5"))
                {
                    DataBase.MoneyEditor(ui, DataBase.MoneyType.Credit, DataBase.EditType.Add, double.Parse(ai.count));
                    awardinfo.Append(ai.count + "点券，");
                }
            }
            if (awardinfo.Length > 0)
            {
                return awardinfo.ToString().Substring(0, awardinfo.Length - 1);
            }
            else
            {
                return "";
            }

        }
    }
}
