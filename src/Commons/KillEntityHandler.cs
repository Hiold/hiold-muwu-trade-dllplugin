using HioldMod.HttpServer;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.service;
using HioldMod.src.UserTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.Commons
{
    class KillEntityHandler
    {
        public static void EntityKilledHandler(Entity _target, Entity _player)
        {
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
                        PlayerGameEventService.addLog(new PlayerGameEvent()
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
                        PlayerGameEventService.addLog(new PlayerGameEvent()
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
