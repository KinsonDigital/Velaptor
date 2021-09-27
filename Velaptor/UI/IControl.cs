// <copyright file="IControl.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Drawing;

namespace Velaptor.UI
{
    using System.Numerics;
    using Velaptor.Content;

    /// <summary>
    /// A user interface object that can be updated and rendered to the screen.
    /// </summary>
    public interface IControl : IUpdatable, IDrawable, IInitialize, IContentLoadable, IDisposable
    {
        /// <summary>
        /// Gets or sets the name of the control.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the position of the <see cref="IControl"/> on the screen.
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Gets or sets the position of the left side of the control.
        /// </summary>
        int Left { get; set; }

        /// <summary>
        /// Gets or sets the position of the right side of the control.
        /// </summary>
        int Right { get; set; }

        /// <summary>
        /// Gets or sets the position of the top of the control.
        /// </summary>
        int Top { get; set; }

        /// <summary>
        /// Gets or sets the position of the bottom of the control.
        /// </summary>
        int Bottom { get; set; }

        /// <summary>
        /// Gets or sets the width of the control.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the control.
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the control is visible.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the control is enabled.
        /// </summary>
        bool Enabled { get; set; }
    }
}
