using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HioldMod.HttpServer.common
{
    class ServerUtils
    {
        public static string GetRandomString(int length)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string GetMD5(string encypStr, string charset)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception ex)
            {
                inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);
            m5.Clear();
            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToLower();
            return retStr;
        }

        public static string md5(string str)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bytValue, bytHash;
                bytValue = System.Text.Encoding.UTF8.GetBytes(str);
                bytHash = md5.ComputeHash(bytValue);
                md5.Clear();
                string sTemp = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
                }
                str = sTemp.ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return str;
        }

        /// <summary>
        /// 处理请求获取参数
        /// </summary>
        /// <param name="request">请求参数</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetParam(HttpListenerRequest request)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (request.QueryString.HasKeys())
            {
                var q = request.Url.Query;
                if (q.Length > 0)
                {
                    foreach (var p in q.Substring(1).Split('&'))
                    {
                        var s = p.Split(new char[] { '=' }, 2);
                        data.Add(HttpUtility.UrlDecode(s[0]), HttpUtility.UrlDecode(s[1]));
                    }
                }
            }

            if (request.HasEntityBody)
            {
                // 获取Post请求中的参数和值帮助类
                HttpListenerPostParaHelper httppost = new HttpListenerPostParaHelper(request);
                // 获取Post过来的参数和数据
                List<HttpListenerPostValue> lst = httppost.GetHttpListenerPostValue();
                for (int i = 0; i < lst.Count; i++)
                {
                    if (lst[i].type == 0)
                    {
                        data[lst[i].name] = Encoding.UTF8.GetString(lst[i].datas);
                    }
                }
            }
            return data;
        }



        internal class HttpListenerPostParaHelper
        {
            #region <变量>

            private HttpListenerRequest request;

            #endregion <变量>

            #region <方法>

            public HttpListenerPostParaHelper(HttpListenerRequest request)
            {
                this.request = request;
            }

            private bool CompareBytes(byte[] source, byte[] comparison)
            {
                try
                {
                    int count = source.Length;
                    if (source.Length != comparison.Length)
                        return false;
                    for (int i = 0; i < count; i++)
                        if (source[i] != comparison[i])
                            return false;
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private byte[] ReadLineAsBytes(Stream SourceStream)
            {
                var resultStream = new MemoryStream();
                while (true)
                {
                    int data = SourceStream.ReadByte();
                    resultStream.WriteByte((byte)data);
                    if (data == 10)
                        break;
                }
                resultStream.Position = 0;
                byte[] dataBytes = new byte[resultStream.Length];
                resultStream.Read(dataBytes, 0, dataBytes.Length);
                return dataBytes;
            }

            /// <summary>
            /// 获取Post过来的参数和数据
            /// </summary>
            /// <returns></returns>
            public List<HttpListenerPostValue> GetHttpListenerPostValue()
            {
                try
                {
                    List<HttpListenerPostValue> HttpListenerPostValueList = new List<HttpListenerPostValue>();
                    if (request.ContentType.Length > 20 && string.Compare(request.ContentType.Substring(0, 20), "multipart/form-data;", true) == 0)
                    {
                        string[] HttpListenerPostValue = request.ContentType.Split(';').Skip(1).ToArray();
                        string boundary = string.Join(";", HttpListenerPostValue).Replace("boundary=", "").Trim();
                        byte[] ChunkBoundary = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
                        byte[] EndBoundary = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");
                        Stream SourceStream = request.InputStream;
                        var resultStream = new MemoryStream();
                        bool CanMoveNext = true;
                        HttpListenerPostValue data = null;
                        while (CanMoveNext)
                        {
                            byte[] currentChunk = ReadLineAsBytes(SourceStream);
                            if (!Encoding.UTF8.GetString(currentChunk).Equals("\r\n"))
                                resultStream.Write(currentChunk, 0, currentChunk.Length);
                            if (CompareBytes(ChunkBoundary, currentChunk))
                            {
                                byte[] result = new byte[resultStream.Length - ChunkBoundary.Length];
                                resultStream.Position = 0;
                                resultStream.Read(result, 0, result.Length);
                                CanMoveNext = true;
                                if (result.Length > 0)
                                    data.datas = result;
                                data = new HttpListenerPostValue();
                                HttpListenerPostValueList.Add(data);
                                resultStream.Dispose();
                                resultStream = new MemoryStream();
                            }
                            else if (Encoding.UTF8.GetString(currentChunk).Contains("Content-Disposition"))
                            {
                                byte[] result = new byte[resultStream.Length - 2];
                                resultStream.Position = 0;
                                resultStream.Read(result, 0, result.Length);
                                CanMoveNext = true;
                                data.name = Encoding.UTF8.GetString(result).Replace("Content-Disposition: form-data; name=\"", "").Replace("\"", "").Split(';')[0];
                                resultStream.Dispose();
                                resultStream = new MemoryStream();
                            }
                            else if (Encoding.UTF8.GetString(currentChunk).Contains("Content-Type"))
                            {
                                CanMoveNext = true;
                                data.type = 1;
                                resultStream.Dispose();
                                resultStream = new MemoryStream();
                            }
                            else if (CompareBytes(EndBoundary, currentChunk))
                            {
                                byte[] result = new byte[resultStream.Length - EndBoundary.Length - 2];
                                resultStream.Position = 0;
                                resultStream.Read(result, 0, result.Length);
                                data.datas = result;
                                resultStream.Dispose();
                                CanMoveNext = false;
                            }
                        }
                    }
                    return HttpListenerPostValueList;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

            #endregion <方法>
        }

        /// <summary>
        /// HttpListenner监听Post请求参数值实体
        /// </summary>
        public class HttpListenerPostValue
        {
            /// <summary>
            /// 0:参数
            /// 1:文件
            /// </summary>
            public int type = 0;

            public string name;
            public byte[] datas;
        }
    }
}
