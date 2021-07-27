// <copyright file="IWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System;
    using System.Threading.Tasks;

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
        /// Shows the window asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task ShowAsync();

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}
