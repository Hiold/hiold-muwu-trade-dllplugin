using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;


namespace HioldMod.src.UserTools
{
    class HttpTools
    {
        public static string HttpService(string serviceUrl, string data, string method)
        {
            //创建Web访问对象
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
                if (data != null)
                {
                    //把用户传过来的数据转成“UTF-8”的字节流
                    byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
                    myRequest.Method = method;
                    myRequest.ContentLength = buf.Length;
                    myRequest.ContentType = "application/json";
                    //myRequest.ContentType = "text/plain";
                    myRequest.MaximumAutomaticRedirections = 1;
                    myRequest.AllowAutoRedirect = true;
                    //发送请求
                    Stream stream = myRequest.GetRequestStream();
                    stream.Write(buf, 0, buf.Length);
                    stream.Close();
                }
                //获取接口返回值
                //通过Web访问对象获取响应内容
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
                string returnXml = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
                reader.Close();
                myResponse.Close();
                return returnXml;
            }
            catch (Exception e)
            {
                return "";
            }
        }




        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public static string HttpPost(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //设置请求类型
            httpWebRequest.Method = "GET";
            //设置超时时间
            httpWebRequest.Timeout = 20000;
            //发送请求
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //读取返回数据
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
            string responseContent = streamReader.ReadToEnd();
            streamReader.Close();
            httpWebResponse.Close();
            httpWebRequest.Abort();
            return responseContent;
        }




        //Stinrg转换Dictionary
        public static Dictionary<string, string> stringToDic(string json)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (json.Equals("")||json==null)
            {
                return result;
            }
            json = json.Replace("{", "").Replace("}", "").Replace("\n", "");
            string[] par = json.Split(',');
            foreach (string temp in par)
            {
                string[] pm = temp.Split(':');
                result.Add(pm[0].Replace("\"", ""), pm[1].Replace("\"", ""));
            }
            return result;
        }

    }
}
