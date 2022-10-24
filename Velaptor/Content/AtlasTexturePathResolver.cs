// <copyright file="AtlasTexturePathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System.IO.Abstractions;

    /// <summary>
    /// Resolves paths to atlas texture content.
    /// </summary>
    internal class AtlasTexturePathResolver : TexturePathResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasTexturePathResolver"/> class.
        /// </summary>
        /// <param name="directory">Performs operations with directories.</param>
        public AtlasTexturePathResolver(IDirectory directory)
            : base(directory) => ContentDirectoryName = "Atlas";
    }
}
