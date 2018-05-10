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
                    var assemblies = GetSafeAssemblies(AppDomain.CurrentDomain.GetAssemblies());

                    // It is useful to dump the assemblies to the console when only unit
                    // tests fail in the Linux container during Travis-CI builds.
                    //Console.WriteLine("\n------------------------------------------------\n");
                    //foreach(var a in assemblies)
                    //    Console.WriteLine(a.FullName);
                    //Console.WriteLine("\n------------------------------------------------\n");

                    _options = ScriptOptions.Default
                        .AddReferences(assemblies)
                        .AddImports(GetAllNamespaces(assemblies));
                }
                return _options;
            }
        }

        private static IEnumerable<Assembly> GetSafeAssemblies(IEnumerable<Assembly> assemblies)
        {
            // Exclude dynamic assemblies which throw a System.NotSupportedException with the
            // message "Can't create a metadata reference to a dynamic assembly" running xUnit
            // tests in Visual Studio or Windows.

            // Exclude xUnit assemblies which throws a System.Reflection.ReflectionTypeLoadException
            // with the message "Unable to load one or more of the requested types" when reading
            // xunit.core namespaces during unit testing on Linux in the Travis-CI process.

            return
                assemblies
                .Where(n =>
                    !n.IsDynamic
                    && !n.FullName.StartsWith("xunit", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

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
