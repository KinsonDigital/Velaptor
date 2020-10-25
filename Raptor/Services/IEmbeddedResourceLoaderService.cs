// <copyright file="IEmbeddedResourceLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Services
{
    /// <summary>
    /// Loads embedded text file resources.
    /// </summary>
    public interface IEmbeddedResourceLoaderService
    {
        /// <summary>
        /// Load a text resources that matches the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The file name of the embedded resource.</param>
        /// <returns>The text content from the embedded text resource.</returns>
        string LoadResource(string name);
    }
}
