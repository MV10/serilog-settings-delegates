using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Settings.Delegates
{
    public static class LoggerFilterConfigurationExtensions
    {
        public static LoggerConfiguration ByExcluding(
            this LoggerFilterConfiguration loggerFilterConfiguration, 
            string exclusionPredicate)
        {
            if (loggerFilterConfiguration == null) throw new ArgumentNullException(nameof(loggerFilterConfiguration));
            if (exclusionPredicate == null) throw new ArgumentNullException(nameof(exclusionPredicate));

            Func<LogEvent, bool> compiledPredicate;

            try
            {
                compiledPredicate =
                    CSharpScript.EvaluateAsync<Func<LogEvent, bool>>
                    (exclusionPredicate, ReflectionHelper.ScriptOptions)
                    .GetAwaiter().GetResult();
            }
            catch(CompilationErrorException ex)
            {
                throw new ArgumentException($"Filter.ByExcluding predicate failed to compile.\nPredicate: {exclusionPredicate}\nException: {ex.Message}");
            }

            return loggerFilterConfiguration.ByExcluding(compiledPredicate);
        }

        public static LoggerConfiguration ByIncludingOnly(
            this LoggerFilterConfiguration loggerFilterConfiguration, 
            string inclusionPredicate)
        {
            if (loggerFilterConfiguration == null) throw new ArgumentNullException(nameof(loggerFilterConfiguration));
            if (inclusionPredicate == null) throw new ArgumentNullException(nameof(inclusionPredicate));

            Func<LogEvent, bool> compiledPredicate;

            try
            {
                compiledPredicate =
                    CSharpScript.EvaluateAsync<Func<LogEvent, bool>>
                    (inclusionPredicate, ReflectionHelper.ScriptOptions)
                    .GetAwaiter().GetResult();
            }
            catch(CompilationErrorException ex)
            {
                throw new ArgumentException($"Filter.ByIncludingOnly predicate failed to compile.\nPredicate: {inclusionPredicate}\nException: {ex.Message}");
            }

            return loggerFilterConfiguration.ByIncludingOnly(compiledPredicate);
        }
    }
}
