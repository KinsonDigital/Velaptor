// <copyright file="IWindowFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    using SILKWindow = Silk.NET.Windowing.IWindow;
    using VelaptorWindow = Velaptor.UI.IWindow;

    /// <summary>
    /// Creates different native windows.
    /// </summary>
    internal interface IWindowFactory
    {
        /// <summary>
        /// Creates a <see cref="Silk"/> specific window.
        /// </summary>
        /// <returns>The window instance.</returns>
        public SILKWindow CreateSilkWindow();
    }
}
