using HioldMod.HttpServer;
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
    }
}
