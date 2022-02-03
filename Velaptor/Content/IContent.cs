// <copyright file="IContent.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents loadable content data.
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// Gets the name of the content.
        /// </summary>
        [SuppressMessage(
            "ReSharper",
            "UnusedMemberInSuper.Global",
            Justification = "Used by library users.")]
        string Name { get; }

        /// <summary>
        /// Gets the path to the content.
        /// </summary>
        string FilePath { get; }
    }
}
