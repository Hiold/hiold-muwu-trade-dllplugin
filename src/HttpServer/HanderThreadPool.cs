using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer
{
    public class HanderThreadPool
    {
        public static void InitThreadPool()
        {
            ThreadProcessingPool tpp = new ThreadProcessingPool(20, 5);
            

        }
    }
}
