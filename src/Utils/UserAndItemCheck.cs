using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.CommonUtils
{
    public static class UserAndItemCheck
    {
        public static CheckResult CheckItem(string itemname)
        {
            return new CheckResult { validate = true, msg = "" };
        }
        public static CheckResult CheckUser(string steamid)
        {
            return new CheckResult { validate = true, msg = "" };
        }

        public static CheckResult CheckItemPrice(string itemname,double price)
        {
            return new CheckResult { validate = true, msg = "" };
        }


        public class CheckResult
        {
            public bool validate { get; set; }
            public string msg { get; set; }
        }
    }
}
