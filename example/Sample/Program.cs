﻿using Microsoft.Extensions.Configuration;
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


            //Configure(
            //    "Filter.ByIncludingOnly",
            //    "Include any word beginning with \"a\"");
            //FilterExamples_GenerateLogs();


            Configure(
                "Filter.ByIncludingOnly.ByExcluding",
                "Include any word beginning with \"a\" except \"ad\"");
            FilterExamples_GenerateLogs();


            //Configure(
            //    "Destructure.ByTransforming",
            //    "Log the account information excluding the user's password.");
            //TransformationExamples_GenerateLogs();


            //Configure(
            //    "Destructure.ByTransformingWhere",
            //    "No transform of account data, but transform String to Type to emit namespace.");
            //TransformationExamples_GenerateLogs(); // will not match (no transformation)
            //Log.Information("For a String type: {@Type}", typeof(String)); // this will match (will apply transformation)


            // Configuration through code might be interesting in scenarios like receiving
            // config information from a service client or loading from a database. The following
            // are equivalents of the various accompanying JSON configuration files.


            //string filterPredicate = "Matching.WithProperty<string>(\"Word\", w => w.StartsWith(\"a\"))";
            //Log.Logger = new LoggerConfiguration()
            //    .Filter.ByIncludingOnly(filterPredicate)
            //    .WriteTo.Console().CreateLogger();
            //FilterExamples_GenerateLogs();


            //string destructureReturnType = "Sample.Account";
            //string destructureTransformation = "a => new { a.id, a.Username, a.AccountType }";
            //Log.Logger = new LoggerConfiguration()
            //    .Destructure.ByTransforming(destructureReturnType, destructureTransformation)
            //    .WriteTo.Console().CreateLogger();
            //TransformationExamples_GenerateLogs();


            //string destructurePredicate = "t => typeof(Type).IsAssignableFrom(t)";
            //string destructureReturnType = "System.Type";
            //string destructureTransformation = "n => new { n.Namespace }";
            //Log.Logger = new LoggerConfiguration()
            //    .Destructure.ByTransformingWhere(destructurePredicate, destructureReturnType, destructureTransformation)
            //    .WriteTo.Console().CreateLogger();
            //TransformationExamples_GenerateLogs(); // will not match (no transformation)
            //Log.Information("For a String type: {@Type}", typeof(String)); // this will match (will apply transformation)


            Log.CloseAndFlush();

            // TravisCI build adds this switch to verify the program runs without needing input
            if(!args.Contains("--nopause"))
            {
                Console.WriteLine($"\nPress any key...");
                Console.ReadKey();
            }
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
