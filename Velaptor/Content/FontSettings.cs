// <copyright file="FontSettings.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The different font settings that can be used when loading fonts.
    /// </summary>
    public class FontSettings
    {
        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the style of the font.
        /// </summary>
        public FontStyle Style { get; set; }
    }
}
