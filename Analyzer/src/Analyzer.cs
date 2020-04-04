// --------------------------------------------------------------------------
// <copyright file="Analyzer.cs" company="Setur">
//   Copyright (c) 2020 Setur (yazilim@setur.com.tr). All rights reserved.
//   Licensed under the Apache-2.0 license. See LICENSE file in the project root
//   for full license information.
//
//   Web: https://setur.com.tr/ GitHub: https://github.com/setur
// </copyright>
// --------------------------------------------------------------------------

using Microsoft.Build.Execution;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeMetrics;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Setur.Metre.Analyzer {
    public class Analyzer : IModule {
        public Analyzer(AnalyzerOptions options) {
            this.Options = options;
        }

        public AnalyzerOptions Options { get; set; }

        public async Task<AnalyzerOutput> ComputeMetricDataForProjectAsync(Project project, CancellationToken cancellationToken) {
            if (project == null) {
                throw new ArgumentNullException(nameof(project));
            }

            if (!project.SupportsCompilation) {
                // TODO log warning
                return new AnalyzerOutput() {
                    Name = project.FilePath,
                    MetricData = null,
                    CompilerDiagnostics = null,
                };
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (project.CompilationOptions == null) {
                // TODO needs more accurate exception type
                throw new InvalidDataException(nameof(project.CompilationOptions));
            }

            // var projectModified = project;
            var projectModified = project
                .WithCompilationOptions(
                    project.CompilationOptions
                        .WithOutputKind(OutputKind.DynamicallyLinkedLibrary)
                        .WithGeneralDiagnosticOption(ReportDiagnostic.Default)
                        .WithReportSuppressedDiagnostics(true)
                        .WithMetadataImportOptions(MetadataImportOptions.All));

            var compilation = await projectModified.GetCompilationAsync(cancellationToken).ConfigureAwait(false);

            if (compilation == null) {
                // TODO needs more accurate exception type
                throw new InvalidDataException(nameof(compilation));
            }

            var buildResult = compilation.Emit(Console.OpenStandardOutput(), cancellationToken: cancellationToken);

            if (!buildResult.Success) {
                // throw new Exception("hata");
            }

            var compilerDiagnostics = buildResult.Diagnostics; // compilation.GetDiagnostics(cancellationToken);
            // compilation.WithAnalyzers
            var effectiveDiagnostics = CompilationWithAnalyzers.GetEffectiveDiagnostics(compilerDiagnostics, compilation);

            var metricData = await CodeAnalysisMetricData.ComputeAsync(compilation, cancellationToken).ConfigureAwait(false);

            return new AnalyzerOutput() {
                Name = project.FilePath,
                MetricData = metricData,
                CompilerDiagnostics = effectiveDiagnostics,
            };
        }

        public async Task<AnalyzerOutput> ComputeMetricDataForProjectAsync(MSBuildWorkspace workspace, string filename, CancellationToken cancellationToken) {
            if (workspace == null) {
                throw new ArgumentNullException(nameof(workspace));
            }

            var project = await workspace.OpenProjectAsync(filename, null, cancellationToken).ConfigureAwait(false);

            return await this.ComputeMetricDataForProjectAsync(project, cancellationToken).ConfigureAwait(false);
        }

        public async IAsyncEnumerable<AnalyzerOutput> ComputeMetricDataForSolutionAsync(Solution solution, [EnumeratorCancellation] CancellationToken cancellationToken) {
            if (solution == null) {
                throw new ArgumentNullException(nameof(solution));
            }

            foreach (var project in solution.Projects) {
                var metricData = await this.ComputeMetricDataForProjectAsync(project, cancellationToken).ConfigureAwait(false);

                yield return metricData;
            }
        }

        public async IAsyncEnumerable<AnalyzerOutput> ComputeMetricDataForSolutionAsync(MSBuildWorkspace workspace, string filename, [EnumeratorCancellation] CancellationToken cancellationToken) {
            if (workspace == null) {
                throw new ArgumentNullException(nameof(workspace));
            }

            var solution = await workspace.OpenSolutionAsync(filename, null, cancellationToken).ConfigureAwait(false);

            var metricDataCollection = this.ComputeMetricDataForSolutionAsync(solution, cancellationToken).ConfigureAwait(false);

            await foreach (var metricData in metricDataCollection) {
                yield return metricData;
            }
        }

        public async IAsyncEnumerable<AnalyzerOutput> ComputeMetricDataAsync(MSBuildWorkspace workspace, IEnumerable<string> filenames, [EnumeratorCancellation] CancellationToken cancellationToken) {
            if (filenames == null) {
                throw new ArgumentNullException(nameof(filenames));
            }

            // TODO add glob support
            foreach (var filename in filenames) {
                var extension = Path.GetExtension(filename);

                if (string.Compare(extension, ".sln", StringComparison.OrdinalIgnoreCase) == 0) {
                    var metricDataCollection = this.ComputeMetricDataForSolutionAsync(workspace, filename, cancellationToken).ConfigureAwait(false);

                    await foreach (var metricData in metricDataCollection) {
                        yield return metricData;
                    }

                    yield break;
                }

                yield return await this.ComputeMetricDataForProjectAsync(workspace, filename, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task WriteOutput(ConfiguredCancelableAsyncEnumerable<AnalyzerOutput> metricDataCollection) { // IAsyncEnumerable ?
            var jsonOptions = new JsonWriterOptions() {
                Indented = true,
            };
            using var jsonWriter = new Utf8JsonWriter(Console.OpenStandardOutput(), jsonOptions);

            var arr = new List<AnalyzerOutput>();
            await foreach (var metricData in metricDataCollection) {
                arr.Add(metricData);
            }

            AnalyzerJsonOutputWriter.WriteMetricFile(arr, jsonWriter);
        }

        public async Task ProcessMetricsAsync(ILogger logger, CancellationToken cancellationToken) {
            MSBuildLocator.RegisterDefaults();

            var workspaceProps = new Dictionary<string, string>() {
                { "CheckForSystemRuntimeDependency", "true" },
            };

            using var workspace = MSBuildWorkspace.Create(workspaceProps);
            // workspace.LoadMetadataForReferencedProjects = true;

            var metricDataCollection = this.ComputeMetricDataAsync(workspace, this.Options.Filenames, cancellationToken).ConfigureAwait(false);
            await this.WriteOutput(metricDataCollection).ConfigureAwait(false);
        }

        public async Task Run(IServiceProvider services, CancellationToken cancellationToken) {
            var logger = services.GetRequiredService<ILogger<Analyzer>>();

            var result = $"Projects = {string.Join(", ", this.Options.Filenames)}";

            logger.LogInformation(result);

            await this.ProcessMetricsAsync(logger, cancellationToken).ConfigureAwait(false);
        }
    }
}
