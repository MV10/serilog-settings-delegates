using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Serilog.Settings.Delegates
{
    /// <summary>
    /// Builds a Rosyln ScriptOptions object with dependent Assembly references and Namespace imports.
    /// </summary>
    public static class ReflectionHelper
    {
        private static ScriptOptions _options = null;

        /// <summary>
        /// Returns a cached ScriptOptions object.
        /// If IncludeTypes has not been used, this builds a ScriptOptions that references all
        /// loaded assemblies and imports all Namespaces from all loaded assemblies.
        /// </summary>
        public static ScriptOptions scriptOptions
        {
            get
            {
                if(_options == null)
                {
                    _options = ScriptOptions.Default
                        .AddReferences(AppDomain.CurrentDomain.GetAssemblies())
                        .AddImports(GetAllNamespaces(AppDomain.CurrentDomain.GetAssemblies()));
                }
                return _options;
            }
        }

        /// <summary>
        /// Locates the real Type based on the string representation of namespace and classname and
        /// references the assembly and namespace.
        /// </summary>
        /// <param name="typeName">Include the namespace and class, such as Serilog.Filters.Match</param>
        /// <returns></returns>
        public static Type IncludeType(string typeName)
        {
            var strongType = Type.GetType(typeName) ??
                AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(typeName))
                .FirstOrDefault(t => t != null);

            _options = (_options ?? ScriptOptions.Default)
                .AddReferences(strongType.Assembly)
                .AddImports(strongType.Namespace);

            return strongType;
        }

        public static async Task<object> InvokeAsync(this MethodInfo methodInfo, object obj, params object[] parameters)
        {
            dynamic awaitable = methodInfo.Invoke(obj, parameters);
            await awaitable;
            return awaitable.GetAwaiter().GetResult();
        }

        private static List<Assembly> GetAssemblies(IEnumerable<Type> types)
            => types
            .Select(t => t.Assembly)
            .Distinct()
            .Where(n => n != null).ToList();

        private static List<string> GetAllNamespaces(IEnumerable<Assembly> assemblies)
            => assemblies.SelectMany(a => GetNamespaces(a)).ToList();

        private static List<string> GetNamespaces(Assembly assembly)
            => assembly.GetTypes()
            .Select(t => t.Namespace)
            .Distinct()
            .Where(n => n != null)
            .ToList();
    }
}
