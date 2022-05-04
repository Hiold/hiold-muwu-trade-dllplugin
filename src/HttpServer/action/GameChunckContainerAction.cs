using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.ChunckLoader;
using HioldMod.src.HttpServer.attributes;
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

    [ActionAttribute]
    public class GameChunckContainerAction
    {
        /// <summary>
        /// 加载容器
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/loadContainerListAround")]
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
                    ResponseUtils.ResponseFail(response, "没有发现您和队友的领地,或者领地中无有效容器");
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
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/getContainerItems")]
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
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/TakeItem")]
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
                if (ii.Code == 1)
                {
                    UserStorage userStorate = new UserStorage()
                    {
                        //id
                        itemtype = "1",
                        name = ii.Data.itemName,
                        translate = ii.Data.translate == null ? ii.Data.itemName : ii.Data.translate,
                        itemicon = (ii.Data.CustomIcon == null ? ii.Data.itemName : ii.Data.CustomIcon) + ".png",
                        itemtint = ii.Data.CustomIconTint == null ? "" : ii.Data.CustomIconTint,
                        quality = parseInt(ii.Data.itemQuality),
                        num = 1,
                        class1 = parseGroup(ii.Data.Groups),
                        class2 = parseGroup(ii.Data.Groups),
                        classmod = "",
                        desc = ii.Data.desc == null ? "" : ii.Data.desc,
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

                    //记录日志数据
                    ActionLogService.addLog(new ActionLog()
                    {
                        actTime = DateTime.Now,
                        actType = LogType.takeItemToTradeSys,
                        atcPlayerEntityId = request.user.gameentityid,
                        extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                        extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(ii),
                        extinfo3 = SimpleJson2.SimpleJson2.SerializeObject(userStorate),
                        desc = string.Format("从位于（{0},{1},{2} 的容器中提取{3}个{4}到交易系统个人仓库）", x, y, z, itemcount, ii.Data.translate)
                    });

                    //添加数据到数据库
                    UserStorageService.addUserStorage(userStorate);
                    ResponseUtils.ResponseSuccessWithData(response, ii.Data);
                }
                else
                {
                    ResponseUtils.ResponseFail(response, ii.Msg);
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
