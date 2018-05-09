using Serilog.Events;
using System;
using Xunit;

namespace Serilog.Settings.Delegates.Tests
{
    public class DestructureTests
    {
        [Fact]
        public void SpecificTypeTransformed()
        {
            string destructureReturnType = "Serilog.Settings.Delegates.Tests.Account";
            string destructureTransformation = "a => new { a.id, a.Username }";
            Account data = new Account { id = 123, Username = "TJefferson", Password = "tr330fl1b3rty" };

            var logConfig = new LoggerConfiguration()
                .Destructure.ByTransforming(
                    returnType: destructureReturnType,
                    transformation: destructureTransformation);

            var msg = GetDestructuredProperty(logConfig, data);

            Assert.Contains("Username", msg);
            Assert.DoesNotContain("Password", msg);
        }

        [Fact]
        public void TransformedWhenPredicateTrue()
        {
            string destructurePredicate = "t => typeof(Type).IsAssignableFrom(t)";
            string destructureReturnType = "System.Type";
            string destructureTransformation = "n => new { n.Namespace }";
            var data = typeof(String);

            var logConfig = new LoggerConfiguration()
                .Destructure.ByTransformingWhere(
                    predicate: destructurePredicate,
                    returnType: destructureReturnType,
                    transformation: destructureTransformation);

            var msg = GetDestructuredProperty(logConfig, data);

            Assert.Contains("Namespace", msg);
        }

        [Fact]
        public void NotTransformedWhenPredicateFalse()
        {
            string destructurePredicate = "t => typeof(Type).IsAssignableFrom(t)";
            string destructureReturnType = "System.Type";
            string destructureTransformation = "n => new { n.Namespace }";
            var data = new Account { id = 123, Username = "TJefferson", Password = "tr330fl1b3rty" };

            var logConfig = new LoggerConfiguration()
                .Destructure.ByTransformingWhere(
                    predicate: destructurePredicate,
                    returnType: destructureReturnType,
                    transformation: destructureTransformation);

            var msg = GetDestructuredProperty(logConfig, data);

            Assert.DoesNotContain("Namespace", msg);
        }

        private string GetDestructuredProperty(LoggerConfiguration logConfig, object x)
        {
            LogEvent evt = null;
            var log = logConfig
                .MinimumLevel.Verbose()
                .WriteTo.Sink(new DelegatingSink(e => evt = e))
                .CreateLogger();

            log.Information("{@X}", x);
            var result = evt.Properties["X"].ToString();
            return result;
        }
    }
}
