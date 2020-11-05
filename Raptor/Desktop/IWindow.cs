// <copyright file="IWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Desktop
{
    using System;
    using System.Numerics;
    using Raptor.Graphics;

    /// <summary>
    /// Provides the core of a game which facilitates how the engine starts, stops,
    /// manages time and how the game loop runs.
    /// </summary>
    public interface IWindow : IDisposable
    {
        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the position of the window.
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the width of the game window.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the game window.
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the buffers should]
        /// be automatically cleared before rendering any textures.
        /// </summary>
        /// <remarks>
        /// <para>
        ///     If this is set to true, this means you do not have to
        ///     use or invoke the <see cref="ISpriteBatch.Clear"/>() method.
        /// </para>
        /// <para>
        ///     Set to the value of false if you want more control over when
        ///     the back buffers will be cleared.
        /// </para>
        /// <para>
        ///     WARNING!! - To prevent performance issues, do not have the clear
        ///     the buffers with the <see cref="ISpriteBatch.Clear"/>() method
        ///     and set this property to true.  That would be a waste of resources.
        /// </para>
        /// </remarks>
        bool AutoClearBuffer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse cursor is visible.
        /// </summary>
        bool MouseCursorVisible { get; set; }

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        StateOfWindow WindowState { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Action"/> delegate to be invoked one time to initialize the window.
        /// </summary>
        Action? Init { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Action"/> delegate that is invoked per frame for updating.
        /// </summary>
        Action<FrameTime>? Update { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Action"/> delegate that is invoked per frame for rendering.
        /// </summary>
        Action<FrameTime>? Draw { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Action"/> delegate that is invoked every time the window is resized.
        /// </summary>
        Action? WinResize { get; set; }

        /// <summary>
        /// Gets or sets the type of border that the <see cref="IWindow"/> will have.
        /// </summary>
        BorderType TypeOfBorder { get; set; }

        /// <summary>
        /// Gets or sets the value of how often the <see cref="Update"/>
        /// and <see cref="Draw"/> actions are invoked in the value of hertz.
        /// </summary>
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
