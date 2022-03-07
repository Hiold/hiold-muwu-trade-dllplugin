using HarmonyLib;
using HioldMod.HttpServer;
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

        public static Dictionary<string, string> reverseTraslate = new Dictionary<string, string>();

        public static void InitReverseTranslate()
        {
            Dictionary<string, string[]> trs = Traverse.Create(typeof(Localization)).Field<Dictionary<string, string[]>>("mDictionary").Value;
            foreach (KeyValuePair<string, string[]> kvp in trs)
            {
                if (kvp.Value != null && kvp.Value.Length >= 17 && !string.IsNullOrEmpty(kvp.Value[16]))
                {
                    ItemValue iv = ItemClass.GetItem(kvp.Key);
                    if (iv != null)
                    {
                        ItemClass _item = iv.ItemClass;
                        if (_item != null)
                        {
                            if (!reverseTraslate.TryGetValue(kvp.Value[16], out string existV))
                            {

                                reverseTraslate.Add(kvp.Value[16], kvp.Key);
                            }
                            else
                            {
                                reverseTraslate[kvp.Value[16]] = existV + "," + kvp.Key;
                            }
                        }
                    }
                }
            }
            LogUtils.Loger("反向查找字典已初始化阶段一，总数量为：" + reverseTraslate.Count);
            foreach (ItemClass ic in ItemClass.list)
            {
                if (ic != null)
                {
                    if (!reverseTraslate.TryGetValue(ic.GetItemName(), out string tmp))
                    {
                        if (!trs.TryGetValue(ic.GetItemName(), out string[] mff))
                        {
                            reverseTraslate.Add(ic.GetItemName(), ic.GetItemName());
                        }
                    }
                }
            }
            LogUtils.Loger("反向查找字典已初始化阶段二，总数量为：" + reverseTraslate.Count);
        }

        public static List<string> searchItem(string itemname)
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, string> ms in reverseTraslate)
            {
                if (ms.Key.ContainsCaseInsensitive(itemname))
                {
                    result.Add(ms.Value);
                }
            }
            return result;
        }

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
