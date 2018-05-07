using Microsoft.CodeAnalysis.Scripting;
using Serilog.Events;
using System;
using Xunit;

namespace Serilog.Settings.Delegates.Tests
{
    public class DestructureExceptionTests
    {
        //[Fact]
        //public void InvalidTransformationThrows()
        //{
        //    string returnType = "System.Type";
        //    string transformation = "n => n";

        //    var ex = Assert.Throws<ArgumentException>(() =>
        //        new LoggerConfiguration()
        //        .Destructure.ByTransforming(
        //            returnType: returnType,
        //            transformation: transformation
        //        ).WriteTo.Sink(new DummyConsoleSink()).CreateLogger());

        //    Assert.Contains("transformation failed to compile", ex.Message);
        //}

        //[Fact]
        //public void InvalidWhereTransformationThrows()
        //{
        //    string predicate = "t => typeof(Type).IsAssignableFrom(t)";
        //    string returnType = "System.Type";
        //    string transformation = "n => invalid.code";

        //    var ex = Assert.Throws<ArgumentException>(() =>
        //        new LoggerConfiguration()
        //        .Destructure.ByTransformingWhere(
        //            predicate: predicate,
        //            returnType: returnType,
        //            transformation: transformation
        //        ).WriteTo.Sink(new DummyConsoleSink()).CreateLogger());

        //    Assert.Contains("transformation failed to compile", ex.Message);
        //}

        //[Fact]
        //public void InvalidWherePredicateThrows()
        //{
        //    string predicate = "t => invalid.code";
        //    string returnType = "System.Type";
        //    string transformation = "n => n";

        //    var ex = Assert.Throws<ArgumentException>(() =>
        //        new LoggerConfiguration()
        //        .Destructure.ByTransformingWhere(
        //            predicate: predicate,
        //            returnType: returnType,
        //            transformation: transformation
        //        ).WriteTo.Sink(new DummyConsoleSink()).CreateLogger());

        //    Assert.Contains("predicate failed to compile", ex.Message);
        //}

        //[Fact]
        //public void InvalidReturnTypeThrows()
        //{
        //    string returnType = "Invalid.Type";
        //    string transformation = "n => n";

        //    var ex = Assert.Throws<ArgumentException>(() =>
        //        new LoggerConfiguration()
        //        .Destructure.ByTransforming(
        //            returnType: returnType,
        //            transformation: transformation
        //        ).WriteTo.Sink(new DummyConsoleSink()).CreateLogger());

        //    Assert.Contains("could not be resolved", ex.Message);
        //}

        //[Fact]
        //public void TransformDynamicThrows()
        //{
        //    string returnType = "dynamic";
        //    string transformation = "n => n";

        //    var ex = Assert.Throws<ArgumentException>(() =>
        //        new LoggerConfiguration()
        //        .Destructure.ByTransforming(
        //            returnType: returnType,
        //            transformation: transformation
        //        ).WriteTo.Sink(new DummyConsoleSink()).CreateLogger());

        //    Assert.Contains("Dynamic is not a type.", ex.Message);
        //}

        //[Fact]
        //public void TransformWhereDynamicThrows()
        //{
        //    string predicate = "t => true";
        //    string returnType = "dynamic";
        //    string transformation = "n => n";

        //    var ex = Assert.Throws<ArgumentException>(() =>
        //        new LoggerConfiguration()
        //        .Destructure.ByTransformingWhere(
        //            predicate: predicate,
        //            returnType: returnType,
        //            transformation: transformation
        //        ).WriteTo.Sink(new DummyConsoleSink()).CreateLogger());

        //    Assert.Contains("Dynamic is not a type.", ex.Message);
        //}
    }
}
