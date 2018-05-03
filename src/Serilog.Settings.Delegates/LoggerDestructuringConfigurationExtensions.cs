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

            var compiledTransformation = EvaluateFuncTValueObject(transformedType, transformation);

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

            var compiledTransformation = EvaluateFuncTValueObject(transformedType, transformation);

            return loggerConfiguration.ByTransformingWhere(compiledPredicate, compiledTransformation);
        }

        private static dynamic EvaluateFuncTValueObject(string transformedType, string transformation)
        {
            Type TValue = ReflectionHelper.IncludeType(transformedType);
            Type funcType = typeof(Func<,>).MakeGenericType(new Type[] { TValue, typeof(object) });
            var evalMethod = typeof(CSharpScript).GetMethods()
                .FirstOrDefault(m => m.Name.Equals("EvaluateAsync") && m.IsGenericMethod)
                .MakeGenericMethod(funcType);
            return evalMethod.InvokeAsync(null, new object[] { transformation, ReflectionHelper.scriptOptions, null, null, null }).GetAwaiter().GetResult();
        }
    }
}

