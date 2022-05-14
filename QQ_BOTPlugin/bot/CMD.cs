using HioldMod.HttpServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HioldMod.HioldMod;

namespace QQ_BOTPlugin.bot
{
    public class CMD
    {
        public static StreamReader ConsoleOutput;
        public static StreamReader ConsoleError;
        public static Process p;
        public static StringBuilder sbConsole = new StringBuilder();
        public static StringBuilder sbError = new StringBuilder();
        //定时清理日志
        private static System.Threading.Timer Onlinetimer = new System.Threading.Timer(new TimerCallback((object c) => { sbConsole.Clear(); sbError.Clear(); }), null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));

        public static void RunQQMCL()
        {
            //删除文件
            string qrcodepath = API.AssemblyPath + @"plugins\Robot\qrcode.png";
            LogUtils.Loger("Qrcode路径:" + qrcodepath);
            if (File.Exists(qrcodepath))
            {
                File.Delete(qrcodepath);
            }
            //重命名文件
            string exePaht = API.AssemblyPath + @"plugins\Robot\bin\16076_main.exef";
            string workingDir = API.AssemblyPath + @"plugins\Robot\";
            string exePah = API.AssemblyPath + @"plugins\Robot\bin\16076_main.exe";
            //如果exef存在、exe不存在
            if (File.Exists(exePaht) && !File.Exists(exePah))
            {
                File.Copy(exePaht, exePah);
            }
            //检查配置是否完整
            string tokenPath = API.AssemblyPath + @"plugins\Robot\session.token";
            string devicePath = API.AssemblyPath + @"plugins\Robot\device.json";

            if (!File.Exists(tokenPath) || !File.Exists(devicePath))
            {
                LogUtils.Loger("QQBOT配置文件不完整，无法启动请完成配置");
                sbConsole.Append("QQBOT配置文件不完整，无法启动请完成配置");
                return;
            }

            LogUtils.Loger("Qrcode路径:" + qrcodepath);
            if (File.Exists(qrcodepath))
            {
                File.Delete(qrcodepath);
            }


            //启动进程
            p = new Process();
            p.StartInfo.FileName = exePah;
            p.StartInfo.WorkingDirectory = workingDir;
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序 
            LogUtils.Loger("程序已启动");
            //向cmd窗口发送输入信息
            //p.StandardInput.WriteLine("exit");

            //p.StandardInput.AutoFlush = true;
            //p.StandardInput.WriteLine("exit");
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



            //获取cmd窗口的输出信息
            //string output = p.StandardOutput.ReadToEnd();

            ConsoleOutput = p.StandardOutput;
            ConsoleError = p.StandardError;
            //执行命令


            //string line=reader.ReadLine();
            new Task(() =>
            {
                while (!ConsoleOutput.EndOfStream)
                {
                    string line = ConsoleOutput.ReadLine();
                    sbConsole.AppendLine(line);
                }

            }).Start();

            new Task(() =>
            {
                while (!ConsoleError.EndOfStream)
                {
                    string line = ConsoleError.ReadLine();
                    sbError.AppendLine(line);
                    LogUtils.Loger("Error信息:" + line);
                }
            }).Start();
            LogUtils.Loger("日志监听已启动");

            //p.CloseMainWindow();
            //p.Close();

        }


        public static void KillJavaProcess(string command)
        {
            Console.WriteLine("请输⼊要执⾏的命令:");
            Process p = new Process();
            //设置要启动的应⽤程序
            p.StartInfo.FileName = "cmd.exe";
            //是否使⽤操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 接受来⾃调⽤程序的输⼊信息
            p.StartInfo.RedirectStandardInput = true;
            //输出信息
            p.StartInfo.RedirectStandardOutput = true;
            // 输出错误
            p.StartInfo.RedirectStandardError = true;
            //不显⽰程序窗⼝
            p.StartInfo.CreateNoWindow = true;
            //启动程序
            p.Start();
            //向cmd窗⼝发送输⼊信息
            p.StandardInput.WriteLine(command);
            p.StandardInput.AutoFlush = true;
            //获取输出信息
            p.Close();
        }
    }
}