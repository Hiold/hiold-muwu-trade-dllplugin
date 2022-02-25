//using HarmonyLib;
using HarmonyLib;
using System;
using System.Reflection;

namespace ServerTools
{
    class RunTimePatch
    {
        public static void PatchAll()
        {
            try
            {

                Log.Out("[HioldMod] 初始化Hook");

                ////打开箱子
                //Harmony harmony = new Harmony("net.hiold.patch");
                //MethodInfo original = AccessTools.Method(typeof(GameManager), "TELockServer");
                //if (original == null)
                //{
                //    Log.Out(string.Format("[HioldMod] 注入失败: GameManager.TELockServer 未找到"));
                //}
                //else
                //{
                //    MethodInfo prefix = typeof(Injections).GetMethod("TELockServer_PostFix");
                //    if (prefix == null)
                //    {
                //        Log.Out(string.Format("[HioldMod] Injection failed: TELockServer_PostFix"));
                //        return;
                //    }
                //    harmony.Patch(original, new HarmonyMethod(prefix), null);
                //}


                ////关闭箱子
                //MethodInfo original2 = AccessTools.Method(typeof(GameManager), "TEUnlockServer");
                //if (original2 == null)
                //{
                //    Log.Out(string.Format("[HioldMod] 注入失败: GameManager.TELockServer 未找到"));
                //}
                //else
                //{
                //    MethodInfo prefix = typeof(Injections).GetMethod("TEUnlockServer_PostFix");
                //    if (prefix == null)
                //    {
                //        Log.Out(string.Format("[HioldMod] Injection failed: TEUnlockServer_PostFix"));
                //        return;
                //    }
                //    harmony.Patch(original2, new HarmonyMethod(prefix), null);
                //}

                Harmony harmony = new Harmony("net.hiold.patch");
                //xml发送拦截
                MethodInfo original = AccessTools.Method(typeof(WorldStaticData), "SendXmlsToClient");
                if (original == null)
                {
                    Log.Out(string.Format("[HioldMod] 注入失败: WorldStaticData.SendXmlsToClient 未找到"));
                }
                else
                {
                    MethodInfo prefix = typeof(Injections).GetMethod("SendXmlsToClient_postfix");
                    if (prefix == null)
                    {
                        Log.Out(string.Format("[HioldMod] 注入失败: Injections.SendXmlsToClient_postfix"));
                        return;
                    }
                    harmony.Patch(original, new HarmonyMethod(prefix), null);
                }

                //寻路异常拦截
                MethodInfo original2 = AccessTools.Method(typeof(Pathfinding.AstarData), "FindGraphTypes");
                if (original2 == null)
                {
                    Log.Out(string.Format("[HioldMod] 注入失败: WorldStaticData.SendXmlsToClient 未找到"));
                }
                else
                {
                    MethodInfo prefix2 = typeof(Injections).GetMethod("FindGraphTypes_postfix");
                    if (prefix2 == null)
                    {
                        Log.Out(string.Format("[HioldMod] 注入失败: Injections.SendXmlsToClient_postfix"));
                        return;
                    }
                    harmony.Patch(original2, new HarmonyMethod(prefix2), null);
                }





            }
            catch (Exception e)
            {
                Log.Out(string.Format("[SERVERTOOLS] Error in PatchTools.PatchAll: {0}", e.Message));
            }
        }
    }
}