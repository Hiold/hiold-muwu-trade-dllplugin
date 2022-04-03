using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("itemexchange")]
    public class ItemExchange
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        public string type { get; set; }
        public string count { get; set; }
        public string itemname { get; set; }
        public string itemquality { get; set; }
        public string itemchinese { get; set; }
        public string itemicon { get; set; }
        public string itemtint { get; set; }
        public string command { get; set; }
        public string status { get; set; }
        public string couCurrType { get; set; }
        public string couPrice { get; set; }
        public string couCond { get; set; }
        public string coudatelimit { get; set; }
        public string couDateStart { get; set; }
        public string couDateEnd { get; set; }
        public string desc { get; set; }
        public string crafttime { get; set; }
        public string extinfo1 { get; set; }
        public string extinfo2 { get; set; }
        public string extinfo3 { get; set; }
        public string extinfo4 { get; set; }
        public string extinfo5 { get; set; }
        public string extinfo6 { get; set; }
    }

    public static class ItemExchangeConfig
    {
        public static string USEITEM = "1";
        public static string CRAFTITEM = "2";
    }
}
