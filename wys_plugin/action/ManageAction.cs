using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.attributes;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static HioldMod.HioldMod;

namespace QQ_BOTPlugin.bot.action
{
    [ActionAttribute]
    public class ManageAction
    {
        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/setBotConfig")]
        public static void setBotConfig(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("lottery", out string containerid);
                queryRequest.TryGetValue("qunNumber", out string qunNumber);
                //
                string path = string.Format("{0}/plugins/bot.json", HioldMod.HioldMod.API.AssemblyPath);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //实例化一个文件流--->与写入文件相关联
                FileStream fs = new FileStream(path, FileMode.Create);
                //实例化BinaryWriter
                StreamWriter bw = new StreamWriter(fs);
                bw.Write(SimpleJson2.SimpleJson2.SerializeObject(queryRequest));
                //清空缓冲区
                bw.Flush();
                //关闭流
                bw.Close();
                //重新加载配置文件
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
