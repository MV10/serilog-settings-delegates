# Serilog.Settings.Delegates [![Build status](https://ci.appveyor.com/api/projects/status/s84mvw8yx6gja024?svg=true)](https://ci.appveyor.com/project/MV10/serilog-settings-delegates)  [![NuGet](https://img.shields.io/nuget/v/Serilog.Settings.Delegates.svg)](https://nuget.org/packages/Serilog.Settings.Delegates)

A Serilog extension that allows you to filter and transform log entries with snippets of code stored in configuration. More specifically, it extends configuration packages such as _Serilog.Settings.Configuration_ to accept `Filter` and `Destructure` delegates that could be stored in config files like `appsettings.json` or even environment variables.

Four methods are added to the root `Serilog` configuration key:

- `Filter.ByIncludingOnly(`_`inclusionPredicate`_`)`
- `Filter.ByExcluding(`_`exclusionPredicate`_`)`
- `Destructure.ByTransforming(`_`returnType, transformation`_`)`
- `Destructure.ByTransformingWhere(`_`predicate, returnType, transformation`_`)`

All arguments are string values because they are intended to be populated from configuration data. The sample application in this repository demonstrates all methods both as code-based configuration (which would be of limited usefulness in practice) as well as loading various JSON configuration files.

## Serilog and .NET Compatibility

_Serilog.Settings.Configuration_

- Filter is supported with any current version
- Destructure requires version [`3.0.0-dev-00112`](https://www.nuget.org/packages/Serilog.Settings.Configuration/3.0.0-dev-00112) or newer

_Serilog.Settings.AppSettings_

- Unknown, not yet tested (please PR a README update if you know!)

_.NET Standard 2.0_

- .NET Core 2.x
- ASP.NET Core 2.x
- .NET Framework 6+

## Delegate Syntax

The syntax of configuration delegates is exactly as you'd write them in C# source. This includes lambda expressions, references to static methods, and so on:

- `n => new { ... }`
- `Matching.WithProperty<string>("Word", w => w.Equals("klaatu"))`

Of course, you must respect the rules of the configuration source you're using, so that second example would require escaping the inner quotation marks in a JSON configuration file:

- `"exclusionPredicate": "Matching.WithProperty<string>(\"Word\", w => w.Equals(\"klaatu\"))"`

## Specifying Return Type

The destructure methods require a `returnType` argument. Use the fully-qualified name of the target type (i.e. namespace and type name). For example, the `Account` class defined in the `Sample` namespace in the repository's sample code is referenced as `Sample.Account`.

Due to language limitations, it is not possible to specify `dynamic` as the `returnType` because it is not actually a type, even though it's possible to write code using the `dynamic` keyword in place of a generic type. For example, in code you can write `.Destrcture.ByTransforming<dynamic>(...)` and it will compile and run, but you cannot express this through reflection, which is used to represent the `returnType` named by configuration.

## Initialization Overhead

This package uses the Roslyn Scripting API to compile the string-formatted delegate expressions. Because the delegates are true compiled code, there is no overhead added to the event logging process compared to delegates created through code.

However, during configuration, the first call to this package adds a slight delay (typically 3 or 4 seconds) while the package initializes the scripting environment. This initialization information is reused for subsequent delegates. (Initialization involves collecting a list of _all_ loaded assemblies and their namespaces. This is more efficient than attempting to only load specific required assemblies and namespaces, because the assembly search process to load specific references would take a few seconds for _each_ reference. Loading specific references for two or more delegates would almost always take longer than just loading all references once, automatically.)

This initialization delay should be acceptable in long-lifecycle scenarios such as starting an ASP.NET Core web application or service, but it may not be acceptable in high-volume, short-lifecycle environments such as Azure Functions with scale-out configurations.

Presently it is not possible to specify additional assemblies or namespaces which are not automatically loaded as part of the project and its dependencies. Thus, you may find that some portion of a delegate expression still fails because it has additional dependencies that aren't otherwise in use throughout the project. If you encounter such a scenario, please open an issue, it should be possible to add extension methods to force loading of custom references, although as explained above, these will have a start-up performance impact.

## Examples

#### Filter Delegates

The sample code in this repository includes examples of using the Filter delegates. In the JSON syntax recognized by the _Serilog.Settings.Configuration_ package:

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console" ],
    "WriteTo": [ "Console" ],
    "Filter": [
      {
        "Name": "ByIncludingOnly",
        "Args": { "inclusionPredicate": "Matching.WithProperty<string>(\"Word\", w => w.StartsWith(\"a\"))" }
      },
      {
        "Name": "ByExcluding",
        "Args": { "exclusionPredicate": "Matching.WithProperty<string>(\"Word\", w => w.Equals(\"ad\"))" }
      }
    ]
  }
}
```

#### Destructure Delegates

The repository's sample code also includes examples of Destructure delegates. In the JSON syntax recognized by the _Serilog.Settings.Configuration_ package:

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console" ],
    "WriteTo": [ "Console" ],
    "Destructure": [
      {
        "Name": "ByTransforming",
        "Args": {
          "returnType": "Sample.Account",
          "transformation": "a => new { a.id, a.Username, a.AccountType }"
        }
      }
    ]
  }
}
```

This example (also from the repository's sample code) applies the transformation to any type that is derived from `System.Type` (which is what the transformation expression is expected to return). The predicate could just as easily call a local method to inspect the properties of the type, or perform any other test to determine whether the transformation expression is applicable.

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console" ],
    "WriteTo": [ "Console" ],
    "Destructure": [
      {
        "Name": "ByTransformingWhere",
        "Args": {
          "predicate": "t => typeof(Type).IsAssignableFrom(t)",
          "returnType": "System.Type",
          "transformation": "n => new { n.Namespace }"
        }
      }
    ]
  }
}
```
