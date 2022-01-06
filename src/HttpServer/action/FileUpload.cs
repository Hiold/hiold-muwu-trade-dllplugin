using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class FileUpload
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void uploadFile(HttpListenerRequest request, HttpListenerResponse response)
        {
            DirectoryInfo di = new DirectoryInfo(API.AssemblyPath);
            string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Data/ItemIcons/";
            string url = request.RawUrl.Replace("/api/image/", "");
            response.ContentType = "image/png";
            try
            {
                if (API.isOnServer)
                {
                    basepath = di.Parent.Parent.FullName + "/Data/ItemIcons/";
                }


                FileStream fs = File.OpenRead(basepath + url);
                fs.CopyTo(response.OutputStream);
                fs.Flush();
                fs.Close();
                response.OutputStream.Flush();
                response.OutputStream.Close();
                LogUtils.Loger(url);
            }
            catch (Exception e)
            {
                response.StatusCode = 404;
                response.OutputStream.Flush();
                response.OutputStream.Close();
                LogUtils.Loger("读取文件异常:" + e.Message);
            }
        }

        /// <summary>
        /// 获取图标文件列表
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getIconFile(HttpListenerRequest request, HttpListenerResponse response)
        {
            List<string> result = new List<string>();
            string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/image/";
            try
            {
                if (API.isOnServer)
                {
                    basepath = string.Format("{0}/image/", API.AssemblyPath);
                }
                DirectoryInfo flooder = new DirectoryInfo(basepath);
                FileInfo[] fss = flooder.GetFiles();
                for (int i = 0; i < fss.Length; i++)
                {
                    result.Add(fss[i].Name);
                }
                ResponseUtils.ResponseSuccessWithData(response, result);
            }
            catch (Exception e)
            {
                ResponseUtils.ResponseFail(response, "获取文件异常");
                LogUtils.Loger("读取文件异常:" + e.Message);
            }
        }

    }
}
