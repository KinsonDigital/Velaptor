// <copyright file="AtlasDataLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO.Abstractions;
    using System.Text.Json;

    /// <summary>
    /// Loads atlas data.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    public class AtlasDataLoader<T> : ILoader<T[]>
    {
        private readonly IContentSource contentSource;
        private readonly IFile file;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasDataLoader{T}"/> class.
        /// </summary>
        /// <param name="contentSource">The atlas content source.</param>
        /// <param name="file">Used to load the texture atlas.</param>
        public AtlasDataLoader(IContentSource contentSource, IFile file)
        {
            this.contentSource = contentSource;
            this.file = file;
        }

        /// <inheritdoc/>
        public T[] Load(string name)
        {
            var contentPath = this.contentSource.GetContentPath(name);

            var rawData = this.file.ReadAllText(contentPath);

            return JsonSerializer.Deserialize<T[]>(rawData);
        }
    }
}
