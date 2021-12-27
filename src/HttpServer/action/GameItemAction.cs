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

                //数据
                Dictionary<string, object> rs = new Dictionary<string, object>();


                //判断参数是否满足条件

                var query = Localization.dictionary.Where(x => x.Value.ContainsCaseInsensitive(itemname)).Select(p => p);

                foreach (var item in query)
                {

                    var query2 = ItemClass.list.Where(x => x.Name.Equals(item.Key)).Select(p => p);
                    foreach (ItemClass _item in query2)
                    {
                        string groupInfo = "";
                        foreach (string temp in _item.Groups)
                        {
                            groupInfo += temp + "|";
                        }
                        if (groupInfo.Length > 0)
                        {
                            groupInfo = groupInfo.Substring(0, groupInfo.Length - 1);
                        }



                        //returnResult += string.Format("{0},{1},{2},{3}", _item.Name, _item.CustomIcon, (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b), groupInfo);
                        rs.Add("itemname", _item.Name);
                        rs.Add("icon", _item.CustomIcon);
                        rs.Add("tint", (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b));
                        rs.Add("group", groupInfo);
                        rs.Add("translate", item.Value);
                        items.Add(rs);
                    }
                }

                var query3 = ItemClass.list.Where(x => x.Name.ContainsCaseInsensitive(itemname)).Select(p => p);

                foreach (var _item in query3)
                {
                    var query2 = Localization.dictionary.Where(x => x.Key.Equals(_item.Name)).Select(p => p);
                    foreach (var item in query2)
                    {
                        string groupInfo = "";
                        foreach (string temp in _item.Groups)
                        {
                            groupInfo += temp + "|";
                        }
                        if (groupInfo.Length > 0)
                        {
                            groupInfo = groupInfo.Substring(0, groupInfo.Length - 1);
                        }



                        //returnResult += string.Format("{0},{1},{2},{3}", _item.Name, _item.CustomIcon, (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b), groupInfo);
                        rs.Add("itemname", _item.Name);
                        rs.Add("icon", _item.CustomIcon);
                        rs.Add("tint", (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b));
                        rs.Add("group", groupInfo);
                        rs.Add("translate", item.Value);
                        items.Add(rs);
                    }
                }





                ResponseUtils.ResponseSuccessWithData(response, SimpleJson2.SimpleJson2.SerializeObject(items));



            }
            catch (Exception e)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.Source);
            }
        }
    }
}
