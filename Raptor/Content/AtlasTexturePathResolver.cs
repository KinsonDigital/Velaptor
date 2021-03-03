// <copyright file="AtlasTexturePathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;

    /// <summary>
    /// Resolves paths to atlas texture content.
    /// </summary>
    public class AtlasTexturePathResolver : TexturePathResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasTexturePathResolver"/> class.
        /// </summary>
        /// <param name="directory">Manages directories.</param>
        public AtlasTexturePathResolver(IDirectory directory)
            : base(directory) => FileDirectoryName = "Atlas";
    }
}
