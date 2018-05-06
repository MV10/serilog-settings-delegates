using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
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

            Func<LogEvent, bool> compiledPredicate = 
                CSharpScript.EvaluateAsync<Func<LogEvent, bool>>
                (exclusionPredicate, ReflectionHelper.scriptOptions)
                .GetAwaiter().GetResult();

            return loggerFilterConfiguration.ByExcluding(compiledPredicate);
        }

        public static LoggerConfiguration ByIncludingOnly(
            this LoggerFilterConfiguration loggerFilterConfiguration, 
            string inclusionPredicate)
        {
            if (loggerFilterConfiguration == null) throw new ArgumentNullException(nameof(loggerFilterConfiguration));
            if (inclusionPredicate == null) throw new ArgumentNullException(nameof(inclusionPredicate));

            Func<LogEvent, bool> compiledPredicate = 
                CSharpScript.EvaluateAsync<Func<LogEvent, bool>>
                (inclusionPredicate, ReflectionHelper.scriptOptions)
                .GetAwaiter().GetResult();

            return loggerFilterConfiguration.ByIncludingOnly(compiledPredicate);
        }
    }
}
