using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    /// <summary>
    /// 系统物品类
    /// </summary>
    class ShopItem : Item
    {
        //id
        public ObjectId id { get; set; }
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
        public int sales { get; set; }
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
    }
}
