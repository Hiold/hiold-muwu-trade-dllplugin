using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    class SpecialItem : ShopItem
    {
        public string couCurrType { get; set; }
        public string couPrice { get; set; }
        public string couCond { get; set; }
        public DateTime couDateStart { get; set; }
        public DateTime couDateEnd { get; set; }
        public string count { get; set; }
    }
}
