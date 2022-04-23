// <copyright file="WindowFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    using Silk.NET.Windowing;

    /// <inheritdoc/>
    internal class WindowFactory : IWindowFactory
    {
        /// <inheritdoc/>
        public IWindow CreateSilkWindow()
        {
            var windowOptions = WindowOptions.Default;
            windowOptions.ShouldSwapAutomatically = false;

            return Window.Create(windowOptions);
        }
    }
}
