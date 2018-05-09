using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Serilog.Settings.Delegates.Tests
{
    public class DummyConsoleSink : ILogEventSink
    {
        private readonly ITextFormatter textFormatter;

        public DummyConsoleSink(ITextFormatter textFormatter = null)
        {
            this.textFormatter = textFormatter;
            Emitted.Clear();
            Messages.Clear();
        }

        [ThreadStatic]
        // ReSharper disable ThreadStaticFieldHasInitializer
        public static List<LogEvent> Emitted = new List<LogEvent>();
        public static StringBuilder Messages = new StringBuilder(256);
        // ReSharper restore ThreadStaticFieldHasInitializer

        public void Emit(LogEvent logEvent)
        {
            Emitted.Add(logEvent);

            var buffer = new StringWriter(new StringBuilder(256));
            textFormatter.Format(logEvent, buffer);
            Messages.Append(buffer.ToString());
        }
    }
}
