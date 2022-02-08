﻿using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class ActionLogService
    {
        /// <summary>
        /// 插入游戏内物品数据
        /// </summary>
        /// <param name="item">物品</param>
        public static void addLog(ActionLog log)
        {
            DataBase.db.Insertable<ActionLog>(log).ExecuteCommand();
        }


        public static Int64 QueryItemLogCount(string id, string itemid, int logtype, string startTime, string endTime)
        {
            DataRow[] dt = null;
            if (startTime == null && endTime == null)
            {
                dt = DataBase.db.Ado.GetDataTable(string.Format("select sum(extinfo3) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' ", id, itemid, logtype)).Select();
            }
            else
            {
                dt = DataBase.db.Ado.GetDataTable(string.Format("select sum(extinfo3) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.extinfo1='{1}' and t.actType='{2}' and t.actTime>'{3}' and t.actTime< '{4}' ", id, itemid, logtype, startTime, endTime)).Select();
            }
            Console.WriteLine(dt);
            foreach (DataRow row in dt)
            {
                foreach (object data in row.ItemArray)
                {
                    return (Int64)data;
                }
            }
            return 0;
        }
    }
}