using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using HarmonyLib;
using UnityEngine;
using XMLData.Item;
using NaiwaziServerKitInterface;
using hide_and_seek.common;

namespace HioldMod
{
    public class APIHiold : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            //注册事件
            ModEvents.GameStartDone.RegisterHandler(GameStartDone);
        }


        private static void GameStartDone()
        {
            Harmony harmony = new Harmony("net.hiold.hideandseek");
            //xml发送拦截
            MethodInfo original = AccessTools.Method(typeof(GameManager), "ChangeBlocks");
            if (original == null)
            {
                Log.Out(string.Format("[HioldMod] 注入失败: WorldStaticData.SendXmlsToClient 未找到"));
            }
            else
            {
                MethodInfo prefix = typeof(Injections).GetMethod("ChangeBlocks_fix");
                if (prefix == null)
                {
                    Log.Out(string.Format("[HioldMod] 注入失败: Injections.SendXmlsToClient_postfix"));
                    return;
                }
                harmony.Patch(original, new HarmonyMethod(prefix), null);
            }




            MethodInfo original2 = AccessTools.Method(typeof(Entity), "SetPosAndQRotFromNetwork");
            if (original2 == null)
            {
                Log.Out(string.Format("[HioldMod] 注入失败: WorldStaticData.SendXmlsToClient 未找到"));
            }
            else
            {
                MethodInfo prefix2 = typeof(Injections).GetMethod("SetPosAndQRotFromNetwork_fix");
                if (prefix2 == null)
                {
                    Log.Out(string.Format("[HioldMod] 注入失败: Injections.SendXmlsToClient_postfix"));
                    return;
                }
                harmony.Patch(original2, new HarmonyMethod(prefix2), null);
            }




            MethodInfo original3 = AccessTools.Method(typeof(Entity), "SetPosAndRotFromNetwork");
            if (original3 == null)
            {
                Log.Out(string.Format("[HioldMod] 注入失败: WorldStaticData.SendXmlsToClient 未找到"));
            }
            else
            {
                MethodInfo prefix3 = typeof(Injections).GetMethod("SetPosAndRotFromNetwork_fix");
                if (prefix3 == null)
                {
                    Log.Out(string.Format("[HioldMod] 注入失败: Injections.SendXmlsToClient_postfix"));
                    return;
                }
                harmony.Patch(original3, new HarmonyMethod(prefix3), null);
            }




            int ret = ChatInterface.ChatWatcher_Register(OnChatMessage, "HioldMuwu");
            Log.Out("[HideAndSeek] 游戏已加载");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cInfo"></param>
        /// <param name="_msg"></param>
        /// <param name="_type"></param>
        /// <param name=""></param>
        private static void OnChatMessage(ClientInfo _cInfo, string _msg, /*uint _timeStamp,*/ EChatType _type)
        {
            Log.Out("接收到消息:"+ _msg);
            if (!string.IsNullOrEmpty(_msg) && _msg.StartsWith("/seek"))
            {
                MainController.Seekers.Add(_cInfo.entityId);
            }


            if (!string.IsNullOrEmpty(_msg) && _msg.StartsWith("/hide"))
            {
                MainController.Hiders.Add(_cInfo.entityId);
                Entity entity = GameManager.Instance.World.GetEntity(_cInfo.entityId);

                MainController.HidersPos.Add(_cInfo.entityId, new Vector3i(entity.position));
            }
        } 
    }
}
