// <copyright file="IContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;

    /// <summary>
    /// Loads various kinds of content.
    /// </summary>
    public interface IContentLoader : IDisposable
    {
        /// <summary>
        /// Loads content with the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of content to load.</typeparam>
        /// <param name="name">The name of the content to load.</param>
        /// <returns>A texture to render.</returns>
        T Load<T>(string name)
            where T : class, IContent;

        /// <summary>
        /// Unloads content with the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of content to unload.</typeparam>
        /// <param name="name">The name of the content to unload.</param>
        void Unload<T>(string name)
            where T : class, IContent;
    }
}
