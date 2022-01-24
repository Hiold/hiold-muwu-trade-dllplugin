using HarmonyLib;
using HioldMod.src.UserTools;
using System;
using System.Collections.Generic;
public static class Injections
{
    public static bool TELockServer_PostFix(int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt)
    {
        try
        {
            //Log.Out(string.Format("打开箱子位置: {0} {1} {2}", _blockPos.x, _blockPos.y, _blockPos.z));
            //Log.Out(string.Format("打开箱子clrIdx: {0}", _clrIdx));
            //Log.Out(string.Format("打开箱子lootEntityId: {0}", _lootEntityId));
            //Log.Out(string.Format("打开箱子entityIdThatOpenedIt: {0}", _entityIdThatOpenedIt));
            // Vector3i tmpPos = new Vector3i(_blockPos.x, _blockPos.y, _blockPos.z);
            // Auction.opendLootList[_blockPos.x + _blockPos.y + _blockPos.z + ""] = tmpPos;
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[HioldMod] 注入GameManager.TELockServer失败: {0}", e.Message));
        }
        return true;
    }


    public static bool TEUnlockServer_PostFix(int _clrIdx, Vector3i _blockPos, int _lootEntityId)
    {
        try
        {
            //Log.Out(string.Format("打开箱子位置: {0} {1} {2}", _blockPos.x, _blockPos.y, _blockPos.z));
            //Log.Out(string.Format("打开箱子clrIdx: {0}", _clrIdx));
            //Log.Out(string.Format("打开箱子lootEntityId: {0}", _lootEntityId));
            try
            {
                // Vector3i tmp = Auction.opendLootList[_blockPos.x + _blockPos.y + _blockPos.z + ""];
                // Auction.opendLootList.TryRemove(_blockPos.x + _blockPos.y + _blockPos.z + "",out Vector3i temp);
                //Log.Out(string.Format("[HioldMod] 比对成功,移除LootopendList"));
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[HioldMod] 注入GameManager.TELockServer失败: {0}", e.Message));
        }
        return true;
    }


    public static void SendXmlsToClient(ClientInfo _cInfo)
    {

    }

    
    public static bool SendXmlsToClient_postfix(ClientInfo _cInfo)
    {
        Log.Out("已进入拦截方法");
        //以__instance实例创建Traverse           挖掘字段mood   取float类型值
        object[] Rootxmls = Traverse.Create(typeof(WorldStaticData)).Field("xmlsToLoad").GetValue<object[]>();
        Log.Out("实际长度为:"+ Rootxmls.Length);
        for (int i=0;i<Rootxmls.Length;i++)
        {
            object xmls = Rootxmls[i];
            string xmlname = Traverse.Create(xmls).Field("XmlName").GetValue<string>();
            Log.Out("读取到xml数据:"+ xmlname);
        }
        return true; //继续执行原方法
    }




}