using HarmonyLib;
using HioldMod.src.UserTools;
using Noemax.GZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

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
                    //lable1
                    XmlElement lable1 = xmlDoc.CreateElement("lable");
                    lable1.SetAttribute("depth", "2");
                    lable1.SetAttribute("pos", "520,-550");
                    lable1.SetAttribute("text", "[D65E62]★--- [FFFFFF]点击此处网页链接，跳转至交易系统[D65E62] ---★");
                    lable1.SetAttribute("width", "100%");
                    lable1.SetAttribute("justify", "center");
                    lable1.SetAttribute("font_size", "32");
                    rect.AppendChild(lable1);
                    //lable2
                    XmlElement lable2 = xmlDoc.CreateElement("lable");
                    lable2.SetAttribute("depth", "11");
                    lable2.SetAttribute("pos", "550,-600");
                    lable2.SetAttribute("width", "480");
                    lable2.SetAttribute("height", "32");
                    lable2.SetAttribute("name", "ServerWebsiteURL");
                    /*下面是网页链接*/
                    lable2.SetAttribute("text", "https://td.hiold.net/");
                    lable2.SetAttribute("justify", "left");
                    lable2.SetAttribute("style", "press,hover");
                    lable2.SetAttribute("font_size", "30");
                    lable2.SetAttribute("pivot", "topleft");
                    lable2.SetAttribute("upper_case", "false");
                    rect.AppendChild(lable2);
                    //panel
                    XmlElement panel = xmlDoc.CreateElement("panel");
                    panel.SetAttribute("name", "content");
                    panel.SetAttribute("pos", "9999,9999");
                    panel.SetAttribute("height", "1");
                    panel.SetAttribute("depth", "1");
                    panel.SetAttribute("pivot", "center");
                    panel.SetAttribute("disableautobackground", "true");

                    //插入目标节点
                    xmlDoc.SelectSingleNode("/xui/ruleset").AppendChild(window);
                    //修改目标数据
                    MemoryStream msenc = new MemoryStream(Encoding.UTF8.GetBytes(xmlDoc.InnerXml));
                    DeflateOutputStream dos = new DeflateOutputStream(msenc, 3);
                    Traverse.Create(xmls).Field("CompressedXmlData").SetValue(StreamToBytes(dos));


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
                    MemoryStream msenc = new MemoryStream(Encoding.UTF8.GetBytes(xmlDoc.InnerXml));
                    DeflateOutputStream dos = new DeflateOutputStream(msenc,3);
                    Traverse.Create(xmls).Field("CompressedXmlData").SetValue(StreamToBytes(dos));
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
        return true; //继续执行原方法
    }

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