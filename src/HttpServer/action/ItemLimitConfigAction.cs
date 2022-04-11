using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
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
    class ItemLimitConfigAction
    {
        public static void QueryItemLimitConfig(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("page", out string page);
                queryRequest.TryGetValue("limit", out string limit);
                ResponseUtils.ResponseSuccessWithData(response, UserConfigService.QueryItemLimitConfig( itemname, page, limit));
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }


        public static void postItemLimitConfig(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("avaliable", out string avaliable);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("ban", out string ban);
                queryRequest.TryGetValue("maxprice", out string maxprice);
                queryRequest.TryGetValue("minprice", out string minprice);
                //添加新的每日奖励
                UserConfigService.addConfig(new UserConfig()
                {
                    gameentityid = "",
                    platformid = "",
                    name = itemname,
                    available = avaliable,
                    configType = ConfigType.Item_Limit,
                    configValue = ban,
                    extinfo1 = maxprice,
                    extinfo2 = minprice,
                    extinfo3 = "",
                    extinfo4 = "",
                    extinfo5 = "",
                    extinfo6 = "",
                });

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.AddItemLimitConfig,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("添加新交易限制")
                });

                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public static void updateItemLimitConfig(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("avaliable", out string avaliable);
                queryRequest.TryGetValue("itemname", out string itemname);
                queryRequest.TryGetValue("ban", out string ban);
                queryRequest.TryGetValue("maxprice", out string maxprice);
                queryRequest.TryGetValue("minprice", out string minprice);
                UserConfig target = UserConfigService.getUserConfigById(id);
                if (target != null)
                {
                    target.available = avaliable;
                    target.name = itemname;
                    target.configValue = ban;
                    target.extinfo1 = maxprice;
                    target.extinfo2 = minprice;
                }

                UserConfigService.updateConfig(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.editItemLimitConfig,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("修改了交易限制")
                });
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        public static void deleteItemLimitConfig(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);

                UserConfig target = UserConfigService.getUserConfigById(id);
                if (target != null)
                {
                    target.configValue = "0";
                }

                UserConfigService.updateConfig(target);

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.deleteItemLimitConfig,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("删除了交易限制")
                });
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }
    }
}
