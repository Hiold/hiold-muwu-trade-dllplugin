using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("userstorage")]
    class UserStorage : TradeManageItem
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_username" })]
        public string name { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_entityid" })]
        public string gameentityid { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_platformid" })]
        public string platformid { get; set; }

        //获取时间
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_collecttime" })]
        public DateTime collectTime { get; set; }

        //分发时间
        [SugarColumn(IndexGroupNameList = new string[] { "us_index_obtaintime" })]
        public DateTime obtainTime { get; set; }

        public string extinfo1 { get; set; }
        public string extinfo2 { get; set; }
        public string extinfo3 { get; set; }
        public string extinfo4 { get; set; }
        public string extinfo5 { get; set; }

    }
}
