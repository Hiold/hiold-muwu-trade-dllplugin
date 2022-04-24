using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model
{
    /// <summary>
    /// 绑定请求
    /// </summary>
    public class BindRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string sessionKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int qq { get; set; }

    }

    /// <summary>
    /// 绑定响应
    /// </summary>
    public class BindResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }

    }




}
