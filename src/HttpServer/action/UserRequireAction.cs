using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.Commons;
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
using static HioldMod.src.HttpServer.database.DataBase;

namespace HioldMod.src.HttpServer.action
{
    [ActionAttribute]
    class UserRequireAction
    {
        /// <summary>
        /// 求购物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/postRequire")]
        public static void postRequire(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                int count = 0;
                if (queryRequest.TryGetValue("Itemcount", out string countStr))
                {
                    count = int.Parse(countStr);
                }
                double price = 0;
                if (queryRequest.TryGetValue("Price", out string priceStr))
                {
                    price = double.Parse(priceStr);
                }
                queryRequest.TryGetValue("Itemname", out string Itemname);
                queryRequest.TryGetValue("Itemchinese", out string Itemchinese);
                queryRequest.TryGetValue("Itemquality", out string Itemquality);
                queryRequest.TryGetValue("Itemusetime", out string Itemusetime);
                queryRequest.TryGetValue("Itemicon", out string Itemicon);
                queryRequest.TryGetValue("Itemicontint", out string Itemicontint);
                queryRequest.TryGetValue("Itemgroups", out string Itemgroups);
                //检查游戏内物品参数
                //获取游戏内物品数据
                ItemClass targetItem = ItemClass.GetItem(Itemname).ItemClass;
                if (!targetItem.HasQuality)
                {
                    if (!Itemquality.Equals("0") && !string.IsNullOrEmpty(Itemquality))
                    {
                        ResponseUtils.ResponseFail(response, "求购的物品没有品质，请删除品质再发布！");
                        return;
                    }
                }

                //检查物品交易限制
                List<UserConfig> ucs = UserConfigService.getUserConfigByItemName(Itemname);
                foreach (UserConfig uc in ucs)
                {
                    //启用了交易限制
                    if (uc.available.Equals("1"))
                    {
                        if (uc.configValue.Equals("0"))
                        {
                            ResponseUtils.ResponseFail(response, "该物品禁止交易！");
                            return;
                        }
                        if (!string.IsNullOrEmpty(uc.extinfo1))
                        {
                            int maxprice = int.Parse(uc.extinfo1);
                            if ((price / count) > maxprice)
                            {
                                ResponseUtils.ResponseFail(response, "此物品最高单价为" + maxprice + "，请调整价格");
                                return;
                            }
                        }
                        if (!string.IsNullOrEmpty(uc.extinfo2))
                        {
                            int minprice = int.Parse(uc.extinfo2);
                            if ((price / count) < minprice)
                            {
                                ResponseUtils.ResponseFail(response, "此物品最低单价为" + minprice + "，请调整价格");
                                return;
                            }
                        }
                    }
                }

                //检查参数
                if (string.IsNullOrEmpty(Itemname) || string.IsNullOrEmpty(Itemchinese) || string.IsNullOrEmpty(Itemicon)
                    || string.IsNullOrEmpty(Itemicontint) || string.IsNullOrEmpty(Itemgroups) || count == 0 || price == 0
                    )
                {
                    ResponseUtils.ResponseFail(response, "参数错误！");
                    return;
                }

                if (request.user.money < price)
                {
                    ResponseUtils.ResponseFail(response, "积分不足，求购失败");
                    return;
                }

                if (!database.DataBase.MoneyEditor(request.user, MoneyType.Money, EditType.Sub, price))
                {
                    ResponseUtils.ResponseFail(response, "积分不足。求购失败！");
                    return;
                }

                UserRequire require = new UserRequire()
                {
                    gameentityid = request.user.gameentityid,
                    platformid = request.user.platformid,
                    username = request.user.name,
                    Itemname = Itemname,
                    Itemchinese = Itemchinese,
                    Itemquality = Itemquality,
                    Itemusetime = Itemusetime,
                    Itemicon = Itemicon,
                    Itemicontint = Itemicontint,
                    Itemgroups = Itemgroups,
                    Requiretime = DateTime.Now,
                    Price = price,
                    Itemcount = count,
                    Status = UserRequireConfig.NORMAL_REQUIRE,
                    //默认字段
                    supplygameentityid = "",
                    supplyplatformid = "",
                    supplyusername = "",
                    Supplytime = DateTime.MinValue,
                    Extinfo1 = "",
                    Extinfo2 = "",
                    Extinfo3 = "",
                };
                //保存求购数据
                UserRequireService.addUserRequire(require);

                //更新交易信息数据
                UserService.UpdateAmount(request.user, UserInfoCountType.REQUIRE_COUNT, 1);
                UserService.UpdateAmount(request.user, UserInfoCountType.REQUIRE_MONEY, price);


                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.PostRequire,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(require),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    extinfo4 = price + "",
                    desc = string.Format("发布求购{0}个{1}，求购金额{2}", count, Itemchinese, price)
                });

                ResponseUtils.ResponseSuccessWithData(response, "求购成功!");
                //全服广播
                HioldsCommons.BroadCast("有玩家发布了求购 （" + Itemchinese + "），为他供货可获得奖励，请前往交易系统查看");
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
        /// 获取求购物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/getUserRequire")]
        public static void getUserRequire(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("name", out string name);
                queryRequest.TryGetValue("class2", out string class2);
                queryRequest.TryGetValue("sort", out string sort);
                List<UserRequire> cous = UserRequireService.selectUserRequiresByUserid(id, class2, name, sort);
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
        /// 取消求购
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/cancelRequire")]
        public static void cancelRequire(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                UserRequire cous = UserRequireService.selectUserRequireByid(id);
                if (cous == null)
                {
                    ResponseUtils.ResponseFail(response, "取消失败，校验错误");
                    return;
                }
                if (cous.Status != UserRequireConfig.NORMAL_REQUIRE)
                {
                    ResponseUtils.ResponseFail(response, "取消失败，校验错误");
                    return;
                }
                cous.Status = UserRequireConfig.DELETE;
                UserRequireService.UpdateUserRequire(cous);
                //检查是否为自己的求购
                if (cous.gameentityid != request.user.gameentityid)
                {
                    return;
                }

                //退还积分
                if (cous.Price > 0)
                {
                    database.DataBase.MoneyEditor(request.user, MoneyType.Money, EditType.Add, cous.Price);
                }

                //List<UserInfo> resultList = UserService.userLogin(_info.username, ServerUtils.md5(_info.password));

                //记录日志数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.DeleteRequire,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(cous),
                    extinfo2 = SimpleJson2.SimpleJson2.SerializeObject(queryRequest),
                    desc = string.Format("取消发布{0}，退还积分{1}", cous.Itemchinese, cous.Price)
                });

                ResponseUtils.ResponseSuccessWithData(response, "取消求购成功!退还" + cous.Price + "积分");
                return;
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
