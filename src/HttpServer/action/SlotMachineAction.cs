using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.attributes;
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
    [ActionAttribute]
    class SlotMachineAction
    {
        /// <summary>
        /// 获取水果机点数
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true,url = "/api/GetSGJPoint")]
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
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/ChargeSGJPoint")]
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
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/ReleaseSGJPoint")]
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

        /// <summary>
        /// 开始rolling
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, url = "/api/SGJRolling")]
        public static void SGJRolling(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("barcount", out string barcount);
                queryRequest.TryGetValue("sevencount", out string sevencount);
                queryRequest.TryGetValue("starcount", out string starcount);
                queryRequest.TryGetValue("xiguacount", out string xiguacount);
                queryRequest.TryGetValue("lingdangcount", out string lingdangcount);
                queryRequest.TryGetValue("lemoncount", out string lemoncount);
                queryRequest.TryGetValue("orangecount", out string orangecount);
                queryRequest.TryGetValue("applecount", out string applecount);
                //
                int intbarcount = int.Parse(barcount);
                int intsevencount = int.Parse(sevencount);
                int intstarcount = int.Parse(starcount);
                int intxiguacount = int.Parse(xiguacount);
                int intlingdangcount = int.Parse(lingdangcount);
                int intlemoncount = int.Parse(lemoncount);
                int intorangecount = int.Parse(orangecount);
                int intapplecount = int.Parse(applecount);
                //获取用户积分数据
                UserConfig uc = UserConfigService.QuerySGJPoint(request.user.gameentityid);
                if (uc == null)
                {
                    ResponseUtils.ResponseFail(response, "没有获取到你的点数信息，请先充值");
                    return;
                }
                //正常获取到数据，计算扣除量
                int final = 0 - intbarcount - intsevencount - intstarcount - intxiguacount - intlingdangcount - intlemoncount - intorangecount - intapplecount;
                //
                int precount = int.Parse(uc.configValue);
                if (precount < (intbarcount + intsevencount + intstarcount + intxiguacount + intlingdangcount + intlemoncount + intorangecount + intapplecount))
                {
                    ResponseUtils.ResponseFail(response, "可用积分不足，请先充值");
                    return;
                }

                //获取概率数列
                List<int> randList = getRandList();
                byte[] b = new byte[4];
                new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
                Random r = new Random(BitConverter.ToInt32(b, 0));
                int rstNumber = randList[r.Next(0, randList.Count)];
                int baster = 0;
                //bar
                if (intbarcount > 0)
                {
                    if (rstNumber == 5)
                    {
                        baster = intbarcount * 25;
                    }
                    if (rstNumber == 6)
                    {
                        baster = intbarcount * 50;
                    }
                }
                //77
                if (intsevencount > 0)
                {
                    if (rstNumber == 16)
                    {
                        baster = intsevencount * 2;
                    }
                    if (rstNumber == 17)
                    {
                        baster = intsevencount * 20;
                    }
                }
                //star
                if (intstarcount > 0)
                {
                    if (rstNumber == 0)
                    {
                        baster = intstarcount * 2;
                    }
                    if (rstNumber == 21)
                    {
                        baster = intstarcount * 20;
                    }
                }
                //xigua
                if (intxiguacount > 0)
                {
                    if (rstNumber == 10)
                    {
                        baster = intxiguacount * 10;
                    }
                    if (rstNumber == 11)
                    {
                        baster = intxiguacount * 2;
                    }
                }
                //lingdang
                if (intlingdangcount > 0)
                {
                    if (rstNumber == 2)
                    {
                        baster = intlingdangcount * 2;
                    }
                    if (rstNumber == 4)
                    {
                        baster = intlingdangcount * 10;
                    }
                    if (rstNumber == 15)
                    {
                        baster = intlingdangcount * 10;
                    }
                }
                //lemon
                if (intlemoncount > 0)
                {
                    if (rstNumber == 9)
                    {
                        baster = intlemoncount * 10;
                    }
                    if (rstNumber == 19)
                    {
                        baster = intlemoncount * 2;
                    }
                    if (rstNumber == 20)
                    {
                        baster = intlemoncount * 10;
                    }
                }
                //orange
                if (intorangecount > 0)
                {
                    if (rstNumber == 3)
                    {
                        baster = intorangecount * 10;
                    }
                    if (rstNumber == 13)
                    {
                        baster = intorangecount * 2;
                    }
                    if (rstNumber == 14)
                    {
                        baster = intorangecount * 10;
                    }
                }
                //apple
                if (intapplecount > 0)
                {
                    if (rstNumber == 1)
                    {
                        baster = intapplecount * 5;
                    }
                    if (rstNumber == 7)
                    {
                        baster = intapplecount * 5;
                    }
                    if (rstNumber == 8)
                    {
                        baster = intapplecount * 2;
                    }
                    if (rstNumber == 12)
                    {
                        baster = intapplecount * 5;
                    }
                    if (rstNumber == 18)
                    {
                        baster = intapplecount * 5;
                    }
                }
                //计算最后量
                final += baster;
                //更新数据
                int leftCount = int.Parse(uc.configValue);
                uc.configValue = (leftCount + final) + "";
                UserConfigService.updateConfig(uc);
                Dictionary<string, int> result = new Dictionary<string, int>();
                result.Add("rand", rstNumber);
                result.Add("points", int.Parse(uc.configValue));
                ResponseUtils.ResponseSuccessWithData(response, SimpleJson2.SimpleJson2.SerializeObject(result));
                return;
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        public static List<int> result = null;
        public static List<int> getRandList()
        {
            if (result == null)
            {
                result = new List<int>();
                for (int i = 0; i < 1000 * (6.7d / 100d); i++)
                {
                    result.Add(0);
                }
                for (int i = 0; i < 1000 * (6.0d / 100d); i++)
                {
                    result.Add(1);
                }
                for (int i = 0; i < 1000 * (5.7d / 100d); i++)
                {
                    result.Add(2);
                }
                for (int i = 0; i < 1000 * (2.8d / 100d); i++)
                {
                    result.Add(3);
                }
                for (int i = 0; i < 1000 * (2.8d / 100d); i++)
                {
                    result.Add(4);
                }
                for (int i = 0; i < 1000 * (2d / 100d); i++)
                {
                    result.Add(5);
                }
                for (int i = 0; i < 1000 * (1d / 100d); i++)
                {
                    result.Add(6);
                }
                for (int i = 0; i < 1000 * (5d / 100d); i++)
                {
                    result.Add(7);
                }
                for (int i = 0; i < 1000 * (5.7d / 100d); i++)
                {
                    result.Add(8);
                }
                for (int i = 0; i < 1000 * (2.8d / 100d); i++)
                {
                    result.Add(9);
                }
                for (int i = 0; i < 1000 * (2.8d / 100d); i++)
                {
                    result.Add(10);
                }
                for (int i = 0; i < 1000 * (5.7d / 100d); i++)
                {
                    result.Add(11);
                }
                for (int i = 0; i < 1000 * (5d / 100d); i++)
                {
                    result.Add(12);
                }
                for (int i = 0; i < 1000 * (5.7d / 100d); i++)
                {
                    result.Add(13);
                }
                for (int i = 0; i < 1000 * (2.8d / 100d); i++)
                {
                    result.Add(14);
                }
                for (int i = 0; i < 1000 * (2.8d / 100d); i++)
                {
                    result.Add(15);
                }
                for (int i = 0; i < 1000 * (5.7d / 100d); i++)
                {
                    result.Add(16);
                }
                for (int i = 0; i < 1000 * (7.5d / 100d); i++)
                {
                    result.Add(17);
                }
                for (int i = 0; i < 1000 * (5d / 100d); i++)
                {
                    result.Add(18);
                }
                for (int i = 0; i < 1000 * (5.7d / 100d); i++)
                {
                    result.Add(19);
                }
                for (int i = 0; i < 1000 * (2.8d / 100d); i++)
                {
                    result.Add(20);
                }
                for (int i = 0; i < 1000 * (7.5d / 100d); i++)
                {
                    result.Add(21);
                }
            }

            return result;
        }

    }
}
