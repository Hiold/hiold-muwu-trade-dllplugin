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
        public long PacketReceived { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long PacketSent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long PacketLost { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long MessageReceived { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long MessageSent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long LastMessageTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long DisconnectTimes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long LostTimes { get; set; }

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
        public long interval { get; set; }

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
        public long self_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Status status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long time { get; set; }

    }
}
