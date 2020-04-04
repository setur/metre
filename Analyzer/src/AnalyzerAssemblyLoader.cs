// --------------------------------------------------------------------------
// <copyright file="AnalyzerAssemblyLoader.cs" company="Setur">
//   Copyright (c) 2020 Setur (yazilim@setur.com.tr). All rights reserved.
//   Licensed under the Apache-2.0 license. See LICENSE file in the project root
//   for full license information.
//
//   Web: https://setur.com.tr/ GitHub: https://github.com/setur
// </copyright>
// --------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using System.Reflection;

namespace Setur.Metre.Analyzer {
    public class AnalyzerAssemblyLoader : IAnalyzerAssemblyLoader {
        public void AddDependencyLocation(string fullPath) {
        }

        public Assembly LoadFromPath(string fullPath) {
            return Assembly.LoadFrom(fullPath);
        }
    }
}
