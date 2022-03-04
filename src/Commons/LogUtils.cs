using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.HttpServer
{
    public class LogUtils
    {
        public static string HioldModMsgPrefix = "[HioldMuwu] ";
        public static void Loger(string msg)
        {
            if (HioldMod.API.isDebug)
            {
                if (HioldMod.API.isOnServer)
                {
                    Log.Out(HioldModMsgPrefix + msg);
                }
                else
                {
                    Console.WriteLine(HioldModMsgPrefix + msg);
                }
            }
        }
    }
}
