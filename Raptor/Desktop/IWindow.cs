// <copyright file="IWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Desktop
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
        /// <param name="dispose">The action to use to dispose of resources once the window has been shut down.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task ShowAsync(Action dispose);

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}
