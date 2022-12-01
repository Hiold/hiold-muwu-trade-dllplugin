using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace hide_and_seek.common
{
    public class Injections
    {
        /// <summary>
        /// GameManager类下OnBlockDamaged方法
        /// </summary>
        /// <param name="_world"></param>
        /// <param name="_clrIdx"></param>
        /// <param name="_blockPos"></param>
        /// <param name="_blockValue"></param>
        /// <param name="_damagePoints"></param>
        /// <param name="_entityIdThatDamaged"></param>
        /// <param name="_bUseHarvestTool"></param>
        /// <param name="_bBypassMaxDamage"></param>
        /// <param name="_recDepth"></param>
        /// <returns></returns>
        public static bool ChangeBlocks_fix(GameManager __instance, PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange)
        {
            Log.Out("进入ChangeBlocks_fix");
            //判断打击的是否为玩家伪装的方块
            int i = 0;
            //获取寻找者ID
            int pid = UserTools.GetEntityPlatformUserIdentifierAbs(persistentPlayerId);
            //检查是寻找者
            if (pid != -1 && MainController.Seekers.Contains(pid))
            {
                while (i < _blocksToChange.Count)
                {
                    BlockChangeInfo blockChangeInfo = _blocksToChange[i];
                    foreach (KeyValuePair<int, Vector3i> tempdt in MainController.HidersPos)
                    {
                        if (MainController.HidersPos.Values.Contains(blockChangeInfo.pos))
                        {
                            //找到了躲藏者
                            //移除躲藏者数据
                            MainController.Hiders.Remove(tempdt.Key);
                            MainController.HidersPos.Remove(tempdt.Key);
                            //给躲藏者发送消息
                            ClientInfo _cinfoHider = UserTools.GetClientInfoFromEntityId(tempdt.Key);
                            _cinfoHider.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "你被找到了!", "[87CEFA]躲猫猫", false, null));


                            ClientInfo _cinfoSeeker = UserTools.GetClientInfoFromEntityId(pid);
                            _cinfoSeeker.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "你找到一名躲藏者!", "[87CEFA]躲猫猫", false, null));
                            return false;
                        }
                    }
                    i++;
                }
            }
            //放行
            return true;
        }



        public static bool ProcessPackage_fix(NetPackageEntityRelPosAndRot __instance, World _world, GameManager _callbacks)
        {
            int pid = Traverse.Create(__instance).Field("entityId").GetValue<int>();
            Log.Out("当前执行者ID为:" + pid);
            if (MainController.Hiders.Contains(pid))
            {
                Log.Out("进入ProcessPackage_fix");
                Entity entity = _world.GetEntity(pid);
                if (entity == null)
                {
                    return true;
                }
                Entity attachedMainEntity = entity.AttachedMainEntity;
                if (attachedMainEntity != null && _world.GetPrimaryPlayerId() == attachedMainEntity.entityId)
                {
                    return true;
                }
                Vector3i dPos = Traverse.Create(__instance).Field("dPos").GetValue<Vector3i>();
                Vector3i newPos = entity.serverPos + dPos;
                Vector3i oldPos = MainController.HidersPos[pid];
                //如果位置发生变化，更新目标位置 burntWoodRoof
                if (newPos != oldPos)
                {
                    BlockTools.RemoveBlock(pid, oldPos);
                    BlockTools.SetBlock(entity, newPos, "burntWoodRoof");
                }
                //阻止玩家移动
                return false;
            }
            //放行
            return true;
        }


        public static bool SetPosAndQRotFromNetwork_fix(Entity __instance, Vector3 _pos, Quaternion _rot, int _steps)
        {
            int pid = __instance.entityId;
            Log.Out("当前执行者ID为:" + pid);
            if (MainController.Hiders.Contains(pid))
            {
                Log.Out("进入SetPosAndQRotFromNetwork_fix");
                Vector3i newPos = new Vector3i(_pos);
                Vector3i oldPos = MainController.HidersPos[pid];
                //如果位置发生变化，更新目标位置 burntWoodRoof
                if (newPos != oldPos)
                {
                    BlockTools.RemoveBlock(pid, oldPos);
                    BlockTools.SetBlock(__instance, newPos, "burntWoodRoof");
                }
                //阻止玩家移动
                return false;
            }
            //放行
            return true;
        }


        public static bool SetPosAndRotFromNetwork_fix(Entity __instance, Vector3 _pos, Vector3 _rot, int _steps)
        {
            int pid = __instance.entityId;
            Log.Out("当前执行者ID为:" + pid);
            if (MainController.Hiders.Contains(pid))
            {
                Log.Out("进入SetPosAndQRotFromNetwork_fix");
                Vector3i newPos = new Vector3i(_pos);
                Vector3i oldPos = MainController.HidersPos[pid];
                //如果位置发生变化，更新目标位置 burntWoodRoof
                if (newPos != oldPos)
                {
                    BlockTools.RemoveBlock(pid, oldPos);
                    BlockTools.SetBlock(__instance, newPos, "burntWoodRoof");
                    //MainController.HidersPos[pid] = newPos;
                }
                //阻止玩家移动
                return false;
            }
            //放行
            return true;
        }





    }
}
