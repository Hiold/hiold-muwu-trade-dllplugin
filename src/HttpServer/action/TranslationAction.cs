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
    class TranslationAction
    {
        /// <summary>
        /// 翻译action
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getTranslation(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                Dictionary<string, string> param = ServerUtils.GetParam(request.request);
                string itemname = param["itemname"];

                Dictionary<string, object> result = new Dictionary<string, object>();
                if (Localization.dictionary.TryGetValue(itemname,out string[] trData)) {
                    result["value"] = trData;
                }
                else
                {
                    result["value"] = "";
                }
                //赋值需要的返回值
                result["selectedLanguage"] = Localization.language;
                result["allLanguage"] = Localization.knownLanguages;
                ResponseUtils.ResponseSuccessWithData(response, SimpleJson2.SimpleJson2.SerializeObject(result));
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
            }
        }
    }
}
