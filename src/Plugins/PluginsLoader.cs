using HioldMod.HttpServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.Plugins
{
    class PluginsLoader
    {
        public static Dictionary<PluginInfo, Assembly> plugins = new Dictionary<PluginInfo, Assembly>();
        public static string PluginPath = string.Format("{0}/plugins/", HioldMod.API.getModDir());
        //加载mod
        public static void LoadAllPlugins()
        {
            LogUtils.Loger("开始加载插件");
            int counter = 0;
            string[] Files = Directory.GetFiles(PluginPath, "*.hiold");
            foreach (string _path in Files)
            {
                Type typeFromHandle = typeof(PluginInterface);
                Assembly plugin = Assembly.Load(File.ReadAllBytes(_path));
                foreach (Type type in plugin.GetTypes())
                {
                    if (typeFromHandle.IsAssignableFrom(type))
                    {
                        PluginInterface interpoint = (Activator.CreateInstance(type) as PluginInterface);
                        //执行初始化
                        interpoint.InitPlugin();
                        //记录已加载插件信息
                        plugins.Add(interpoint.PluginInfo(), plugin);
                        //只加载一个入口
                        counter++;
                        break;
                    }
                }
            }
            LogUtils.Loger("成功加载" + counter + "个插件");
        }
    }
}
