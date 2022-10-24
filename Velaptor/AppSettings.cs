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
#pragma warning restore CA1822
    }
}
