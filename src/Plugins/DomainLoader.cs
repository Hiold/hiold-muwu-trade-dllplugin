using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod.src.Plugins
{
    public class RemoteLoader : MarshalByRefObject
    {

        private Assembly _assembly;

        public void LoadAssembly(string assemblyFile)
        {
            _assembly = Assembly.Load(File.ReadAllBytes(assemblyFile));
        }

        public T GetInstance<T>(string typeName) where T : class
        {
            if (_assembly == null) return null;
            var type = _assembly.GetType(typeName);
            if (type == null) return null;
            return Activator.CreateInstance(type) as T;
        }

        public PluginInfo InitPlugin()
        {

            Type typeFromHandle = typeof(PluginInterface);
            foreach (Type type in _assembly.GetTypes())
            {
                if (typeFromHandle.IsAssignableFrom(type))
                {
                    PluginInterface interpoint = (Activator.CreateInstance(type) as PluginInterface);
                    //执行初始化
                    interpoint.InitPlugin();
                    return interpoint.PluginInfo();
                }

            }
            return null;
        }
    }


    public class AssemblyDynamicLoader
    {
        private AppDomain appDomain;
        private RemoteLoader remoteLoader;
        public AssemblyDynamicLoader(string pluginName)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "app_" + pluginName;
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
            setup.CachePath = setup.ApplicationBase;
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = setup.ApplicationBase;
            AppDomain.CurrentDomain.SetShadowCopyFiles();
            this.appDomain = AppDomain.CreateDomain("app_" + pluginName, null, setup);
            //加载依赖
            //int count = 0;
            //foreach (Assembly _tbs in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    try
            //    {
            //        this.appDomain.Load(_tbs.GetName());
            //        count++;
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(_tbs.GetName().Name);
            //    }
            //}
            //Console.WriteLine("共加载"+count+"个");

            String name = Assembly.GetExecutingAssembly().GetName().FullName;
            Console.WriteLine(name);
            this.remoteLoader = (RemoteLoader)this.appDomain.CreateInstanceAndUnwrap(name, typeof(RemoteLoader).FullName);
        }




        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="assemblyFile"></param>
        public void LoadAssembly(string assemblyFile)
        {
            remoteLoader.LoadAssembly(assemblyFile);
        }

        /// <summary>
        /// 创建对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public T GetInstance<T>(string typeName) where T : class
        {
            if (remoteLoader == null) return null;
            return remoteLoader.GetInstance<T>(typeName);
        }

        /// <summary>
        /// 执行类型方法
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public PluginInfo InitPlugin()
        {
            return remoteLoader.InitPlugin();
        }

        /// <summary>
        /// 卸载应用程序域
        /// </summary>
        public void Unload()
        {
            try
            {
                if (appDomain == null) return;
                AppDomain.Unload(this.appDomain);
                this.appDomain = null;
                this.remoteLoader = null;
            }
            catch (CannotUnloadAppDomainException ex)
            {
                throw ex;
            }
        }

        public Assembly[] GetAssemblies()
        {
            return this.appDomain.GetAssemblies();
        }
    }

}
