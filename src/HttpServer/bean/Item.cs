using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.bean
{
    class Item
    {

        public string itemtype { get; set; }
        public string name { get; set; }
        public string translate { get; set; }
        //图片
        public string itemIcon { get; set; }
        public string itemTint { get; set; }
        //品质
        public int quality { get; set; }
        //数量
        public int num { get; set; }

        //类型2
        public string class1 { get; set; }
        //类型2
        public string class2 { get; set; }
        //是否为mod物品
        public string classMod { get; set; }
        //描述
        public string desc { get; set; }

    }
}
