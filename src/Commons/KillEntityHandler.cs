using HioldMod.HttpServer;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.service;
using HioldMod.src.UserTools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConfigTools.LoadMainConfig;

namespace HioldMod.src.Commons
{
    class KillEntityHandler
    {
        public static ConcurrentQueue<PlayerGameEvent> eventLog = new ConcurrentQueue<PlayerGameEvent>();

        //多线程中处理玩家在线心跳任务
        public static void AddEventLog(object nce)
        {
            while (eventLog.TryDequeue(out PlayerGameEvent events))
            {
                PlayerGameEventService.addLog(events);
            }
        }

        public static void EntityKilledHandler(Entity _target, Entity _player)
        {
            //默认不开启记录
            if (MainConfig.killevent.Equals("False"))
            {
                return;
            }
            try
            {
                //非空校验
                if (_target != null && _player != null)
                {
                    ClientInfo _cInfo = HioldsCommons.GetClientInfoFromEntityId(_player.entityId);

                    //Log.Out(string.Format("[HioldMod] 玩家:{0} {1} 击杀了:{2} 发放奖励", _cInfo.playerId, _cInfo.playerName, _target.EntityClass.entityClassName));
                    //击杀目标信息
                    string _tags = _target.EntityClass.Tags.ToString();
                    string entityClassName = _target.EntityClass.entityClassName;

                    //击杀目标为僵尸
                    if (_tags.Contains("zombie"))
                    {
                        eventLog.Enqueue(new PlayerGameEvent()
                        {
                            actTime = DateTime.Now,
                            actType = PlayerGameEventType.KILL_ZOMBIE,
                            atcPlayerEntityId = _cInfo.PlatformId.ReadablePlatformUserIdentifier,
                            extinfo1 = _tags,
                            extinfo2 = entityClassName,
                            extinfo3 = LocalizationUtils.getTranslate(entityClassName),
                            desc = string.Format("击杀了僵尸 {0}", entityClassName)
                        });
                    }

                    //击杀目标为动物
                    if (_tags.Contains("animal"))
                    {
                        eventLog.Enqueue(new PlayerGameEvent()
                        {
                            actTime = DateTime.Now,
                            actType = PlayerGameEventType.KILL_ANIMAL,
                            atcPlayerEntityId = _cInfo.PlatformId.ReadablePlatformUserIdentifier,
                            extinfo1 = _tags,
                            extinfo2 = entityClassName,
                            extinfo3 = LocalizationUtils.getTranslate(entityClassName),
                            desc = string.Format("击杀了动物 {0}", entityClassName)
                        });
                    }
                }


            }
            catch (Exception)
            {
                LogUtils.Loger("可能非玩家击杀系统无法处理，忽略该击杀");
            }

        }
    }
}
