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
        public uint WindowWidth => 1500;

        /// <summary>
        /// Gets the height of the application window.
        /// </summary>
        public uint WindowHeight => 800;

        /// <summary>
        /// Gets a value indicating whether or not all logging is enabled.
        /// </summary>
        public bool LoggingEnabled
        {
#if DEBUG
            get => true;
#else
            get => false;
#endif
        }

        /// <summary>
        /// Gets a value indicating whether or not logging to the console is enabled.
        /// </summary>
        public bool ConsoleLoggingEnabled => true;

        /// <summary>
        /// Gets a value indicating whether or not logging to a file is enabled.
        /// </summary>
        public bool FileLoggingEnabled => true;
#pragma warning restore CA1822
    }
}
