// <copyright file="IControl.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.UI
{
    using System.Numerics;
    using Raptor.Content;

    /// <summary>
    /// A user interface object that can be updated and rendered to the screen.
    /// </summary>
    public interface IControl : IUpdatable, IInitialize, IContentLoadable
    {
        /// <summary>
        /// Gets or sets the position of the <see cref="IControl"/> on the screen.
        /// </summary>
        Vector2 Position { get; set; }

        /// <summary>
        /// Gets the width of the <see cref="IControl"/>.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the <see cref="IControl"/>.
        /// </summary>
        int Height { get; }
    }
}
