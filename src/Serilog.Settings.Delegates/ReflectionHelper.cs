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
