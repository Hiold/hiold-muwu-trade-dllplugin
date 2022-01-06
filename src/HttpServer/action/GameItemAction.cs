using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.action
{
    class GameItemAction
    {
        /// <summary>
        /// 获取系统物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getSystemItem(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                Dictionary<string, string> param = ServerUtils.GetParam(request);
                List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
                //声明查询参数
                string itemname = null;
                param.TryGetValue("itemname", out itemname);




                //判断参数是否满足条件

                var query = Localization.dictionary.Where(x => x.Value != null && x.Value.Length >= 17 && x.Value[16] != null && x.Value[16].ContainsCaseInsensitive(itemname));

                if (query != null)
                {
                    foreach (var item in query.ToList())
                    {
                        //数据
                        Dictionary<string, object> rs = new Dictionary<string, object>();
                        var query2 = ItemClass.list.Where(x => x != null && x.Name != null && x.Name.Equals(item.Key));
                        if (query2 != null)
                        {
                            foreach (ItemClass _item in query2.ToList())
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


                    }
                }


                var query3 = ItemClass.list.Where(x => x != null && x.Name.ContainsCaseInsensitive(itemname));

                if (query3 != null)
                {
                    foreach (var _item in query3.ToList())
                    {
                        //数据
                        Dictionary<string, object> rs = new Dictionary<string, object>();
                        var query2 = Localization.dictionary.Where(x => x.Key.Equals(_item.Name));
                        if (query2 != null)
                        {
                            foreach (var item in query2.ToList())
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

        /// <summary>
        /// 获取系统图标
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getImage(HttpListenerRequest request, HttpListenerResponse response)
        {
            DirectoryInfo di = new DirectoryInfo(API.AssemblyPath);
            string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Data/ItemIcons/";
            string url = request.RawUrl.Replace("/api/image/", "");
            response.ContentType = "image/png";
            try
            {
                if (API.isOnServer)
                {
                    basepath = di.Parent.Parent.FullName + "/Data/ItemIcons/";
                }


                FileStream fs = File.OpenRead(basepath + url);
                fs.CopyTo(response.OutputStream);
                fs.Flush();
                fs.Close();
                response.OutputStream.Flush();
                response.OutputStream.Close();
                // LogUtils.Loger(url);
            }
            catch (Exception e)
            {
                response.StatusCode = 404;
                response.OutputStream.Flush();
                response.OutputStream.Close();
                // LogUtils.Loger("读取文件异常:" + e.Message);
            }
        }

        /// <summary>
        /// 获取自定义图片
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getImageIcon(HttpListenerRequest request, HttpListenerResponse response)
        {
            string url = request.RawUrl.Replace("/api/iconImage/", "");
            response.ContentType = "image/png";
            string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/image/";
            try
            {
                if (API.isOnServer)
                {
                    basepath = string.Format("{0}/image/", API.AssemblyPath);
                }


                FileStream fs = File.OpenRead(basepath + url);
                fs.CopyTo(response.OutputStream);
                fs.Flush();
                fs.Close();
                response.OutputStream.Flush();
                response.OutputStream.Close();
                LogUtils.Loger(url);
            }
            catch (Exception e)
            {
                response.StatusCode = 404;
                response.OutputStream.Flush();
                response.OutputStream.Close();
                LogUtils.Loger("读取文件异常:" + e.Message);
            }
        }
    }
}
