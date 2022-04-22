using HioldMod.HttpServer;
using HioldMod.src.HttpServer.attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.HttpServer.router
{
    public class AttributeAnalysis
    {
        public static Dictionary<string, RouterInfo> routers = new Dictionary<string, RouterInfo>();

        /// <summary>
        /// 开始分析
        /// </summary>
        public static void AnalysisStart()
        {
            int count = 0;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            int c = 0;
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    foreach (Type _t in assemblies[i].GetTypes())
                    {
                        //获取当前属性得AbstractValidateAttribute特性对象 返回得是应该object
                        foreach (var attr in _t.GetCustomAttributes(typeof(ActionAttribute), true))
                        {
                            //遍历所有方法
                            foreach (MethodInfo _mi in _t.GetMethods())
                            {
                                //遍历指定Attribute
                                foreach (var methodAttr in _mi.GetCustomAttributes(typeof(RequestHandlerAttribute), true))
                                {
                                    try
                                    {
                                        RequestHandlerAttribute attribute = methodAttr as RequestHandlerAttribute;
                                        RouterInfo ri = new RouterInfo()
                                        {
                                            action = _t,
                                            method = _mi,
                                            IsAdmin = attribute.IsAdmin,
                                            IsServerReady = attribute.IsServerReady,
                                            IsUserLogin = attribute.IsUserLogin,
                                        };
                                        routers.Add(attribute.url, ri);
                                        count++;
                                    }
                                    catch (Exception e)
                                    {
                                        LogUtils.Loger("初始化Action出错！" + e.Message);
                                        LogUtils.Loger(e.StackTrace);
                                        c++;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogUtils.Loger(e.Message);
                    c++;
                }
            }
            LogUtils.Loger("成功初始化" + count + "个Action! 错误" + c + "个");
        }

        public class RouterInfo
        {
            public Type action { get; set; }
            public MethodInfo method { get; set; }
            public bool IsServerReady { get; set; } = false;
            //是否开启登录检查
            public bool IsUserLogin { get; set; } = false;
            //是否开启管理员检查
            public bool IsAdmin { get; set; } = false;
        }

    }
}
