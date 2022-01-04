using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("shopitem")]
    class TradeManageItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        public string itemtype { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_name" })]
        public string name { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_translate" }, IsNullable = true)]
        public string translate { get; set; }
        //图片
        public string itemIcon { get; set; }
        public string itemTint { get; set; }
        //品质
        public int quality { get; set; }
        //数量
        public int num { get; set; }

        //类型2
        public string class1 { get; set; }
        //类型2
        public string class2 { get; set; }
        //是否为mod物品
        public string classMod { get; set; }
        //描述
        [SugarColumn(ColumnDataType = "text")]
        public string desc { get; set; }

        public string couCurrType { get; set; }
        public string couPrice { get; set; }
        public string couCond { get; set; }
        public DateTime couDateStart { get; set; }
        public DateTime couDateEnd { get; set; }
        public string count { get; set; }

        //物品类型

        //货币类型
        public string currency { get; set; }
        //价格
        public double price { get; set; }
        //折扣
        public double discount { get; set; }
        //折后价格
        public double prefer { get; set; }
        //出售类型 false  vip  或者为空
        public int sellType { get; set; }
        //是否热卖 true false auto
        public string hot { get; set; }
        //达到热卖后自动添加热卖
        public int hotSet { get; set; }
        //是否显示 t  f  a
        public string show { get; set; }
        //剩余库存量
        public int stock { get; set; }
        //收藏量
        public int collect { get; set; }
        //售出量
        public string sell { get; set; }
        //限购登记 + - fasevv
        public string xgLevel { get; set; }
        public string xgAll { get; set; }
        public string xgLevelset { get; set; }
        //总限购
        public string xgCount { get; set; }
        //是否跟档
        public string follow { get; set; }
        //限购截止日期
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        //是否已收藏
        public string collected { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_postTime" })]
        public DateTime postTime { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_deleteTime" }, IsNullable = true, DefaultValue = null)]
        public DateTime deleteTime { get; set; }
    }
}
