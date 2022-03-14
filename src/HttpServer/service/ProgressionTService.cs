using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.service
{
    class ProgressionTService
    {
        /// <summary>
        /// 添加新的红包
        /// </summary>
        /// <param name="da"></param>
        public static void addProgressionT(ProgressionT da)
        {
            DataBase.db.Insertable<ProgressionT>(da).ExecuteCommand();
        }


        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static ProgressionT getProgressionTByid(string id)
        {
            return DataBase.db.Queryable<ProgressionT>().Where(string.Format("id = '{0}' ", id)).First();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static List<ProgressionT> getProgressionTs()
        {
            return DataBase.db.Queryable<ProgressionT>().Where(string.Format("status = '1' ")).ToList();
        }

        /// <summary>
        /// 根据SteamID获取玩家数据
        /// </summary>
        /// <param name="id">用户名</param>
        /// <returns></returns>
        public static void deleteProgressionTs(string id)
        {
            //字典
            var dt = new Dictionary<string, object>();
            dt.Add("id", id);
            dt.Add("status", "0");
            var t66 = DataBase.db.Updateable(dt).AS("progression").WhereColumns("id").ExecuteCommand();
        }

        public static void UpdateProgressionT(ProgressionT da)
        {
            DataBase.db.Updateable<ProgressionT>(da).ExecuteCommand();
        }


        /// <summary>
        /// 检查用户是否完成对应活动任务
        /// </summary>
        /// <param name="_ttype">活动任务类型</param>
        /// <param name="_ptype">完成时间类型</param>
        /// <param name="playerId">用户id</param>
        /// <param name="value">完成任务阈值</param>
        /// <returns></returns>
        public static Int64 IsProgressionComplete(int _ttype, int _ptype, string playerId, int value)
        {
            //本周日期
            string[] weekpair = ServerUtils.getDayOfThisWeek();
            //当天日期
            string[] daypair = ServerUtils.getDayOfToday();
            //逐一处理任务成就类型
            switch (_ttype)
            {
                #region 击杀僵尸
                case HttpServer.bean.ProgressionPType.ZOMBIE_KILL:
                    DataRow[] dt = null;
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], PlayerGameEventType.KILL_ZOMBIE)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], PlayerGameEventType.KILL_ZOMBIE)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, PlayerGameEventType.KILL_ZOMBIE)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 击杀动物
                case HttpServer.bean.ProgressionPType.ANIMAL_KILL:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], PlayerGameEventType.KILL_ANIMAL)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], PlayerGameEventType.KILL_ANIMAL)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, PlayerGameEventType.KILL_ANIMAL)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 点赞
                case HttpServer.bean.ProgressionPType.LIKE:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], PlayerGameEventType.LIKE)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], PlayerGameEventType.LIKE)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.gameeventdb.Ado.GetDataTable(string.Format("select count(*) cnt from gameeventlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, PlayerGameEventType.LIKE)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 在线时长
                case HttpServer.bean.ProgressionPType.ONLINE_TIME:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.db.Ado.GetDataTable(string.Format("SELECT online_time FROM userinfo where gameentityid='{0}' ", playerId)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 交易量
                case HttpServer.bean.ProgressionPType.TRADE_COUNT:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], LogType.BuyUserTrade)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], LogType.BuyUserTrade)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, LogType.BuyUserTrade)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 交易额
                case HttpServer.bean.ProgressionPType.TRADE_AMOUNT:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], LogType.BuyUserTrade)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], LogType.BuyUserTrade)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, LogType.BuyUserTrade)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 求购量
                case HttpServer.bean.ProgressionPType.REQUIRE_COUNT:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], LogType.PostRequire)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], LogType.PostRequire)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, LogType.PostRequire)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 求购额
                case HttpServer.bean.ProgressionPType.REQUIRE_AMOUNT:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], LogType.PostRequire)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], LogType.PostRequire)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, LogType.PostRequire)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 供货量
                case HttpServer.bean.ProgressionPType.SUPPLY_COUNT:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], LogType.SupplyItem)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], LogType.SupplyItem)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, LogType.SupplyItem)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 供货额
                case HttpServer.bean.ProgressionPType.SUPPLY_AMOUNT:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], LogType.SupplyItem)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], LogType.SupplyItem)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select sum(extinfo4) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, LogType.SupplyItem)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 签到
                case HttpServer.bean.ProgressionPType.DAILY_SIGN:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.DAILY:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, daypair[0], daypair[1], LogType.dailySign)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.WEEK:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and actTime>='{1}' and actTime<='{2}' and t.actType='{3}' ", playerId, weekpair[0], weekpair[1], LogType.dailySign)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.logdb.Ado.GetDataTable(string.Format("select count(*) cnt from actionlog t where t.atcPlayerEntityId='{0}' and t.actType='{1}' ", playerId, LogType.dailySign)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 等级
                case HttpServer.bean.ProgressionPType.LEVEL:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.db.Ado.GetDataTable(string.Format("SELECT level FROM userinfo where gameentityid='{0}' ", playerId)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region 制作物品
                case HttpServer.bean.ProgressionPType.CRAFTED:
                    switch (_ptype)
                    {
                        case HttpServer.bean.ProgressionType.MAIN:
                            dt = DataBase.db.Ado.GetDataTable(string.Format("SELECT total_crafted FROM userinfo where gameentityid='{0}' ", playerId)).Select();
                            //Console.WriteLine(dt);
                            foreach (DataRow row in dt)
                            {
                                foreach (object data in row.ItemArray)
                                {
                                    try
                                    {
                                        return (Int64)data;
                                    }
                                    catch (Exception)
                                    {
                                        return 0;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion
                default:
                    return 0;
            }
            return 0;
        }

    }
}
