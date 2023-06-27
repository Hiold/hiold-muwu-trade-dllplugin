using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using HioldMod.src.UserTools;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
using hiold_trade_starter.src;

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
            //加载lib
            string PluginPath = string.Format("{0}/lib/", HioldMod.API.getModDir());
            //加载mod

            Log.Out("开始加载插件");
            int counter = 0;
            string[] Files = Directory.GetFiles(PluginPath, "*.dll");
            foreach (string _path in Files)
            {
                try { 
                Assembly.LoadFile(_path);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
            //检查mod更新
            API.updateDLL(null);

            //生成文件名
            string tempFileName = GetRandomString(16, true, true, true, false, "");
            //删除以往生成的文件
            string[] oldFiles = Directory.GetFiles(AssemblyPath, "*.dlc");
            if (oldFiles != null && oldFiles.Length > 0)
            {

                foreach (string oldfilename in oldFiles)
                {
                    try
                    {
                        File.Delete(oldfilename);
                    }
                    catch (Exception)
                    {
                        Log.Out("[HIOLD] 正在运行的dll不删除");
                    }
                }
            }
            //当前目标bin路径
            string targetBinfile = AssemblyPath + tempFileName + ".dlc";
            //复制主文件进行加载
            File.Copy(AssemblyPath + "trade.bin", targetBinfile);
            LoadAssembly(targetBinfile);

            //定时器发送心跳数据
            Log.Out("正在初始化定时更新任务");
            System.Threading.Timer updateDLLtimer = new System.Threading.Timer(new TimerCallback(API.updateDLL), null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(20));
            System.Threading.Timer updateWEBtimer = new System.Threading.Timer(new TimerCallback(API.updateWEB), null, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(20));
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
                Myrq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36";
                Myrq.Headers.Add("Accept-Encoding", "utf-8");
                Myrq.Accept = "*/*";
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
                Console.WriteLine(e.Message);
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


        public static void updateDLL(object j)
        {
            string version = "";
            GithubRelease Modversion = null;
            string downloadurl = "";
            try
            {
                string data = HttpTools.HttpPost("https://api.github.com/repos/Hiold/hiold-muwu-trade-dllplugin/releases/latest");
                Modversion = SimpleJson2.SimpleJson2.DeserializeObject<GithubRelease>(data);
                version = Modversion.name;
                downloadurl = Modversion.assets[0].browser_download_url;
            }
            catch (Exception e)
            {
                Log.Out(e.Message);
            }
            if (version.Equals(""))
            {
                Log.Out("[HIOLD] 获取远程versoin信息失败不更新");
                return;
            }

            Log.Out("[HIOLD] 远程version信息：{0}", version);
            //是否执行更新
            bool modhasUpdate = false;
            //读取本地文件
            string txt = "";
            try
            {
                StreamReader sr = new StreamReader(@AssemblyPath + "modversion");
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
                //删除文件
                File.Delete(AssemblyPath + "modversion");
                using (StreamWriter sw = new StreamWriter(@AssemblyPath + "modversion", true))
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
                if (DownloadFile(downloadurl, AssemblyPath + "trade.zip"))
                {
                    Log.Out("[HIOLD] 下载完成正在解压");
                    File.Delete(AssemblyPath + "trade.bin");
                    //解压
                    zipUtils.UnZip(AssemblyPath + "trade.zip", AssemblyPath);
                    File.Delete(AssemblyPath + "trade.zip");
                    //写入文件
                    File.Delete(AssemblyPath + "modversion");
                    using (StreamWriter sw = new StreamWriter(@AssemblyPath + "modversion", true))
                    {
                        sw.WriteLine(version);
                        sw.Flush();
                        sw.Close();
                    }
                    Log.Out("[HIOLD] 更新完毕，此更新需要等待服务器重启（或无缝重启）");
                }
            }
            else
            {
                Log.Out("[HIOLD] mod已为最新，不再更新");
            }
        }
        public static void updateWEB(object j)
        {
            string version = "";
            GithubRelease Modversion = null;
            string downloadurl = "";
            try
            {
                string data = HttpTools.HttpPost("https://api.github.com/repos/Hiold/hiold-muwu-trade/releases/latest");
                Modversion = SimpleJson2.SimpleJson2.DeserializeObject<GithubRelease>(data);
                version = Modversion.name;
                downloadurl = Modversion.assets[0].browser_download_url;
            }
            catch (Exception e)
            {
                Log.Out(e.Message);
            }
            if (version.Equals(""))
            {
                Log.Out("[HIOLD] 获取远程webversion信息失败不更新");
                return;
            }

            Log.Out("[HIOLD] 远程webversion信息：{0}", version);
            //是否执行更新
            bool modhasUpdate = false;
            //读取本地文件
            string txt = "";
            try
            {
                StreamReader sr = new StreamReader(@AssemblyPath + "webversion");
                while (!sr.EndOfStream)
                {
                    string str = sr.ReadLine();
                    txt += str;
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Log.Out("[HIOLD] 没有发现webversion信息，执行下载");
            }
            Log.Out("[HIOLD] 本地webversion信息：{0}", txt);
            //校验本地文件
            //没有检测到文件，写入新文件 并执行更新
            if (txt.Equals("") || txt == null)
            {
                modhasUpdate = true;
                //删除文件
                File.Delete(AssemblyPath + "webversion");
                using (StreamWriter sw = new StreamWriter(@AssemblyPath + "webversion", true))
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
                if (DownloadFile(downloadurl, AssemblyPath + "web.zip"))
                {
                    Log.Out("[HIOLD] 下载完成正在解压");
                    try
                    {
                        File.Delete(AssemblyPath + @"\web\index.html");
                    }
                    catch (Exception)
                    {
                        Log.Out("[HIOLD] 找不到index.html跳过删除步骤");
                    }
                    try
                    {
                        Directory.Delete(AssemblyPath + @"\web\assets\", true);
                    }
                    catch (Exception)
                    {
                        Log.Out("[HIOLD] 找不到assets跳过删除步骤");
                    }
                    //解压
                    zipUtils.UnZip(AssemblyPath + "web.zip", AssemblyPath + @"\web\");
                    File.Delete(AssemblyPath + "web.zip");
                    //写入文件
                    File.Delete(AssemblyPath + "webversion");
                    using (StreamWriter sw = new StreamWriter(@AssemblyPath + "webversion", true))
                    {
                        sw.WriteLine(version);
                        sw.Flush();
                        sw.Close();
                    }
                }
                Log.Out("[HIOLD] 更新完毕，此更新立即生效");
            }
            else
            {
                Log.Out("[HIOLD] mod已为最新，不再更新");
            }
        }

    }
    class zipUtils
    {
        public static void UnZip(string SrcFile, string DstFile)
        {
            (new FastZip()).ExtractZip(SrcFile, DstFile, "");
        }
    }

    class Commandupdate : ConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            API.updateWEB(null);
        }

        protected override string[] getCommands()
        {
            return new string[] { "tradeup-web" };
        }

        protected override string getDescription()
        {
            return "更新交易系统web";
        }

    }

    class commandupdate2 : ConsoleCmdAbstract
    {
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            API.updateDLL(null);
        }

        protected override string[] getCommands()
        {
            return new string[] { "tradeup-dll" };
        }

        protected override string getDescription()
        {
            return "更新交易系统dll";
        }
    }
}