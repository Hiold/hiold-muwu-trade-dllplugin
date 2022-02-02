using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    class HioldRequest
    {
        public HttpListenerRequest request { get; set; }
        public UserInfo user { get; set; }
        public string sessionid { get; set; }
    }
}
