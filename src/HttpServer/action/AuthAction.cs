using HioldMod.HttpServer.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class AuthAction
    {
        private static string ApiKey = "8523A031BCC59CA8EB27075AA7F53648";
        private static string OpenIdURI = "https://steamcommunity.com/openid/login";

        /// <summary>
        /// 创建Steam OpenID登录链接
        /// </summary>
        /// <param name="returnUrl">返回链接</param>
        /// <returns></returns>
        public static void steamAuth(HttpListenerRequest request, HttpListenerResponse responsel)
        {
            string authUrl = string.Empty;
            string returnUrl = "http://localhost:26911/api/debug";
            string host = request.Url.Scheme + "://" + request.Url.Authority.Trim('/');
            StringBuilder sb = new StringBuilder();
            sb.Append(OpenIdURI);
            sb.Append("?");
            sb.AppendFormat("openid.ns={0}&", ServerUtils.UrlDecode("http://specs.openid.net/auth/2.0"));
            sb.AppendFormat("openid.mode=checkid_setup&");
            sb.AppendFormat("openid.return_to={0}&", ServerUtils.UrlDecode(host + "/" + returnUrl.Trim('/')));
            sb.AppendFormat("openid.realm={0}&", ServerUtils.UrlDecode(host));
            sb.AppendFormat("openid.identity={0}&", ServerUtils.UrlDecode("http://specs.openid.net/auth/2.0/identifier_select"));
            sb.AppendFormat("openid.claimed_id={0}", ServerUtils.UrlDecode("http://specs.openid.net/auth/2.0/identifier_select"));
            authUrl = sb.ToString();
            Console.WriteLine(authUrl);
            responsel.Redirect(authUrl);
            //return authUrl;
        }
    }
}
