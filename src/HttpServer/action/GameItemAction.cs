using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.common;
using HioldMod.src.UserTools;
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
        public static void getSystemItem(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                Dictionary<string, string> param = ServerUtils.GetParam(request.request);
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
                return;


            }
            catch (Exception e)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.Source);
                return;
            }
        }

        /// <summary>
        /// 获取系统图标
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getImage(HioldRequest request, HttpListenerResponse response)
        {
            DirectoryInfo di = new DirectoryInfo(HioldMod.API.AssemblyPath);
            string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Data/ItemIcons/";
            string basepath2 = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/image/";
            string url = request.request.RawUrl.Replace("/api/image/", "");
            response.ContentType = "image/png";
            try
            {
                if (HioldMod.API.isOnServer)
                {
                    basepath = di.Parent.Parent.FullName + "/Data/ItemIcons/";
                    basepath2 = string.Format("{0}/image/", HioldMod.API.AssemblyPath);
                }

                //判断两个路径是否存在文件，均不存在返回404
                if (File.Exists(basepath + url))
                {
                    FileStream fs = File.OpenRead(basepath + url);
                    fs.CopyTo(response.OutputStream);
                    fs.Flush();
                    fs.Close();
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                }
                else if (File.Exists(basepath2 + url))
                {
                    FileStream fs = File.OpenRead(basepath2 + url);
                    fs.CopyTo(response.OutputStream);
                    fs.Flush();
                    fs.Close();
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                }
                else
                {
                    response.StatusCode = 404;
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                }


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
        public static void getImageIcon(HioldRequest request, HttpListenerResponse response)
        {
            string url = request.request.RawUrl.Replace("/api/iconImage/", "");
            response.ContentType = "image/png";
            string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/image/";
            try
            {
                if (HioldMod.API.isOnServer)
                {
                    basepath = string.Format("{0}/image/", HioldMod.API.AssemblyPath);
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



        /// <summary>
        /// 获取静态资源
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getStaticSource(HioldRequest request, HttpListenerResponse response)
        {
            //string url = request.request.RawUrl.Replace("/api/iconImage/", "");
            //response.ContentType = "image/png";
            string basepath = "D:/Steam/steamapps/common/7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/web";
            try
            {
                if (HioldMod.API.isOnServer)
                {
                    basepath = string.Format("{0}/web", HioldMod.API.AssemblyPath);
                }

                //application/javascript
                if (request.request.RawUrl.Contains(".js"))
                {
                    response.ContentType = "application/javascript";
                }
                //text/css
                if (request.request.RawUrl.Contains(".css"))
                {
                    //response.AddHeader("max-age", "86400");
                    response.ContentType = "text/css";
                }
                //image/x-icon
                if (request.request.RawUrl.Contains(".ico"))
                {
                    //response.AddHeader("max-age", "8640000");
                    response.ContentType = "image/x-icon";
                }
                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Methods", "POST,OPTIONS,GET");
                response.AddHeader("Access-Control-Allow-Headers", "accept,x-requested-with,Content-Type,X-Custom-Header");
                response.AddHeader("Access-Control-Allow-Credentials", "true");
                response.AddHeader("Access-Control-Max-Age", "3600");
                //max-age:86400
                //if (request.request.RawUrl.Contains(".png") || request.request.RawUrl.Contains(".jpg") || request.request.RawUrl.Contains(".mov"))
                //{
                //    response.AddHeader("max-age", "86400");
                //}
                string appendUrl = "";
                if (request.request.RawUrl.Equals("/"))
                {
                    appendUrl += "index.html";
                }
                FileStream fs = File.OpenRead(basepath + request.request.RawUrl + appendUrl);
                fs.CopyTo(response.OutputStream);
                fs.Flush();
                fs.Close();
                response.OutputStream.Flush();
                response.OutputStream.Close();
                //LogUtils.Loger(basepath + request.request.RawUrl);
            }
            catch (Exception e)
            {
                response.StatusCode = 404;
                response.OutputStream.Flush();
                response.OutputStream.Close();
                LogUtils.Loger("读取文件异常:" + e.Message);
            }
        }




        /// <summary>
        /// 获取系统物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        public static void getSystemItemByPage(HioldRequest request, HttpListenerResponse response)
        {
            try
            {
                Dictionary<string, string> param = ServerUtils.GetParam(request.request);
                List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();
                //声明查询参数
                string itemname = "";
                int skip = 0;
                int take = 50;
                param.TryGetValue("skip", out string skipStr);
                param.TryGetValue("take", out string takeStr);
                int.TryParse(skipStr, out skip);
                int.TryParse(takeStr, out take);
                param.TryGetValue("itemname", out itemname);


                //判断参数是否满足条件


                //if (itemname != null && itemname != "")
                //{
                //    var query = Localization.dictionary.Where(x => x.Value != null && x.Value.Length >= 17 && x.Value[16] != null && x.Value[16].ContainsCaseInsensitive(itemname));
                //    foreach (var item in query.ToList())
                //    {
                //        //数据
                //        Dictionary<string, object> rs = new Dictionary<string, object>();
                //        var query2 = ItemClass.list.Where(x => x != null && x.Name != null && x.Name.Equals(item.Key));
                //        if (query2 != null)
                //        {
                //            foreach (ItemClass _item in query2.ToList())
                //            {
                //                string groupInfo = "";
                //                foreach (string temp in _item.Groups)
                //                {
                //                    groupInfo += temp + "|";
                //                }
                //                if (groupInfo.Length > 0)
                //                {
                //                    groupInfo = groupInfo.Substring(0, groupInfo.Length - 1);
                //                }



                //                //returnResult += string.Format("{0},{1},{2},{3}", _item.Name, _item.CustomIcon, (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b), groupInfo);
                //                rs.Add("itemname", _item.Name);
                //                rs.Add("icon", _item.CustomIcon);
                //                rs.Add("tint", (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b));
                //                rs.Add("group", groupInfo);
                //                rs.Add("translate", item.Value);
                //                items.Add(rs);
                //            }
                //        }


                //    }
                //}



                //if (itemname != null && itemname != "")
                //{
                //    var query3 = ItemClass.list.Where(x => x != null && x.Name.ContainsCaseInsensitive(itemname));
                //    if (query3 != null)
                //    {
                //        foreach (var _item in query3.ToList())
                //        {
                //            //数据
                //            Dictionary<string, object> rs = new Dictionary<string, object>();
                //            var query2 = Localization.dictionary.Where(x => x.Key.Equals(_item.Name));
                //            if (query2 != null)
                //            {
                //                foreach (var item in query2.ToList())
                //                {
                //                    string groupInfo = "";
                //                    foreach (string temp in _item.Groups)
                //                    {
                //                        groupInfo += temp + "|";
                //                    }
                //                    if (groupInfo.Length > 0)
                //                    {
                //                        groupInfo = groupInfo.Substring(0, groupInfo.Length - 1);
                //                    }



                //                    //returnResult += string.Format("{0},{1},{2},{3}", _item.Name, _item.CustomIcon, (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b), groupInfo);
                //                    rs.Add("itemname", _item.Name);
                //                    rs.Add("icon", _item.CustomIcon);
                //                    rs.Add("tint", (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b));
                //                    rs.Add("group", groupInfo);
                //                    rs.Add("translate", item.Value);
                //                    items.Add(rs);
                //                }
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    LogUtils.Loger("总长度为：" + ItemClass.list.Length);
                //    if (ItemClass.list.Length > (skip + take))
                //    {
                //        foreach (var _item in ItemClass.list.ToList().GetRange(skip, take))
                //        {
                //            if (_item != null)
                //            {
                //                //数据
                //                Dictionary<string, object> rs = new Dictionary<string, object>();
                //                try
                //                {
                //                    var query2 = Localization.dictionary.Where(x => x.Key.Equals(_item.Name));
                //                    if (query2 != null)
                //                    {
                //                        foreach (var item in query2.ToList())
                //                        {
                //                            rs.Add("translate", item.Value);
                //                        }
                //                    }
                //                }
                //                catch (Exception)
                //                {

                //                }

                //                string groupInfo = "";
                //                foreach (string temp in _item.Groups)
                //                {
                //                    groupInfo += temp + "|";
                //                }
                //                if (groupInfo.Length > 0)
                //                {
                //                    groupInfo = groupInfo.Substring(0, groupInfo.Length - 1);
                //                }
                //                rs.Add("itemname", _item.Name);
                //                rs.Add("icon", _item.CustomIcon);
                //                rs.Add("tint", (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b));
                //                rs.Add("group", groupInfo);
                //                items.Add(rs);
                //            }
                //        }
                //    }
                //}
                List<string> ms = LocalizationUtils.searchItem(itemname);
                LogUtils.Loger("查询出结果长度:" + ms.Count);
                try
                {
                    ms = ms.GetRange(skip, take);
                }
                catch (Exception)
                {

                }

                foreach (string itemN in ms)
                {
                    ItemClass _item = ItemClass.GetItem(itemN).ItemClass;
                    if (_item != null)
                    {
                        //数据
                        Dictionary<string, object> rs = new Dictionary<string, object>();
                        try
                        {
                            var query2 = Localization.dictionary.Where(x => x.Key.Equals(_item.Name));
                            if (query2 != null)
                            {
                                foreach (var item in query2.ToList())
                                {
                                    rs.Add("translate", item.Value);
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }

                        string groupInfo = "";
                        foreach (string temp in _item.Groups)
                        {
                            groupInfo += temp + "|";
                        }
                        if (groupInfo.Length > 0)
                        {
                            groupInfo = groupInfo.Substring(0, groupInfo.Length - 1);
                        }
                        rs.Add("itemname", _item.Name);
                        rs.Add("icon", _item.CustomIcon);
                        rs.Add("tint", (_item.CustomIconTint.a + "|" + _item.CustomIconTint.r + "|" + _item.CustomIconTint.g + "|" + _item.CustomIconTint.b));
                        rs.Add("group", groupInfo);
                        items.Add(rs);
                    }
                }

                ResponseUtils.ResponseSuccessWithData(response, SimpleJson2.SimpleJson2.SerializeObject(items));
                return;


            }
            catch (Exception e)
            {
                ResponseUtils.ResponseFail(response, "参数异常");
                LogUtils.Loger(e.Message);
                LogUtils.Loger(e.StackTrace);
                LogUtils.Loger(e.Source);
                return;
            }
        }
    }
}
