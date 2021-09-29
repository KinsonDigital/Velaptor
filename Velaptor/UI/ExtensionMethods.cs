// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System.Drawing;
    using System.Numerics;

    /// <summary>
    /// Provides extension methods for various types in the <see cref="Velaptor.UI"/> namespace.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Converts a <see cref="Vector2"/> to a <see cref="Point"/>.
        /// </summary>
        /// <param name="vector">The vector to convert.</param>
        /// <returns>The <see cref="Point"/> representation of a <see cref="Vector2"/>.</returns>
        /// <remarks>
        ///     Converting from floating point components of a <see cref="Vector2"/> to
        ///     integer components of a <see cref="Point"/> could result in a loss of information.
        ///     Regular casting rules apply.
        /// </remarks>
        public static Point ToPoint(this Vector2 vector) => new Point((int)vector.X, (int)vector.Y);
    }
}
