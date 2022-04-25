using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model.message
{


    public class OtherClientMessageSender
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string platform { get; set; }

    }



    public class OtherClientMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OtherClientMessageSender sender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<object> messageChain { get; set; }

        public OtherClientMessage(string param)
        {
            OtherClientMessage resp = SimpleJson2.SimpleJson2.DeserializeObject<OtherClientMessage>(param);
            this.type = resp.type;
            this.sender = resp.sender;
            this.messageChain = MessageParser.ParseMessage(resp.messageChain); ;
        }
        public OtherClientMessage()
        {

        }

    }




}
