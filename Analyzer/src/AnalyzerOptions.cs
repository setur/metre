// --------------------------------------------------------------------------
// <copyright file="AnalyzerOptions.cs" company="Setur">
//   Copyright (c) 2020 Setur (yazilim@setur.com.tr). All rights reserved.
//   Licensed under the Apache-2.0 license. See LICENSE file in the project root
//   for full license information.
//
//   Web: https://setur.com.tr/ GitHub: https://github.com/setur
// </copyright>
// --------------------------------------------------------------------------

using System.Collections.Generic;

namespace Setur.Metre.Analyzer {
    public class AnalyzerOptions : BaseModuleOptions {
        public IEnumerable<string> Filenames { get; set; }
    }
}
