// --------------------------------------------------------------------------
// <copyright file="Runtime.cs" company="Setur">
//   Copyright (c) 2020 Setur (yazilim@setur.com.tr). All rights reserved.
//   Licensed under the Apache-2.0 license. See LICENSE file in the project root
//   for full license information.
//
//   Web: https://setur.com.tr/ GitHub: https://github.com/setur
// </copyright>
// --------------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Setur.Metre.Cli {
    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "It's the console", MessageId = "CancellationTokenSource")]
    public class Runtime : IDisposable {
        public const string MessageOperationIsCancelled = nameof(Runtime.MessageOperationIsCancelled);

        private Stream Output;
        private TextWriter OutputWriter;
        private CancellationTokenSource CancellationTokenSource;
        private bool DisposedValue;

        public Runtime(Stream output) {
            this.CancellationTokenSource = new CancellationTokenSource();
            this.Output = output;
            this.OutputWriter = new StreamWriter(output);
        }

        public IConfigurationRoot InitializeConfiguration() {
            var configuration = new ConfigurationBuilder();

            configuration.AddEnvironmentVariables();

            return configuration.Build();
        }

        public IServiceProvider InitializeServices(Action<IServiceCollection> serviceCollectionBuilder) {
            if (serviceCollectionBuilder == null) {
                throw new ArgumentNullException(nameof(serviceCollectionBuilder));
            }

            var serviceCollection = new ServiceCollection();

            serviceCollectionBuilder.Invoke(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        public IServiceProvider Initialize() {
            var configuration = this.InitializeConfiguration();

            return this.InitializeServices((serviceCollection) => {
                serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
                serviceCollection.AddOptions();

                serviceCollection.AddLocalization(options => {
                    options.ResourcesPath = "Resources";
                });

                serviceCollection.AddLogging((loggingBuilder) => {
                    loggingBuilder.SetMinimumLevel(LogLevel.Information);
                    loggingBuilder.AddConsole();
                });
            });
        }

        public async Task<int> Run(string[] args) {
            if (args == null) {
                throw new ArgumentNullException(nameof(args));
            }

            var cliInput = new CliInput();
            var commandResult = cliInput.Interprete(args);

            if (commandResult == null) {
                return 1;
            }

            var services = this.Initialize();

            var localizer = services.GetRequiredService<IStringLocalizer<Runtime>>();

            var exitCode = 0;

            try
            {
                await commandResult.Run(services, this.CancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                await this.OutputWriter.WriteLineAsync(localizer[Runtime.MessageOperationIsCancelled]).ConfigureAwait(false);
                exitCode = -1;
            }

            return exitCode;
        }

        public void Cancel() {
            this.CancellationTokenSource.Cancel();
        }

        protected virtual void Dispose(bool disposing) {
            if (!this.DisposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects)
                    this.OutputWriter.Dispose();
                    this.CancellationTokenSource.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null

                this.DisposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Runtime()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
