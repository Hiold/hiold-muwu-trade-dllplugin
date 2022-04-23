using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.Plugins
{
    public interface PluginInterface
    {
        PluginInfo PluginInfo();
        void InitPlugin();
    }

    /// <summary>
    /// 插件信息
    /// </summary>
    public class PluginInfo
    {
        public string Author { get; set; }
        public string PluginName { get; set; }
        public string Version { get; set; }
    }
}
