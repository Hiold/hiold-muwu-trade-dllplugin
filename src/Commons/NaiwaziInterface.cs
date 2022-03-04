using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.IO;
using System.Reflection;

namespace NaiwaziServerKitInterface
{
    public static class CommonInterface
    {
        public static bool IsServerKitEnabled()
        {
            IConsoleCommand command = SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommand("naiwazi-serverkit-webapi", false);
            if (command == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }


    public static class ChatInterface
    {
        public delegate void ChatInfoDelegate(ClientInfo _cinfo, string _msg, EChatType _chattype);
        
        public static int ChatWatcher_Register(ChatInfoDelegate callBack, string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly starter = null;
            for (int i = 0; i < assemblies.Length; i++)
            {
                if(assemblies[i].GetName().Name.Contains("Naiwazi_Optimize_Internal"))
                {
                    starter = assemblies[i];
                }
            }

            if(starter == null)
            {
                return -1;
            }
            
            Type chatWatcher = starter.GetType("ServerKit_Interface.ModImplement", false, true);

            if (chatWatcher == null)
            {
                return -2;
            }

            MethodInfo chatWatcher_Register = chatWatcher.GetMethod("RegisterChatWatcher", BindingFlags.Static | BindingFlags.Public);

            if (chatWatcher_Register == null)
            {
                return -3;
            }
            
            bool ret = (bool)chatWatcher_Register.Invoke(null, new object[] { callBack, name });

            if (ret)
                return 0;
            else
                return -4;
        }
        
    }

    public class ChatHider
    {
        private string m_Ip = "127.0.0.1";
        private int m_Port = 0;
        private string m_ApiToken = "";
        

        private string HttpGet(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 30000;
            string result = null;
            try
            {
                using (WebResponse webResponse = request.GetResponse())
                {
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
        
        public string Ip
        {
            set { m_Ip = value; }
            get { return m_Ip; }
        }

        public void UpdateApiToken()
        {
            List<string> result = SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync("naiwazi-serverkit-webapi", null);
            if (result != null && result.Count == 2)
            {
                if (!int.TryParse(result[0], out m_Port))
                {
                    m_Port = GamePrefs.GetInt(EnumGamePrefs.ServerPort) + 1;
                }
                m_ApiToken = result[1];
            }
        }
        
        public bool SetChatHider(string prefix)
        {
            string url = "http://" + m_Ip + ":" + m_Port + "/api/setchathider?admintoken=" + m_ApiToken + "&prefix=" + prefix;
            string response = HttpGet(url);
            if (response != null && response.Contains("{\"result\":1"))
            {
                return true;
            }

            return false;
        }
        
        public ChatHider()
        {
            UpdateApiToken();
        }
    }



    public static class FastRestartNoticeInterface
    {
        public delegate void NoticeDelegate();
        
        public static int Notice_Register(NoticeDelegate callBack, string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly starter = null;
            for (int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].GetName().Name.Contains("Naiwazi_Optimize_Internal"))
                {
                    starter = assemblies[i];
                }
            }

            if (starter == null)
            {
                return -1;
            }

            Type chatWatcher = starter.GetType("ServerKit_Interface.ModImplement", false, true);

            if (chatWatcher == null)
            {
                return -2;
            }

            MethodInfo noticeRegister = chatWatcher.GetMethod("RegisterFastRestartNotice", BindingFlags.Static | BindingFlags.Public);

            if (noticeRegister == null)
            {
                return -3;
            }

            bool ret = (bool)noticeRegister.Invoke(null, new object[] { callBack, name });

            if (ret)
                return 0;
            else
                return -4;
        }

    }
}
