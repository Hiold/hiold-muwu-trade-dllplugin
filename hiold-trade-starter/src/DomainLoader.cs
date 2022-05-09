using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HioldMod
{

    public class RemoteLoader : MarshalByRefObject
    {

        private Assembly _assembly;

        public void LoadAssembly(string assemblyFile)
        {
            _assembly = Assembly.LoadFrom(assemblyFile);
        }

        public T GetInstance<T>(string typeName) where T : class
        {
            if (_assembly == null) return null;
            var type = _assembly.GetType(typeName);
            if (type == null) return null;
            return Activator.CreateInstance(type) as T;
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
            setup.ApplicationBase = API.AssemblyPath;
            setup.PrivateBinPath = Path.Combine(API.AssemblyPath, Directory.GetParent(API.AssemblyPath).Parent.Parent.FullName + @"\7DaysToDieServer_Data\Managed\");
            setup.CachePath = setup.ApplicationBase;
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = API.AssemblyPath + @"temp";
            this.appDomain = AppDomain.CreateDomain("app_" + pluginName, null, setup);

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
            Log.Out("加载依赖:" + assemblyFile);
            remoteLoader.LoadAssembly(assemblyFile);
        }

        /*
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
        */

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
