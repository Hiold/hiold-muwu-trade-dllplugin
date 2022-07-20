using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using HarmonyLib;
using UnityEngine;
using XMLData.Item;

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
            
            Log.Out("[HioldDamageFixer] 伤害修正Hook初始化完毕");
        }
    }
}
