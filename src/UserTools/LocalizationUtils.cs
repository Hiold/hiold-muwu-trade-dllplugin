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
        public static Dictionary<string, string[]> TranslateCache = new Dictionary<string, string[]>();
        public static string getTranslate(string key)
        {
            //AccessTools.Field<>(typeof(Localization), "mDictionary");
            //读取缓存
            if (TranslateCache.TryGetValue(key, out string[] cacheHit))
            {
                return cacheHit[16];
            }

            //没有缓存从系统中进行查找
            Dictionary<string, string[]> trs = Traverse.Create(typeof(Localization)).Field<Dictionary<string, string[]>>("mDictionary").Value;
            trs.TryGetValue(key, out string[] ttl);
            if (ttl != null && ttl.Length >= 17)
            {
                //存放缓存
                TranslateCache.Add(key, ttl);
                return ttl[16];
            }
            return key;
        }

        public static string getDesc(string key)
        {
            //AccessTools.Field<>(typeof(Localization), "mDictionary");
            //读取缓存
            if (TranslateCache.TryGetValue(key, out string[] cacheHit))
            {
                return cacheHit[16];
            }

            //没有缓存从系统中进行查找
            Dictionary<string, string[]> trs = Traverse.Create(typeof(Localization)).Field<Dictionary<string, string[]>>("mDictionary").Value;
            trs.TryGetValue(key, out string[] ttl);
            if (ttl != null && ttl.Length >= 17)
            {
                //存放缓存
                TranslateCache.Add(key, ttl);
                return ttl[16];
            }
            return "";
        }
    }
}
