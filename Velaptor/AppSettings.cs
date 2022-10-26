// <copyright file="AppSettings.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable MemberCanBeMadeStatic.Global
namespace Velaptor
{
    /// <summary>
    /// Application settings.
    /// </summary>
    internal sealed record AppSettings
    {
#pragma warning disable CA1822
        /// <summary>
        /// Gets the width of the application window.
        /// </summary>
        public uint WindowWidth { get; init; } = 1500;

        /// <summary>
        /// Gets the height of the application window.
        /// </summary>
        public uint WindowHeight { get; init; } = 800;

        /// <summary>
        /// Gets a value indicating whether or not all logging is enabled.
        /// </summary>
#if DEBUG || DEBUG_CONSOLE
        public bool LoggingEnabled { get; init; } = true;
#else
        public bool LoggingEnabled { get; init; } = false;
#endif

        /// <summary>
        /// Gets a value indicating whether or not logging to the console is enabled.
        /// </summary>
        public bool ConsoleLoggingEnabled { get; init; } = true;

        /// <summary>
        /// Gets a value indicating whether or not logging to a file is enabled.
        /// </summary>
        public bool FileLoggingEnabled { get; init; } = true;
#pragma warning restore CA1822
    }
}
