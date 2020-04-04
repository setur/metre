// --------------------------------------------------------------------------
// <copyright file="AnalyzerJsonOutputWriter.cs" company="Setur">
//   Copyright (c) 2020 Setur (yazilim@setur.com.tr). All rights reserved.
//   Licensed under the Apache-2.0 license. See LICENSE file in the project root
//   for full license information.
//
//   Web: https://setur.com.tr/ GitHub: https://github.com/setur
// </copyright>
// --------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeMetrics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Setur.Metre.Analyzer {
    public static class AnalyzerJsonOutputWriter {
        public static void WriteMetricFile(IEnumerable<AnalyzerOutput> data, Utf8JsonWriter writer) {
            if (data == null) {
                throw new ArgumentNullException(nameof(data));
            }

            writer.WriteStartObject();
            writer.WriteStartArray("targets");

            foreach (var kvp in data) {
                writer.WriteStartObject();
                writer.WriteString("filepath", kvp.Name);

                if (kvp.MetricData != null) {
                    WriteMetricData(kvp.MetricData, writer);
                }

                if (kvp.CompilerDiagnostics != null) {
                    WriteCompilerDiagnostics(kvp.CompilerDiagnostics, writer);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private static void WriteCompilerDiagnostics(IEnumerable<Diagnostic>? data, Utf8JsonWriter writer) {
            writer.WriteStartArray("compilerMessages");

            if (data != null) {
                foreach (var diagnostic in data) {
                    if (diagnostic.IsSuppressed) {
                        continue;
                    }

                    writer.WriteStartObject();
                    writer.WriteString("id", diagnostic.Descriptor.Id);
                    writer.WriteString("severity", diagnostic.Severity.ToString());
                    writer.WriteString("category", diagnostic.Descriptor.Category);

                    writer.WriteStartObject("location");
                    writer.WriteString("kind", diagnostic.Location.Kind.ToString());

                    if (diagnostic.Location.Kind == LocationKind.SourceFile) {
                        var sourceTree = diagnostic.Location?.SourceTree;

                        writer.WriteString("filepath", sourceTree?.FilePath);

                        writer.WriteStartArray("position");
                        writer.WriteNumberValue(diagnostic.Location.GetLineSpan().StartLinePosition.Line);
                        writer.WriteNumberValue(diagnostic.Location.GetLineSpan().StartLinePosition.Character);
                        writer.WriteNumberValue(diagnostic.Location.GetLineSpan().EndLinePosition.Line);
                        writer.WriteNumberValue(diagnostic.Location.GetLineSpan().EndLinePosition.Character);
                        writer.WriteEndArray();
                    }

                    writer.WriteEndObject();

                    writer.WriteString("message", diagnostic.GetMessage());
                    writer.WriteEndObject();
                }
            }

            writer.WriteEndArray();
        }

        private static void WriteMetricData(CodeAnalysisMetricData data, Utf8JsonWriter writer) {
            WriteHeader();
            WriteMetrics();
            WriteChildren();

            return;

            void WriteHeader() {
                switch (data.Symbol.Kind) {
                    case SymbolKind.NamedType:
                        var minimalTypeName = new StringBuilder(data.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));

                        var containingType = data.Symbol.ContainingType;
                        while (containingType != null) {
                            minimalTypeName.Insert(0, ".");
                            minimalTypeName.Insert(0, containingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                            containingType = containingType.ContainingType;
                        }

                        writer.WriteString("kind", data.Symbol.Kind.ToString());
                        writer.WriteString("name", minimalTypeName.ToString());
                        break;

                    case SymbolKind.Method:
                    case SymbolKind.Field:
                    case SymbolKind.Event:
                    case SymbolKind.Property:
                        var location = data.Symbol.Locations.First();
                        writer.WriteString("filepath", location.SourceTree?.FilePath ?? "Unknown");
                        writer.WriteString("line", (location.GetLineSpan().StartLinePosition.Line + 1).ToString(CultureInfo.InvariantCulture));
                        writer.WriteString("kind", data.Symbol.Kind.ToString());
                        writer.WriteString("name", data.Symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                        break;

                    default:
                        writer.WriteString("kind", data.Symbol.Kind.ToString());
                        writer.WriteString("name", data.Symbol.ToDisplayString());
                        break;
                }
            }

            void WriteMetrics() {
                writer.WriteStartObject("metrics");

                writer.WriteString("maintainabilityIndex", data.MaintainabilityIndex.ToString(CultureInfo.InvariantCulture));
                writer.WriteString("cyclomaticComplexity", data.CyclomaticComplexity.ToString(CultureInfo.InvariantCulture));
                writer.WriteString("classCoupling", data.CoupledNamedTypes.Count.ToString(CultureInfo.InvariantCulture));
                if (data.DepthOfInheritance.HasValue) {
                    writer.WriteString("depthOfInheritance", data.DepthOfInheritance.Value.ToString(CultureInfo.InvariantCulture));
                }

                writer.WriteString("sourceLines", data.SourceLines.ToString(CultureInfo.InvariantCulture));
                writer.WriteString("executableLines", data.ExecutableLines.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndObject();
            }

            void WriteChildren() {
                if (data.Children.Length == 0) {
                    return;
                }

                writer.WriteStartArray("children");
                foreach (var child in data.Children) {
                    writer.WriteStartObject();
                    WriteMetricData(child, writer);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }
        }
    }
}
