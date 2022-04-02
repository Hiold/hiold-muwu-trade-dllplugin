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
        string fromitem  {get;set;}
        string toitem { get; set; }
        string craftTime { get;set }
        public string type { get; set; }
        public string desc { get; set; }
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
