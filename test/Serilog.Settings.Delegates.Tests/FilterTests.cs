using System.Collections;
using Xunit;
using Serilog.Formatting.Display;
using System.Globalization;

namespace Serilog.Settings.Delegates.Tests
{
    public class FilterTests
    {
        [Fact]
        public void InclusionPredicate()
        {
            string filterPredicate = "Matching.WithProperty<string>(\"Letter\", w => w.Equals(\"b\"))";
            var inputCollection = new[] { "a", "b", "c" };

            var logConfig = new LoggerConfiguration()
                .Filter.ByIncludingOnly(filterPredicate);

            var msg = GetMessages(logConfig, inputCollection);

            Assert.Contains("b", msg);
            Assert.DoesNotContain("a", msg);
            Assert.DoesNotContain("c", msg);
        }

        [Fact]
        public void ExclusionPredicate()
        {
            string filterPredicate = "Matching.WithProperty<string>(\"Letter\", w => w.Equals(\"b\"))";
            var inputCollection = new[] { "a", "b", "c" };

            var logConfig = new LoggerConfiguration()
                .Filter.ByExcluding(filterPredicate);

            var msg = GetMessages(logConfig, inputCollection);

            Assert.DoesNotContain("b", msg);
            Assert.Contains("a", msg);
            Assert.Contains("c", msg);
        }

        private string GetMessages(LoggerConfiguration logConfig, IEnumerable inputs)
        {
            var formatter = new MessageTemplateTextFormatter("{Message}", CultureInfo.InvariantCulture);
            var log = logConfig
                .MinimumLevel.Verbose()
                .WriteTo.Sink(new DummyConsoleSink(formatter))
                .CreateLogger();

            foreach(string n in inputs)
                log.Information("{Letter}", n);

            return DummyConsoleSink.Messages.ToString();
        }
    }
}
