using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model.message
{

    public class GroupMessageGroup
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string permission { get; set; }

    }



    public class GroupMessageSender
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string memberName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string specialTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string permission { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int joinTimestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int lastSpeakTimestamp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int muteTimeRemaining { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GroupMessageGroup group { get; set; }

    }



    public class GroupMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GroupMessageSender sender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> messageChain { get; set; }

    }


}
