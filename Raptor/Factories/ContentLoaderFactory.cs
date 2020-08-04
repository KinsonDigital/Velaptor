// <copyright file="ContentLoaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Content;

    /// <summary>
    /// Creates instances of a content loader.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ContentLoaderFactory
    {
        /// <summary>
        /// Creates a single instance of a content loader.
        /// </summary>
        /// <returns>A framework content loader implementation.</returns>
        public static IContentLoader CreateContentLoader() => IoC.Container.GetInstance<IContentLoader>();
    }
}
