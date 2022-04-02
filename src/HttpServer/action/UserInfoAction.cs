using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.Commons;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.service;
using HioldMod.src.UserTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class UserInfoAction
    {

        /// <summary>
        /// 根据玩家gameentityid获取玩家数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void getUserInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                List<UserInfo> rest = UserService.getUserBySteamid(id);
                if (rest != null && rest.Count > 0)
                {
                    foreach (UserInfo infoTemp in rest)
                    {
                        infoTemp.password = "[masked]";
                    }

                    ResponseUtils.ResponseSuccessWithData(response, rest[0]);
                    return;
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "未找到用户信息");
                    return;
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        /// <summary>
        /// 获取用户折扣券
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getdisCountTicket(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //string postData = ServerUtils.getPostData(request.request);
                ////Dictionary<string, string> param = ServerUtils.GetParam(request);
                //info _info = new info();
                //_info = (info)SimpleJson2.SimpleJson2.DeserializeObject(postData, _info.GetType());
                List<UserStorage> cous = UserStorageService.selectPlayersCou(request.user.gameentityid);
                //List<UserInfo> resultList = UserService.userLogin(_info.username, ServerUtils.md5(_info.password));

                ResponseUtils.ResponseSuccessWithData(response, cous);
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        /// <summary>
        /// 获取用户库存
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getPlayerStorage(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                string itemname = "";
                int pageIndex = 1;
                int pageSize = 10;
                queryRequest.TryGetValue("itemname", out itemname);
                if (queryRequest.TryGetValue("pageIndex", out string pageIndexStr))
                {
                    pageIndex = int.Parse(pageIndexStr);
                }
                if (queryRequest.TryGetValue("pageSize", out string pageSizeStr))
                {
                    pageSize = int.Parse(pageSizeStr);
                }
                queryRequest.TryGetValue("class1", out string class1);
                queryRequest.TryGetValue("class2", out string class2);



                Dictionary<string, object> items = UserStorageService.selectPlayersStorage(request.user.gameentityid, itemname, pageIndex, pageSize, class1, class2);
                //List<UserInfo> resultList = UserService.userLogin(_info.username, ServerUtils.md5(_info.password));

                ResponseUtils.ResponseSuccessWithData(response, items);
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }



        /// <summary>
        /// 获取用户在售物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getPlayerOnSell(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                string itemname = "";
                int pageIndex = 1;
                int pageSize = 10;
                queryRequest.TryGetValue("itemname", out itemname);
                if (queryRequest.TryGetValue("pageIndex", out string pageIndexStr))
                {
                    pageIndex = int.Parse(pageIndexStr);
                }
                if (queryRequest.TryGetValue("pageSize", out string pageSizeStr))
                {
                    pageSize = int.Parse(pageSizeStr);
                }
                queryRequest.TryGetValue("class1", out string class1);
                queryRequest.TryGetValue("class2", out string class2);
                queryRequest.TryGetValue("id", out string id);



                Dictionary<string, object> items = UserTradeService.selectPlayersOnSell(id, itemname, pageIndex, pageSize, class1, class2);
                //List<UserInfo> resultList = UserService.userLogin(_info.username, ServerUtils.md5(_info.password));

                ResponseUtils.ResponseSuccessWithData(response, items);
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }



        /// <summary>
        /// 获取用户库存
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void dispachItemToGame(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                int count = 0;
                if (queryRequest.TryGetValue("count", out string countStr))
                {
                    count = int.Parse(countStr);
                }
                queryRequest.TryGetValue("id", out string id);
                UserStorage us = UserStorageService.selectUserStorageByid(id);

                //获取客户端信息
                ClientInfo _cInfo = HioldsCommons.GetClientInfoByEOSorSteamid(us.gameentityid);

                if (_cInfo == null)
                {
                    Log.Out("玩家 " + us.gameentityid + " 不在线");
                    ResponseUtils.ResponseFail(response, "提取失败,玩家不在线");
                    return;
                }

                EntityPlayer _entity = (EntityPlayer)GameManager.Instance.World.GetEntity(_cInfo.entityId);
                if (_entity == null || !_entity.IsSpawned())
                {
                    Log.Out("玩家 " + us.gameentityid + " 正在加载");
                    ResponseUtils.ResponseFail(response, "提取失败，玩家正在加载，请耐心等待进入游戏再操作");
                    return;
                }

                //if (!Auction.IsEnable)
                //{
                //    Log.Out("玩家 {0} 提取物品，服务器未开启拍卖系统，无法发放", _cInfo.playerId);
                //    _cInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, "[FF0000]服务器未开启拍卖系统，物品发放失败！", "[87CEFA]交易系统", false, null));
                //    ResponseUtils.ResponseFail(response, "未开启拍卖系统");
                //    return ;
                //}


                //检查物品属性
                if (!request.user.gameentityid.Equals(us.gameentityid))
                {
                    ResponseUtils.ResponseFail(response, "非个人物品，发放失败");
                    return;
                }
                if (count <= 0)
                {
                    ResponseUtils.ResponseFail(response, "数量填写异常，发放失败");
                    return;
                }
                if (us.storageCount <= 0 && us.storageCount < count)
                {
                    ResponseUtils.ResponseFail(response, "库存不足，发放失败，当前剩余" + us.storageCount);
                    return;
                }
                Console.WriteLine(us.itemStatus);
                if (us.itemStatus != 1 && us.itemStatus != 2)
                {
                    ResponseUtils.ResponseFail(response, "该物品无法领取，发放失败");
                    return;
                }
                if (!us.itemtype.Equals("1"))
                {
                    ResponseUtils.ResponseFail(response, "该物品类型无法领取，发放失败");
                    return;
                }

                //检查完毕开始添加数据到发放队列
                if (us.itemdata != null && !us.itemdata.Equals(""))
                {
                    //加入发放队列
                    DeliverItemTools.deliverDataItemQueue.Enqueue(new DeliverItemWithData()
                    {
                        steamid = us.gameentityid,
                        data = us.itemdata,
                        count = (count * us.num) + ""
                    });
                }
                else
                {
                    //加入发放队列
                    DeliverItemTools.deliverQueue.Enqueue(new DeliverItem
                    {
                        steamid = us.gameentityid,
                        itemName = us.name,
                        count = count * us.num,
                        itemquality = us.quality
                    }); ;
                }

                //
                if (us.storageCount > count)
                {
                    us.storageCount -= count;
                    us.itemStatus = UserStorageStatus.DISPACHED_APART;
                }
                else
                {
                    us.storageCount = 0;
                    us.itemStatus = UserStorageStatus.DISPACHED;
                }

                //发放完成 更新数据
                us.obtainTime = DateTime.Now;
                UserStorageService.UpdateUserStorage(us);


                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.dispachToGame,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(us),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("提取了{0}个{1}到游戏中", (count * us.num), us.translate)
                });

                ResponseUtils.ResponseSuccessWithData(response, "成功发放" + count * us.num + "个物品");
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        /// <summary>
        /// 获取用户库存
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void deleteItem(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                int count = 0;
                if (queryRequest.TryGetValue("count", out string countStr))
                {
                    count = int.Parse(countStr);
                }
                queryRequest.TryGetValue("id", out string id);
                UserStorage us = UserStorageService.selectUserStorageByid(id);
                //检查物品属性
                //检查物品属性
                if (!request.user.gameentityid.Equals(us.gameentityid))
                {
                    ResponseUtils.ResponseFail(response, "非个人物品，删除失败");
                    return;
                }
                if (us.storageCount < count || count <= 0)
                {
                    ResponseUtils.ResponseFail(response, "删除失败，数量异常");
                    return;
                }
                if (us.storageCount == count)
                {
                    us.storageCount = 0;
                    us.itemStatus = UserStorageStatus.USERDELETED;
                }
                else
                {
                    us.storageCount -= count;
                }


                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.deleteItem,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(us),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("丢弃了{0}个{1}（此操作不可逆无补偿）", count, us.translate)
                });

                UserStorageService.UpdateUserStorage(us);
                ResponseUtils.ResponseSuccessWithData(response, "删除成功!");
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }



        /// <summary>
        /// 用户登录action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getItemBuyLimit(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                limitinfo _limit = new limitinfo();
                _limit = (limitinfo)SimpleJson2.SimpleJson2.DeserializeObject(postData, _limit.GetType());
                //获取当日购买数量
                string tdStart = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                string tdEnd = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                Int64 tdCount = ActionLogService.QueryItemLogCount(request.user.gameentityid, _limit.id, LogType.BuyItem, tdStart, tdEnd);
                Int64 allCount = ActionLogService.QueryItemLogCount(request.user.gameentityid, _limit.id, LogType.BuyItem, null, null);
                limitCountInfo lci = new limitCountInfo()
                {
                    tdCount = tdCount,
                    allCount = allCount,
                };

                ResponseUtils.ResponseSuccessWithData(response, lci);
                return;

            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void debug(HttpListenerRequest request, HttpListenerResponse response)
        {
            FileStream fs = new FileStream("D:/test.txt", FileMode.OpenOrCreate);
            byte[] data = Encoding.UTF8.GetBytes(SimpleJson2.SimpleJson2.SerializeObject(request.Headers));
            fs.Write(data, 0, data.Length);


            fs.Flush();
            fs.Close();
            response.StatusCode = 200;
            response.Close();
        }



        /// <summary>
        /// 用户登录action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void updateCollect(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("value", out string value);
                //获取配置
                List<UserConfig> cfgs = UserConfigService.QueryConfig(request.user.gameentityid, ConfigType.Collect, id);
                if (cfgs != null && cfgs.Count > 0)
                {
                    UserConfig cfg = cfgs[0];
                    cfg.available = value;
                    cfg.updated_at = DateTime.Now;
                    UserConfigService.updateConfig(cfg);

                    //记录日志数据
                    if (value.Equals("1"))
                    {
                        ActionLogService.addLog(new ActionLog()
                        {
                            actTime = DateTime.Now,
                            actType = LogType.collect,
                            atcPlayerEntityId = request.user.gameentityid,
                            extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                            desc = string.Format("添加了新的收藏物品")
                        });

                        ShopTradeService.updateCollectAdd(id);
                    }
                    else
                    {
                        ActionLogService.addLog(new ActionLog()
                        {
                            actTime = DateTime.Now,
                            actType = LogType.discollect,
                            atcPlayerEntityId = request.user.gameentityid,
                            extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                            desc = string.Format("删除了收藏物品")
                        });
                        ShopTradeService.updateCollectSub(id);
                    }
                    ResponseUtils.ResponseSuccessWithData(response, cfg);
                    return;
                }
                else
                {
                    UserConfig cfg = new UserConfig()
                    {
                        created_at = DateTime.Now,
                        name = request.user.name,
                        gameentityid = request.user.gameentityid,
                        platformid = request.user.platformid,
                        configType = ConfigType.Collect,
                        configValue = id,
                        available = "1"
                    };

                    //记录日志数据
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.collect,
                        atcPlayerEntityId = request.user.gameentityid,
                        extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                        desc = string.Format("添加了新的收藏物品")
                    });

                    ShopTradeService.updateCollectAdd(id);

                    UserConfigService.addConfig(cfg);
                    ResponseUtils.ResponseSuccessWithData(response, cfg);
                    return;
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public static void getUserShopList(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("name", out string name);
                queryRequest.TryGetValue("orderby", out string orderby);
                List<UserInfo> rest = UserService.getUserShopList(name, orderby);
                foreach (UserInfo infoTemp in rest)
                {
                    infoTemp.password = "[masked]";
                }

                ResponseUtils.ResponseSuccessWithData(response, rest);
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }



        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void updateUserInfo(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/image/";
                if (HioldMod.API.isOnServer)
                {
                    basepath = string.Format("{0}/image/", HioldMod.API.AssemblyPath);
                }

                //检查路径
                if (!Directory.Exists(basepath))
                {
                    Directory.CreateDirectory(basepath);
                }

                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("shopname", out string shopname);
                queryRequest.TryGetValue("qq", out string qq);
                queryRequest.TryGetValue("avatar", out string avatar);
                //校验关键参数
                if (string.IsNullOrWhiteSpace(shopname))
                {
                    ResponseUtils.ResponseFail(response, "正输入正确的商店名");
                    return;
                }
                if (string.IsNullOrWhiteSpace(qq))
                {
                    ResponseUtils.ResponseFail(response, "正输入正确的QQ号");
                    return;
                }


                //写入文件
                if (!string.IsNullOrEmpty(avatar))
                {
                    string extName = "";
                    if (avatar.Contains("image/png"))
                    {
                        extName = "png";
                    }
                    else if (avatar.Contains("image/jpg"))
                    {
                        extName = "jpg";
                    }
                    else if (avatar.Contains("image/jpeg"))
                    {
                        extName = "jpeg";
                    }
                    else if (avatar.Contains("image/jfif"))
                    {
                        extName = "jfif";
                    }
                    else if (avatar.Contains("image/gif"))
                    {
                        extName = "gif";
                    }
                    else
                    {
                        ResponseUtils.ResponseFail(response, "文件校验错误，请重新更新信息");
                        return;
                    }
                    byte[] arr = Convert.FromBase64String(avatar.Substring(avatar.IndexOf(",") + 1));
                    string fileName = request.user.gameentityid + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + extName;
                    using (Stream stream = new MemoryStream(arr))
                    {
                        //判断文件是否存在

                        FileStream fs = new FileStream(basepath + fileName, FileMode.Create);
                        fs.Write(arr, 0, arr.Length);
                        fs.Flush();
                        fs.Close();
                    }
                    UserInfo us = UserService.getUserById(request.user.id + "")[0];
                    us.avatar = fileName;
                    us.qq = qq;
                    us.shopname = shopname;
                    //生成字符串前清除过长的图片信息
                    queryRequest["avatar"] = "图片：" + basepath + fileName;
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.updateUserinfo,
                        atcPlayerEntityId = request.user.gameentityid,
                        extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                        extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(us),
                        desc = string.Format("更新了基础信息(含头像)")
                    });
                    UserService.UpdateUserInfo(us);
                }
                else
                {
                    UserInfo us = UserService.getUserById(request.user.id + "")[0];
                    us.qq = qq;
                    us.shopname = shopname;
                    UserService.UpdateUserInfo(us);

                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.updateUserinfo,
                        atcPlayerEntityId = request.user.gameentityid,
                        extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                        extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(us),
                        desc = string.Format("更新了基础信息(不包含头像)")
                    });
                }
                ResponseUtils.ResponseSuccessWithData(response, "更新成功");
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        /// <summary>
        /// 根据玩家gameentityid获取玩家数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void getLogs(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("type", out string type);
                int pageindex = 1;
                int pagesize = 10;
                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("size", out string size);
                int.TryParse(page, out pageindex);
                int.TryParse(size, out pagesize);
                Dictionary<string, object> als = ActionLogService.QueryLogs(request.user.gameentityid, type, pageindex, pagesize);

                if (als != null && als.Count > 0)
                {
                    ResponseUtils.ResponseSuccessWithData(response, als);
                    return;
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "未找到操作日志");
                    return;
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }



        /// <summary>
        /// 根据玩家gameentityid获取玩家数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void getCollectItems(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));

                List<UserConfig> ucs = UserConfigService.QueryUserCollect(request.user.gameentityid, ConfigType.Collect);

                if (ucs != null && ucs.Count > 0)
                {
                    List<TradeManageItem> result = new List<TradeManageItem>();
                    foreach (UserConfig tp in ucs)
                    {
                        List<TradeManageItem> its = ShopTradeService.getShopItemById(int.Parse(tp.configValue));
                        if (its != null && its.Count > 0)
                        {
                            result.AddRange(its);
                        }
                    }
                    ResponseUtils.ResponseSuccessWithData(response, result);
                    return;
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "未找到操作日志");
                    return;
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }



        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void like(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("steamid", out string steamid);
                List<UserInfo> rest = UserService.getUserBySteamid(steamid);
                if (rest != null && rest.Count > 0)
                {
                    long count = ActionLogService.QueryLikeCount(request.user.gameentityid, steamid, DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                    if (count <= 0)
                    {
                        //记录日志数据
                        ActionLogService.addLog(new ActionLog()
                        {
                            actTime = DateTime.Now,
                            actType = LogType.Like,
                            atcPlayerEntityId = request.user.gameentityid,
                            extinfo1 = steamid,
                            desc = string.Format("为 " + rest[0].name + " 的店铺点赞")
                        });
                        UserInfo ui = rest[0];
                        ui.likecount = ui.likecount + 1;
                        UserService.UpdateUserInfo(ui);
                        ResponseUtils.ResponseSuccess(response);
                        return;
                    }
                    else
                    {
                        ResponseUtils.ResponseFail(response, "今天已经点赞过了，请明天在为ta点赞");
                        return;
                    }
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "未找到用户信息");
                    return;
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        public static void likecount(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("steamid", out string steamid);
                long count = ActionLogService.QueryLikeCount(request.user.gameentityid, steamid, DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                ResponseUtils.ResponseSuccessWithData(response, count);
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public class info
        {
            public string userid { get; set; }
        }

        public class limitinfo
        {
            public string id { get; set; }
        }

        public class limitCountInfo
        {
            public Int64 tdCount { get; set; }
            public Int64 allCount { get; set; }
        }
    }
}
