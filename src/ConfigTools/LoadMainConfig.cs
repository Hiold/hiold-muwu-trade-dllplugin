using HioldMod;
using HioldMod.HttpServer;
using HioldMod.HttpServer.common;
using HioldMod.src.HttpServer.bean;
using HioldMod.src.HttpServer.service;
using HioldMod.src.UserTools;
using System;
using System.IO;
using System.Xml;

namespace ConfigTools
{
    public class LoadMainConfig
    {
        public const string version = "19.3.2";
        public static string Server_Response_Name = "[FFCC00]HioldMod";
        public static string Chat_Response_Color = "[00FF00]";
        private const string configFile = "MainConfig.xml";
        public static string configFilePath = string.Format("{0}/{1}", HioldMod.HioldMod.API.ConfigPath, configFile);
        public static FileSystemWatcher fileWatcher = new FileSystemWatcher(HioldMod.HioldMod.API.ConfigPath, configFile);
        public static string OldXmlDirectory = "";


        public static class MainConfig
        {
            public static string Host;
            public static string Port;
            public static string username;
            public static string password;
            public static string killevent = "False";
        }


        public static void Load()
        {
            //Log.Out("[HioldMod] 文件路径" + configFilePath);

            LoadXml();
            InitFileWatcher();
        }

        public static void LoadXml()
        {
            Log.Out("---------------------------------------------------------------");
            Log.Out("[HioldMod] 主配置文件----验证配置文件 & 保存新Entity");
            Log.Out("---------------------------------------------------------------");
            if (!File.Exists(configFilePath))
            {
                WriteXml();
            }
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(configFilePath);
            }
            catch (XmlException e)
            {
                Log.Out(string.Format("[HioldMod] 加载错误 {0}: {1}", configFilePath, e.Message));
                return;
            }
            XmlNode _XmlNode = xmlDoc.DocumentElement;
            foreach (XmlNode childNode in _XmlNode.ChildNodes)
            {
                //
                if (childNode.Name == "HioldMod")
                {
                    foreach (XmlNode subChild in childNode.ChildNodes)
                    {
                        if (subChild.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (subChild.NodeType != XmlNodeType.Element)
                        {
                            Log.Warning(string.Format("[HioldMod] 在HioldMod存在不支持的 section: {0}", subChild.OuterXml));
                            continue;
                        }
                        XmlElement _line = (XmlElement)subChild;
                        if (!_line.HasAttribute("Name"))
                        {
                            Log.Warning(string.Format("[HioldMod] 跳过Option， 缺失'Name' 属性: {0}", subChild.OuterXml));
                            continue;
                        }
                        switch (_line.GetAttribute("Name"))
                        {
                            case "Host":
                                if (!_line.HasAttribute("value"))
                                {
                                    Log.Warning(string.Format("[HioldMod] Ignoring Action entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                MainConfig.Host = _line.GetAttribute("value");
                                break;
                            case "Port":
                                if (!_line.HasAttribute("value"))
                                {
                                    Log.Warning(string.Format("[HioldMod] Ignoring Action entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                MainConfig.Port = _line.GetAttribute("value");
                                break;
                            case "AdminUsername":
                                if (!_line.HasAttribute("value"))
                                {
                                    Log.Warning(string.Format("[HioldMod] Ignoring Action entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                MainConfig.username = _line.GetAttribute("value");
                                break;
                            case "AdminPassword":
                                if (!_line.HasAttribute("value"))
                                {
                                    Log.Warning(string.Format("[HioldMod] Ignoring Action entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                MainConfig.password = _line.GetAttribute("value");
                                break;
                            case "KillEvent":
                                if (!_line.HasAttribute("value"))
                                {
                                    Log.Warning(string.Format("[HioldMod] Ignoring Action entry because of missing 'Enable' attribute: {0}", subChild.OuterXml));
                                    continue;
                                }
                                MainConfig.killevent = _line.GetAttribute("value");
                                break;

                        }
                    }
                }
            }
            //
            try
            {
                if (!string.IsNullOrEmpty(MainConfig.username) && !string.IsNullOrEmpty(MainConfig.password))
                {
                    //获取管理员用户
                    UserInfo ui = UserService.getAdmin(MainConfig.username);
                    if (ui != null)
                    {
                        if (!ServerUtils.md5(MainConfig.password).Equals(ui.password))
                        {
                            ui.password = ServerUtils.md5(MainConfig.password);
                            LogUtils.Loger("检测到管理员密码变动，执行修改");
                        }
                        ui.type = "1";
                        UserService.UpdateUserInfo(ui);
                    }
                    else
                    {
                        int id = UserService.userRegister(new UserInfo()
                        {
                            created_at = DateTime.Now,
                            name = MainConfig.username,
                            gameentityid = MainConfig.username,
                            platformid = MainConfig.username,
                            money = 0,
                            credit = 0,
                            status = 1,
                            password = ServerUtils.md5(MainConfig.password),
                            qq = "",
                            email = "",
                            avatar = MainConfig.username + ".png",
                            sign = "",
                            extinfo1 = "",
                            extinfo2 = "",
                            extinfo3 = "",
                            extinfo4 = "",
                            extinfo5 = "",
                            extinfo6 = "",
                            trade_count = "0",
                            store_count = "0",
                            require_count = "0",
                            type = "1",
                            level = 0,
                            online_time = "0",
                            zombie_kills = "0",
                            player_kills = "0",
                            total_crafted = "0",
                            vipdiscount = 0,
                            creditcharge = 0,
                            creditcost = 0,
                            moneycharge = 0,
                            moneycost = 0,
                            signdays = 0,
                            likecount = 0,
                            trade_money = 0,
                            require_money = 0,
                            buy_count = "0",
                            shopname = MainConfig.username + "的小店",
                            ncode = "",
                        });
                        LogUtils.Loger("管理员注册成功");
                    }
                }
            }
            catch (Exception e)
            {
                LogUtils.Loger("初始化管理员异常 跳过");
            }
        }

        public static void WriteXml()
        {
            fileWatcher.EnableRaisingEvents = false;
            using (StreamWriter sw = new StreamWriter(configFilePath))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                //生成配置文件
                sw.WriteLine("<HioldModConfig>");
                sw.WriteLine("    <HioldMod>");
                sw.WriteLine(string.Format("        <!--交易系统host 当配置为auto时系统自动获取服务器ip-->"));
                sw.WriteLine(string.Format("        <Option Name=\"Host\" value=\"{0}\" />", "auto"));
                sw.WriteLine(string.Format("        <!--交易系统port 当配置为auto时系统自动计算端口号，计算方式为游戏端口+11（如游戏端口为26900，则本交易系统端口为26911）-->"));
                sw.WriteLine(string.Format("        <Option Name=\"Port\" value=\"{0}\" />", "auto"));
                sw.WriteLine(string.Format("        <!--交易系统port 当配置为auto时系统自动计算端口号，计算方式为游戏端口+11（如游戏端口为26900，则本交易系统端口为26911）-->"));
                sw.WriteLine(string.Format("        <Option Name=\"AdminUsername\" value=\"{0}\" />", "admin"));
                sw.WriteLine(string.Format("        <!--交易系统port 当配置为auto时系统自动计算端口号，计算方式为游戏端口+11（如游戏端口为26900，则本交易系统端口为26911）-->"));
                sw.WriteLine(string.Format("        <Option Name=\"AdminPassword\" value=\"{0}\" />", "auto"));
                sw.WriteLine("    </HioldMod>");
                sw.WriteLine("</HioldModConfig>");
                //配置文件结束
                sw.Flush();
                sw.Close();
            }
            fileWatcher.EnableRaisingEvents = true;
        }

        private static void InitFileWatcher()
        {
            fileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.Created += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged);
            fileWatcher.EnableRaisingEvents = true;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            LoadXml();
        }

        /// <summary>
        /// 修改配置项
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        public static void UpdateXml(string name, string value)
        {
            //加载主Document
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(configFilePath);
            }
            catch (XmlException e)
            {
                Log.Error(string.Format("XML加载错误加载错误 {0}: {1}", configFilePath, e.Message));
                return;
            }
            //HioldModConfig层
            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                XmlNode rootNode = doc.ChildNodes[i];
                //HioldMod层
                for (int j = 0; j < rootNode.ChildNodes.Count; j++)
                {
                    XmlNode HioldNode = rootNode.ChildNodes[j];
                    //Option层
                    for (int k = 0; k < HioldNode.ChildNodes.Count; k++)
                    {
                        //节点为Option时执行修改
                        //跳过注释
                        if (HioldNode.ChildNodes[k].NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        if (HioldNode.ChildNodes[k].Name.Equals("Option"))
                        {
                            XmlElement tempNode = (XmlElement)HioldNode.ChildNodes[k];
                            if (tempNode.GetAttribute("Name").Equals(name))
                            {
                                tempNode.SetAttribute("value", value);
                                Log.Out("修改配置{0} , {1}", name, value);
                            }
                        }
                    }
                }
            }
            doc.Save(configFilePath);
        }







        private static void SetXml(XmlDocument _newXml, XmlNodeList _newNodeList, string _elementName, XmlAttribute _oldAttribute)
        {
            try
            {
                for (int i = 0; i < _newNodeList.Count; i++)
                {
                    XmlNode _newChildNode = _newNodeList[i];
                    if (_newChildNode.Name == "HioldMod")
                    {
                        for (int j = 0; j < _newChildNode.ChildNodes.Count; j++)
                        {
                            XmlNode _newSubChild = _newChildNode.ChildNodes[j];
                            if (_newSubChild.Name == "Option")
                            {
                                XmlElement _newElement = (XmlElement)_newSubChild;
                                XmlAttributeCollection _newAttributes = _newElement.Attributes;
                                if (_newElement.Attributes[0].Value == _elementName)
                                {
                                    for (int k = 1; k < _newElement.Attributes.Count; k++)
                                    {
                                        XmlAttribute _newAttribute = _newElement.Attributes[k];
                                        if (_newAttribute.Name == _oldAttribute.Name)
                                        {
                                            if (_newAttribute.Value != _oldAttribute.Value)
                                            {
                                                _newAttribute.Value = _oldAttribute.Value;
                                                _newXml.Save(configFilePath);
                                            }
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[HioldMod] Error in LoadConfig.SetXml: {0}", e.Message));
            }
        }
    }
}