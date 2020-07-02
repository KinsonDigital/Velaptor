// <copyright file="IWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System;

    /// <summary>
    /// Provides the core of a game which facilitates how the engine starts, stops,
    /// manages time and how the game loop runs.
    /// </summary>
    public interface IWindow : IDisposable
    {
        /// <summary>
        /// Gets or sets the width of the game window.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the game window.
        /// </summary>
        int Height { get; set; }

        string Title { get; set; }

        Action? Init { get; set; }

        Action<FrameTime>? Update { get; set; }

        Action<FrameTime>? Draw { get; set; }

        Action? WinResize { get; set; }

        int UpdateFreq { get; set; }

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
