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
        /// Gets a value indicating whether or not the content for an object is loaded.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Loads the content for an object.
        /// </summary>
        void LoadContent();

        /// <summary>
        /// Unloads the content for an object.
        /// </summary>
        void UnloadContent();
    }
}
