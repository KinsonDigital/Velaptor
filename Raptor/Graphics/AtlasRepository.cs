// <copyright file="AtlasRepository.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manages all of the atlases loaded into the game.
    /// </summary>
    internal sealed class AtlasRepository : IDisposable
    {
        private static readonly object LockObject = new object();
        private static bool isDisposed;
        private static AtlasRepository? instance;
        private readonly Dictionary<string, IAtlasData> allAtlasData = new Dictionary<string, IAtlasData>(); // The atlas data

        /// <summary>
        /// Initializes static members of the <see cref="AtlasRepository"/> class.
        /// </summary>
        static AtlasRepository()
        {
            // DO NOT DELETE THIS CONSTRUCTOR!!
            // This explicit static constructor tells the c# compiler not to mark type as 'beforefieldinit'

            // This singleton pattern came from : https://csharpindepth.com/articles/singleton
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasRepository"/> class.
        /// </summary>
        private AtlasRepository()
        {
            // DO NOT DELETE THIS CONSTRUCTOR!!
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="AtlasRepository"/>.
        /// </summary>
        public static AtlasRepository Instance
        {
            get
            {
                lock (LockObject)
                {
                    if (instance is null)
                    {
                        instance = new AtlasRepository();
                    }

                    return instance;
                }
            }
        }

        /// <summary>
        /// Gets the total number of atlas data items in the repository.
        /// </summary>
        public int TotalItems => this.allAtlasData.Count;

        /// <summary>
        /// Adds the given atlas data to the atlas manager and assigns it the given unique atlas ID.
        /// </summary>
        /// <param name="atlasId">The unique atlas ID to assign to the given texture.</param>
        /// <param name="data">The atlas data to add.</param>
        public void AddAtlasData(string atlasId, IAtlasData data)
        {
            lock (LockObject)
            {
                if (this.allAtlasData.ContainsKey(atlasId))
                {
                    throw new Exception($"Texture atlas data with the ID of '{atlasId}' already exists.");
                }

                this.allAtlasData.Add(atlasId, data);
            }
        }

        /// <summary>
        /// Returns a value indicating whether an atlas that matches
        /// the given <paramref name="atlasId"/> has been loaded into the repository.
        /// </summary>
        /// <param name="atlasId">The name/ID of the atlas.</param>
        /// <returns>True if the atlas is loaded.</returns>
        public bool IsAtlasLoaded(string atlasId)
        {
            lock (LockObject)
            {
                return this.allAtlasData.ContainsKey(atlasId);
            }
        }

        /// <summary>
        /// Empties all of the atlas data from the repository.
        /// </summary>
        public void EmptyRepository()
        {
            lock (LockObject)
            {
                foreach (var data in this.allAtlasData.Values)
                {
                    data.Texture.Dispose();
                }

                this.allAtlasData.Clear();
            }
        }

        /// <summary>
        /// Removes the atlas data and texture that matches the given ID.
        /// </summary>
        /// <param name="atlasId">The atlas ID of the atlas data and texture to remove.</param>
        public void RemoveAtlasData(string atlasId)
        {
            lock (LockObject)
            {
                if (!this.allAtlasData.ContainsKey(atlasId))
                {
                    throw new Exception($"Texture atlas data with the ID of '{atlasId}' does not exist.");
                }

                this.allAtlasData.Remove(atlasId);
            }
        }

        /// <summary>
        /// Gets the atlas data that matches the given ID.
        /// </summary>
        /// <param name="atlasId">The atlas ID of the atlas data to get.</param>
        /// <returns>The atlas data.</returns>
        public IAtlasData GetAtlasData(string atlasId)
        {
            lock (LockObject)
            {
                if (!this.allAtlasData.ContainsKey(atlasId))
                {
                    throw new Exception($"Texture atlas data with the ID of '{atlasId}' does not exist.");
                }

                return this.allAtlasData[atlasId];
            }
        }

        /// <summary>
        /// Gets the atlas texture that matches the given ID.
        /// </summary>
        /// <param name="atlasId">The id of the atlas texture to get.</param>
        /// <returns>The atlas texture.</returns>
        public ITexture GetAtlasTexture(string atlasId)
        {
            if (!this.allAtlasData.ContainsKey(atlasId))
            {
                throw new Exception($"Texture atlas data with the ID of '{atlasId}' does not exist.");
            }

            return this.allAtlasData[atlasId].Texture;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var data in this.allAtlasData.Values)
                {
                    data.Dispose();
                }

                this.allAtlasData.Clear();
            }

            isDisposed = true;
        }
    }
}
