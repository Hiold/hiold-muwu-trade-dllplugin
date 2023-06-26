using System;
using System.Collections.Generic;
using System.Reflection;

namespace HioldMod
{
    class HioldTestConsole : ConsoleCmdAbstract
    {
        protected override string getDescription()
        {
            return "[HioldMod] - 测试开关";
        }

        public override string GetHelp()
        {
            return "用法:\n" +
                   "  1. test1 on （开启测试1）\n" +
                   "  2. test1 off （关闭测试1）\n";
        }

        protected override string[] getCommands()
        {
            return new string[] { "test1" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1)
                {
                    SdtdConsole.Instance.Output(string.Format("[HioldMod] 这个指令需要1个参数, 但是实际有 {0} 个参数", _params.Count));
                    return;
                }
                if (_params[0].ToLower().Equals("off"))
                {
                    //关闭测试
                    //API.isFastRestarting = false;
                    Log.Out("[HioldMod]：关闭测试");
                }
                else if (_params[0].ToLower().Equals("on"))
                {
                    //开启测试
                    //API.isFastRestarting = true;
                    Log.Out("[HioldMod]：开启测试");
                }
                else
                {
                    SdtdConsole.Instance.Output(string.Format("[HioldMod] 参数错误 {0}", _params[0]));
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[HioldMod] 指令执行出错: {0}", e.Message));
            }
        }
    }
}

