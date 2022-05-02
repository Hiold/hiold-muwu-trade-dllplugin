using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QQ_BOTPlugin.utils
{
    public class RegexUtils
    {
        public static string ReplaceBBCode(string source)
        {
            source = Regex.Replace(source, @"([\\[][\w\/]*[\]])", "");
            source=Regex.Replace(source, @"([\\{].*[\\}])", "");
            return source;
        }
    }
}
