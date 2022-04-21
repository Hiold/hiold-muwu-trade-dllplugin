using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.HttpServer.database;
using HioldMod.src.HttpServer.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class SlotMachineAction
    {
        /// <summary>
        /// 获取水果机点数
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void GetSGJPoint(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                //queryRequest.TryGetValue("id", out string id);
                ResponseUtils.ResponseSuccessWithData(response, UserConfigService.QuerySGJPoint(request.user.gameentityid));
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
        /// 充值水果机点数
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void ChargeSGJPoint(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("count", out string count);
                int intcount = int.Parse(count);
                if (intcount <= 0)
                {
                    ResponseUtils.ResponseFail(response, "数量异常");
                    return;
                }
                if (!DataBase.MoneyEditor(request.user, DataBase.MoneyType.Money, DataBase.EditType.Sub, intcount * 10000))
                {
                    ResponseUtils.ResponseFail(response, "积分不足");
                    return;
                }
                UserConfig uc = UserConfigService.QuerySGJPoint(request.user.gameentityid);
                if (uc == null)
                {
                    //添加新的每日奖励
                    UserConfigService.addConfig(new UserConfig()
                    {
                        gameentityid = request.user.gameentityid,
                        platformid = "",
                        name = "",
                        available = "1",
                        configType = ConfigType.Sgj_Point,
                        configValue = (intcount * 100) + "",
                        extinfo1 = "",
                        extinfo2 = "",
                        extinfo3 = "",
                        extinfo4 = "",
                        extinfo5 = "",
                        extinfo6 = "",
                    });
                }
                else
                {
                    int leftCount = int.Parse(uc.configValue);
                    uc.configValue = (leftCount + intcount * 100) + "";
                    UserConfigService.updateConfig(uc);
                }
                //返回成功
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }


        /// <summary>
        /// 兑换水果机点数
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void ReleaseSGJPoint(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("count", out string count);
                int intcount = int.Parse(count);
                if (intcount <= 0)
                {
                    ResponseUtils.ResponseFail(response, "数量异常");
                    return;
                }
                UserConfig uc = UserConfigService.QuerySGJPoint(request.user.gameentityid);
                if (uc != null)
                {
                    int points = int.Parse(uc.configValue);
                    if (points >= intcount * 100)
                    {
                        uc.configValue = (points - (intcount * 100)) + "";
                        UserConfigService.updateConfig(uc);
                        //添加积分
                        DataBase.MoneyEditor(request.user, DataBase.MoneyType.Money, DataBase.EditType.Add, intcount * 10000);
                        //返回成功
                        ResponseUtils.ResponseSuccess(response);
                    }
                    else
                    {
                        ResponseUtils.ResponseFail(response, "点数不足");
                        return;
                    }
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "点数不足");
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

    }
}
