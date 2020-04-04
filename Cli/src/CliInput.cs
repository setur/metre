// --------------------------------------------------------------------------
// <copyright file="CliInput.cs" company="Setur">
//   Copyright (c) 2020 Setur (yazilim@setur.com.tr). All rights reserved.
//   Licensed under the Apache-2.0 license. See LICENSE file in the project root
//   for full license information.
//
//   Web: https://setur.com.tr/ GitHub: https://github.com/setur
// </copyright>
// --------------------------------------------------------------------------

using Microsoft.Extensions.CommandLineUtils;
using Setur.Metre.Analyzer;
using System;

namespace Setur.Metre.Cli {
    public class CliInput {
        public IModule? Interprete(string[] args) {
            const int ExitSuccess = 0;

            if (args == null) {
                throw new ArgumentNullException(nameof(args));
            }

            IModule? result = null;

            var commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: true);
            commandLineApplication.FullName = "Metre";

            var helpOption = commandLineApplication.HelpOption("--help | -h | -?");
            var versionOption = commandLineApplication.VersionOption("--version | -v", "1.0.0");

            var outputTypeOption = commandLineApplication.Option("--output-type | -ot", "Output type", CommandOptionType.SingleValue);
            var outputFormatOption = commandLineApplication.Option("--output-format | -of", "Output format", CommandOptionType.SingleValue);

            T InitModuleOptions<T>()
                where T : BaseModuleOptions, new() {
                var options = new T();

                if (outputTypeOption.HasValue()) {
                    options.OutputType = (ModuleOutputType)Enum.Parse(typeof(ModuleOutputType), outputTypeOption.Value()); // Enum.Parse<ModuleOutputType>(outputTypeOption.Value());
                }

                if (outputFormatOption.HasValue()) {
                    options.OutputFormat = (ModuleOutputFormat)Enum.Parse(typeof(ModuleOutputFormat), outputFormatOption.Value()); // Enum.Parse<ModuleOutputFormat>(outputFormatOption.Value());
                }

                return options;
            }

            var analyzeCommand = commandLineApplication.Command(
                "analyze",
                (command) => {
                    command.Description = "Analyzes projects";

                    var projectsArgument = command.Argument("projects", "Enter project path(s) will be analyzed", multipleValues: true);

                    command.OnExecute(() => {
                        var options = InitModuleOptions<AnalyzerOptions>();
                        options.Filenames = projectsArgument.Values;

                        result = new Analyzer.Analyzer(options);

                        return ExitSuccess;
                    });
                },
                throwOnUnexpectedArg: true);

            commandLineApplication.OnExecute(() => {
                commandLineApplication.ShowHelp();

                return ExitSuccess;
            });

            commandLineApplication.Execute(args);

            return result;
        }
    }
}
