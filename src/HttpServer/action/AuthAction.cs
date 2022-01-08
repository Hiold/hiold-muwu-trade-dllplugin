using HioldMod.HttpServer.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        public static void steamAuth(HttpListenerRequest request, HttpListenerResponse response)
        {
            string authUrl = string.Empty;
            string host = request.Url.Scheme + "://" + request.Url.Authority.Trim('/');
            StringBuilder sb = new StringBuilder();
            sb.Append(OpenIdURI);
            sb.Append("?");
            sb.AppendFormat("openid.ns={0}&", ServerUtils.UrlDecode("http://specs.openid.net/auth/2.0"));
            sb.AppendFormat("openid.mode=checkid_setup&");
            sb.AppendFormat("openid.return_to={0}&", ServerUtils.UrlDecode(host + "/api/verification"));
            sb.AppendFormat("openid.realm={0}&", ServerUtils.UrlDecode(host));
            sb.AppendFormat("openid.identity={0}&", ServerUtils.UrlDecode("http://specs.openid.net/auth/2.0/identifier_select"));
            sb.AppendFormat("openid.claimed_id={0}", ServerUtils.UrlDecode("http://specs.openid.net/auth/2.0/identifier_select"));
            authUrl = sb.ToString();
            Console.WriteLine(authUrl);
            response.Redirect(authUrl);
            //return authUrl;
        }

        /// <summary>
        /// 是否从Steam登录返回
        /// </summary>
        /// <returns></returns>
        public static bool IsFromSteam(HttpListenerRequest Request)
        {
            if (Request != null)
            {
                string a = Request.QueryString["openid.identity"],
                       b = Request.QueryString["openid.response_nonce"],
                       c = Request.QueryString["openid.assoc_handle"],
                       d = Request.QueryString["openid.signed"],
                       e = Request.QueryString["openid.sig"];
                if (Regex.IsMatch(a, "steamcommunity.com/openid/id/\\d+", RegexOptions.IgnoreCase) && !string.IsNullOrWhiteSpace(b) && !string.IsNullOrWhiteSpace(c) && !string.IsNullOrWhiteSpace(d) && !string.IsNullOrWhiteSpace(e))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 验证OpenID登录返回数据
        /// </summary>
        /// <returns></returns>
        public static SteamOpenIDIdentity Profile(HttpListenerRequest Request, HttpListenerResponse response)
        {
            SteamOpenIDIdentity identity = null;
            if (Request != null)
            {
                string query = Regex.Replace(Request.Url.Query, "(?<=openid.mode=).+?(?=\\&)", "check_authentication", RegexOptions.IgnoreCase).Trim('?');
                Console.WriteLine(query);
                try
                {
                    WebClient client = new WebClient();
                    client.Encoding = Encoding.UTF8;
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36";
                    string result = client.UploadString(OpenIdURI, "POST", query);
                    Console.WriteLine("------------");
                    Console.WriteLine(result);
                    Console.WriteLine("------------");
                    if (result.ToLower().Contains("is_valid:true"))
                    {
                        identity = new SteamOpenIDIdentity();
                        identity.SteamId = Regex.Match(ServerUtils.UrlDecode(Request.Url.Query), "(?<=openid/id/)\\d+", RegexOptions.IgnoreCase).Value;

                        result = client.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + ApiKey + "&steamids=" + identity.SteamId);
                        identity.Avatar = Regex.Match(result, "(?<=\"avatarfull\":\\s*?\").+?(?=\")", RegexOptions.IgnoreCase).Value;
                        identity.Profile = Regex.Match(result, "(?<=\"profileurl\":\\s*?\").+?(?=\")", RegexOptions.IgnoreCase).Value;
                        identity.UserName = Regex.Match(result, "(?<=\"personaname\":\\s*?\").+?(?=\")", RegexOptions.IgnoreCase).Value;
                        identity.ReturnTo = new Uri(Request.QueryString["openid.return_to"]).PathAndQuery;
                    }
                }
                catch (Exception ex) { }
            }
            return identity;
        }

        //登录成功返回
        public static void Verification(HttpListenerRequest Request, HttpListenerResponse response)
        {
            if (IsFromSteam(Request))
            {
                #region OpenID验证
                SteamOpenIDIdentity identity = Profile(Request, response);
                if (identity != null)
                {
                    Console.WriteLine(SimpleJson2.SimpleJson2.SerializeObject(identity));
                    //登录处理逻辑

                    #endregion OpenID验证
                }
            }
        }
        /// <summary>
        /// Steam用户信息
        /// </summary>
        public class SteamOpenIDIdentity
        {
            public SteamOpenIDIdentity()
            {
                this.ReturnTo = "/";
            }
            /// <summary>
            /// 获取用户图像
            /// </summary>
            public string Avatar { set; get; }
            /// <summary>
            /// 获取用户中心地址
            /// </summary>
            public string Profile { set; get; }
            /// <summary>
            /// 获取Steam ID
            /// </summary>
            public string SteamId { set; get; }
            /// <summary>
            /// 获取用户名称
            /// </summary>
            public string UserName { set; get; }
            /// <summary>
            /// 获取返回地址
            /// </summary>
            public string ReturnTo { set; get; }
        }
    }
}
