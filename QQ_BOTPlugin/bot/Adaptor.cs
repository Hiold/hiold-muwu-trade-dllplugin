using QQ_BOTPlugin.bot.model;
using QQ_BOTPlugin.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.bot
{
    public class Adaptor
    {
        public static string GetToken()
        {
            VerifyRequest req = new VerifyRequest() { verifyKey = BOT.key };
            string response = HttpUtils.HttpPost(BOT.host + "/verify", SimpleJson2.SimpleJson2.SerializeObject(req));
            Console.WriteLine(response);
            VerifyResponse resp = SimpleJson2.SimpleJson2.DeserializeObject<VerifyResponse>(response);
            return resp.session;
        }
    }
}
