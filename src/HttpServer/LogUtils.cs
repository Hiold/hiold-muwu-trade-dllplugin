using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.HttpServer
{
    class LogUtils
    {
        public static void Loger(string msg)
        {
            if (HioldMod.API.isDebug)
            {
                if (HioldMod.API.isOnServer) { 
                Log.Out(msg);
                }
                else
                {
                    Console.WriteLine(msg);
                }
            }
        }
    }
}
