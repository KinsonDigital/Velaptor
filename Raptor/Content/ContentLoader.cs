// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.Json;
    using FileIO.Core;
    using FileIO.File;
    using Raptor.Graphics;
    using SixLabors.ImageSharp;

    /// <summary>
    /// Loads content.
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        private readonly string baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly IImageFile imageFile;
        private readonly ITextFile textFile;
        private string? graphicsDir;
        private string? contentDir;

        public ContentLoader()
        {
            this.imageFile = new ImageFile();
            this.textFile = new TextFile();
            SetupPaths();
        }

        /// <summary>
        /// Creates a new instace of <see cref="ContentLoader"/>.
        /// </summary>
        public ContentLoader(IImageFile imageFile, ITextFile textFile)
        {
            this.imageFile = imageFile;
            this.textFile = textFile;
            SetupPaths();
        }

        /// <summary>
        /// Gets or sets the root directory for the content.
        /// </summary>
        public string ContentRootDirectory
        {
            get => this.contentDir is null ? string.Empty : this.contentDir;
            set => this.contentDir = $@"{value}Content\";
        }

        /// <summary>
        /// Loads a texture that has the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the texture to load.</param>
        /// <returns></returns>
        public Texture? LoadTexture(string name)
        {
            var textureImagePath = $"{this.graphicsDir}{name}";

            var (pixels, width, height) = this.imageFile.Load(textureImagePath);

            return new Texture(pixels, width, height, name);
        }

        public Dictionary<string, AtlasSubRect> LoadAtlasData(string fileName)
        {
            var result = new Dictionary<string, AtlasSubRect>();

            var contentDir = $@"{this.baseDir}Content\";
            var graphicsContent = $@"{contentDir}Graphics\";

            var rawData = this.textFile.Load($"{graphicsContent}{fileName}");

            var rectItems = JsonSerializer.Deserialize<AtlasSubRect[]>(rawData);

            foreach (var item in rectItems)
            {
                result.Add(item.Name, item);
            }

            return result;
        }

        /// <summary>
        /// Sets up the pathing variables for the content location on disk.
        /// </summary>
        private void SetupPaths()
        {
            this.contentDir = $@"{this.baseDir}Content\";
            this.graphicsDir = @$"{this.contentDir}Graphics\";
        }
    }
}
