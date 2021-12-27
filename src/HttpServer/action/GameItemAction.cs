using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class GameItemAction
    {
        public static void getSystemItem(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                Dictionary<string, string> param = ServerUtils.GetParam(request);
                List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
                //声明查询参数
                string itemname = null;
                param.TryGetValue("itemname", out itemname);



                //遍历所有物品
                foreach (ItemClass _item in ItemClass.list)
                {
                    if (_item != null)
                    {

                        //判断参数是否满足条件
                        if (itemname != null && _item.Name != null && _item.Name != "" && !_item.Name.ContainsCaseInsensitive(itemname))
                        {
                            //不满足则跳过
                            continue;
                        }

                        


                        Dictionary<string, object> rs = new Dictionary<string, object>();
                        string groupInfo = "";
                        foreach (string temp in _item.Groups)
                        {
                            groupInfo += temp + "|";
                        }
                        if (groupInfo.Length > 0)
                        {
                            groupInfo = groupInfo.Substring(0, groupInfo.Length - 1);
                        }


                        //是否翻译
                        if (param.TryGetValue("translate", out string istranslate))
                        {
                            if (istranslate.Equals("true"))
                            {
                                if (Localization.dictionary.TryGetValue(itemname, out string[] trData))
                                {
                                    rs["value"] = trData;
                                }
                                else
                                {
                                    rs["value"] = "";
                                }
                            }
                        }


                        //returnResult += string.Format("{0},{1},{2},{3}", _item.Name, _item.CustomIcon, (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b), groupInfo);
                        rs.Add("itemname", _item.Name);
                        rs.Add("icon", _item.CustomIcon);
                        rs.Add("tint", (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b));
                        rs.Add("group", groupInfo);
                        items.Add(rs);



                    }
                }

                ResponseUtils.ResponseSuccessWithData(response, SimpleJson2.SimpleJson2.SerializeObject(items));



            }
            catch (Exception e)
            {
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.Source);
            }
        }
    }
}
