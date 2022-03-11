using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HioldMod.src.Commons
{
    class HartBeatHandler
    {
        //多线程中处理玩家在线心跳任务
        public static void HandlePlayerHartbeat(object nce)
        {
            Log.Out(string.Format("[Hiold]当前在线{0}人，正在更新用户信息", ConnectionManager.Instance.Clients.List.Count + ""));
            if (ConnectionManager.Instance.Clients.List.Count > 0)
            {
                foreach (var s in ConnectionManager.Instance.Clients.List)
                {
                    //心跳数据
                    EntityPlayer _entity = (EntityPlayer)GameManager.Instance.World.GetEntity(s.entityId);
                    Dictionary<string, string> hartbeat = new Dictionary<string, string>();
                    hartbeat["steamid"] = s.PlatformId.ReadablePlatformUserIdentifier;
                    hartbeat["playedtime"] = _entity.totalTimePlayed + "";
                    hartbeat["level"] = _entity.Progression.Level + "";
                    hartbeat["totalCrafted"] = _entity.totalItemsCrafted + "";
                    hartbeat["zombieKill"] = _entity.KilledZombies + "";
                    hartbeat["playerKill"] = _entity.KilledPlayers + "";

                    List<UserInfo> us = UserService.getUserBySteamid(s.PlatformId.ReadablePlatformUserIdentifier);
                    for (int i = 0; i < us.Count; i++)
                    {
                        UserInfo _user = us[i];

                        //在线时长
                        if (int.TryParse(_user.online_time, out int ont))
                        {
                            _user.online_time = (ont + 30) + "";
                        }
                        else
                        {
                            _user.online_time = "30";
                        }
                        //等级
                        _user.level = _entity.Progression.Level;
                        //总制作数量
                        _user.total_crafted = _entity.totalItemsCrafted + "";
                        //击杀僵尸
                        _user.zombie_kills = _entity.KilledZombies + "";
                        //击杀动物
                        _user.player_kills = _entity.KilledPlayers + "";
                        UserService.UpdateUserInfo(_user);
                    }
                }
            }
        }
    }
}
