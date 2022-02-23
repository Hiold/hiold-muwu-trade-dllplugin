﻿using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
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
                if (API.isOnServer)
                {
                    basepath = string.Format("{0}/image/", API.AssemblyPath);
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
                    UserService.UpdateUserInfo(us);
                }
                else
                {
                    UserInfo us = UserService.getUserById(request.user.id + "")[0];
                    us.qq = qq;
                    us.shopname = shopname;
                    UserService.UpdateUserInfo(us);
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
