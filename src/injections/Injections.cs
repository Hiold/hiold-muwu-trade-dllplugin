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
        foreach (WorldStaticData.XmlLoadInfo xmlLoadInfo in WorldStaticData.xmlsToLoad)
        {
            if (xmlLoadInfo.SendToClients && (xmlLoadInfo.LoadClientFile || xmlLoadInfo.CompressedXmlData != null))
            {
                _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageConfigFile>().Setup(xmlLoadInfo.XmlName, xmlLoadInfo.LoadClientFile ? null : xmlLoadInfo.CompressedXmlData));
            }
        }
    }




}