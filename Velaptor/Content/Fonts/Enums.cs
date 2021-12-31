// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
{
    using System;

    /// <summary>
    /// The kind of font styles.
    /// </summary>
    /// <summary>
    /// The font style can be a combination of bold and italic.
    /// </summary>
    [Flags]
    public enum FontStyle
    {
        /// <summary>
        /// Regular font style.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Italic font style.
        /// </summary>
        Italic = 1,

        /// <summary>
        /// Bold font style.
        /// </summary>
        Bold = 2,
    }
}
