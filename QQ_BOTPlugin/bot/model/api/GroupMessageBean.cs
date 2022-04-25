using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model.api
{

    public class GroupMessageRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string sessionKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int target { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<object> messageChain { get; set; }

    }

    public class GroupMessageResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int messageId { get; set; }

    }
}
