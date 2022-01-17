// <copyright file="IEmbeddedResourceLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    /// <summary>
    /// Loads embedded file resources.
    /// </summary>
    /// <typeparam name="TResourceType">The type of data being returned from the contents of the embedded resource.</typeparam>
    internal interface IEmbeddedResourceLoaderService<out TResourceType>
    {
        /// <summary>
        /// Load an embedded resources that matches the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The file name of the embedded resource.</param>
        /// <returns>The content from the embedded resource.</returns>
        TResourceType LoadResource(string name);
    }
}
