using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Settings.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each example-block below is mutually exclusive


            // Configuration through code might be interesting in scenarios like receiving
            // config information from a service client or loading from a database.
            //string filterPredicate = "Matching.WithProperty<string>(\"Word\", w => w.StartsWith(\"a\"))";
            //Log.Logger = new LoggerConfiguration()
            //    .Filter.ByIncludingOnly(filterPredicate)
            //    .WriteTo.Console().CreateLogger();
            //FilterExamples_GenerateLogs();


            //Configure("Filter.ByIncludingOnly", "Include any word beginning with \"a\"");
            //FilterExamples_GenerateLogs();


            //Configure("Filter.ByIncludingOnly.ByExcluding", "Include any word beginning with \"a\" except \"ad\"");
            //FilterExamples_GenerateLogs();


            // *** NOT WORKING YET: Serilog.Settings.Configuration doesn't have an ApplyDestructure feature yet
            //Configure("Destructure.ByTransforming", "Log the account information excluding the user's password.");
            //TransformationExamples_GenerateLogs();


            // *** NOT WORKING YET: Serilog.Settings.Configuration doesn't have an ApplyDestructure feature yet
            //Configure("Destructure.ByTransformingWhere", "Log the account information excluding the user's password with predicate control.");
            //TransformationExamples_GenerateLogs();


            //string destructureTransformedType = "Sample.Account";
            //string destructureTransformation = "a => new { a.id, a.Username, a.AccountType }";
            //Log.Logger = new LoggerConfiguration()
            //    .Destructure.ByTransforming(destructureTransformedType, destructureTransformation)
            //    .WriteTo.Console().CreateLogger();
            //TransformationExamples_GenerateLogs();


            //Log.Logger = new LoggerConfiguration()
            //    .Destructure.ByTransforming<Account>(a => new { a.id, a.Username, a.AccountType })
            //    .WriteTo.Console().CreateLogger();
            //TransformationExamples_GenerateLogs();


            //Log.Logger = new LoggerConfiguration()
            //    .Destructure.ByTransforming<String>(a => new { a.GetType().Namespace })
            //    .WriteTo.Console().CreateLogger();
            //Log.Information("Foo: {@SayWhat}", typeof(String));


            //string destructurePredicate = "t => typeof(Account).Equals(t)";
            //string destructureTransformedType = "Sample.Account";
            //string destructureTransformation = "a => new { a.id, a.Username, a.AccountType }";
            //Log.Logger = new LoggerConfiguration()
            //    .Destructure.ByTransformingWhere(destructurePredicate, destructureTransformedType, destructureTransformation)
            //    .WriteTo.Console().CreateLogger();
            //TransformationExamples_GenerateLogs(); // will not match (no transformation)
            //Log.Information("For a String type: {@Type}", typeof(String)); // this will match (will apply transformation)


            string destructurePredicate = "t => typeof(Type).IsAssignableFrom(t)";
            string destructureTransformedType = "System.Type";
            string destructureTransformation = "n => new { n.Namespace }";
            Log.Logger = new LoggerConfiguration()
                .Destructure.ByTransformingWhere(destructurePredicate, destructureTransformedType, destructureTransformation)
                .WriteTo.Console().CreateLogger();
            TransformationExamples_GenerateLogs(); // will not match (no transformation)
            Log.Information("For a String type: {@Type}", typeof(String)); // this will match (will apply transformation)


            Log.CloseAndFlush();
            Console.WriteLine($"\nPress any key...");
            Console.ReadKey();
        }

        static void TransformationExamples_GenerateLogs()
        {
            Console.WriteLine("\nThe Destructure examples log a series of account login events.\n");
            var logins = new List<Account>
            {
                new Account { id = 123, Username = "Deckard", Password = "NotLittlePeople", AccountType = "admin" },
                new Account { id = 345, Username = "Rachel", Password = "MoreHumanThanHuman", AccountType = "staff" },
                new Account { id = 456, Username = "Roy", Password = "TannhauserGate", AccountType = "retired" },
                new Account { id = 678, Username = "Pris", Password = "IThinkSebastian", AccountType = "retired" },
            };

            foreach(var acct in logins)
            {
                Log.Information("Login for {@User}", acct);
            }
        }

        static void FilterExamples_GenerateLogs()
        {
            string filterData = "entitas ipsa involvit aptitudinem ad extorquendum certum assensum";
            Console.WriteLine($"\nThe Filter examples log each word in this string:\n\"{filterData}\"\n");
            foreach(string s in filterData.Split(" "))
            {
                Log.Information("Have some latin! {Word}", s);
            }
        }

        static void Configure(string filename, string description)
        {
            Console.WriteLine($"Running configuration: {filename}\nEffect: {description}");

            // json files need "Copy To Output Directory" in the file build properties
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"{filename}.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }

    // Used in the destructuring examples
    public class Account
    {
        public int id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccountType { get; set; }
    }
}
