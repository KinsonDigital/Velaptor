// <copyright file="IContent.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    /// <summary>
    /// Represents loadable content data.
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// Gets the name of the content.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the path to the content.
        /// </summary>
        string Path { get; }
    }
}
