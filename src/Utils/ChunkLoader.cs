using HioldMod.src.Commons;
using HioldMod.src.Reflection;
using HioldMod.src.UserTools;
using HioldMod.src.CommonUtils;
using ServerTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static HioldMod.HioldMod;
using static HioldMod.src.CommonUtils.UserAndItemCheck;
using HioldMod.src.HttpServer.service;
using HioldMod.src.HttpServer.bean;

namespace HioldMod.src.ChunckLoader
{
    public class ChunkLoader
    {
        #region 根据EOSID获取玩家领地内容器
        /// <summary>
        /// 根据EOSID获取玩家领地内容器
        /// </summary>
        /// <param name="eosid"></param>
        /// <returns></returns>
        /// 
        public static List<Dictionary<string, object>> loadContainerListAround(string eosid)
        {
            List<Dictionary<string, object>> loots = new List<Dictionary<string, object>>();
            ChunkManager.ChunkObserver co = null;
            //区块信息
            //获取玩家领地信息
            //Log.Out("访问的steamid为：" + eosid);
            PersistentPlayerData ppdd = HioldsCommons.GetPersistentPlayerDataByEOS(eosid);
            //Log.Out("获取到的pdd：" + ppdd);
            List<Vector3i> AllLppoition = new List<Vector3i>();
            if (ppdd != null && ppdd.LPBlocks != null)
            {
                AllLppoition.AddRange(loadPosSurround(ppdd.LPBlocks));
            }
            //Log.Out("ACL数量：" + AllLppoition.Count);

            //获取ACL 友军相关信息
            if (ppdd.ACL != null)
            {
                foreach (PlatformUserIdentifierAbs sts in ppdd.ACL)
                {
                    PersistentPlayerData tempPdd = HioldsCommons.GetPersistentPlayerDataBySteamId(sts);
                    if (tempPdd != null && tempPdd.LPBlocks != null)
                    {
                        AllLppoition.AddRange(loadPosSurround(ppdd.LPBlocks));
                    }
                }
            }
            //Log.Out("领地石个数：" + AllLppoition.Count);

            try
            {
                //寻找容器
                int landProtectSize = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize);
                //Log.Out(string.Format("领地保护范围:{0}", landProtectSize));
                DictionaryList<Vector3i, TileEntity> _tiles = new DictionaryList<Vector3i, TileEntity>();
                foreach (Vector3i lpbPos in AllLppoition)
                {
                    Chunk _c = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(lpbPos);
                    //判断区块是否已加载
                    int count = 0;
                    if (_c == null)
                    {
                        UnityEngine.Vector3 v3 = new UnityEngine.Vector3(lpbPos.x, lpbPos.y, lpbPos.z);
                        co = GameManager.Instance.AddChunkObserver(v3, false, 0, -1);
                        while (true)
                        {
                            _c = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(lpbPos);
                            if (_c != null || ++count >= 50)
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }


                    _tiles = _c.GetTileEntities();
                    foreach (TileEntity _tile in _tiles.dict.Values)
                    {
                        TileEntityType _type = _tile.GetTileEntityType();
                        if (_type.ToString().Contains("Loot"))
                        {
                            try
                            {
                                //有密码容器
                                TileEntityLootContainer SecureLoot = (TileEntityLootContainer)_tile;
                                Vector3i vec3i = SecureLoot.ToWorldPos();
                                SecureLoot.GetClrIdx();
                                //检测领地内箱子
                                //检测附近箱子数量
                                if (lpbPos.x - (landProtectSize / 2) < vec3i.x && lpbPos.x + (landProtectSize / 2) > vec3i.x)
                                {
                                    if (lpbPos.y - (landProtectSize / 2) < vec3i.y && lpbPos.y + (landProtectSize / 2) > vec3i.y)
                                    {
                                        if (lpbPos.z - (landProtectSize / 2) < vec3i.z && lpbPos.z + (landProtectSize / 2) > vec3i.z)
                                        {
                                            Dictionary<string, object> lt = new Dictionary<string, object>();
                                            lt.Add("name", SecureLoot.blockValue.ToItemValue().ItemClass.Name);
                                            lt.Add("nametranslate", LocalizationUtils.getTranslate(SecureLoot.blockValue.ToItemValue().ItemClass.Name));
                                            lt.Add("icon", SecureLoot.blockValue.ToItemValue().ItemClass.CustomIcon);
                                            lt.Add("x", vec3i.x);
                                            lt.Add("y", vec3i.y);
                                            lt.Add("z", vec3i.z);
                                            lt.Add("clr", SecureLoot.GetClrIdx());
                                            try
                                            {
                                                ILockable lockable = (ILockable)_tile;
                                                //密码
                                                if (lockable.HasPassword())
                                                {
                                                    lt.Add("pw", "1");
                                                }
                                                else
                                                {
                                                    lt.Add("pw", "0");
                                                }
                                                //锁定状态
                                                if (lockable.IsLocked())
                                                {
                                                    lt.Add("locked", "1");
                                                }
                                                else
                                                {
                                                    lt.Add("locked", "0");
                                                }
                                                //容器所属
                                                lt.Add("owner", UserService.getUserByEOS(lockable.GetOwner().ReadablePlatformUserIdentifier));

                                            }
                                            catch (Exception)
                                            {
                                                lt.Add("pw", "0");
                                                lt.Add("locked", "0");
                                                lt.Add("owner", "");
                                            }

                                            try
                                            {
                                                ITileEntitySignable sign = (ITileEntitySignable)_tile;
                                                lt.Add("sign", sign.GetText());
                                            }
                                            catch (Exception e)
                                            {
                                                lt.Add("sign", "");
                                            }

                                            loots.Add(lt);


                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Out(e.StackTrace);
                            }
                        }
                    }
                }
                return loots;
            }
            catch (Exception e)
            {
                Log.Out(e.StackTrace);
            }
            finally
            {
                //移除区块
                if (co != null)
                {
                    GameManager.Instance.RemoveChunkObserver(co);
                }
            }
            return loots;
        }
        #endregion

        #region 根据玩家ClientInfo将玩家领地内容器保存到文件
        /// <summary>
        /// 根据玩家ClientInfo将玩家领地内容器保存到文件
        /// </summary>
        /// <param name="_cInfo"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> loadContainerListAroundToFile(ClientInfo _cInfo)
        {
            List<Dictionary<string, object>> loots = new List<Dictionary<string, object>>();
            ChunkManager.ChunkObserver co = null;
            //区块信息
            //获取玩家领地信息
            List<Vector3i> AllLppoition = new List<Vector3i>();
            EntityPlayer _entity = (EntityPlayer)GameManager.Instance.World.GetEntity(_cInfo.entityId);
            Vector3i pos = new Vector3i(_entity.position.x, _entity.position.y, _entity.position.z);
            AllLppoition.Add(pos);
            try
            {
                //寻找容器
                int landProtectSize = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize);
                //Log.Out(string.Format("领地保护范围:{0}", landProtectSize));
                DictionaryList<Vector3i, TileEntity> _tiles = new DictionaryList<Vector3i, TileEntity>();
                foreach (Vector3i lpbPos in AllLppoition)
                {
                    Chunk _c = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(lpbPos);
                    //判断区块是否已加载
                    int count = 0;
                    if (_c == null)
                    {
                        UnityEngine.Vector3 v3 = new UnityEngine.Vector3(lpbPos.x, lpbPos.y, lpbPos.z);
                        co = GameManager.Instance.AddChunkObserver(v3, false, 0, -1);
                        while (true)
                        {
                            _c = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(lpbPos);
                            if (_c != null || ++count >= 50)
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }


                    _tiles = _c.GetTileEntities();
                    foreach (TileEntity _tile in _tiles.dict.Values)
                    {
                        TileEntityType _type = _tile.GetTileEntityType();
                        if (_type.ToString().Contains("Loot"))
                        {
                            try
                            {
                                //有密码容器
                                TileEntityLootContainer SecureLoot = (TileEntityLootContainer)_tile;
                                Vector3i vec3i = SecureLoot.ToWorldPos();
                                SecureLoot.GetClrIdx();
                                //检测领地内箱子
                                //检测附近箱子数量
                                if (lpbPos.x - (landProtectSize / 2) < vec3i.x && lpbPos.x + (landProtectSize / 2) > vec3i.x)
                                {
                                    if (lpbPos.y - (landProtectSize / 2) < vec3i.y && lpbPos.y + (landProtectSize / 2) > vec3i.y)
                                    {
                                        if (lpbPos.z - (landProtectSize / 2) < vec3i.z && lpbPos.z + (landProtectSize / 2) > vec3i.z)
                                        {

                                            Dictionary<string, object> lt = new Dictionary<string, object>();
                                            lt.Add("name", SecureLoot.blockValue.ToItemValue().ItemClass.Name);
                                            lt.Add("icon", SecureLoot.blockValue.ToItemValue().ItemClass.CustomIcon);
                                            lt.Add("x", vec3i.x);
                                            lt.Add("y", vec3i.y);
                                            lt.Add("z", vec3i.z);
                                            lt.Add("clr", SecureLoot.GetClrIdx());
                                            try
                                            {
                                                ILockable lockable = (ILockable)_tile;
                                                //密码
                                                if (lockable.HasPassword())
                                                {
                                                    lt.Add("pw", "1");
                                                }
                                                else
                                                {
                                                    lt.Add("pw", "0");
                                                }
                                                //锁定状态
                                                if (lockable.IsLocked())
                                                {
                                                    lt.Add("locked", "1");
                                                }
                                                else
                                                {
                                                    lt.Add("locked", "0");
                                                }
                                                //容器所属
                                                lt.Add("owner", lockable.GetOwner());


                                            }
                                            catch (Exception)
                                            {
                                                lt.Add("pw", "0");
                                                lt.Add("locked", "0");
                                                lt.Add("owner", "");
                                            }

                                            try
                                            {
                                                ITileEntitySignable sign = (ITileEntitySignable)_tile;
                                                lt.Add("sign", sign.GetText());
                                            }
                                            catch (Exception e)
                                            {
                                                lt.Add("sign", "");
                                            }

                                            loots.Add(lt);


                                        }
                                    }
                                }
                                //输出到文件
                                string file = string.Format("Container_{0}.txt", DateTime.Today.ToString("yyyy-MM-dd"));
                                string filepath = string.Format("{0}/Logs/{1}", API.ConfigPath, file);
                                using (StreamWriter sw = new StreamWriter(filepath, true))
                                {
                                    BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                                    //ItemStack[] istack = new ItemStack[1];
                                    //istack[0] = items[idx];
                                    //GameUtils.WriteItemStack(bw, istack);
                                    //sw.WriteLine(JsonUtils.SerializeObject(_i));
                                    //sw.WriteLine();
                                    sw.WriteLine(SimpleJson2.SimpleJson2.SerializeObject(loots));
                                    sw.Flush();
                                    sw.Close();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Out(e.StackTrace);
                            }
                        }
                    }
                }
                return loots;
            }
            catch (Exception e)
            {
                Log.Out(e.StackTrace);
            }
            finally
            {
                //移除区块
                if (co != null)
                {
                    GameManager.Instance.RemoveChunkObserver(co);
                }
            }
            return loots;
        }
        #endregion

        #region 根据eos获取玩家领地内容器（只加载自己容器）
        /// <summary>
        /// 根据eos获取玩家领地内容器（只加载自己容器）
        /// </summary>
        /// <param name="eos"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> loadContainerListAroundJustSelf(string eos)
        {
            List<Dictionary<string, object>> loots = new List<Dictionary<string, object>>();
            ChunkManager.ChunkObserver co = null;
            //区块信息
            //获取玩家领地信息
            PersistentPlayerData ppdd = HioldsCommons.GetPersistentPlayerDataByEOS(eos);
            List<Vector3i> AllLppoition = new List<Vector3i>();
            if (ppdd != null && ppdd.LPBlocks != null)
            {
                AllLppoition.AddRange(loadPosSurround(ppdd.LPBlocks));
            }
            Log.Out("ACL数量：" + AllLppoition.Count);
            if (ppdd.ACL != null)
            {
                foreach (PlatformUserIdentifierAbs sts in ppdd.ACL)
                {
                    PersistentPlayerData tempPdd = HioldsCommons.GetPersistentPlayerDataBySteamId(sts);
                    if (tempPdd != null && tempPdd.LPBlocks != null)
                    {
                        AllLppoition.AddRange(loadPosSurround(tempPdd.LPBlocks));
                    }
                }
            }
            Log.Out("领地石个数：" + AllLppoition.Count);
            try
            {
                //寻找容器
                int landProtectSize = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize);
                //Log.Out(string.Format("领地保护范围:{0}", landProtectSize));
                DictionaryList<Vector3i, TileEntity> _tiles = new DictionaryList<Vector3i, TileEntity>();
                foreach (Vector3i lpbPos in AllLppoition)
                {
                    Chunk _c = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(lpbPos);

                    //判断区块是否已加载
                    int count = 0;
                    if (_c == null)
                    {
                        UnityEngine.Vector3 v3 = new UnityEngine.Vector3(lpbPos.x, lpbPos.y, lpbPos.z);
                        co = GameManager.Instance.AddChunkObserver(v3, false, 0, -1);
                        while (true)
                        {
                            _c = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(lpbPos);
                            if (_c != null || ++count >= 50)
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }


                    _tiles = _c.GetTileEntities();
                    foreach (TileEntity _tile in _tiles.dict.Values)
                    {
                        TileEntityType _type = _tile.GetTileEntityType();
                        if (_type.ToString().Contains("Loot"))
                        {
                            try
                            {
                                //有密码容器
                                TileEntityLootContainer SecureLoot = (TileEntityLootContainer)_tile;
                                Vector3i vec3i = SecureLoot.ToWorldPos();
                                SecureLoot.GetClrIdx();
                                //检测领地内箱子
                                //检测附近箱子数量
                                if (lpbPos.x - (landProtectSize / 2) < vec3i.x && lpbPos.x + (landProtectSize / 2) > vec3i.x)
                                {
                                    if (lpbPos.y - (landProtectSize / 2) < vec3i.y && lpbPos.y + (landProtectSize / 2) > vec3i.y)
                                    {
                                        if (lpbPos.z - (landProtectSize / 2) < vec3i.z && lpbPos.z + (landProtectSize / 2) > vec3i.z)
                                        {

                                            Dictionary<string, object> lt = new Dictionary<string, object>();
                                            lt.Add("name", SecureLoot.blockValue.ToItemValue().ItemClass.Name);
                                            lt.Add("icon", SecureLoot.blockValue.ToItemValue().ItemClass.CustomIcon);
                                            lt.Add("x", vec3i.x);
                                            lt.Add("y", vec3i.y);
                                            lt.Add("z", vec3i.z);
                                            lt.Add("clr", SecureLoot.GetClrIdx());
                                            try
                                            {
                                                ILockable lockable = (ILockable)_tile;
                                                //只加载自己的容器
                                                if (lockable.GetOwner().ReadablePlatformUserIdentifier.Equals(eos))
                                                {


                                                    //密码
                                                    if (lockable.HasPassword())
                                                    {
                                                        lt.Add("pw", "1");
                                                    }
                                                    else
                                                    {
                                                        lt.Add("pw", "0");
                                                    }
                                                    //锁定状态
                                                    if (lockable.IsLocked())
                                                    {
                                                        lt.Add("locked", "1");
                                                    }
                                                    else
                                                    {
                                                        lt.Add("locked", "0");
                                                    }
                                                    //容器所属
                                                    lt.Add("owner", lockable.GetOwner());
                                                    //加载Sign
                                                    try
                                                    {
                                                        ITileEntitySignable sign = (ITileEntitySignable)_tile;
                                                        lt.Add("sign", sign.GetText());
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        lt.Add("sign", "");
                                                    }
                                                    loots.Add(lt);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Out(e.StackTrace);
                            }
                        }
                    }
                }
                return loots;
            }
            catch (Exception e)
            {
                Log.Out(e.StackTrace);
            }
            finally
            {
                //移除区块
                if (co != null)
                {
                    GameManager.Instance.RemoveChunkObserver(co);
                }
            }
            return loots;
        }
        #endregion

        #region 加载指定位置的容器内物品数据
        /// <summary>
        /// 加载指定位置的容器内物品数据
        /// </summary>
        /// <param name="vec3i">容器位置坐标</param>
        /// <param name="_clridx">容器再区块内的顺序号</param>
        /// <param name="eos">用户id</param>
        /// <param name="password">访问密码</param>
        /// <returns></returns>
        public static ContainerInfo getContainerItems(Vector3i vec3i, int _clridx, string eos, string password)
        {
            //获取客户端信息
            ChunkManager.ChunkObserver co = null;
            //加载所有区块
            //获取玩家领地信息
            PersistentPlayerData ppdd = HioldsCommons.GetPersistentPlayerDataByEOS(eos);
            List<Vector3i> AllLppoition = new List<Vector3i>();
            AllLppoition.AddRange(loadPosSurround(ppdd.LPBlocks));
            if (ppdd.ACL != null)
            {
                foreach (PlatformUserIdentifierAbs sts in ppdd.ACL)
                {
                    PersistentPlayerData tempPdd = HioldsCommons.GetPersistentPlayerDataBySteamId(sts);
                    if (tempPdd != null && tempPdd.LPBlocks != null)
                    {
                        AllLppoition.AddRange(loadPosSurround(tempPdd.LPBlocks));
                    }
                }
            }
            //加载区块
            foreach (Vector3i lpbPos in AllLppoition)
            {
                Chunk _c = (Chunk)GameManager.Instance.World.GetChunkSync(lpbPos);
                //判断区块是否已加载
                int count = 0;
                if (_c == null)
                {
                    UnityEngine.Vector3 v3 = new UnityEngine.Vector3(lpbPos.x, lpbPos.y, lpbPos.z);
                    co = GameManager.Instance.AddChunkObserver(v3, false, 0, -1);
                    while (true)
                    {
                        _c = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(lpbPos);
                        if (_c != null || ++count >= 50)
                        {
                            break;
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }

            List<Dictionary<string, object>> containerItem = new List<Dictionary<string, object>>();
            TileEntityLootContainer SecureLoot = (TileEntityLootContainer)GameManager.Instance.World.GetTileEntity(_clridx, vec3i);

            if (SecureLoot != null)
            {
                //校验是否可以访问
                if (LockedEntity.ValidateLoot(SecureLoot))
                {
                    return new ContainerInfo
                    {
                        Code = 0,
                        Msg = "容器正在使用中，无法查看",
                        Data = containerItem
                    };
                }

                //校验密码
                try
                {
                    ILockable lockable = (ILockable)GameManager.Instance.World.GetTileEntity(_clridx, vec3i);
                    //是否为自己的箱子 自己的箱子不执行密码保护检测
                    if (!lockable.GetOwner().ReadablePlatformUserIdentifier.Equals(eos))
                    {
                        //是否密码保护
                        if (lockable.HasPassword())
                        {
                            //Log.Out("容器密码：" + lockable.GetPassword()+" 用户输入密码："+Utils.HashString(password));
                            if (!lockable.GetPassword().Equals(Utils.HashString(password)))
                            {
                                return new ContainerInfo
                                {
                                    Code = 0,
                                    Msg = "密码错误无法访问",
                                    Data = containerItem
                                };
                            }
                        }
                        else
                        {
                            //锁定状态
                            if (lockable.IsLocked())
                            {
                                return new ContainerInfo
                                {
                                    Code = 0,
                                    Msg = "你没有权限访问此容器",
                                    Data = containerItem
                                };
                            }
                        }
                    }


                }
                catch (Exception)
                {
                }

                ItemStack[] items = SecureLoot.items;
                for (int idx = 0; idx < SecureLoot.items.Length; idx++)
                {
                    ItemStack _item = items[idx];
                    if (_item != null && !_item.IsEmpty())
                    {
                        ItemDataSerializable _serializedItemStack = new ItemDataSerializable();
                        _serializedItemStack.name = "";
                        _serializedItemStack.steamid = eos;
                        if (_item.itemValue.ItemClass.CustomIcon != null)
                        {
                            _serializedItemStack.CustomIcon = _item.itemValue.ItemClass.CustomIcon.Value;
                        }
                        if (_item.itemValue.ItemClass.CustomIconTint != null)
                        {
                            var color = _item.itemValue.ItemClass.CustomIconTint;
                            _serializedItemStack.CustomIconTint = color.a + "," + color.r + "," + color.g + "," + color.b;

                        }
                        _serializedItemStack.itemCount = _item.count + "";
                        _serializedItemStack.itemName = _item.itemValue.ItemClass.GetItemName();
                        _serializedItemStack.translate = LocalizationUtils.getTranslate(_item.itemValue.ItemClass.GetItemName());
                        _serializedItemStack.itemUseTime = _item.itemValue.UseTimes + "";
                        _serializedItemStack.itemQuality = _item.itemValue.Quality + "";
                        _serializedItemStack.itemMaxUseTime = _item.itemValue.MaxUseTimes + "";
                        _serializedItemStack.desc = LocalizationUtils.getDesc(_item.itemValue.ItemClass.GetItemName()+ "Desc");
                        ItemStack[] wtItem = new ItemStack[1];
                        wtItem[0] = _item;
                        var itemString = JsonUtils.ByteStringFromItem(wtItem);
                        _serializedItemStack.itemData = itemString;
                        //物品内模组信息
                        List<string> modList = new List<string>();
                        foreach (ItemValue mod in _item.itemValue.Modifications)
                        {
                            if (mod.ItemClass != null)
                            {
                                modList.Add(mod.ItemClass.GetItemName());
                            }
                        }
                        //物品Groups信息
                        List<string> GroupList = new List<string>();
                        foreach (string group in _item.itemValue.ItemClass.Groups)
                        {
                            GroupList.Add(group);
                        }
                        _serializedItemStack.Groups = GroupList;
                        _serializedItemStack.Modifications = modList;
                        Dictionary<string, object> lt = new Dictionary<string, object>();
                        lt.Add("idx", idx);
                        lt.Add("itemStack", _serializedItemStack);
                        containerItem.Add(lt);
                    }
                }
                //读取完成卸载区块
                //移除区块
                if (co != null)
                {
                    GameManager.Instance.RemoveChunkObserver(co);
                }
            }
            else
            {
                return new ContainerInfo
                {
                    Code = 0,
                    Msg = "用户不在线或未找到容器",
                    Data = containerItem
                };
            }
            return new ContainerInfo
            {
                Code = 1,
                Msg = "获取成功",
                Data = containerItem
            };
        }
        #endregion

        #region 取走物品
        /// <summary>
        /// 取走物品
        /// </summary>
        /// <param name="vec3i">容器坐标</param>
        /// <param name="_clridx">容器顺序</param>
        /// <param name="eos">玩家eosid</param>
        /// <param name="itemidx">物品下表</param>
        /// <param name="itemdata">物品数据</param>
        /// <param name="itemcount">提取数量</param>
        /// <param name="pw">访问密码</param>
        /// <param name="price">价格</param>
        /// <returns></returns>
        public static ItemInfo TakeItem(Vector3i vec3i, int _clridx, string eos, int itemidx, string itemdata, int itemcount, string pw, string price)
        {

            /*解析参数*/
            ItemDataSerializable _serializedItemStack = new ItemDataSerializable();
            //获取客户端信息
            ChunkManager.ChunkObserver co = null;
            //加载所有区块
            //获取玩家领地信息
            try
            {
                PersistentPlayerData ppdd = HioldsCommons.GetPersistentPlayerDataByEOS(eos);
                List<Vector3i> AllLppoition = new List<Vector3i>();
                AllLppoition.AddRange(loadPosSurround(ppdd.LPBlocks));
                if (ppdd.ACL != null)
                {
                    foreach (PlatformUserIdentifierAbs sts in ppdd.ACL)
                    {
                        PersistentPlayerData tempPdd = HioldsCommons.GetPersistentPlayerDataBySteamId(sts);
                        if (tempPdd != null && tempPdd.LPBlocks != null)
                        {
                            AllLppoition.AddRange(loadPosSurround(tempPdd.LPBlocks));
                        }
                    }
                }
                //加载区块
                foreach (Vector3i lpbPos in AllLppoition)
                {
                    Chunk _c = (Chunk)GameManager.Instance.World.GetChunkSync(lpbPos);
                    //判断区块是否已加载
                    int count = 0;
                    if (_c == null)
                    {
                        UnityEngine.Vector3 v3 = new UnityEngine.Vector3(lpbPos.x, lpbPos.y, lpbPos.z);
                        co = GameManager.Instance.AddChunkObserver(v3, false, 0, -1);
                        while (true)
                        {
                            _c = (Chunk)GameManager.Instance.World.GetChunkFromWorldPos(lpbPos);
                            if (_c != null || ++count >= 50)
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }
                }

                //获取客户端信息
                TileEntityLootContainer SecureLoot = (TileEntityLootContainer)GameManager.Instance.World.GetTileEntity(_clridx, vec3i);
                if (SecureLoot != null)
                {

                    //校验是否可以访问
                    if (LockedEntity.ValidateLoot(SecureLoot))
                    {
                        return new ItemInfo
                        {
                            Code = 2,
                            Msg = "此容器正在使用中，无法访问",
                            Data = _serializedItemStack
                        };
                    }

                    //校验密码
                    try
                    {
                        ILockable lockable = (ILockable)GameManager.Instance.World.GetTileEntity(_clridx, vec3i);
                        //是否为自己的箱子 自己的箱子不执行密码保护检测
                        if (!lockable.GetOwner().ReadablePlatformUserIdentifier.Equals(eos))
                        {
                            //是否密码保护
                            if (lockable.HasPassword())
                            {
                                //Log.Out("容器密码：" + lockable.GetPassword()+" 用户输入密码："+Utils.HashString(password));
                                if (!lockable.GetPassword().Equals(Utils.HashString(pw)))
                                {
                                    return new ItemInfo
                                    {
                                        Code = 0,
                                        Msg = "密码错误无法访问",
                                        Data = _serializedItemStack
                                    };
                                }
                            }
                            else
                            {
                                //锁定状态
                                if (lockable.IsLocked())
                                {
                                    return new ItemInfo
                                    {
                                        Code = 0,
                                        Msg = "你没有权限访问此容器",
                                        Data = _serializedItemStack
                                    };
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    ItemStack[] items = SecureLoot.items;
                    ItemStack _item = items[itemidx];



                    //检查用户权限--白名单
                    //if (Auction.UserContorlMode.Equals("WhiteList"))
                    //{
                    //    if (Auction.UserContorlList.Contains(steamid))
                    //    {
                    //        Log.Out("玩家 {0} 拥有拍卖白名单,允许操作", steamid);
                    //    }
                    //    else
                    //    {
                    //        Log.Out("玩家 {0} 没有拍卖白名单,不允许操作", steamid);
                    //        return new ItemInfo
                    //        {
                    //            Code = 3,
                    //            Msg = "服务器启用了拍卖白名单,您不在白名单中,不允许拍卖",
                    //            Data = _serializedItemStack
                    //        };
                    //    }
                    //}



                    ////检查用户权限--黑名单
                    //if (Auction.UserContorlMode.Equals("BlackList"))
                    //{
                    //    if (Auction.UserContorlList.Contains(steamid))
                    //    {
                    //        Log.Out("玩家 {0} 在黑名单中,不允许操作", steamid);
                    //        return new ItemInfo
                    //        {
                    //            Code = 3,
                    //            Msg = "服务器启用了拍卖黑名单,您在黑名单列表中,不允许拍卖",
                    //            Data = _serializedItemStack
                    //        };
                    //    }
                    //    else
                    //    {
                    //        Log.Out("玩家 {0} 不在黑名单中,允许操作", steamid);
                    //    }
                    //}


                    ////检查物品--白名单
                    //if (Auction.ItemContorlMode.Equals("WhiteList"))
                    //{
                    //    if (Auction.ItemContorlList.Contains(_item.itemValue.ItemClass.GetItemName()))
                    //    {
                    //        Log.Out("玩家 {0} 拍卖物品 {1},物品为白名单,允许操作", steamid, _item.itemValue.ItemClass.GetItemName());
                    //    }
                    //    else
                    //    {
                    //        Log.Out("玩家 {0} 拍卖物品 {1},物品非白名单,不用允许操作", steamid, _item.itemValue.ItemClass.GetItemName());
                    //        return new ItemInfo
                    //        {
                    //            Code = 3,
                    //            Msg = "服务器启用了拍卖物品白名单,物品" + _item.itemValue.ItemClass.GetItemName() + "不在白名单中,不允许拍卖",
                    //            Data = _serializedItemStack
                    //        };
                    //    }

                    //}



                    ////检查物品--黑名单
                    //if (Auction.ItemContorlMode.Equals("BlackList"))
                    //{
                    //    if (Auction.ItemContorlList.Contains(_item.itemValue.ItemClass.GetItemName()))
                    //    {
                    //        Log.Out("玩家 {0} 拍卖物品 {1},物品为黑名单,不允许操作", steamid, _item.itemValue.ItemClass.GetItemName());
                    //        return new ItemInfo
                    //        {
                    //            Code = 3,
                    //            Msg = "服务器启用了拍卖物品黑名单,物品" + _item.itemValue.ItemClass.GetItemName() + "在黑名单中,不允许拍卖",
                    //            Data = _serializedItemStack
                    //        };
                    //    }
                    //    else
                    //    {
                    //        Log.Out("玩家 {0} 拍卖物品 {1},物品非黑名单,允许操作", steamid, _item.itemValue.ItemClass.GetItemName());
                    //    }
                    //}

                    CheckResult userResult = UserAndItemCheck.CheckUser(eos);
                    CheckResult itemResult = UserAndItemCheck.CheckItem(_item.itemValue.ItemClass.GetItemName());

                    if (!userResult.validate)
                    {
                        return new ItemInfo
                        {
                            Code = 3,
                            Msg = userResult.msg,
                            Data = _serializedItemStack
                        };
                    }

                    if (!itemResult.validate)
                    {
                        return new ItemInfo
                        {
                            Code = 3,
                            Msg = userResult.msg,
                            Data = _serializedItemStack
                        };
                    }




                    if (price != null && price != "")
                    {
                        if (int.TryParse(price, out int intprice))
                        {
                            //检查物品价格限制
                            try
                            {

                                CheckResult priceResult = UserAndItemCheck.CheckItemPrice(_item.itemValue.ItemClass.GetItemName(), intprice);

                                if (!priceResult.validate)
                                {
                                    return new ItemInfo
                                    {
                                        Code = 3,
                                        Msg = priceResult.msg,
                                        Data = _serializedItemStack
                                    };
                                }

                                //ItemPriceLimit plimit = Auction.priceLimit[_item.itemValue.ItemClass.GetItemName()];
                                //if (intprice < plimit.min)
                                //{
                                //    Log.Out("玩家 {0} 拍卖物品 {1},价格小于最低价 {2},拍卖失败", steamid, _item.itemValue.ItemClass.GetItemName(), plimit.min);
                                //    return new ItemInfo
                                //    {
                                //        Code = 3,
                                //        Msg = "您输入的价格太低了，请提高价格(此物品最低价:" + plimit.min + ")",
                                //        Data = _serializedItemStack
                                //    };
                                //}





                                //else if (intprice > plimit.max)
                                //{
                                //    Log.Out("玩家 {0} 拍卖物品 {1},价格高于最高价 {2},拍卖失败", steamid, _item.itemValue.ItemClass.GetItemName(), plimit.max);
                                //    return new ItemInfo
                                //    {
                                //        Code = 3,
                                //        Msg = "您输入的价格太高了，请降低价格(此物品最高价:" + plimit.max + ")",
                                //        Data = _serializedItemStack
                                //    };
                                //}
                            }
                            catch (Exception e)
                            {
                                Log.Error(e.Message);
                                Log.Out(string.Format("未配置测物品价格限制不做处理"));
                            }
                        }
                    }


                    if (_item != null && !_item.IsEmpty())
                    {
                        //移除整个Stack
                        if (itemcount < 0 || _item.count == itemcount)
                        {
                            ItemStack[] wtItem = new ItemStack[1];
                            wtItem[0] = _item;
                            var itemString = JsonUtils.ByteStringFromItem(wtItem);
                            //物品已发生变动，操作失败
                            if (!itemdata.Equals(itemString))
                            {
                                return new ItemInfo
                                {
                                    Code = 3,
                                    Msg = "检测到容器中物品已被修改，无法访问，请刷新后重试",
                                    Data = _serializedItemStack
                                };
                            }
                            _serializedItemStack.name = "";
                            _serializedItemStack.steamid = eos;
                            if (_item.itemValue.ItemClass.CustomIcon != null)
                            {
                                _serializedItemStack.CustomIcon = _item.itemValue.ItemClass.CustomIcon.Value;
                            }
                            if (_item.itemValue.ItemClass.CustomIconTint != null)
                            {
                                var color = _item.itemValue.ItemClass.CustomIconTint;
                                _serializedItemStack.CustomIconTint = color.a + "," + color.r + "," + color.g + "," + color.b;

                            }
                            _serializedItemStack.itemCount = _item.count + "";
                            _serializedItemStack.itemName = _item.itemValue.ItemClass.GetItemName();
                            _serializedItemStack.itemUseTime = _item.itemValue.UseTimes + "";
                            _serializedItemStack.itemQuality = _item.itemValue.Quality + "";
                            _serializedItemStack.itemMaxUseTime = _item.itemValue.MaxUseTimes + "";
                            _serializedItemStack.itemData = itemString;
                            _serializedItemStack.desc = LocalizationUtils.getDesc(_item.itemValue.ItemClass.GetItemName() + "Desc");
                            //物品内模组信息
                            List<string> modList = new List<string>();
                            foreach (ItemValue mod in _item.itemValue.Modifications)
                            {
                                if (mod.ItemClass != null)
                                {
                                    modList.Add(mod.ItemClass.GetItemName());
                                }
                            }
                            //物品Groups信息
                            List<string> GroupList = new List<string>();
                            foreach (string group in _item.itemValue.ItemClass.Groups)
                            {
                                GroupList.Add(group);
                            }
                            _serializedItemStack.Groups = GroupList;
                            _serializedItemStack.Modifications = modList;
                            /*解析参数结束*/

                            //检测通过物品信息一致，删除物品
                            items[itemidx] = ItemStack.Empty.Clone();
                            SecureLoot.SetModified();
                            return new ItemInfo
                            {
                                Code = 1,
                                Msg = "操作成功",
                                Data = _serializedItemStack
                            };

                        }
                        else
                        {
                            if (_item.count < itemcount)
                            {
                                //数量不足
                                return new ItemInfo
                                {
                                    Code = 6,
                                    Msg = "物品数量不足",
                                    Data = _serializedItemStack
                                };
                            }
                            ItemStack[] wtItem = new ItemStack[1];
                            wtItem[0] = _item;
                            var itemString = JsonUtils.ByteStringFromItem(wtItem);
                            //物品已发生变动，操作失败
                            if (!itemdata.Equals(itemString))
                            {
                                return new ItemInfo
                                {
                                    Code = 3,
                                    Msg = "检测到容器中物品已被修改，无法访问，请刷新后重试",
                                    Data = _serializedItemStack
                                };
                            }
                            _serializedItemStack.name = "";
                            _serializedItemStack.steamid = eos;
                            if (_item.itemValue.ItemClass.CustomIcon != null)
                            {
                                _serializedItemStack.CustomIcon = _item.itemValue.ItemClass.CustomIcon.Value;
                            }
                            if (_item.itemValue.ItemClass.CustomIconTint != null)
                            {
                                var color = _item.itemValue.ItemClass.CustomIconTint;
                                _serializedItemStack.CustomIconTint = color.a + "," + color.r + "," + color.g + "," + color.b;

                            }
                            _serializedItemStack.itemCount = _item.count + "";
                            _serializedItemStack.itemName = _item.itemValue.ItemClass.GetItemName();
                            _serializedItemStack.itemUseTime = _item.itemValue.UseTimes + "";
                            _serializedItemStack.itemQuality = _item.itemValue.Quality + "";
                            _serializedItemStack.itemMaxUseTime = _item.itemValue.MaxUseTimes + "";
                            _serializedItemStack.itemData = itemString;
                            _serializedItemStack.desc = LocalizationUtils.getDesc(_item.itemValue.ItemClass.GetItemName() + "Desc");
                            //物品内模组信息
                            List<string> modList = new List<string>();
                            foreach (ItemValue mod in _item.itemValue.Modifications)
                            {
                                if (mod.ItemClass != null)
                                {
                                    modList.Add(mod.ItemClass.GetItemName());
                                }
                            }
                            //物品Groups信息
                            List<string> GroupList = new List<string>();
                            foreach (string group in _item.itemValue.ItemClass.Groups)
                            {
                                GroupList.Add(group);
                            }
                            _serializedItemStack.Groups = GroupList;
                            _serializedItemStack.Modifications = modList;
                            /*解析参数结束*/

                            _item.count = _item.count - itemcount;
                            items[itemidx] = _item;
                            SecureLoot.SetModified();
                            return new ItemInfo
                            {
                                Code = 1,
                                Msg = "操作成功",
                                Data = _serializedItemStack
                            };
                        }
                    }
                    else
                    {
                        return new ItemInfo
                        {
                            Code = 5,
                            Msg = "物品数据为空，无法继续操作",
                            Data = _serializedItemStack
                        };
                    }
                }
                else
                {
                    return new ItemInfo
                    {
                        Code = 4,
                        Msg = "玩家不在线或者无法获取到容器信息",
                        Data = _serializedItemStack
                    };
                }
            }
            catch (Exception e)
            {
                Log.Out(e.StackTrace);
            }
            finally
            {
                //移除区块
                if (co != null)
                {
                    GameManager.Instance.RemoveChunkObserver(co);
                }
            }
            return null;
        }
        #endregion

        #region 坐标计算
        /// <summary>
        /// 计算附近容器坐标
        /// </summary>
        /// <param name="basePosList"></param>
        /// <returns></returns>
        public static List<Vector3i> loadPosSurround(List<Vector3i> basePosList)
        {
            List<Vector3i> result = new List<Vector3i>();
            foreach (Vector3i basePos in basePosList)
            {
                result.Add(new Vector3i(basePos.x + 16, basePos.y, basePos.z + 16));
                result.Add(new Vector3i(basePos.x + 16, basePos.y, basePos.z - 16));
                result.Add(new Vector3i(basePos.x + 16, basePos.y, basePos.z));
                result.Add(new Vector3i(basePos.x - 16, basePos.y, basePos.z + 16));
                result.Add(new Vector3i(basePos.x - 16, basePos.y, basePos.z - 16));
                result.Add(new Vector3i(basePos.x - 16, basePos.y, basePos.z));
                result.Add(new Vector3i(basePos.x, basePos.y, basePos.z + 16));
                result.Add(new Vector3i(basePos.x, basePos.y, basePos.z - 16));
                result.Add(new Vector3i(basePos.x, basePos.y, basePos.z));
            }
            return result;
        }
        #endregion
    }
    public class ContainerInfo
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
    }

    public class ItemInfo
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public ItemDataSerializable Data { get; set; }
    }



}
