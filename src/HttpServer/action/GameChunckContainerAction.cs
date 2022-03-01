using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.ChunckLoader;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    public class GameChunckContainerAction
    {
        public static void loadContainerListAround(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                List<Dictionary<string, object>> result = ChunckLoader.ChunkLoader.loadContainerListAround(request.user.platformid);
                if (result != null && result.Count > 0)
                {
                    ResponseUtils.ResponseSuccessWithData(response, result);
                    return;
                }
                else
                {
                    ResponseUtils.ResponseFail(response, "账号或密码错误");
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

        public static void getContainerItems(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                //获取参数
                string postData = ServerUtils.getPostData(request.request);
                Dictionary<string, string> queryRequest = (Dictionary<string, string>)SimpleJson2.SimpleJson2.DeserializeObject(postData, typeof(Dictionary<string, string>));
                queryRequest.TryGetValue("x", out string x);
                queryRequest.TryGetValue("y", out string y);
                queryRequest.TryGetValue("z", out string z);
                //
                queryRequest.TryGetValue("clridx", out string clridx);
                queryRequest.TryGetValue("password", out string password);

                float fx = float.Parse(x);
                float fy = float.Parse(y);
                float fz = float.Parse(z);

                ContainerInfo ci = ChunckLoader.ChunkLoader.getContainerItems(new Vector3i(fx, fy, fz), int.Parse(clridx), request.user.platformid, password);
                if (ci.Code == 0)
                {
                    ResponseUtils.ResponseFail(response, ci.Msg);
                }
                else
                {
                    ResponseUtils.ResponseSuccessWithData(response, ci.Data);
                }


            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.ToString());
                ResponseUtils.ResponseFail(response, "参数异常");
                return;
            }
        }

    }
}
