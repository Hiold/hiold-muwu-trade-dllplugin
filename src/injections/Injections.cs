using HarmonyLib;
using HioldMod;
using HioldMod.src.UserTools;
using Noemax.GZip;
using Pathfinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEngine;
using static ConfigTools.LoadMainConfig;

public static class Injections
{
    public static bool TELockServer_PostFix(int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt)
    {
        try
        {
            //Log.Out(string.Format("打开箱子位置: {0} {1} {2}", _blockPos.x, _blockPos.y, _blockPos.z));
            //Log.Out(string.Format("打开箱子clrIdx: {0}", _clrIdx));
            //Log.Out(string.Format("打开箱子lootEntityId: {0}", _lootEntityId));
            //Log.Out(string.Format("打开箱子entityIdThatOpenedIt: {0}", _entityIdThatOpenedIt));
            // Vector3i tmpPos = new Vector3i(_blockPos.x, _blockPos.y, _blockPos.z);
            // Auction.opendLootList[_blockPos.x + _blockPos.y + _blockPos.z + ""] = tmpPos;
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[HioldMod] 注入GameManager.TELockServer失败: {0}", e.Message));
        }
        return true;
    }


    public static bool TEUnlockServer_PostFix(int _clrIdx, Vector3i _blockPos, int _lootEntityId)
    {
        try
        {
            //Log.Out(string.Format("打开箱子位置: {0} {1} {2}", _blockPos.x, _blockPos.y, _blockPos.z));
            //Log.Out(string.Format("打开箱子clrIdx: {0}", _clrIdx));
            //Log.Out(string.Format("打开箱子lootEntityId: {0}", _lootEntityId));
            try
            {
                // Vector3i tmp = Auction.opendLootList[_blockPos.x + _blockPos.y + _blockPos.z + ""];
                // Auction.opendLootList.TryRemove(_blockPos.x + _blockPos.y + _blockPos.z + "",out Vector3i temp);
                //Log.Out(string.Format("[HioldMod] 比对成功,移除LootopendList"));
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
        catch (Exception e)
        {
            Log.Out(string.Format("[HioldMod] 注入GameManager.TELockServer失败: {0}", e.Message));
        }
        return true;
    }


    public static void SendXmlsToClient(ClientInfo _cInfo)
    {

    }


    public static bool SendXmlsToClient_postfix(ClientInfo _cInfo)
    {
        Log.Out("正在向玩家发送xml");
        try
        {
            //以__instance实例创建Traverse           挖掘字段mood   取float类型值
            object[] Rootxmls = Traverse.Create(typeof(WorldStaticData)).Field("xmlsToLoad").GetValue<object[]>();
            for (int i = 0; i < Rootxmls.Length; i++)
            {
                object xmls = Rootxmls[i];
                string xmlname = Traverse.Create(xmls).Field("XmlName").GetValue<string>();
                //
                if (xmlname.Equals("XUi/windows"))
                {
                    MemoryStream ms = new MemoryStream(Traverse.Create(xmls).Field("CompressedXmlData").GetValue<byte[]>());
                    DeflateInputStream dis = new DeflateInputStream(ms);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(dis);
                    XmlNode targetNode = xmlDoc.SelectSingleNode("/windows/window[@name='HioldShopWindow']");
                    //如果存在 尝试移除数据
                    if (targetNode != null)
                    {
                        xmlDoc.SelectSingleNode("/windows").RemoveChild(targetNode);
                    }
                    //<window_group name="HioldshopWindows"> <window name="HioldShopWindow" /> </window_group>

                    //生成数据结构
                    //window
                    //window
                    XmlElement window = xmlDoc.CreateElement("window");
                    window.SetAttribute("name", "HioldShopWindow");
                    window.SetAttribute("anchor", "LeftTop");
                    window.SetAttribute("depth", "51");
                    window.SetAttribute("pos", "200,200");
                    window.SetAttribute("width", "800");
                    window.SetAttribute("height", "800");
                    window.SetAttribute("cursor_area", "true");
                    //rect
                    XmlElement rect = xmlDoc.CreateElement("rect");
                    rect.SetAttribute("name", "serverinfo");
                    rect.SetAttribute("controller", "ServerInfo");
                    rect.SetAttribute("name", "HioldShop");
                    //label1
                    XmlElement label1 = xmlDoc.CreateElement("label");
                    label1.SetAttribute("depth", "2");
                    label1.SetAttribute("pos", "520,-550");
                    label1.SetAttribute("text", "[D65E62]★--- [FFFFFF]点击此处网页链接，跳转至交易系统[D65E62] ---★");
                    label1.SetAttribute("width", "100%");
                    label1.SetAttribute("justify", "center");
                    label1.SetAttribute("font_size", "32");
                    rect.AppendChild(label1);
                    //添加背景色
                    XmlElement sprite = xmlDoc.CreateElement("sprite");
                    sprite.SetAttribute("depth", "8");
                    sprite.SetAttribute("name", "backgroundMain");
                    sprite.SetAttribute("sprite", "menu_empty3px");
                    sprite.SetAttribute("pos", "500,-510");
                    sprite.SetAttribute("width", "900");
                    sprite.SetAttribute("height", "400");
                    sprite.SetAttribute("color", "[black]");
                    sprite.SetAttribute("type", "sliced");
                    sprite.SetAttribute("fillcenter", "true");
                    sprite.SetAttribute("globalopacity", "true");
                    sprite.SetAttribute("globalopacitymod", "1");
                    rect.AppendChild(sprite);
                    //label2
                    XmlElement label2 = xmlDoc.CreateElement("label");
                    label2.SetAttribute("depth", "11");
                    label2.SetAttribute("pos", "550,-600");
                    label2.SetAttribute("width", "480");
                    label2.SetAttribute("height", "32");
                    label2.SetAttribute("name", "ServerWebsiteURL");
                    /*下面是网页链接*/
                    //label2.SetAttribute("text", "https://td.hiold.net/");
                    //生成动态码
                    string ncode = "";

                    if (HioldModServer.Server.userToken.ContainsValue(_cInfo.PlatformId.ReadablePlatformUserIdentifier))
                    {
                        foreach (string keytemp in HioldModServer.Server.userToken.Keys)
                        {
                            if (HioldModServer.Server.userToken[keytemp].Equals(_cInfo.PlatformId.ReadablePlatformUserIdentifier))
                            {
                                ncode = keytemp;
                            }
                        }
                    }
                    else
                    {
                        ncode = HioldMod.HttpServer.common.ServerUtils.GetRandomString(32);
                        HioldModServer.Server.userToken.Add(ncode, _cInfo.PlatformId.ReadablePlatformUserIdentifier);
                    }

                    int port = 26911;
                    string host = "localhost";

                    if (MainConfig.Host.Equals("auto"))
                    {
                        host = GamePrefs.GetString(EnumGamePrefs.ServerIP);

                    }
                    else
                    {
                        host = MainConfig.Host;
                    }

                    if (MainConfig.Port.Equals("auto"))
                    {
                        port = GamePrefs.GetInt(EnumGamePrefs.ServerPort) + 11;
                    }
                    else
                    {
                        int.TryParse(MainConfig.Port, out port);
                    }

                    label2.SetAttribute("text", "http://" + host + ":" + port + "/#/login?ncode=" + ncode);
                    label2.SetAttribute("justify", "left");
                    label2.SetAttribute("style", "press,hover");
                    label2.SetAttribute("font_size", "30");
                    label2.SetAttribute("pivot", "topleft");
                    label2.SetAttribute("upper_case", "false");
                    rect.AppendChild(label2);
                    //panel
                    XmlElement panel = xmlDoc.CreateElement("panel");
                    panel.SetAttribute("name", "content");
                    panel.SetAttribute("pos", "9999,9999");
                    panel.SetAttribute("height", "1");
                    panel.SetAttribute("depth", "1");
                    panel.SetAttribute("pivot", "center");
                    panel.SetAttribute("disableautobackground", "true");

                    //panel1label
                    XmlElement panellabel1 = xmlDoc.CreateElement("label");
                    panellabel1.SetAttribute("depth", "6");
                    panellabel1.SetAttribute("pos", "10,0");
                    panellabel1.SetAttribute("width", "547");
                    panellabel1.SetAttribute("height", "200");
                    panellabel1.SetAttribute("name", "ServerDescription");
                    panellabel1.SetAttribute("justify", "center");
                    panellabel1.SetAttribute("font_size", "32");
                    panellabel1.SetAttribute("pivot", "topleft");
                    panellabel1.SetAttribute("upper_case", "false");
                    panel.AppendChild(panellabel1);
                    //
                    XmlElement browsergameoptioninfo1 = xmlDoc.CreateElement("browsergameoptioninfo");
                    browsergameoptioninfo1.SetAttribute("name", "CurrentServerTime");
                    browsergameoptioninfo1.SetAttribute("title", "goServerTime");
                    XmlElement browsergameoptioninfo2 = xmlDoc.CreateElement("browsergameoptioninfo");
                    browsergameoptioninfo2.SetAttribute("name", "GameMode");
                    browsergameoptioninfo2.SetAttribute("title", "goGameMode");
                    browsergameoptioninfo2.SetAttribute("value_localization_prefix", "gm");
                    XmlElement browsergameoptioninfo3 = xmlDoc.CreateElement("browsergameoptioninfo");
                    browsergameoptioninfo3.SetAttribute("name", "StockSettings");
                    browsergameoptioninfo3.SetAttribute("title", "goStockSettings");
                    XmlElement browsergameoptioninfo4 = xmlDoc.CreateElement("browsergameoptioninfo");
                    browsergameoptioninfo4.SetAttribute("name", "StockFiles");
                    browsergameoptioninfo4.SetAttribute("title", "goStockFiles");
                    XmlElement browsergameoptioninfo5 = xmlDoc.CreateElement("browsergameoptioninfo");
                    browsergameoptioninfo5.SetAttribute("name", "RequiresMod");
                    browsergameoptioninfo5.SetAttribute("title", "goRequiresMod");
                    XmlElement browsergameoptioninfo6 = xmlDoc.CreateElement("browsergameoptioninfo");
                    browsergameoptioninfo6.SetAttribute("name", "IP");
                    browsergameoptioninfo6.SetAttribute("title", "goIp");
                    XmlElement browsergameoptioninfo7 = xmlDoc.CreateElement("browsergameoptioninfo");
                    browsergameoptioninfo7.SetAttribute("name", "Port");
                    browsergameoptioninfo7.SetAttribute("title", "goPort");
                    XmlElement browsergameoptioninfo8 = xmlDoc.CreateElement("browsergameoptioninfo");
                    browsergameoptioninfo8.SetAttribute("name", "Version");
                    browsergameoptioninfo8.SetAttribute("title", "goVersion");
                    panel.AppendChild(browsergameoptioninfo1);
                    panel.AppendChild(browsergameoptioninfo2);
                    panel.AppendChild(browsergameoptioninfo3);
                    panel.AppendChild(browsergameoptioninfo4);
                    panel.AppendChild(browsergameoptioninfo5);
                    panel.AppendChild(browsergameoptioninfo6);
                    panel.AppendChild(browsergameoptioninfo7);
                    panel.AppendChild(browsergameoptioninfo8);
                    rect.AppendChild(panel);
                    window.AppendChild(rect);


                    //插入目标节点
                    xmlDoc.SelectSingleNode("/windows").AppendChild(window);
                    //开始压缩
                    MemoryStream msencTarget = new MemoryStream(Encoding.UTF8.GetBytes(xmlDoc.InnerXml));
                    MemoryStream msenc = new MemoryStream();
                    DeflateOutputStream dos = new DeflateOutputStream(msenc, 3, true);
                    StreamUtils.StreamCopy(msencTarget, dos);

                    Log.Out("CompressedXmlData修改前长度为:" + Traverse.Create(xmls).Field("CompressedXmlData").GetValue<byte[]>().Length);
                    Traverse.Create(xmls).Field("CompressedXmlData").SetValue(msenc.ToArray());
                    Log.Out("CompressedXmlData修改后长度为:" + Traverse.Create(xmls).Field("CompressedXmlData").GetValue<byte[]>().Length);
                    //
                    //MemoryStream msenc2 = new MemoryStream(Traverse.Create(xmls).Field("CompressedXmlData").GetValue<byte[]>());
                    //DeflateInputStream dos2 = new DeflateInputStream(msenc2);
                    //FileStream fs = new FileStream("D:\\xmlCongif\\window.xml", FileMode.OpenOrCreate);
                    //dos2.CopyTo(fs);


                }
                //
                if (xmlname.Equals("XUi/xui"))
                {
                    MemoryStream ms = new MemoryStream(Traverse.Create(xmls).Field("CompressedXmlData").GetValue<byte[]>());
                    DeflateInputStream dis = new DeflateInputStream(ms);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(dis);
                    XmlNode targetNode = xmlDoc.SelectSingleNode("/xui/ruleset/window_group[@name='HioldshopWindows']");
                    //如果存在 尝试移除数据
                    if (targetNode != null)
                    {
                        xmlDoc.SelectSingleNode("/xui/ruleset").RemoveChild(targetNode);
                    }
                    //<window_group name="HioldshopWindows"> <window name="HioldShopWindow" /> </window_group>

                    //生成数据结构
                    XmlElement window_group = xmlDoc.CreateElement("window_group");
                    window_group.SetAttribute("name", "HioldshopWindows");
                    XmlElement window = xmlDoc.CreateElement("window");
                    window.SetAttribute("name", "HioldShopWindow");
                    window_group.AppendChild(window);
                    //插入目标节点
                    xmlDoc.SelectSingleNode("/xui/ruleset").AppendChild(window_group);
                    //修改目标数据

                    //开始压缩
                    MemoryStream msencTarget = new MemoryStream(Encoding.UTF8.GetBytes(xmlDoc.InnerXml));
                    MemoryStream msenc = new MemoryStream();
                    DeflateOutputStream dos = new DeflateOutputStream(msenc, 3, true);
                    StreamUtils.StreamCopy(msencTarget, dos);

                    Log.Out("CompressedXmlData修改前长度为:" + Traverse.Create(xmls).Field("CompressedXmlData").GetValue<byte[]>().Length);
                    Traverse.Create(xmls).Field("CompressedXmlData").SetValue(msenc.ToArray());
                    Log.Out("CompressedXmlData修改后长度为:" + Traverse.Create(xmls).Field("CompressedXmlData").GetValue<byte[]>().Length);
                    //
                    //MemoryStream msenc2 = new MemoryStream(Traverse.Create(xmls).Field("CompressedXmlData").GetValue<byte[]>());
                    //DeflateInputStream dos2 = new DeflateInputStream(msenc2);
                    //FileStream fs = new FileStream("D:\\xmlCongif\\xui.xml", FileMode.OpenOrCreate);
                    //dos2.CopyTo(fs);

                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
        return true; //继续执行原方法
    }

    public static List<Type> list = new List<Type>();
    public static bool FindGraphTypes_postfix(AstarData __instance)
    {
        try
        {
            list.Clear();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            int c = 0;
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    foreach (Type type in assemblies[i].GetTypes())
                    {
                        Type baseType = type.BaseType;
                        while (baseType != null)
                        {
                            if (object.Equals(baseType, typeof(NavGraph)))
                            {
                                list.Add(type);
                                break;
                            }
                            baseType = baseType.BaseType;
                        }
                    }
                }
                catch (Exception)
                {
                    c++;
                }
            }
            Log.Out("[Hiold Injection]：本次共跳过" + c + "个异常Assembly");
            Traverse.Create(__instance).Field("graphTypes").SetValue(list.ToArray());
            MethodInfo mi = AccessTools.PropertySetter(__instance.GetType(), "graphTypes");
            AccessTools.MethodDelegate<Action<Type[]>>(mi, __instance).Invoke(list.ToArray());
        }
        catch (Exception e)
        {
            Log.Out("[Hiold Injection]：处理发生错误" + e.Message + e.StackTrace);
        }
        //拦截原方法执行
        return false;
    }

    public static Type[] getGrath_PostFix()
    {
        Log.Out("调用了get的方法");
        return list.ToArray();
    }




    //public static NavGraph AddGraph(Type type)
    //{

    //}

    /// <summary>
    /// steam转byte[]
    /// </summary>
    /// <param name="stream">流</param>
    /// <returns></returns>
    public static byte[] StreamToBytes(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);

        // 设置当前流的位置为流的开始 
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }




}