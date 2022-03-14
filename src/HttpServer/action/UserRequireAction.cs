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
using static HioldMod.src.HttpServer.database.DataBase;

namespace HioldMod.src.HttpServer.action
{
    class UserRequireAction
    {
        /// <summary>
        /// 求购物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
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

                //记录用户购买数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.PostRequire,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(require)
                });

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
        public static void getUserRequire(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                queryRequest.TryGetValue("class2", out string class2);
                List<UserRequire> cous = UserRequireService.selectUserRequiresByUserid(id, class2);
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
        public static void cancelRequire(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("id", out string id);
                UserRequire cous = UserRequireService.selectUserRequireByid(id);
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
