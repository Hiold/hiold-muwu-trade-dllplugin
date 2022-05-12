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
                queryRequest.TryGetValue("host", out string funcid);
                queryRequest.TryGetValue("key", out string key);
                queryRequest.TryGetValue("qq", out string qq);
                queryRequest.TryGetValue("password", out string password);
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
                BOT.loadConfig();
                ResponseUtils.ResponseSuccess(response);
            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getBotLog")]
        public static void getBotLog(HioldRequest request, HttpListenerResponse response)
        {
            System.IO.Stream output = response.OutputStream;
            StreamWriter sw = new StreamWriter(output);
            sw.WriteLine(CMD.sbConsole.ToString());
            sw.Flush();
            sw.Close();
            output.Flush();
            output.Close();
        }

        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getBotError")]
        public static void getBotError(HioldRequest request, HttpListenerResponse response)
        {
            System.IO.Stream output = response.OutputStream;
            StreamWriter sw = new StreamWriter(output);
            sw.WriteLine(CMD.sbError.ToString());
            sw.Flush();
            sw.Close();
            output.Flush();
            output.Close();
        }

        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/getQrcode")]
        public static void getQrcode(HioldRequest request, HttpListenerResponse response)
        {
            string qrcodepath = API.AssemblyPath + @"plugins\Robot\qrcode.png";
            //判断两个路径是否存在文件，均不存在返回404
            if (File.Exists(qrcodepath))
            {
                FileStream fs = File.OpenRead(qrcodepath);
                fs.CopyTo(response.OutputStream);
                fs.Flush();
                fs.Close();
                response.OutputStream.Flush();
                response.OutputStream.Close();
            }
        }

        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/restartBOT")]
        public static void restartBOT(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                CMD.p.Kill();
            }
            catch (Exception)
            {

            }
            CMD.sbConsole.Clear();
            CMD.sbError.Clear();
            CMD.RunQQMCL();
            ResponseUtils.ResponseSuccess(response);
        }


        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/resetBOT")]
        public static void restresetBOTartBOT(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                CMD.p.Kill();
            }
            catch (Exception)
            {

            }
            //删除文件
            string qrcodepath = API.AssemblyPath + @"plugins\Robot\device.json";
            LogUtils.Loger("Qrcode路径:" + qrcodepath);
            if (File.Exists(qrcodepath))
            {
                File.Delete(qrcodepath);
            }


            CMD.sbConsole.Clear();
            CMD.sbError.Clear();
            CMD.RunQQMCL();
            ResponseUtils.ResponseSuccess(response);
        }

        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/isQrcodeReady")]
        public static void isQrcodeReady(HioldRequest request, HttpListenerResponse response)
        {
            string qrcodepath = API.AssemblyPath + @"plugins\Robot\qrcode.png";
            //判断两个路径是否存在文件，均不存在返回404
            if (File.Exists(qrcodepath))
            {
                ResponseUtils.ResponseSuccess(response);
            }
            else
            {
                ResponseUtils.ResponseFail(response, "未就绪");
            }
        }

        [RequestHandlerAttribute(IsServerReady = true, IsUserLogin = true, IsAdmin = true, url = "/api/ExeBotCommand")]
        public static void ExeBotCommand(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("command", out string command);
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
