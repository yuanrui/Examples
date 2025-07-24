using Microsoft.SemanticKernel;
using System.Reflection;

namespace Study.Chat.Ollama.Core
{
    public static class KernelBuilderExtension
    {
        private const string PluginDirectory = "plugins";

        /// <summary>
        /// Automatically registers all classes containing KernelFunction methods as plugins
        /// </summary>
        public static IKernelBuilder AutoRegisterPlugins(
            this IKernelBuilder builder,
            params Assembly[] assemblies)
        {
            const string ExcludeNamespace = "Microsoft.SemanticKernel.";

            if (assemblies.Length == 0)
                assemblies = new[] { Assembly.GetExecutingAssembly() };

            // Find non-abstract classes containing at least one KernelFunction method
            var pluginTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsDefined(typeof(ObsoleteAttribute), inherit: true))
                .Where(t => t.GetMethods().Any(m => m.GetCustomAttribute<KernelFunctionAttribute>() != null))
                .Where(t => t.Namespace == null || !t.Namespace.StartsWith(ExcludeNamespace)) // Exclude specified namespace
                .Distinct(); // Ensure each type is processed only once

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
        /// Automatically registers classes with KernelFunction methods from the plugins directory and subdirectories
        /// </summary>
        public static IKernelBuilder AutoRegisterPluginsFromDirectory(
            this IKernelBuilder builder,
            string pluginsFolder = PluginDirectory)
        {
            var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };

            // Get plugins folder path
            var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginsFolder);

            if (Directory.Exists(pluginsPath))
            {
                // Recursively search all DLL files containing "Plugin"
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

        public static IKernelBuilder AutoRegisterPluginsFromDirectories(this IKernelBuilder builder)
        {
            return AutoRegisterPluginsFromDirectories(builder, "", PluginDirectory);
        }

        /// <summary>
        /// Automatically registers classes with KernelFunction methods from multiple directories and subdirectories
        /// </summary>
        public static IKernelBuilder AutoRegisterPluginsFromDirectories(
            this IKernelBuilder builder,
            params string[] dirs)
        {
            var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
            var processedPaths = new HashSet<string>();

            foreach (var dir in dirs)
            {
                var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);

                if (Directory.Exists(pluginsPath))
                {
                    // Recursively search all DLL files containing "Plugin" and deduplicate paths
                    var pluginPaths = Directory.GetFiles(pluginsPath, "*Plugin*.dll", SearchOption.AllDirectories)
                        .Where(path => processedPaths.Add(path)) // Only add unprocessed paths
                        .ToArray();

                    var pluginAssemblies = pluginPaths
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

                    if (pluginAssemblies.Any())
                    {
                        assemblies.AddRange(pluginAssemblies);
                    }
                }
            }

            // Ensure assemblies are unique
            var uniqueAssemblies = assemblies
                .GroupBy(a => a.FullName)
                .Select(g => g.First())
                .ToArray();

            return builder.AutoRegisterPlugins(uniqueAssemblies);
        }

        private static object CreatePluginInstance(Type type)
        {
            // Try parameterless constructor
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null) return ctor.Invoke(null);

            // Try to get constructor with Kernel parameter (supports dependency injection)
            ctor = type.GetConstructor(new[] { typeof(Kernel) });
            if (ctor != null)
            {
                // Note: Actual kernel instance needs to be set later
                return ctor.Invoke(new object[] { null! });
            }

            // Try first public constructor (with default values)
            var ctors = type.GetConstructors();
            if (ctors.Length > 0)
            {
                var parameters = ctors[0].GetParameters()
                    .Select(p => p.HasDefaultValue ? p.DefaultValue : GetDefault(p.ParameterType))
                    .ToArray();

                return ctors[0].Invoke(parameters);
            }

            //throw new InvalidOperationException($
            return null;
        }

        private static object GetDefault(Type type) =>
            type.IsValueType ? Activator.CreateInstance(type) : null;

        private static string GetDefaultPluginName(Type type)
        {
            // Remove common suffixes
            var name = type.Name;
            if (name.EndsWith("Plugin")) name = name[..^6];
            if (name.EndsWith("Service")) name = name[..^7];
            if (name.EndsWith("Operations")) name = name[..^10];

            return name;
        }
    }
}
