using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// For the commented-out configure-from-code examples:
using Serilog.Settings.Delegates;
using System.Reflection;

// JSON files require these properties (otherwise the examples throw File Not Found):
// Build Action: Content
// Copy to Output Directory: Copy If Newer

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // TravisCI build adds the nopause switch to verify the program runs without needing input
            while (!args.Contains("--nopause") && DemoMenu()) ;
        }

        static bool DemoMenu()
        {
            Console.Clear();
            Console.WriteLine("Serilog.Settings.Delegates\n");
            if(elapsed == null)
            {
                Console.WriteLine("(Startup overhead of Rosyln init will be measured on first run.)\n");
            }
            else
            {
                Console.WriteLine($"First run init overhead: {elapsed} (see README).\n");
            }
            Console.WriteLine("Examples loaded from JSON configuration:\n");
            Console.WriteLine("1. Filter: Include any word beginning with \"a\"");
            Console.WriteLine("2. Filter: Include any word beginning with \"a\" except \"ad\"");
            Console.WriteLine("3. Destructure: Log account objects excluding user passwords");
            Console.WriteLine("4. Destructure: Log namespace for objects derived from Type\n");
            Console.Write("Choose an example, or press any other key to quit: ");

            var opt = Console.ReadKey();
            switch(opt.Key)
            {
                case ConsoleKey.D1:
                    StartDemo(
                        "Filter.ByIncludingOnly",
                        "Include any word beginning with \"a\"");
                    FilterExamples_GenerateLogs();
                    break;

                case ConsoleKey.D2:
                    StartDemo(
                        "Filter.ByIncludingOnly.ByExcluding",
                        "Include any word beginning with \"a\" except \"ad\"");
                    FilterExamples_GenerateLogs();
                    break;

                case ConsoleKey.D3:
                    StartDemo(
                        "Destructure.ByTransforming",
                        "Log account data omitting user passwords");
                    TransformationExamples_GenerateLogs(false);
                    break;

                case ConsoleKey.D4:
                    StartDemo(
                        "Destructure.ByTransformingWhere",
                        "Log namespace if derived from Type, account logs untouched");
                    TransformationExamples_GenerateLogs(true);
                    break;

                default:
                    return false;
            }

            Log.CloseAndFlush();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return true;

            /*
            The following are code-configured equivalents to the JSON examples.
            Configuration through code might be interesting for loading expressions
            from a database or other sources not readily available as config targets.

            string filterPredicate = "Matching.WithProperty<string>(\"Word\", w => w.StartsWith(\"a\"))";
            Log.Logger = new LoggerConfiguration()
                .Filter.ByIncludingOnly(filterPredicate)
                .WriteTo.Console().CreateLogger();
            FilterExamples_GenerateLogs();

            string destructureReturnType = "Sample.Account";
            string destructureTransformation = "a => new { a.id, a.Username, a.AccountType }";
            Log.Logger = new LoggerConfiguration()
                .Destructure.ByTransforming(destructureReturnType, destructureTransformation)
                .WriteTo.Console().CreateLogger();
            TransformationExamples_GenerateLogs(false);

            string destructurePredicate = "t => typeof(Type).IsAssignableFrom(t)";
            string destructureReturnType = "System.Type";
            string destructureTransformation = "n => new { n.Namespace }";
            Log.Logger = new LoggerConfiguration()
                .Destructure.ByTransformingWhere(destructurePredicate, destructureReturnType, destructureTransformation)
                .WriteTo.Console().CreateLogger();
            TransformationExamples_GenerateLogs(true);
            */
        }

        static void TransformationExamples_GenerateLogs(bool withPredicateDemo)
        {
            Console.WriteLine("Without destructuring:");
            foreach (var acct in logins)
            {
                consoleOnlyLogger.Information("Login for {@User}", acct);
            }
            if(withPredicateDemo)
                consoleOnlyLogger.Information("For typeof(String): {@Type}", typeof(String));

            Console.WriteLine("\nWith destructuring:");
            foreach (var acct in logins)
            {
                Log.Information("Login for {@User}", acct);
            }
            if (withPredicateDemo)
                Log.Information("For typeof(String): {@Type}", typeof(String));
        }

        static void FilterExamples_GenerateLogs()
        {
            Console.WriteLine($"This example logs each word in this string:\n\"{filterData}\"\n\nWithout filtering:");
            foreach (string s in filterData.Split(" "))
            {
                consoleOnlyLogger.Information("Have some latin! {Word}", s);
            }

            Console.WriteLine("\nWith filtering:");
            foreach(string s in filterData.Split(" "))
            {
                Log.Information("Have some latin! {Word}", s);
            }
        }

        static void StartDemo(string filename, string description)
        {
            Console.Clear();

            var consoleConfig = new ConfigurationBuilder()
                .AddJsonFile("ConsoleOutputOnly.json")
                .Build();

            consoleOnlyLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(consoleConfig)
                .CreateLogger();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"{filename}.json")
                .Build();

            Stopwatch timing = new Stopwatch();
            if(elapsed == null)
            {
                Console.WriteLine($"Timing Serilog startup with scripting engine init...");
                timing.Start();
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            if(elapsed == null)
            {
                timing.Stop();
                elapsed = $"{timing.Elapsed.ToString(@"s\.f")} seconds";
                Console.WriteLine($"Startup time: {elapsed} (first run only, see README).\n");
            }

            Console.WriteLine($"{filename}.json\n{description}\n");
        }

        static List<Account> logins = new List<Account>
            {
                new Account { id = 123, Username = "Deckard", Password = "NotLittlePeople", AccountType = "admin" },
                new Account { id = 345, Username = "Rachel", Password = "MoreHumanThanHuman", AccountType = "staff" },
                new Account { id = 456, Username = "Roy", Password = "TannhauserGate", AccountType = "retired" },
                new Account { id = 678, Username = "Pris", Password = "IThinkSebastian", AccountType = "retired" },
            };

        static string filterData = "entitas ipsa involvit aptitudinem ad extorquendum certum assensum";

        static ILogger consoleOnlyLogger;

        static string elapsed = null;

        class Account
        {
            public int id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string AccountType { get; set; }
        }
    }
}
