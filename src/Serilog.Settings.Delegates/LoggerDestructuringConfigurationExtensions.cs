using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Serilog.Configuration;

namespace Serilog.Settings.Delegates
{
    public static class LoggerDestructuringConfigurationExtensions
    {
        public static LoggerConfiguration ByTransforming(
            this LoggerDestructuringConfiguration loggerConfiguration,
            string transformedType,
            string transformation)
        {
            if(loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if(transformedType == null) throw new ArgumentNullException(nameof(transformedType));
            if(transformation == null) throw new ArgumentNullException(nameof(transformation));

            var compiledTransformation = CompileTransformation(transformedType, transformation);

            return loggerConfiguration.ByTransforming(compiledTransformation);
        }

        public static LoggerConfiguration ByTransformingWhere(
            this LoggerDestructuringConfiguration loggerConfiguration,
            string predicate,
            string transformedType,
            string transformation)
        {
            if(loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if(predicate == null) throw new ArgumentNullException(nameof(predicate));
            if(transformedType == null) throw new ArgumentNullException(nameof(transformedType));
            if(transformation == null) throw new ArgumentNullException(nameof(transformation));

            Func<Type, bool> compiledPredicate = CSharpScript.EvaluateAsync<Func<Type, bool>>
                (predicate, ReflectionHelper.scriptOptions)
                .GetAwaiter().GetResult();

            var compiledTransformation = CompileTransformation(transformedType, transformation);

            return loggerConfiguration.ByTransformingWhere(compiledPredicate, compiledTransformation);
        }

        private static dynamic CompileTransformation(string transformedType, string transformation)
        {
            // get a Type that corresponds to namespace.type in transformedType
            Type TValue = Type.GetType(transformedType) ??
                AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(transformedType))
                .FirstOrDefault(t => t != null);

            // get a representation of Func<TValue, object>
            Type funcType = typeof(Func<,>).MakeGenericType(new Type[] { TValue, typeof(object) });

            // get a representation of CSharpScript.EvaluateAsync<Func<TValue, object>>()
            var evalMethod = typeof(CSharpScript).GetMethods()
                .FirstOrDefault(m => m.Name.Equals("EvaluateAsync") && m.IsGenericMethod)
                .MakeGenericMethod(funcType);

            // execute EvaluateAsync
            dynamic evalTask = evalMethod.Invoke(null, new object[] { transformation, ReflectionHelper.scriptOptions, null, null, null });
            dynamic compiledFunc = evalTask.GetAwaiter().GetResult();

            return compiledFunc;
        }
    }
}

