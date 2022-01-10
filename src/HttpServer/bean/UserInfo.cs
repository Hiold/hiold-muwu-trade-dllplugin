using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    [SugarTable("userinfo")]
    public class UserInfo
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]//数据库是自增才配自增
        public int id { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_usercreateat" },IsNullable =true)]
        public DateTime created_at { get; set; }
        [SugarColumn(IsNullable =true)]
        public DateTime updated_at { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime deleted_at { get; set; }

        [SugarColumn(IndexGroupNameList = new string[] { "index_username" })]
        public string name { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_entityid" })]
        public string gameentityid { get; set; }
        [SugarColumn(IndexGroupNameList = new string[] { "index_platformid" })]
        public string platformid { get; set; }

        public float money { get; set; }

        public float credit { get; set; }

        public long status { get; set; }

        public string password { get; set; }

        public string qq { get; set; }

        public string email { get; set; }

        public string avatar { get; set; }

        public string sign { get; set; }

        public string extinfo1 { get; set; }

        public string extinfo2 { get; set; }

        public string extinfo3 { get; set; }

        public string extinfo4 { get; set; }

        public string extinfo5 { get; set; }

        public string extinfo6 { get; set; }

        public string trade_count { get; set; }

        public string store_count { get; set; }

        public string require_count { get; set; }

        public string type { get; set; }

        public string level { get; set; }

        public string online_time { get; set; }

        public string zombie_kills { get; set; }

        public string player_kills { get; set; }

        public string total_crafted { get; set; }

        public long vipdiscount { get; set; }

        public long creditcharge { get; set; }

        public long creditcost { get; set; }

        public long moneycharge { get; set; }

        public long moneycost { get; set; }

        public long signdays { get; set; }

        public long likecount { get; set; }

        public string shopname { get; set; }
    }
}
