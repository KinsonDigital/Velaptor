// <copyright file="IContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    /// <summary>
    /// Loads various kinds of content.
    /// </summary>
    public interface IContentLoader
    {
        /// <summary>
        /// Loads a texture with the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of content to load.</typeparam>
        /// <param name="name">The name of the texture to load.</param>
        /// <returns>A texture to render.</returns>
        T Load<T>(string name)
            where T : class, IContent;
    }
}
