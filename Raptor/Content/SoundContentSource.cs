// <copyright file="SoundContentSource.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.IO.Abstractions;

    /// <summary>
    /// Manages content source for sound content.
    /// </summary>
    public class SoundContentSource : ContentSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundContentSource"/> class.
        /// </summary>
        /// <param name="directory">Manages directories.</param>
        public SoundContentSource(IDirectory directory)
            : base(directory) => ContentDirectoryName = "Sounds";
    }
}
