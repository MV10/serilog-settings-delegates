using Microsoft.CodeAnalysis.Scripting;
using Serilog.Events;
using System;
using Xunit;

namespace Serilog.Settings.Delegates.Tests
{
    public class DestructureTests
    {

        //private string GetDestructuredProperty(object x, string json)
        //{
        //    LogEvent evt = null;
        //    var log = ConfigFromJson(json)
        //        .WriteTo.Sink(new DelegatingSink(e => evt = e))
        //        .CreateLogger();
        //    log.Information("{@X}", x);
        //    var result = evt.Properties["X"].ToString();
        //    return result;
        //}
    }
}
