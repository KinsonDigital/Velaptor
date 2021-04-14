// <copyright file="IGameWindowFacade.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using OpenTK.Mathematics;
    using OpenTK.Windowing.Common;
    using OpenTK.Windowing.GraphicsLibraryFramework;

    /// <summary>
    /// The internal OpenGL window for the purpose of getting access to the window's pointer.
    /// </summary>
    internal interface IGameWindowFacade : IDisposable
    {
        /// <summary>
        /// Occurs before the window is displayed for the first time.
        /// </summary>
        event Action? Load;

        /// <summary>
        /// Occurs before the window is destroyed.
        /// </summary>
        event Action? Unload;

        /// <summary>
        /// Occurs when it is time to update a frame.
        /// </summary>
        event Action<FrameEventArgs>? UpdateFrame;

        /// <summary>
        /// Occurs when it is time to render a frame.
        /// </summary>
        event Action<FrameEventArgs>? RenderFrame;

        /// <summary>
        /// Occurs whenever the window is resized.
        /// </summary>
        event Action<ResizeEventArgs>? Resize;

        /// <summary>
        /// Occurs whenever a keyboard key is pressed.
        /// </summary>
        event Action<KeyboardKeyEventArgs>? KeyDown;

        /// <summary>
        /// Occurs whenever a keyboard key is released.
        /// </summary>
        event Action<KeyboardKeyEventArgs>? KeyUp;

        /// <summary>
        /// Occurs whenever a <see cref="MouseButton"/> is clicked.
        /// </summary>
        event Action<MouseButtonEventArgs>? MouseDown;

        /// <summary>
        /// Occurs whenever a <see cref="MouseButton"/> is released.
        /// </summary>
        event Action<MouseButtonEventArgs>? MouseUp;

        /// <summary>
        /// Occurs whenever the mouse cursor is moved;
        /// </summary>
        event Action<MouseMoveEventArgs>? MouseMove;

        /// <summary>
        /// Occurs after the window has closed.
        /// </summary>
        event Action? Closed;

        /// <summary>
        /// Gets or sets a <see cref="Vector2i"/> structure that contains the external size of this window.
        /// </summary>
        unsafe Vector2i Size { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Vector2i"/> structure that contains the location of this window on the desktop.
        /// </summary>
        unsafe Vector2i Location { get; set; }

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
        unsafe bool CursorVisible { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OpenTK.Windowing.Common.WindowState"/> for this window.
        /// </summary>
        unsafe WindowState WindowState { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OpenTK.Windowing.Common.WindowBorder"/> for this window.
        /// </summary>
        unsafe WindowBorder WindowBorder { get; set; }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        unsafe string Title { get; set; }

        /// <summary>
        /// Gets the pointer to the window.
        /// </summary>
        unsafe Window* WindowPtr { get; }

        /// <summary>
        /// Initializes the window.
        /// </summary>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        void Init(int width, int height);

        /// <summary>
        /// Initialize the update thread (if using a multi-threaded context, and enter the game loop of the GameWindow.
        /// </summary>
        void Run();

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
