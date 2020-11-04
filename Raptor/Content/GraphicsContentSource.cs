// <copyright file="GraphicsContentSource.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO.Abstractions;

    /// <summary>
    /// Manages content source for graphical content.
    /// </summary>
    public class GraphicsContentSource : ContentSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsContentSource"/> class.
        /// </summary>
        /// <param name="directory">Manages directories.</param>
        public GraphicsContentSource(IDirectory directory)
            : base(directory) => ContentDirectoryName = "Graphics";
    }
}
