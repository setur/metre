// --------------------------------------------------------------------------
// <copyright file="AnalyzerOutput.cs" company="Setur">
//   Copyright (c) 2020 Setur (yazilim@setur.com.tr). All rights reserved.
//   Licensed under the Apache-2.0 license. See LICENSE file in the project root
//   for full license information.
//
//   Web: https://setur.com.tr/ GitHub: https://github.com/setur
// </copyright>
// --------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeMetrics;
using System.Collections.Generic;

namespace Setur.Metre.Analyzer {
    public class AnalyzerOutput {
        public string? Name { get; set; }

        public CodeAnalysisMetricData? MetricData { get; set; }

        public IEnumerable<Diagnostic>? CompilerDiagnostics { get; set; }
    }
}
