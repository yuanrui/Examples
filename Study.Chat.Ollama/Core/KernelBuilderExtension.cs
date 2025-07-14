using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Study.Chat.Ollama.Core
{
    public static class KernelBuilderExtension
    {
        /// <summary>
        /// 自动注册所有包含KernelFunction方法的类作为插件
        /// </summary>
        public static IKernelBuilder AutoRegisterPlugins(
            this IKernelBuilder builder,
            params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new[] { Assembly.GetExecutingAssembly() };

            // 查找包含至少一个KernelFunction方法的非抽象类
            var pluginTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsDefined(typeof(ObsoleteAttribute), inherit: true))
                .Where(t => t.GetMethods().Any(m =>
                    m.GetCustomAttribute<KernelFunctionAttribute>() != null));

            foreach (var type in pluginTypes)
            {
                var pluginInstance = CreatePluginInstance(type);
                if (pluginInstance == null) continue;

                string pluginName = GetDefaultPluginName(type);
                builder.Plugins.AddFromObject(pluginInstance, pluginName);
            }

            return builder;
        }

        /// <summary>
        /// 自动注册plugins目录及其子目录中包含KernelFunction方法的类作为插件
        /// </summary>
        public static IKernelBuilder AutoRegisterPluginsFromDirectory(
            this IKernelBuilder builder,
            string pluginsFolder = "plugins")
        {
            var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
            
            // 获取 plugins 文件夹路径
            var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginsFolder);
            
            if (Directory.Exists(pluginsPath))
            {
                // 递归搜索所有包含"Plugin"的DLL文件
                var pluginAssemblies = Directory.GetFiles(pluginsPath, "*Plugin*.dll", SearchOption.AllDirectories)
                    .Select(pluginPath => 
                    {
                        try
                        {
                            return Assembly.LoadFrom(pluginPath);
                        }
                        catch
                        {
                            return null;
                        }
                    })
                    .Where(assembly => assembly != null)
                    .ToArray();

                if (pluginAssemblies != null && pluginAssemblies.Any())
                {
                    assemblies.AddRange(pluginAssemblies);
                }
            }

            return builder.AutoRegisterPlugins(assemblies.ToArray());
        }

        private static object CreatePluginInstance(Type type)
        {
            // 尝试无参数构造函数
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null) return ctor.Invoke(null);

            // 尝试获取带Kernel参数的构造函数（支持依赖注入）
            ctor = type.GetConstructor(new[] { typeof(Kernel) });
            if (ctor != null)
            {
                // 注意：实际内核实例需稍后设置
                return ctor.Invoke(new object[] { null! });
            }

            // 尝试第一个公共构造函数（带默认值）
            var ctors = type.GetConstructors();
            if (ctors.Length > 0)
            {
                var parameters = ctors[0].GetParameters()
                    .Select(p => p.HasDefaultValue ? p.DefaultValue : GetDefault(p.ParameterType))
                    .ToArray();

                return ctors[0].Invoke(parameters);
            }

            //throw new InvalidOperationException($"没有合适的构造函数用于类型 {type.Name}");
            return null;
        }

        private static object GetDefault(Type type) =>
            type.IsValueType ? Activator.CreateInstance(type) : null;

        private static string GetDefaultPluginName(Type type)
        {
            // 移除常见后缀
            var name = type.Name;
            if (name.EndsWith("Plugin")) name = name[..^6];
            if (name.EndsWith("Service")) name = name[..^7];
            if (name.EndsWith("Operations")) name = name[..^10];

            return name;
        }

    }
}
