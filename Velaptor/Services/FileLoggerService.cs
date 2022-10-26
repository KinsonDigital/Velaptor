// <copyright file="FileLoggerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Serilog;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal sealed class FileLoggerService : IFileLoggerService
    {
        private const string LogsDirName = "logs";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoggerService"/> class.
        /// </summary>
        public FileLoggerService() =>
            Logger = new LoggerConfiguration()
                .WriteTo.File($"{Directory.GetCurrentDirectory().ToCrossPlatPath()}/{LogsDirName}/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

        /// <inheritdoc/>
        public ILogger Logger { get; }
    }
}
