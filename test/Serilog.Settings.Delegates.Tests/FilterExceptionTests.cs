using System;
using Xunit;

namespace Serilog.Settings.Delegates.Tests
{
    public class FilterExceptionTests
    {
        [Fact]
        public void InvalidInclusionPredicateThrows()
        {
            string predicate = "invalid.code";

            var ex = Assert.Throws<ArgumentException>(() =>
                new LoggerConfiguration()
                .Filter.ByIncludingOnly(predicate));

            Assert.Contains("predicate failed to compile", ex.Message);
        }

        [Fact]
        public void InvalidExclusionPredicateThrows()
        {
            string predicate = "invalid.code";

            var ex = Assert.Throws<ArgumentException>(() =>
                new LoggerConfiguration()
                .Filter.ByExcluding(predicate));

            Assert.Contains("predicate failed to compile", ex.Message);
        }

    }
}
