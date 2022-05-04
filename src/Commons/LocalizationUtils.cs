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
                if (kvp.Value != null)
                {
                    ItemValue iv = ItemClass.GetItem(kvp.Key);
                    if (iv != null)
                    {
                        string chinese = "";
                        if (kvp.Value[16] != null)
                        {
                            chinese = kvp.Value[16];
                        }
                        else if (kvp.Value[4] != null)
                        {
                            chinese = kvp.Value[4];
                        }
                        else
                        {
                            foreach (string tmp in kvp.Value)
                            {
                                if (tmp != null)
                                {
                                    chinese = tmp;
                                    break;
                                }
                            }
                            //仍未找到不在翻译
                            if (chinese.Equals(""))
                            {
                                chinese = kvp.Key;
                            }
                        }

                        if (!reverseTraslate.TryGetValue(chinese, out string existV))
                        {

                            reverseTraslate.Add(chinese, kvp.Key);
                        }
                        else
                        {
                            reverseTraslate[chinese] = existV + "," + kvp.Key;
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
                string chinese = "";
                if (cacheHit[16] != null)
                {
                    chinese = cacheHit[16];
                }
                else if (cacheHit[4] != null)
                {
                    chinese = cacheHit[4];
                }
                else
                {
                    foreach (string tmp in cacheHit)
                    {
                        if (tmp != null)
                        {
                            chinese = tmp;
                            break;
                        }
                    }
                    //仍未找到不在翻译
                    if (chinese.Equals(""))
                    {
                        chinese = key;
                    }
                }
                return chinese;
            }

            //没有缓存从系统中进行查找
            Dictionary<string, string[]> trs = Traverse.Create(typeof(Localization)).Field<Dictionary<string, string[]>>("mDictionary").Value;
            trs.TryGetValue(key, out string[] ttl);
            if (ttl != null && ttl.Length >= 17)
            {
                //存放缓存
                TranslateCache.Add(key, ttl);
                string chinese = "";
                if (ttl[16] != null)
                {
                    chinese = ttl[16];
                }
                else if (ttl[4] != null)
                {
                    chinese = ttl[4];
                }
                else
                {
                    foreach (string tmp in ttl)
                    {
                        if (tmp != null)
                        {
                            chinese = tmp;
                            break;
                        }
                    }
                    //仍未找到不在翻译
                    if (chinese.Equals(""))
                    {
                        chinese = key;
                    }
                }
                return chinese;
            }
            return key;
        }

        public static string getDesc(string key)
        {
            //AccessTools.Field<>(typeof(Localization), "mDictionary");
            //读取缓存
            if (TranslateCache.TryGetValue(key, out string[] cacheHit))
            {
                string chinese = "";
                if (cacheHit[16] != null)
                {
                    chinese = cacheHit[16];
                }
                else if (cacheHit[4] != null)
                {
                    chinese = cacheHit[4];
                }
                else
                {
                    foreach (string tmp in cacheHit)
                    {
                        if (tmp != null)
                        {
                            chinese = tmp;
                            break;
                        }
                    }
                    //仍未找到不在翻译
                    if (chinese.Equals(""))
                    {
                        chinese = key;
                    }
                }
                return chinese;
            }

            //没有缓存从系统中进行查找
            Dictionary<string, string[]> trs = Traverse.Create(typeof(Localization)).Field<Dictionary<string, string[]>>("mDictionary").Value;
            trs.TryGetValue(key, out string[] ttl);
            if (ttl != null && ttl.Length >= 17)
            {
                //存放缓存
                TranslateCache.Add(key, ttl);
                string chinese = "";
                if (ttl[16] != null)
                {
                    chinese = ttl[16];
                }
                else if (ttl[4] != null)
                {
                    chinese = ttl[4];
                }
                else
                {
                    foreach (string tmp in ttl)
                    {
                        if (tmp != null)
                        {
                            chinese = tmp;
                            break;
                        }
                    }
                    //仍未找到不在翻译
                    if (chinese.Equals(""))
                    {
                        chinese = key;
                    }
                }
                return chinese;
            }
            return "";
        }
    }
}
