using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class SignInfoService
    {

        public static void addSignInfo(SignInfo da)
        {
            DataBase.db.Insertable<SignInfo>(da).ExecuteCommand();
        }

        public static List<SignInfo> ValidateCircle(string day)
        {
            List<SignInfo> listCircle = DataBase.db.Queryable<SignInfo>().Where(string.Format("status = '1' and date ='0001-01-01 00:00:00' and day='{0}'", day)).ToList();
            return listCircle;
        }

        public static List<SignInfo> ValidateDate(string date)
        {
            List<SignInfo> listCircle = DataBase.db.Queryable<SignInfo>().Where(string.Format("status = '1' and date ='{0} 00:00:00'", date)).ToList();
            return listCircle;
        }

        public static SignInfo getSignInfoByid(string id)
        {
            return DataBase.db.Queryable<SignInfo>().Where(string.Format("id = '{0}' ", id)).First();
        }


        public static List<SignInfo> getSignInfos()
        {
            return DataBase.db.Queryable<SignInfo>().Where(string.Format("status = '1'")).ToList();
        }

        public static List<SignInfo> getSignInfosAvalible()
        {
            string[] thisWeekPair = ServerUtils.getDayOfThisWeek();
            List<SignInfo> listCircle = DataBase.db.Queryable<SignInfo>().Where(string.Format("status = '1' and date ='0001-01-01 00:00:00'")).ToList();
            List<SignInfo> listThisWeek = DataBase.db.Queryable<SignInfo>().Where(string.Format("status = '1' and date >='{0}' and date <='{1}'", thisWeekPair[0], thisWeekPair[1])).ToList();
            listCircle.AddRange(listThisWeek);
            return listCircle;
        }


        public static void deleteSignInfos(string id)
        {
            //字典
            var dt = new Dictionary<string, object>();
            dt.Add("id", id);
            dt.Add("status", "0");
            var t66 = DataBase.db.Updateable(dt).AS("SignInfo").WhereColumns("id").ExecuteCommand();
        }

        public static void UpdateSignInfo(SignInfo da)
        {
            DataBase.db.Updateable<SignInfo>(da).ExecuteCommand();
        }

    }
}
