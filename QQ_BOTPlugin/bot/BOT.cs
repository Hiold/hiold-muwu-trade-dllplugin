using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot
{
    public class BOT
    {
        public static string host = "http://dx.s1.hiold.net:9991";
        public static string key = "a1010000";
        public static int qq = 14021367;

        /// <summary>
        /// 初始化bot
        /// </summary>
        public static void initBot()
        {
            string token = Adaptor.GetToken(key);
            Adaptor.Bind(token, qq);
        }
    }
}
