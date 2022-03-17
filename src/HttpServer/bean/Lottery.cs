using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("lottery")]
    public class Lottery
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        public string type { get; set; }
        public string limit { get; set; }
        public string desc { get; set; }
        public string one { get; set; }
        public string ten { get; set; }
        public string status { get; set; }
        public string itemname { get; set; }
        public string itemchinese { get; set; }
        public string itemicon { get; set; }
        public string itemtint { get; set; }
        public string quality { get; set; }
        public string extinfo1 { get; set; }
        public string extinfo2 { get; set; }
        public string extinfo3 { get; set; }
        public string extinfo4 { get; set; }
        public string extinfo5 { get; set; }
        public string extinfo6 { get; set; }
    }
}
