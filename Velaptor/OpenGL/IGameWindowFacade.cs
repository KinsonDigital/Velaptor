// <copyright file="IGameWindowFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using System.Numerics;

    /// <summary>
    /// The internal OpenGL window.
    /// </summary>
    internal interface IGameWindowFacade : IDisposable
    {
        /// <summary>
        /// Occurs before the window is displayed for the first time.
        /// </summary>
        event EventHandler<EventArgs>? Load;

        /// <summary>
        /// Occurs before the window is destroyed.
        /// </summary>
        event EventHandler<EventArgs>? Unload;

        /// <summary>
        /// Occurs when it is time to update a frame.
        /// </summary>
        event EventHandler<FrameTimeEventArgs>? UpdateFrame;

        /// <summary>
        /// Occurs when it is time to render a frame.
        /// </summary>
        event EventHandler<FrameTimeEventArgs>? RenderFrame;

        /// <summary>
        /// Occurs whenever the window is resized.
        /// </summary>
        event EventHandler<WindowSizeEventArgs>? Resize;

        /// <summary>
        /// Gets or sets a <see cref="Vector2"/> structure that contains the external size of this window.
        /// </summary>
        Vector2 Size { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Vector2"/> structure that contains the location of this window on the desktop.
        /// </summary>
        Vector2 Location { get; set; }

        /// <summary>
        /// Gets or sets a value representing the update frequency, in hertz.
        /// </summary>
        /// <remarks>
        ///     A value of 0.0 indicates that UpdateFrame events are generated at the maximum
        ///     possible frequency (i.e. only limited by the hardware's capabilities).
        ///     Values lower than 1.0Hz are clamped to 0.0. Values higher than 500.0Hz are clamped to 500.0Hz.
        /// </remarks>
        double UpdateFrequency { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse cursor is visible.
        /// </summary>
        bool CursorVisible { get; set; }

        /// <summary>
        /// Gets or sets state of the window.
        /// </summary>
        StateOfWindow WindowState { get; set; }

        /// <summary>
        /// Gets or sets the type of border for this window.
        /// </summary>
        WindowBorder WindowBorder { get; set; }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Runs initialization code before the <see cref="Init(uint, uint)"/> code is ran.
        /// </summary>
        void PreInit();

        /// <summary>
        /// Initializes the window.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        void Init(uint width, uint height);

        /// <summary>
        /// Shows the window.
        /// </summary>
        void Show();

        /// <summary>
        /// Swaps the front and back buffers of the current GraphicsContext, presenting the rendered scene to the user.
        /// </summary>
        void SwapBuffers();

        /// <summary>
        /// Closes the window.
        /// </summary>
        void Close();
    }
}
