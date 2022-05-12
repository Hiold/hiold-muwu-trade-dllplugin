using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.websocket.bean
{
    public class Sender
    {
        /// <summary>
        /// 
        /// </summary>
        public int age { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string card { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string level { get; set; }

        /// <summary>
        /// 海鸥令主
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string role { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int user_id { get; set; }

    }



    public class GroupMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string anonymous { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int font { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int group_id { get; set; }

        /// <summary>
        /// 还在做 你当然看不到[CQ:image,file=37ac5d6ff56619e88ec1bad8204a128b.image,url=https://gchat.qpic.cn/gchatpic_new/960269073/4190687713-2768013990-37AC5D6FF56619E88EC1BAD8204A128B/0?term=3,subType=1]
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int message_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int message_seq { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string message_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string post_type { get; set; }

        /// <summary>
        /// 还在做 你当然看不到[CQ:image,file=37ac5d6ff56619e88ec1bad8204a128b.image,subType=1]
        /// </summary>
        public string raw_message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int self_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Sender sender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string sub_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int user_id { get; set; }

    }


}
