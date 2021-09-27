// <copyright file="IContentLoadable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    /// <summary>
    /// Provides the ability to load content.
    /// </summary>
    public interface IContentLoadable
    {
        /// <summary>
        /// Gets or sets a value indicating if the content for an object is loaded.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Load the content for an object.
        /// </summary>
        void LoadContent();
    }
}
