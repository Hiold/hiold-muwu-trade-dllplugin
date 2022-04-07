using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using HioldMod.src.UserTools;
using System.Net;

namespace HioldMod
{
    public class API : IModApi
    {
        public static string AssemblyPath = string.Format("{0}/", getModDir());
        private static Assembly MainAssembly;
        private static IModApi ApiInstance;
        private static Mod _modInstance;
        public void InitMod(Mod _modInstance)
        {
            API._modInstance = _modInstance;
            string version = "";
            try
            {
                version = HttpTools.HttpPost("https://qc.hiold.net/hioldapi/modversion.json");
            }
            catch (Exception e)
            {
                Log.Out(e.Message);
            }
            Log.Out("[HIOLD] 远程version信息：{0}", version);
            //是否执行更新
            bool modhasUpdate = false;
            //读取本地文件
            string txt = "";
            try
            {
                StreamReader sr = new StreamReader(@AssemblyPath + "modversion.json");
                while (!sr.EndOfStream)
                {
                    string str = sr.ReadLine();
                    txt += str;
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Log.Out("[HIOLD] 没有发现version信息，执行下载");
            }
            Log.Out("[HIOLD] 本地version信息：{0}", txt);
            //校验本地文件
            //没有检测到文件，写入新文件 并执行更新
            if (txt.Equals("") || txt == null)
            {
                modhasUpdate = true;
                //写入文件
                File.Delete(AssemblyPath + "modversion.json");
                using (StreamWriter sw = new StreamWriter(@AssemblyPath + "modversion.json", true))
                {
                    sw.WriteLine(version);
                    sw.Flush();
                    sw.Close();
                }
            }




            //判断版本情况
            if (version.Length > 0 && txt.Length > 0)
            {
                //
                try
                {
                    if (version.Equals(txt))
                    {
                        modhasUpdate = false;
                    }
                    else
                    {
                        modhasUpdate = true;
                    }


                }
                catch (Exception e)
                {
                    Log.Out(e.Message);
                    modhasUpdate = false;
                }
            }
            //下载dll
            if (modhasUpdate)
            {
                Log.Out("[HIOLD] 检测到更新mod更新，正在下载");
                //覆盖旧文件
                if (DownloadFile("https://qc.hiold.net/hioldapi/main.bin", AssemblyPath + "main.bin.new"))
                {
                    File.Delete(AssemblyPath + "main.bin");
                    File.Copy(AssemblyPath + "main.bin.new", AssemblyPath + "main.bin");
                    File.Delete(AssemblyPath + "main.bin.new");
                    //写入文件
                    File.Delete(AssemblyPath + "modversion.json");
                    using (StreamWriter sw = new StreamWriter(@AssemblyPath + "modversion.json", true))
                    {
                        sw.WriteLine(version);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            else
            {
                Log.Out("[HIOLD] mod已为最新，不再更新");
            }






            //赋值主文件进行加载
            try
            {
                File.Delete(AssemblyPath + "main.dlc");
                File.Copy(AssemblyPath + "main.bin", AssemblyPath + "main.dlc");
                //反射获取数据
                LoadAssembly(AssemblyPath + "main.dlc");
            }
            catch (Exception)
            {
                File.Delete(AssemblyPath + "main.dlc2");
                File.Copy(AssemblyPath + "main.bin", AssemblyPath + "main.dlc2");
                //反射获取数据
                LoadAssembly(AssemblyPath + "main.dlc2");
            }
        }

        //加载mod
        public static void LoadAssembly(string _path)
        {
            Log.Out("[HIOLD] 正在载入MOD , 创建实例");
            Type typeFromHandle = typeof(IModApi);
            MainAssembly = Assembly.LoadFrom(_path);
            foreach (Type type in MainAssembly.GetTypes())
            {
                if (typeFromHandle.IsAssignableFrom(type))
                {
                    //Log.Out("[MODS] Found ModAPI in " + System.IO.Path.GetFileName(keyValuePair.Key) + ", creating instance");
                    ApiInstance = (Activator.CreateInstance(type) as IModApi);
                    //return true;
                }
            }
            //执行初始化
            ApiInstance.InitMod(_modInstance);

        }

        public static string getModDir()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }


        private static bool DownloadFile(string URL, string filename)
        {
            try
            {
                HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                Stream st = myrp.GetResponseStream();
                Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, (int)by.Length);
                }
                so.Close();
                st.Close();
                myrp.Close();
                Myrq.Abort();
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

    }
}