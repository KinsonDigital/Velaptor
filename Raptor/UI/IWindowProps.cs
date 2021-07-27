// <copyright file="IWindowProps.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System.Numerics;
    using Velaptor.Content;

    /// <summary>
    /// Provides properties to hold the state of a window.
    /// </summary>
    public interface IWindowProps
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
        /// Gets or sets the width of the window.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the window.
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Gets or sets the value of how often the <see cref="Update"/>
        /// and <see cref="Draw"/> actions are invoked in the value of hertz.
        /// </summary>
        int UpdateFrequency { get; set; }

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
        ///     Set to the value of <see langword="false"/> if you want more control over when
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
        /// Gets a value indicating whether the window has been initialized.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Gets or sets the state of the window.
        /// </summary>
        StateOfWindow WindowState { get; set; }

        /// <summary>
        /// Gets or sets the type of border that the <see cref="IWindow"/> will have.
        /// </summary>
        WindowBorder TypeOfBorder { get; set; }

        /// <summary>
        /// Gets or sets the content loader for loading content.
        /// </summary>
        IContentLoader ContentLoader { get; set; }
    }
}
