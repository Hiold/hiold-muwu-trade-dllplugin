using HioldMod.HttpServer;
using HioldMod.src.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQ_BOTPlugin
{
    public class Class1 : HioldMod.src.Plugins.PluginInterface
    {
        public void InitPlugin()
        { 

            LogUtils.Loger("开始初始化QQBot插件");
            bot.BOT.initBot();
            LogUtils.Loger("已加载QQBot插件");
        }

        public PluginInfo PluginInfo()
        {
            return new HioldMod.src.Plugins.PluginInfo()
            {
                Author = "海鸥",
                PluginName = "QQ机器人插件",
                Version = "alpha0.1"
            };
        }
    }
}
