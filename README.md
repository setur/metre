# 🌡 [Metre](https://github.com/setur/metre)

Metre is a DevOps tool that simply integrates into your build pipeline
and utilizes msbuild to generate some static code analysis metrics and
diagnostics of compilation output. Output reports are generated in
structured format and can be pushed to elasticsearch-like services in
order to aggregate them via reporting tools.


## Sample Output
```
{
  "targets": [
    {
      "filepath": "metre/Cli/Cli.csproj",
      "kind": "Assembly",
      "name": "Metre.Cli, Version=0.6.0.0, Culture=neutral, PublicKeyToken=null",
      "metrics": {
        "maintainabilityIndex": "64",
        "cyclomaticComplexity": "9",
        "classCoupling": "17",
        "depthOfInheritance": "1",
        "sourceLines": "128",
        "executableLines": "54"
      },
      "children": [
        {
          "kind": "Namespace",
          "name": "Setur.Metre.Cli",
          "metrics": {
            "maintainabilityIndex": "64",
            "cyclomaticComplexity": "9",
            "classCoupling": "17",
            "depthOfInheritance": "1",
            "sourceLines": "128",
            "executableLines": "54"
          },
          "children": [
            {
              "kind": "NamedType",
              "name": "CliInput",
              "metrics": {
                "maintainabilityIndex": "55",
                "cyclomaticComplexity": "2",
                "classCoupling": "6",
                "depthOfInheritance": "1",
                "sourceLines": "42",
                "executableLines": "19"
              },
              "children": [
                {
                  "filepath": "metre/Cli/src/CliInput.cs",
                  "line": "17",
                  "kind": "Method",
                  "name": "ICommand? CliInput.Interprete(string[] args)",
                  "metrics": {
                    "maintainabilityIndex": "55",
                    "cyclomaticComplexity": "2",
                    "classCoupling": "6",
                    "sourceLines": "40",
                    "executableLines": "19"
                  }
                }
              ]
            },
            ...
          ],
          ...
        },
        ...
      ],
      "compilerMessages": [
        {
          "id": "CS8021",
          "severity": "Warning",
          "category": "Compiler",
          "location": {
            "kind": "None"
          },
          "message": "No value for RuntimeMetadataVersion found. No assembly containing System.Object was found nor was a value for RuntimeMetadataVersion specified through options."
        },
        {
          "id": "CS0246",
          "severity": "Error",
          "category": "Compiler",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/CliInput.cs",
            "position": [
              10,
              6,
              10,
              15
            ]
          },
          "message": "The type or namespace name \u0027Microsoft\u0027 could not be found (are you missing a using directive or an assembly reference?)"
        },
        {
          "id": "CS0518",
          "severity": "Error",
          "category": "Compiler",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/Startup.cs",
            "position": [
              16,
              17,
              16,
              24
            ]
          },
          "message": "Predefined type \u0027System.Object\u0027 is not defined or imported"
        },
        {
          "id": "CS0246",
          "severity": "Error",
          "category": "Compiler",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/Startup.cs",
            "position": [
              17,
              15,
              17,
              33
            ]
          },
          "message": "The type or namespace name \u0027IConfigurationRoot\u0027 could not be found (are you missing a using directive or an assembly reference?)"
        },
        {
          "id": "CS0518",
          "severity": "Error",
          "category": "Compiler",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/Startup.cs",
            "position": [
              25,
              51,
              25,
              77
            ]
          },
          "message": "Predefined type \u0027System.Object\u0027 is not defined or imported"
        },
        {
          "id": "CS0246",
          "severity": "Error",
          "category": "Compiler",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/Startup.cs",
            "position": [
              25,
              51,
              25,
              77
            ]
          },
          "message": "The type or namespace name \u0027Action\u003C\u003E\u0027 could not be found (are you missing a using directive or an assembly reference?)"
        },
        {
          "id": "CS1729",
          "severity": "Error",
          "category": "Compiler",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/Startup.cs",
            "position": [
              16,
              17,
              16,
              24
            ]
          },
          "message": "\u0027object\u0027 does not contain a constructor that takes 0 arguments"
        },
        {
          "id": "CS0103",
          "severity": "Error",
          "category": "Compiler",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/Program.cs",
            "position": [
              51,
              12,
              51,
              19
            ]
          },
          "message": "The name \u0027Console\u0027 does not exist in the current context"
        },
        {
          "id": "CS0656",
          "severity": "Error",
          "category": "Compiler",
          "location": {
            "kind": "None"
          },
          "message": "Missing compiler required member \u0027System.AttributeUsageAttribute..ctor\u0027"
        },
        {
          "id": "CS1591",
          "severity": "Warning",
          "category": "Compiler",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/CliInput.cs",
            "position": [
              15,
              17,
              15,
              25
            ]
          },
          "message": "Missing XML comment for publicly visible type or member \u0027CliInput\u0027"
        },
        {
          "id": "SA1306",
          "severity": "Warning",
          "category": "Style",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/CliInput.cs",
            "position": [
              45,
              13,
              45,
              25
            ]
          },
          "message": "Field \u0027CancellationTokenSource\u0027 should begin with lower-case letter"
        },
        {
          "id": "CA1724",
          "severity": "Error",
          "category": "Design",
          "location": {
            "kind": "SourceFile",
            "filepath": "metre/Cli/src/CliInput.cs",
            "position": [
              8,
              11,
              8,
              22
            ]
          },
          "message": "The type name Runtime conflicts in whole or in part with the namespace name \u0027System.Runtime\u0027 defined in the .NET Framework. Rename the type to eliminate the conflict."
        },
        ...
      ]
    }
  ]
}
```

## CI/CD Targets

Not yet

## Quick start to use

Not yet

## Quick start to contribute

Ensure that `.NET Core SDK` is installed on your system first.

Clone a sample app's git repo `git clone
   https://github.com/setur/metre.git` - and switch to branch
   you'd like to contribute on.

Execute `dotnet restore` to install dependencies. Then run `dotnet build`
to ensure it is building properly.


## Todo List

See [GitHub Projects](https://github.com/setur/metre/projects) for more.


## Requirements

* .NET Core (https://www.microsoft.com/net/download)


## License

Apache License, Version 2.0, for further details, please see [LICENSE](LICENSE) file


## Contributing

See [contributors.md](contributors.md)

It is publicly open for any contribution. Bugfixes, new features and extra modules are welcome.

* To contribute to code: Fork the repo, push your changes to your fork, and submit a pull request.
* To report a bug: If something does not work, please report it using [GitHub Issues](https://github.com/setur/metre/issues).
