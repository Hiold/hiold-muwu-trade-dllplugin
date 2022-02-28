using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.UserTools
{
    public class LocalizationUtils
    {
        public static string getTranslate(string key)
        {
            //AccessTools.Field<>(typeof(Localization), "mDictionary");
            Dictionary<string, string[]> trs = Traverse.Create(typeof(Localization)).Field<Dictionary<string, string[]>>("mDictionary").Value;
            trs.TryGetValue(key, out string[] ttl);
            if (ttl!=null&&ttl.Length >= 17)
            {
                return ttl[16];
            }

            return key;
        }
    }
}
