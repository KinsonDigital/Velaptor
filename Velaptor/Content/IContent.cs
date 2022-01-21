// <copyright file="IContent.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;

    /// <summary>
    /// Represents loadable content data.
    /// </summary>
    public interface IContent : IDisposable
    {
        /// <summary>
        /// Gets the name of the content.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the path to the content.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Gets a value indicating whether or not the content item has been disposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}
