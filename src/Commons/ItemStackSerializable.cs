using System;
using System.Collections.Generic;

namespace ServerTools
{
    [Serializable]

    public class ItemDataSerializable
    {
        public ItemDataSerializable()
        {  
        }

        public ItemDataSerializable(string steamid, string name, string tradeid, int price, string customIcon, string customIconTint, List<string> groups, List<string> modifications, string itemName, string itemUseTime, string itemMaxUseTime, string itemCount, string itemQuality, string itemData,string translate,string desc)
        {
            this.steamid = steamid;
            this.name = name;
            this.tradeid = tradeid;
            this.price = price;
            this.CustomIcon = customIcon;
            this.CustomIconTint = customIconTint;
            this.Groups = groups;
            this.Modifications = modifications;
            this.itemName = itemName;
            this.itemUseTime = itemUseTime;
            this.itemMaxUseTime = itemMaxUseTime;
            this.itemCount = itemCount;
            this.itemQuality = itemQuality;
            this.itemData = itemData;
            this.translate = translate;
            this.desc = desc;
        }



        //steamid
        public string steamid;
        //用户名
        public string name;
        //交易id
        public string tradeid;
        //价格
        public int price;
        //物品信息
        //自定义图片
        public string CustomIcon;
        //自定义颜色
        public string CustomIconTint;
        //tag信息
        public List<string> Groups;
        //模组数据
        public List<string> Modifications;
        //物品名
        public string itemName;
        //物品已使用耐久度
        public string itemUseTime;
        //物品最大耐久度
        public string itemMaxUseTime;
        //物品数量
        public string itemCount;
        //物品质量
        public string itemQuality;
        //物品数据
        public string itemData;
        //翻译
        public string translate;
        //描述
        public string desc;


    }
}
