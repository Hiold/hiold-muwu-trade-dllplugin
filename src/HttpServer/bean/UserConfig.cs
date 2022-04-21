using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("userconfig")]
    class UserConfig
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_userconfigcreateat" }, IsNullable = true)]
        public DateTime created_at { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime updated_at { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime deleted_at { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "index_configusername" })]
        public string name { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_configentityid" })]
        public string gameentityid { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_configplatformid" })]
        public string platformid { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "index_configtype" })]
        public string configType { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_configvalue" })]
        public string configValue { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_available" })]
        public string available { get; set; }
        public string extinfo1 { get; set; }
        public string extinfo2 { get; set; }
        public string extinfo3 { get; set; }
        public string extinfo4 { get; set; }
        public string extinfo5 { get; set; }
        public string extinfo6 { get; set; }
    }
    public class ConfigType
    {
        public static string Collect = "1";
        public static string Item_Limit = "2";
        public static string Sgj_Point = "3";
    }
}
