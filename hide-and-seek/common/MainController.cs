using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hide_and_seek.common
{
    public class MainController
    {
        //寻找者
        public static List<int> Seekers = new List<int>();
        //躲藏者
        public static List<int> Hiders = new List<int>();

        //躲藏者位置
        public static Dictionary<int, Vector3i> HidersPos = new Dictionary<int, Vector3i>();

    }
}
