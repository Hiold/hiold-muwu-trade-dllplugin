using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{

    [SugarTable("userrequire")]
    class UserRequire
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ur_index_username" })]
        public string username { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ur_index_entityid" })]
        public string gameentityid { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ur_index_platformid" })]
        public string platformid { get; set; }

        public string Itemname { get; set; }
        public string Itemchinese { get; set; }
        public int Itemcount { get; set; }
        public double Price { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ut_index_supplyusername" })]
        public string supplyusername { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ut_index_supplyentityid" })]
        public string supplygameentityid { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "ut_index_supplyplatformid" })]
        public string supplyplatformid { get; set; }
        public string Status { get; set; }
        public string Extinfo1 { get; set; }
        public string Extinfo2 { get; set; }
        public string Extinfo3 { get; set; }
        public DateTime Requiretime { get; set; }
        public DateTime Supplytime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Itemquality { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Itemusetime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Itemicon { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Itemicontint { get; set; }
        [SugarColumn(IsNullable = true)]
        public string Itemgroups { get; set; }
    }

    public class UserRequireConfig
    {
        public static string NORMAL_REQUIRE = "0";
        public static string SUPPLYED = "1";
        public static string DELETE = "2";
    }
}
