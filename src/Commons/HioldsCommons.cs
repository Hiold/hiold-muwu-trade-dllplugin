using Platform;
using Platform.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HioldMod.src.Commons
{
    class HioldsCommons
    {
        public static string Prefix = "[87CEFA]系统";
        public static void SendMessage(ClientInfo _receiver, string _message)
        {
            _receiver.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, _message, Prefix, false, null));
        }

        /// <summary>
        /// 传送玩家
        /// </summary>
        /// <param name="_cinfo"></param>
        /// <param name="_pos"></param>
        public static void TeleportPlayer(ClientInfo _cinfo, Vector3i _pos)
        {
            Vector3 pos;
            pos.x = _pos.x;
            pos.y = _pos.y;
            pos.z = _pos.z;
            NetPackageTeleportPlayer netNetPackageTeleportPlayer = NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(pos);
            _cinfo.SendPackage(netNetPackageTeleportPlayer);
        }

        /// <summary>
        /// 获取ClientInfo
        /// </summary>
        /// <param name="_EntityId"></param>
        /// <returns></returns>
        public static ClientInfo GetClientInfoFromEntityId(int _EntityId)
        {
            ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForEntityId(_EntityId);
            if (_cInfo != null)
            {
                return _cInfo;
            }
            return null;
        }

        /// <summary>
        /// 获取玩家pdf数据
        /// </summary>
        /// <param name="_playerId"></param>
        /// <returns></returns>
        public static PersistentPlayerData GetPersistentPlayerDataBySteamId(PlatformUserIdentifierAbs _identify)
        {
            PersistentPlayerList _persistentPlayerList = GameManager.Instance.persistentPlayers;
            if (_persistentPlayerList != null)
            {
                PersistentPlayerData _persistentPlayerData = _persistentPlayerList.GetPlayerData(_identify);
                if (_persistentPlayerData != null)
                {
                    return _persistentPlayerData;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取玩家pdf数据
        /// </summary>
        /// <param name="_playerId"></param>
        /// <returns></returns>
        public static PersistentPlayerData GetPersistentPlayerDataByEOS(string _playerId)
        {
            PersistentPlayerList _persistentPlayerList = GameManager.Instance.persistentPlayers;
            Log.Out("[HioldMod]服务器中总玩家数为:" + _persistentPlayerList.Players.Count);
            foreach (KeyValuePair<PlatformUserIdentifierAbs, PersistentPlayerData> pls in _persistentPlayerList.Players)
            {
                Log.Out("[HioldMod]Identify=" + pls.Key.CombinedString);
                if (pls.Key.CombinedString.Equals("EOS_"+_playerId))
                {
                    return pls.Value;
                }
            }
            return null;


            //if (_persistentPlayerList != null)
            //{
            //    PersistentPlayerData _persistentPlayerData = _persistentPlayerList.GetPlayerData(PlatformUserIdentifierAbs.FromCombinedString("Steam_" + _playerId, true));
            //    if (_persistentPlayerData != null)
            //    {
            //        return _persistentPlayerData;
            //    }
            //}
            //return null;
        }

        /// <summary>
        /// 坐标检查
        /// </summary>
        /// <param name="person_pos"></param>
        /// <param name="keystone_pos"></param>
        /// <returns></returns>
        private bool CheckisInLandClaimArea(Vector3 person_pos, Vector3i keystone_pos)
        {
            int range_inc = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize) / 2;
            int x_max = keystone_pos.x + range_inc + 1;
            int x_min = keystone_pos.x - range_inc - 1;
            int z_max = keystone_pos.z + range_inc + 1;
            int z_min = keystone_pos.z - range_inc - 1;

            if (person_pos.x >= x_min && person_pos.x <= x_max && person_pos.z >= z_min && person_pos.z <= z_max)
                return true;
            return false;
        }
    }
}
