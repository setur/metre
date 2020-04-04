// --------------------------------------------------------------------------
// <copyright file="Program.cs" company="Setur">
//   Copyright (c) 2020 Setur (yazilim@setur.com.tr). All rights reserved.
//   Licensed under the Apache-2.0 license. See LICENSE file in the project root
//   for full license information.
//
//   Web: https://setur.com.tr/ GitHub: https://github.com/setur
// </copyright>
// --------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Setur.Metre.Cli {
    public static class Program {
        public static async Task<int> Main(string[] args) {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");

            using var stream = Console.OpenStandardOutput();
            using var runtime = new Runtime(stream);

            Console.CancelKeyPress += (sender, e) => {
                runtime.Cancel();
            };

            var exitCode = await runtime.Run(args).ConfigureAwait(false);

            // FIXME I don't know why I had to do this
            Console.ResetColor();
            Thread.Sleep(TimeSpan.FromMilliseconds(200));

            return exitCode;
        }
    }
}
