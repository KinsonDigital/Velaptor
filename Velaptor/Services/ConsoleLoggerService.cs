// <copyright file="ConsoleLoggerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System.Diagnostics.CodeAnalysis;
    using Serilog;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal sealed class ConsoleLoggerService : IConsoleLoggerService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLoggerService"/> class.
        /// </summary>
        public ConsoleLoggerService() =>
            Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

        /// <inheritdoc/>
        public ILogger Logger { get; }
    }
}
