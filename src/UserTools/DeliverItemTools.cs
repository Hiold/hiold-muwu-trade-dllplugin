using Platform.Steam;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HioldMod.src.UserTools
{
    class DeliverItemTools
    {
        public static ConcurrentQueue<DeliverItem> deliverQueue = new ConcurrentQueue<DeliverItem>();
        public static ConcurrentQueue<string> commandExecution = new ConcurrentQueue<string>();
        public static ConcurrentQueue<DeliverItemWithData> deliverDataItemQueue = new ConcurrentQueue<DeliverItemWithData>();

        public static bool islockdItem = false;
        public static bool islockdCommand = false;
        public static bool islockditemData = false;
        /// <summary>
        /// 主线程分发物品
        /// </summary>
        public static void TryDeliverItem()
        {
            if (!islockdItem)
            {
                //上锁
                islockdItem = true;
                if (deliverQueue.TryDequeue(out DeliverItem item))
                {
                    //获取客户端信息
                    PlatformUserIdentifierAbs identify = new UserIdentifierSteam(item.steamid);
                    ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForUserId(identify);
                    if (_cInfo != null)
                    {
                        Log.Out("发放物品: " + item.itemName);
                        GiveItemToInventory(_cInfo, item.itemName, item.count, item.itemquality);
                        Log.Out("发放了物品 name {0} count {1}  quality {2} ", item.itemName, item.count, item.itemquality);
                    }
                }
                //释放锁
                islockdItem = false;
            }
        }

        public static void TryExecuteCommand()
        {
            if (!islockdCommand)
            {
                //上锁
                islockdCommand = true;
                if (commandExecution.TryDequeue(out string item))
                {
                    Log.Out("执行了指令: " + item);
                    SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(item, null);
                }
                //释放锁
                islockdCommand = false;
            }
        }

        /// <summary>
        /// 发放带Data的数据
        /// </summary>
        public static void TryDeleverItemWithData()
        {
            if (!islockditemData)
            {
                //上锁
                islockditemData = true;
                if (deliverDataItemQueue.TryDequeue(out DeliverItemWithData item))
                {
                    //获取客户端信息
                    PlatformUserIdentifierAbs identify = new UserIdentifierSteam(item.steamid);
                    ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForUserId(identify);
                    deliverItemWithData(_cInfo, item);
                    Log.Out("发放物品给: " + item.steamid);
                }
                //释放锁
                islockditemData = false;
            }
        }

        /// <summary>
        /// 分发物品
        /// </summary>
        /// <param name="_cinfo"></param>
        /// <param name="item_name"></param>
        /// <param name="count"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        private static bool GiveItemToInventory(ClientInfo _cinfo, string item_name, int count, int quality)
        {
            World world = GameManager.Instance.World;
            ItemValue _itemValue;
            ItemClass _class;
            Block _block;

            _class = ItemClass.GetItemClass(item_name, true);
            _block = Block.GetBlockByName(item_name, true);

            if (_class == null && _block == null)
            {
                Log.Warning("[HioldMuwuTrade] Unable to find item " + item_name);
                return false;
            }
            else
            {
                _itemValue = new ItemValue(ItemClass.GetItem(item_name).type, quality, quality, false, null, 1);
            }
            if (count > _class.Stacknumber.Value)
            {
                int dcount = count / _class.Stacknumber.Value;
                int dsam = count % _class.Stacknumber.Value;

                for (int a = 0; a < dcount; a++)
                {
                    var entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                    {
                        entityClass = EntityClass.FromString("item"),
                        id = EntityFactory.nextEntityID++,
                        itemStack = new ItemStack(_itemValue, _class.Stacknumber.Value),
                        pos = world.Players.dict[_cinfo.entityId].position,
                        rot = new Vector3(20f, 0f, 20f),
                        lifetime = 60f,
                        belongsPlayerId = _cinfo.entityId
                    });
                    world.SpawnEntityInWorld(entityItem);
                    _cinfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, _cinfo.entityId));
                    world.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Killed);
                }

                if (dsam > 0)
                {
                    var entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                    {
                        entityClass = EntityClass.FromString("item"),
                        id = EntityFactory.nextEntityID++,
                        itemStack = new ItemStack(_itemValue, dsam),
                        pos = world.Players.dict[_cinfo.entityId].position,
                        rot = new Vector3(20f, 0f, 20f),
                        lifetime = 60f,
                        belongsPlayerId = _cinfo.entityId
                    });
                    world.SpawnEntityInWorld(entityItem);
                    _cinfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, _cinfo.entityId));
                    world.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Killed);
                }

            }
            else
            {
                var entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                {
                    entityClass = EntityClass.FromString("item"),
                    id = EntityFactory.nextEntityID++,
                    itemStack = new ItemStack(_itemValue, count),
                    pos = world.Players.dict[_cinfo.entityId].position,
                    rot = new Vector3(20f, 0f, 20f),
                    lifetime = 60f,
                    belongsPlayerId = _cinfo.entityId
                });
                world.SpawnEntityInWorld(entityItem);
                _cinfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, _cinfo.entityId));
                world.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Killed);
            }
            return true;
        }


        public static bool deliverItemWithData(ClientInfo _cInfo, DeliverItemWithData itemData)
        {

            ItemStack[] itemStacks = JsonUtils.ItemFromString(itemData.data);
            //发放物品
            if (itemStacks != null && itemStacks.Length > 0)
            {

                var _itemStack = itemStacks[0];

                World world = GameManager.Instance.World;
                //根据客户端提供数量修改对应数量
                var prepireStack = itemStacks[0];
                if (int.TryParse(itemData.count, out int tmpcount))
                {
                    prepireStack.count = tmpcount;
                }
                //执行发放物品
                EntityItem entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
                {
                    entityClass = EntityClass.FromString("item"),
                    id = EntityFactory.nextEntityID++,
                    pos = world.Players.dict[_cInfo.entityId].position,
                    rot = new Vector3(20f, 0f, 20f),
                    itemStack = prepireStack,
                    lifetime = 60f,
                    belongsPlayerId = _cInfo.entityId
                });
                world.SpawnEntityInWorld(entityItem);
                _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, _cInfo.entityId));
                world.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Despawned);
            }
            return true;
        }

    }

    class DeliverItem
    {
        public string steamid { get; set; }
        public string itemName { get; set; }
        public int itemquality { get; set; }
        public int count { get; set; }
    }

    class DeliverItemWithData
    {
        public string steamid { get; set; }
        public string data { get; set; }
        public string count { get; set; }
    }

    //Json转换工具
    class JsonUtils
    {
        public static string SerializeObject(object _item)
        {
            return SimpleJson2.SimpleJson2.SerializeObject(_item);
        }

        /// <summary>
        /// 从ItemStack解析string
        /// </summary>
        /// <param name="itemStack">原始物品信息</param>
        /// <returns>物品string信息</returns>
        public static string ByteStringFromItem(ItemStack[] itemStack)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            GameUtils.WriteItemStack(bw, itemStack);
            var hex = BitConverter.ToString(ms.ToArray(), 0).Replace("-", string.Empty).ToLower();
            return hex;
        }

        /// <summary>
        /// 从string获取ItemStack信息
        /// </summary>
        /// <param name="itemString">原始物品信息</param>
        /// <returns>Unity物品</returns>
        public static ItemStack[] ItemFromString(string itemString)
        {
            MemoryStream ms = new MemoryStream(HexToByte(itemString));
            BinaryReader br = new BinaryReader(ms);
            ItemStack[] resultStack = GameUtils.ReadItemStack(br);
            return resultStack;
        }


        public static byte[] HexToByte(string hexString)
        {
            //運算後的位元組長度:16進位數字字串長/2
            byte[] byteOUT = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i = i + 2)
            {
                //每2位16進位數字轉換為一個10進位整數
                byteOUT[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return byteOUT;
        }

    }
}
