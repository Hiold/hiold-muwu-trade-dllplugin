using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
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
        public static void uploadFile(HioldRequest request, HttpListenerResponse response)
        {
            List<string> result = new List<string>();
            string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/image/";
            try
            {
                if (API.isOnServer)
                {
                    basepath = string.Format("{0}/image/", API.AssemblyPath);
                }
                FileUploadUtils fuu = new FileUploadUtils(request.request, Encoding.UTF8);
                List<MultipartFormItem> files = fuu.ParseIntoElementList();
                for (int i = 0; i < files.Count; i++)
                {
                    FileStream fs = new FileStream(basepath + DateTime.Now.ToString("yyyyMMddHHmmssfff") + files[i].FileName, FileMode.Create);
                    fs.Write(files[i].Data, 0, files[i].Data.Length);
                    fs.Flush();
                    fs.Close();
                }
                response.StatusCode = 200;
                response.OutputStream.Flush();
                response.OutputStream.Close();
            }
            catch (Exception e)
            {
                response.StatusCode = 500;
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
        public static void getIconFile(HioldRequest request, HttpListenerResponse response)
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
                //对结果进行排序
                SortAsFileCreationTime(ref fss);
                for (int i = 0; i < fss.Length; i++)
                {
                    result.Add(fss[i].Name);
                }
                ResponseUtils.ResponseSuccessWithData(response, result);
                return;
            }
            catch (Exception e)
            {
                ResponseUtils.ResponseFail(response, "获取文件异常");
                LogUtils.Loger("读取文件异常:" + e.Message);
                return;
            }
        }

        /// <summary>
        /// 文件排序
        /// </summary>
        /// <param name="arrFi">文件列表</param>
        public static void SortAsFileCreationTime(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return y.CreationTime.CompareTo(x.CreationTime); });
        }

    }
}
