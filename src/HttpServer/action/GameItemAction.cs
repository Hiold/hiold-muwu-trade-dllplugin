using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.attributes;
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
    [ActionAttribute]
    class GameItemAction
    {
        /// <summary>
        /// 获取系统物品
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, url = "/api/getSystemItem")]
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
        /// 
        //[RequestHandlerAttribute(IsServerReady = true, url = "/api/getImage")]
        public static void getImage(HioldRequest request, HttpListenerResponse response)
        {
            DirectoryInfo di = new DirectoryInfo(HioldMod.API.AssemblyPath);
            string basepath = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server/Data/ItemIcons/";
            string basepath2 = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server/Mods/";
            string url = request.request.RawUrl.Replace("/api/image/", "");
            response.ContentType = "image/png";
            try
            {
                if (HioldMod.API.isOnServer)
                {
                    basepath = di.Parent.Parent.FullName + "/Data/ItemIcons/";
                    basepath2 = di.Parent.Parent.FullName + "/Mods/";
                }


                //判断两个路径是否存在文件，均不存在返回404
                if (File.Exists(basepath + url))
                {
                    FileInfo fi = new FileInfo(basepath + url);
                    string modified = request.request.Headers.Get("if-modified-since");
                    //Console.WriteLine(modified);
                    if (modified != null && DateTime.Parse(modified).ToString().Equals(fi.LastWriteTime.ToString()))
                    {
                        response.StatusCode = 304;
                        response.OutputStream.Flush();
                        response.OutputStream.Close();
                        return;
                    }
                    else
                    {
                        response.Headers.Add("Last-Modified", fi.LastWriteTime.ToString());
                    }

                    FileStream fs = File.OpenRead(basepath + url);
                    fs.CopyTo(response.OutputStream);
                    fs.Flush();
                    fs.Close();
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                }
                else
                {

                    string[] files = Directory.GetFiles(basepath2, url, SearchOption.AllDirectories);
                    if (files != null && files.Length > 0)
                    {
                        FileInfo fi = new FileInfo(files[0]);
                        string modified = request.request.Headers.Get("if-modified-since");
                        //Console.WriteLine(modified);
                        if (modified != null && DateTime.Parse(modified).ToString().Equals(fi.LastWriteTime.ToString()))
                        {
                            response.StatusCode = 304;
                            response.OutputStream.Flush();
                            response.OutputStream.Close();
                            return;
                        }
                        else
                        {
                            response.Headers.Add("Last-Modified", fi.LastWriteTime.ToString());
                        }
                        FileStream fs = File.OpenRead(files[0]);
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



            }
            catch (Exception e)
            {
                response.StatusCode = 404;
                response.OutputStream.Flush();
                response.OutputStream.Close();
                // LogUtils.Loger("读取文件异常:" + e.Message);
            }
        }

        public static string TryGetItemIconFile(string itemname)
        {
            string basepathSystemIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server\Data\ItemIcons\";
            string basepathModIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server\Mods\";
            string basepathcustomIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server\Mods\hiold-muwu-trade-dllplugin_funcs\customimage\";
            string filepath = "";
            //替换basepath路径
            if (HioldMod.API.isOnServer)
            {
                DirectoryInfo di = new DirectoryInfo(HioldMod.API.AssemblyPath);
                basepathSystemIcon = di.Parent.Parent.FullName + "/Data/ItemIcons/";
                basepathModIcon = di.Parent.Parent.FullName + "/Mods/";
                basepathcustomIcon = di.Parent.Parent.FullName + "/Mods/hiold-muwu-trade-dllplugin_funcs/customimage/";
            }

            if (File.Exists(basepathSystemIcon + itemname + ".png"))
            {
                filepath = basepathSystemIcon + itemname + ".png";
            }
            //在自定义icon库中
            else if (File.Exists(basepathcustomIcon + itemname + ".png"))
            {
                filepath = basepathcustomIcon + itemname + ".png";
            }
            else
            {
                try
                {
                    string[] files = Directory.GetFiles(basepathModIcon, itemname + ".png", SearchOption.AllDirectories);
                    //在拓展mod库中
                    if (files != null && files.Length > 0)
                    {
                        filepath = files[0];
                    }
                }
                catch (Exception)
                {
                    filepath = "";
                }
            }
            return filepath;
        }

        public static string TryGetItemIconFileOrg(string itemname)
        {
            string basepathSystemIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server\Data\ItemIcons\";
            string basepathModIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server\Mods\";
            string basepathcustomIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server\Mods\hiold-muwu-trade-dllplugin_funcs\customimage\";
            string filepath = "";
            //替换basepath路径
            if (HioldMod.API.isOnServer)
            {
                DirectoryInfo di = new DirectoryInfo(HioldMod.API.AssemblyPath);
                basepathSystemIcon = di.Parent.Parent.FullName + "/Data/ItemIcons/";
                basepathModIcon = di.Parent.Parent.FullName + "/Mods/";
                basepathcustomIcon = di.Parent.Parent.FullName + "/Mods/hiold-muwu-trade-dllplugin_funcs/customimage/";
            }

            if (File.Exists(basepathSystemIcon + itemname))
            {
                filepath = basepathSystemIcon + itemname;
            }
            //在自定义icon库中
            else if (File.Exists(basepathcustomIcon + itemname))
            {
                filepath = basepathcustomIcon + itemname;
            }
            else
            {
                try
                {
                    string[] files = Directory.GetFiles(basepathModIcon, itemname, SearchOption.AllDirectories);
                    //在拓展mod库中
                    if (files != null && files.Length > 0)
                    {
                        filepath = files[0];
                    }
                }
                catch (Exception)
                {
                    filepath = "";
                }
            }
            return filepath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        public static void getImageTint(HioldRequest request, HttpListenerResponse response)
        {
            DirectoryInfo di = new DirectoryInfo(HioldMod.API.AssemblyPath);
            string basepathSystemIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server/Data/ItemIcons/";
            string basepathModIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server/Mods/";
            string basepathcustomIcon = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/customimage/";
            string itemname = request.request.RawUrl.Replace("/api/getimagetint/", "");

            //替换数据
            if (itemname.EndsWith(".png"))
            {
                itemname = itemname.Replace(".png", "");
            }
            string itemnameOrg = ServerUtils.UrlDecode(itemname);
            itemname = itemnameOrg.Replace(":", "-");
            response.ContentType = "image/png";
            try
            {
                //替换basepath路径
                if (HioldMod.API.isOnServer)
                {
                    basepathSystemIcon = di.Parent.Parent.FullName + "/Data/ItemIcons/";
                    basepathModIcon = di.Parent.Parent.FullName + "/Mods/";
                    basepathcustomIcon = di.Parent.Parent.FullName + "/Mods/hiold-muwu-trade-dllplugin_funcs/customimage/";
                }


                string filepath = TryGetItemIconFile(itemname);
                if (string.IsNullOrEmpty(filepath))
                {
                    filepath = TryGetItemIconFileOrg(itemname);
                }
                //LogUtils.Loger(itemname + "获取到路径为:" + filepath);
                if (string.IsNullOrEmpty(filepath))
                {
                    ItemClass _class = ItemClass.GetItemClass(itemnameOrg, true);
                    if (_class != null)
                    {
                        string newfilename = _class.CustomIcon.Value + ".png";
                        //LogUtils.Loger("获取到文件名为:" + newfilename);
                        string newiconpath = TryGetItemIconFile(newfilename);
                        //LogUtils.Loger("获取到文件路径为:" + filepath);
                        if (!string.IsNullOrEmpty(newiconpath))
                        {
                            double mr = (_class.CustomIconTint == null || _class.CustomIconTint.r == null) ? 1 : _class.CustomIconTint.r;
                            double mg = (_class.CustomIconTint == null || _class.CustomIconTint.g == null) ? 1 : _class.CustomIconTint.g;
                            double mb = (_class.CustomIconTint == null || _class.CustomIconTint.b == null) ? 1 : _class.CustomIconTint.b;
                            //计算图标
                            System.Drawing.Bitmap iconbitmap = ServerUtils.loadItemIcon(newiconpath, mr, mg, mb);
                            //判断文件夹是否存在
                            if (!Directory.Exists(basepathcustomIcon))
                            {
                                Directory.CreateDirectory(basepathcustomIcon);
                            }
                            //保存图片
                            iconbitmap.Save(basepathcustomIcon + itemname + ".png");
                            //赋值图片路径数据
                            filepath = basepathcustomIcon + itemname + ".png";

                        }
                        else
                        {
                            filepath = basepathSystemIcon + "missingIcon.png";
                        }
                    }
                    else
                    //没有找到对应ItemClass
                    {
                        filepath = basepathSystemIcon + "missingIcon.png";
                    }
                }



                FileInfo fi = new FileInfo(filepath);
                string modified = request.request.Headers.Get("if-modified-since");
                //Console.WriteLine(modified);
                if (modified != null && DateTime.Parse(modified).ToString().Equals(fi.LastWriteTime.ToString()))
                {
                    response.StatusCode = 304;
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                    return;
                }
                else
                {
                    response.Headers.Add("Last-Modified", fi.LastWriteTime.ToString());
                }

                FileStream fs = File.OpenRead(filepath);
                fs.CopyTo(response.OutputStream);
                fs.Flush();
                fs.Close();
                response.OutputStream.Flush();
                response.OutputStream.Close();




            }
            catch (Exception e)
            {
                response.StatusCode = 404;
                response.OutputStream.Flush();
                response.OutputStream.Close();
                LogUtils.Loger("读取文件异常:" + e.Message);
                LogUtils.Loger(e.StackTrace);
            }
        }

        /// <summary>
        /// 获取自定义图片
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">响应</param>
        /// 
        [RequestHandlerAttribute(IsServerReady = true, url = "/api/getImageIcon")]
        public static void getImageIcon(HioldRequest request, HttpListenerResponse response)
        {
            string url = request.request.RawUrl.Replace("/api/iconImage/", "");
            response.ContentType = "image/png";
            string basepath = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/image/";
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
        /// 
        public static void getStaticSource(HioldRequest request, HttpListenerResponse response)
        {
            //string url = request.request.RawUrl.Replace("/api/iconImage/", "");
            //response.ContentType = "image/png";
            string basepath = @"E:\SteamLibrary\steamapps\common\7 Days to Die Dedicated Server/Mods/hiold-muwu-trade-dllplugin_funcs/web";
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
                FileInfo fi = new FileInfo(basepath + request.request.RawUrl + appendUrl);
                string modified = request.request.Headers.Get("if-modified-since");
                //Console.WriteLine(modified);
                if (modified != null && DateTime.Parse(modified).ToString().Equals(fi.LastWriteTime.ToString()))
                {
                    response.StatusCode = 304;
                    response.OutputStream.Flush();
                    response.OutputStream.Close();
                    return;
                }
                else
                {
                    response.Headers.Add("Last-Modified", fi.LastWriteTime.ToString());
                }
                //比对失败回写文件文件
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
        /// 
        [RequestHandlerAttribute(IsServerReady = true, url = "/api/getSystemItemByPage")]
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
                                    string[] cache = item.Value;
                                    string chinese = "";
                                    if (cache[16] != null)
                                    {
                                        chinese = cache[16];
                                    }
                                    else if (cache[4] != null)
                                    {
                                        chinese = cache[4];
                                    }
                                    else
                                    {
                                        foreach (string tmp in cache)
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
                                            chinese = itemN;
                                        }
                                    }

                                    if (cache.Length < 16)
                                    {
                                        cache = new string[17];
                                    }
                                    cache[16] = chinese;
                                    rs.Add("translate", cache);
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
