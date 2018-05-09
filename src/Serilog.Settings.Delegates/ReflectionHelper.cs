using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public static ScriptOptions ScriptOptions
        {
            get
            {
                if(_options == null)
                {
                    // Referencing a dynamic assembly (which is most easily seen with xUnit testing) throws
                    // a System.NotSupportedException: Can't create a metadata reference to a dynamic assembly. 
                    var assemblies = GetNonDynamicAssemblies(AppDomain.CurrentDomain.GetAssemblies());
                    _options = ScriptOptions.Default
                        .AddReferences(assemblies)
                        .AddImports(GetAllNamespaces(assemblies));
                }
                return _options;
            }
        }

        private static IEnumerable<Assembly> GetNonDynamicAssemblies(IEnumerable<Assembly> assemblies)
            => assemblies
            .Where(n => !n.IsDynamic)
            .ToList();

        private static IEnumerable<string> GetAllNamespaces(IEnumerable<Assembly> assemblies)
            => assemblies.SelectMany(a => GetNamespaces(a)).ToList();

        private static IEnumerable<string> GetNamespaces(Assembly assembly)
            => assembly.GetTypes()
            .Select(t => t.Namespace)
            .Distinct()
            .Where(n => n != null)
            .ToList();
    }
}
