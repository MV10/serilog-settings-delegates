# Serilog.Settings.Delegates

A Serilog extension that allows configuration packages to create `Filter` and `Destructure` delegates from configuration data, such as using _Serilog.Settings.Configuration_ to read `appsettings.json`.

Four delegate-based methods are available.

- `Filter.ByIncludingOnly(`_`inclusionPredicate`_`)`
- `Filter.ByExcluding(`_`exclusionPredicate`_`)`
- `Destructure.ByTransforming(`_`transformedType, transformation`_`)`
- `Destructure.ByTransformingWhere(`_`predicate, transformedType, transformation`_`)`

All arguments are string values because they are intended to be populated from configuration data. The sample application in this repository demonstrates all methods both as code-based configuration (which would be of limited usefulness in practice) as well as loading various JSON configuration files.

## Delegate Syntax

The syntax of delegates loaded through configuration is exactly as you'd write them in C# source. This includes lambda expressions, references to static methods, and so on:

- `n => new { ... }`
- `Matching.WithProperty<string>("Word", w => w.Equals("klaatu"))`

Of course, you must respect the syntax of the configuration source you're using, so that second example would require escaping the inner quotation marks in a JSON configuration file:

- `"exclusionPredicate": "Matching.WithProperty<string>(\"Word\", w => w.Equals(\"klaatu\"))"`

## Specifying Types

The destructure methods accept a `transformedType` argument. Use the fully-qualified name of the target type (i.e. namespace and type name). For example, the `Account` class defined in the `Sample` namespace in the repository's sample code is referenced as `Sample.Account`.

Due to language limitations, it is not possible to specify `dynamic` as the `transformedType` (because it is not actually a type, even though it's possible to write code using the keyword in place of a generic type).

## Performance and Limitations

This package uses the Roslyn Scripting API to compile the string-formatted delegate expressions.

Destructuring support is compatible with _Serilog.Settings.Configuration_ NuGet packages _**newer than**_ `3.0.0-dev-00108`. Filtering support is compatible with any current _Serilog.Settings.Configuration_ NuGet package. It has not been tested by the author with other Serilog configuration packages.

Because the delegates are true compiled code, there is no overhead added to the event logging process compared to delegates created through code.

However, during configuration, the first call to this package adds a slight delay (typically 3 or 4 seconds) while the package initializes the scripting environment. This initialization information is reused for subsequent delegates. Initialization involves collecting a list of all loaded assemblies and their namespaces. (This is more efficient than attempting to only load specific required assemblies and namespaces, because the assembly search process to load specific references would take a few seconds _each time_. Loading specific references for two or more delegates could easily take longer than just loading all references automatically.)

Presently it is not possible to specify additional assemblies or namespaces which are not automatically loaded as part of the project. Thus, you may find that some portion of a delegate expression still fails because it has additional dependencies that aren't otherwise in use throughout the project. If you encounter such a scenario, please open an issue, it should be possible to add extension methods to force loading of custom references, although as explained above, these will have a start-up performance impact.

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
          "transformedType": "Sample.Account",
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
          "transformedType": "System.Type",
          "transformation": "n => new { n.Namespace }"
        }
      }
    ]
  }
}
```
