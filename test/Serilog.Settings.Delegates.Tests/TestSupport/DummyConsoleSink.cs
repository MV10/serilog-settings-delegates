using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;

namespace Serilog.Settings.Delegates.Tests
{
    public class DummyConsoleSink : ILogEventSink
    {
        public DummyConsoleSink() { }

        [ThreadStatic]
        // ReSharper disable ThreadStaticFieldHasInitializer
        public static List<LogEvent> Emitted = new List<LogEvent>();
        // ReSharper restore ThreadStaticFieldHasInitializer

        public void Emit(LogEvent logEvent)
        {
            Emitted.Add(logEvent);
        }
    }
}
