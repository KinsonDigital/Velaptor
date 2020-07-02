// <copyright file="WindowFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using OpenToolkit.Mathematics;
    using OpenToolkit.Windowing.Desktop;
    using Raptor.OpenGL;

    /// <summary>
    /// Creates an instance of a raptor window.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class WindowFactory
    {
        /// <summary>
        /// Creats a single instance of a raptor window implementation.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        /// <returns>A raptor framework window implementation.</returns>
        public static IWindow CreateWindow(int width, int height)
        {
            var gameWindowSettings = new GameWindowSettings();
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
            };

            return new GLWindow(gameWindowSettings, nativeWindowSettings);
        }
    }
}
