// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.VelcroPhysicsImp
{
    using System.Linq;
    using VelcroPhysics.Primitives;

    /// <summary>
    /// Provides extensions to various things to help make better code.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The physics unit to pixel conversion value.
        /// </summary>
        private const float UnitToPixel = 100.0f;

        /// <summary>
        /// The pixel to unit conversion value.
        /// </summary>
        private const float PixelToUnit = 1f / UnitToPixel;

        /// <summary>
        /// Converts the given physics unit <paramref name="value"/> to pixel units.
        /// </summary>
        /// <param name="value">The physics value to convert.</param>
        /// <returns></returns>
        public static float ToPixels(this float value) => value * UnitToPixel;

        /// <summary>
        /// Converts the given pixel units <paramref name="value"/> to physics units.
        /// </summary>
        /// <param name="value">The pixel units to convert.</param>
        /// <returns></returns>
        public static float ToPhysics(this float value) => value * PixelToUnit;

        /// <summary>
        /// Converts all of the given physics <paramref name="values"/> to pixel values.
        /// </summary>
        /// <param name="values">The list of physics values to convert.</param>
        /// <returns></returns>
        public static float[] ToPixels(this float[] values) => (from p in values select p.ToPixels()).ToArray();

        /// <summary>
        /// Converts the given pixel unit <see cref="Vector2"/> <paramref name="value"/> to a
        /// physics unit <see cref="Vector2"/> value.
        /// </summary>
        /// <param name="value">The pixel unit vector to convert.</param>
        /// <returns></returns>
        public static Vector2 ToPhysics(this Vector2 value) => new Vector2(value.X.ToPhysics(), value.Y.ToPhysics());
    }
}
