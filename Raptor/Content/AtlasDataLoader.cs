// <copyright file="AtlasDataLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.Text.Json;
    using FileIO.Core;

    /// <summary>
    /// Loads atlas data.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    public class AtlasDataLoader<T> : ILoader<T[]>
    {
        private readonly ITextFile textFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasDataLoader{T}"/> class.
        /// </summary>
        /// <param name="textFile">Loads text files.</param>
        public AtlasDataLoader(ITextFile textFile) => this.textFile = textFile;

        /// <inheritdoc/>
        public T[] Load(string filePath)
        {
            var rawData = this.textFile.Load(filePath);

            return JsonSerializer.Deserialize<T[]>(rawData);
        }
    }
}
