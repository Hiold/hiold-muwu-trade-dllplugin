﻿using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.ChunckLoader;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    public class GameChunckContainerAction
    {
        public static void loadContainerListAround(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                List<Dictionary<string, object>> result = ChunckLoader.ChunkLoader.loadContainerListAround(request.user.platformid);
                if (result != null && result.Count > 0)
                {
                    ResponseUtils.ResponseSuccessWithData(response, result);
                    return;
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "账号或密码错误");
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
        /// 获取容器内物品
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void getContainerItems(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("x", out string x);
                queryRequest.TryGetValue("y", out string y);
                queryRequest.TryGetValue("z", out string z);
                //
                queryRequest.TryGetValue("clridx", out string clridx);
                queryRequest.TryGetValue("password", out string password);

                float fx = float.Parse(x);
                float fy = float.Parse(y);
                float fz = float.Parse(z);

                ContainerInfo ci = ChunckLoader.ChunkLoader.getContainerItems(new Vector3i(fx, fy, fz), int.Parse(clridx), request.user.platformid, password);
                if (ci.Code == 0)
                {
                    ResponseUtils.ResponseFail(response, ci.Msg);
                }
                else
                {
                    ResponseUtils.ResponseSuccessWithData(response, ci.Data);
                }


            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.ToString());
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        /// <summary>
        /// 取走物品
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void TakeItem(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("x", out string x);
                queryRequest.TryGetValue("y", out string y);
                queryRequest.TryGetValue("z", out string z);
                //
                queryRequest.TryGetValue("clridx", out string clridx);
                queryRequest.TryGetValue("password", out string password);

                //取走物品的参数
                queryRequest.TryGetValue("itemidx", out string itemidx);
                queryRequest.TryGetValue("itemdata", out string itemdata);
                queryRequest.TryGetValue("itemcount", out string itemcount);
                queryRequest.TryGetValue("price", out string price);


                float fx = float.Parse(x);
                float fy = float.Parse(y);
                float fz = float.Parse(z);

                ItemInfo ii = ChunckLoader.ChunkLoader.TakeItem(new Vector3i(fx, fy, fz), int.Parse(clridx), request.user.platformid, int.Parse(itemidx), itemdata, int.Parse(itemcount), password, price);

                //保存玩家物品到交易系统内
                UserStorage userStorate = new UserStorage()
                {
                    //id
                    itemtype = "1",
                    name = ii.Data.itemName,
                    translate = ii.Data.translate,
                    itemicon = (ii.Data.CustomIcon == null ? ii.Data.itemName : ii.Data.CustomIcon) + ".png",
                    itemtint = ii.Data.CustomIconTint == null ? "" : ii.Data.CustomIconTint,
                    quality = parseInt(ii.Data.itemQuality),
                    num = 0,
                    class1 = parseGroup(ii.Data.Groups),
                    class2 = parseGroup(ii.Data.Groups),
                    classmod = "",
                    desc = "",
                    couCurrType = "",
                    couPrice = "",
                    couCond = "",
                    coudatelimit = "",
                    couDateStart = DateTime.MinValue,
                    couDateEnd = DateTime.MinValue,
                    count = "",
                    currency = "",
                    price = 0,
                    discount = 0,
                    prefer = 0,
                    selltype = 0,
                    hot = "",
                    hotset = 0,
                    show = "",
                    stock = 0,
                    collect = 0,
                    selloutcount = "",
                    follow = "",
                    xglevel = "",
                    xglevelset = "",
                    xgday = "",
                    xgdayset = "",
                    xgall = "",
                    xgallset = "",
                    xgdatelimit = "",
                    dateStart = DateTime.MinValue,
                    dateEnd = DateTime.MinValue,
                    collected = "",
                    postTime = DateTime.MinValue,
                    deleteTime = DateTime.MinValue,
                    //非继承属性
                    username = request.user.name,
                    platformid = request.user.platformid,
                    gameentityid = request.user.gameentityid,
                    collectTime = DateTime.Now,
                    storageCount = int.Parse(itemcount),
                    itemGetChenal = UserStorageGetChanel.GAME_STORAGE,
                    itemStatus = UserStorageStatus.NORMAL_STORAGED,
                    //拓展属性
                    extinfo1 = "",
                    extinfo2 = "",
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    itemdata = ii.Data.itemData,
                };



                //添加数据到数据库
                UserStorageService.addUserStorage(userStorate);




                if (ii.Code == 0)
                {
                    ResponseUtils.ResponseFail(response, ii.Msg);
                }
                else
                {
                    ResponseUtils.ResponseSuccessWithData(response, ii.Data);
                }


            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.ToString());
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }
        public static int parseInt(string str)
        {
            if (int.TryParse(str, out int result))
            {
                return result;
            }
            return 0;
        }

        public static string parseGroup(List<string> groups)
        {
            string result = "";
            if (groups != null && groups.Count > 0)
            {
                foreach (string temp in groups)
                {
                    result += temp + "|";
                }
                if (result.Length > 0)
                {
                    result = result.Substring(0, result.Length - 1);
                }
            }
            return result;
        }
    }
}
