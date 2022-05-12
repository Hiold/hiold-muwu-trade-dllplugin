using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot.websocket.bean
{
    

    public class Stat
    {
        /// <summary>
        /// 
        /// </summary>
        public int PacketReceived { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PacketSent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PacketLost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MessageReceived { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MessageSent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LastMessageTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int DisconnectTimes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LostTimes { get; set; }

    }



    public class Status
    {
        /// <summary>
        /// 
        /// </summary>
        public string app_enabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string app_good { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string app_initialized { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string good { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string online { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string plugins_good { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Stat stat { get; set; }

    }



    public class HeartBeat
    {
        /// <summary>
        /// 
        /// </summary>
        public int interval { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string meta_event_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string post_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int self_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Status status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int time { get; set; }

    }
}
