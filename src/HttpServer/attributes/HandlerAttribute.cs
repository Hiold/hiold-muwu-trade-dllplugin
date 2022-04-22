using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.attributes
{
    public class ActionAttribute : Attribute { }
    /// <summary>
    /// 声明请求处理特性
    /// </summary>
    public class RequestHandlerAttribute : Attribute
    {
        //url
        public string url { get; set; } = "";
        //是否开启无缝重启检查
        public bool IsServerReady { get; set; } = false;
        //是否开启登录检查
        public bool IsUserLogin { get; set; } = false;
        //是否开启管理员检查
        public bool IsAdmin { get; set; } = false;
    }
}
