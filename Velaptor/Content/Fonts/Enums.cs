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

    /// <summary>
    /// Represents the source of where a font was loaded from.
    /// </summary>
    /// <remarks>
    ///     When loading fonts, the font that is attempting to be loaded will be
    ///     checked for its existence in the application's content directory first.
    ///     If the font does not exist there, then the font will be checked for
    ///     its existence in the system.  If the font does not exist in the application's
    ///     content directory or the system, then an exception will be thrown.
    /// </remarks>
    public enum FontSource
    {
        /// <summary>
        /// Loaded from another location other then the system or applications font content directory.
        /// </summary>
        Unknown,

        /// <summary>
        /// Loaded from the application's content directory.
        /// </summary>
        AppContent,

        /// <summary>
        /// Loaded from the system.
        /// </summary>
        System,
    }
}
