// <copyright file="AtlasContentSource.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO.Abstractions;

    /// <summary>
    /// Manages content source for sound content.
    /// </summary>
    public class AtlasContentSource : ContentSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasContentSource"/> class.
        /// </summary>
        /// <param name="directory">Manages directories.</param>
        public AtlasContentSource(IDirectory directory)
            : base(directory) => ContentDirectoryName = "Atlases";
    }
}
