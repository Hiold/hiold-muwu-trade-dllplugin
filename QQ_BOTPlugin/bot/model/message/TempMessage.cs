﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.model.message
{
    

    public class TempMessageGroup
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



    public class TempMessageSender
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
        public TempMessageGroup group { get; set; }

    }



    public class TempMessage
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TempMessageSender sender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<object> messageChain { get; set; }

        public TempMessage(string param)
        {
            TempMessage resp = SimpleJson2.SimpleJson2.DeserializeObject<TempMessage>(param);
            this.type = resp.type;
            this.sender = resp.sender;
            this.messageChain = MessageParser.ParseMessage(resp.messageChain); ;            
        }
        public TempMessage()
        {

        }

    }


}
