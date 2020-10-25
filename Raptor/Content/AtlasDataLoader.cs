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
        private readonly IFile file;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasDataLoader{T}"/> class.
        /// </summary>
        /// <param name="file">Used to load the texture atlas.</param>
        public AtlasDataLoader(IFile file) => this.file = file;

        /// <inheritdoc/>
        public T[] Load(string filePath)
        {
            var rawData = this.file.ReadAllText(filePath);

            return JsonSerializer.Deserialize<T[]>(rawData);
        }
    }
}
