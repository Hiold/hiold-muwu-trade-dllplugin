using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model.message
{
    

    public class StrangerMessageSender
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string remark { get; set; }

    }



    public class StrangerMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StrangerMessageSender sender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> messageChain { get; set; }

    }


}
