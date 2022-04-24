using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model
{

    public class VerifyRequest
    {
        /// <summary>
        /// 鉴权请求
        /// </summary>
        public string verifyKey { get; set; }

    }

    /// <summary>
    /// 鉴权响应
    /// </summary>
    public class VerifyResponse
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// sessionid
        /// </summary>
        public string session { get; set; }

    }

}
