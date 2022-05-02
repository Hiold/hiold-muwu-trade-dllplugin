using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("shopitem")]
    public class TradeManageItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        public string itemtype { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_name" })]
        public string name { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_translate" }, IsNullable = true)]
        public string translate { get; set; }
        //图片
        public string itemicon { get; set; }
        public string itemtint { get; set; }
        //品质
        public int quality { get; set; }
        //数量
        public int num { get; set; }
        //类型2
        public string class1 { get; set; }
        //类型2
        public string class2 { get; set; }
        //是否为mod物品
        public string classmod { get; set; }
        //描述
        [SugarColumn(ColumnDataType = "text")]
        public string desc { get; set; }

        //特殊物品数据
        public string couCurrType { get; set; }
        public string couPrice { get; set; }
        public string couCond { get; set; }
        public string coudatelimit { get; set; }
        public DateTime couDateStart { get; set; }
        public DateTime couDateEnd { get; set; }
        public string count { get; set; }

        //货币类型
        public string currency { get; set; }
        //价格
        public double price { get; set; }
        //折扣
        public double discount { get; set; }
        //折后价格
        public double prefer { get; set; }
        //出售类型 false  vip  或者为空
        public int selltype { get; set; }
        //是否热卖 true false auto
        public string hot { get; set; }
        //达到热卖后自动添加热卖
        public int hotset { get; set; }
        //是否显示 t  f  a
        public string show { get; set; }
        //剩余库存量
        public int stock { get; set; }
        //收藏量
        public int collect { get; set; }
        //售出量
        public string selloutcount { get; set; }
        //是否跟档
        public string follow { get; set; }

        //等级限购
        public string xglevel { get; set; }
        public string xglevelset { get; set; }
        //每日限购
        public string xgday { get; set; }
        public string xgdayset { get; set; }
        //总限购
        public string xgall { get; set; }
        public string xgallset { get; set; }

        //限购截止日期
        public string xgdatelimit { get; set; }
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
