// <copyright file="IWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Desktop
{
    using System;

    /// <summary>
    /// Provides the core of an application window which facilitates how the
    /// window behaves, its state and the ability to be used in various types
    /// of applications.
    /// </summary>
    public interface IWindow : IWindowActions, IWindowProps, IDisposable
    {
        /// <summary>
        /// Shows the window.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}
