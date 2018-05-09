using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Serilog.Configuration;

namespace Serilog.Settings.Delegates
{
    public static class LoggerDestructuringConfigurationExtensions
    {
        public static LoggerConfiguration ByTransforming(
            this LoggerDestructuringConfiguration loggerConfiguration,
            string returnType,
            string transformation)
        {
            if(loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if(returnType == null) throw new ArgumentNullException(nameof(returnType));
            if(transformation == null) throw new ArgumentNullException(nameof(transformation));
            if(returnType.Equals("dynamic")) throw new ArgumentException("Dynamic is not a type.", nameof(returnType));

            dynamic compiledTransformation;

            try
            {
                compiledTransformation = CompileTransformation(returnType, transformation);
            }
            catch(TargetInvocationException ex)
            {
                throw new ArgumentException($"Destructure.ByTransforming transformation failed to compile.\nTransformation: {transformation}\nException: {ex.InnerException.Message}");
            }

            return loggerConfiguration.ByTransforming(compiledTransformation);
        }

        public static LoggerConfiguration ByTransformingWhere(
            this LoggerDestructuringConfiguration loggerConfiguration,
            string predicate,
            string returnType,
            string transformation)
        {
            if(loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if(predicate == null) throw new ArgumentNullException(nameof(predicate));
            if(returnType == null) throw new ArgumentNullException(nameof(returnType));
            if(transformation == null) throw new ArgumentNullException(nameof(transformation));
            if(returnType.Equals("dynamic")) throw new ArgumentException("Dynamic is not a type.", nameof(returnType));

            Func<Type, bool> compiledPredicate;
            dynamic compiledTransformation;

            try
            {
                compiledPredicate =
                    CSharpScript.EvaluateAsync<Func<Type, bool>>
                    (predicate, ReflectionHelper.ScriptOptions)
                    .GetAwaiter().GetResult();
            }
            catch(CompilationErrorException ex)
            {
                throw new ArgumentException($"Destructure.ByTransformingWhere predicate failed to compile.\nPredicate: {predicate}\nException: {ex.Message}");
            }

            try
            {
                compiledTransformation = CompileTransformation(returnType, transformation);
            }
            catch(TargetInvocationException ex)
            {
                throw new ArgumentException($"Destructure.ByTransformingWhere transformation failed to compile.\nTransformation: {transformation}\nException: {ex.InnerException.Message}");
            }

            return loggerConfiguration.ByTransformingWhere(compiledPredicate, compiledTransformation);
        }

        private static dynamic CompileTransformation(string returnType, string transformation)
        {
            // get a Type that corresponds to namespace.type in transformedType
            Type TValue = Type.GetType(returnType) ??
                AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(returnType))
                .FirstOrDefault(t => t != null);

            if(TValue == null)
                throw new ArgumentException($"Destructure {nameof(returnType)} could not be resolved. Did you provide both namespace and type name?\nRequested {nameof(returnType)}: {returnType}");

            // get a representation of Func<TValue, object>
            Type funcType = typeof(Func<,>).MakeGenericType(new Type[] { TValue, typeof(object) });

            // get a representation of CSharpScript.EvaluateAsync<Func<TValue, object>>()
            var evalMethod = typeof(CSharpScript).GetMethods()
                .FirstOrDefault(m => m.Name.Equals("EvaluateAsync") && m.IsGenericMethod)
                .MakeGenericMethod(funcType);

            // execute EvaluateAsync
            dynamic evalTask = evalMethod.Invoke(null, new object[] { transformation, ReflectionHelper.ScriptOptions, null, null, null });
            dynamic compiledFunc = evalTask.GetAwaiter().GetResult();

            return compiledFunc;
        }
    }
}

