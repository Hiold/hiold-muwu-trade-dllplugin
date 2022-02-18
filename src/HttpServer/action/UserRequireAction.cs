﻿using HioldMod.HttpServer;
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
                    supplygameentityid="",
                    supplyplatformid="",
                    supplyusername="",
                    Supplytime=DateTime.MinValue,
                    Extinfo1="",
                    Extinfo2="",
                    Extinfo3="",
                };
                //保存求购数据
                UserRequireService.addUserRequire(require);


                //记录用户购买数据
                ActionLogService.addLog(new ActionLog()
                {
                    actTime = DateTime.Now,
                    actType = LogType.PostRequire,
                    atcPlayerEntityId = request.user.gameentityid,
                    extinfo1 = SimpleJson2.SimpleJson2.SerializeObject(require)
                });
                ResponseUtils.ResponseSuccessWithData(response, "求购成功!");
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
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
                List<UserRequire> cous = UserRequireService.selectUserRequiresByUserid(id);
                //List<UserInfo> resultList = UserService.userLogin(_info.username, ServerUtils.md5(_info.password));
                ResponseUtils.ResponseSuccessWithData(response, cous);

            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }
}