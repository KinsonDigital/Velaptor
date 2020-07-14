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
        private const float PhysicsToScreenConversionFactor = 100.0f;

        /// <summary>
        /// The pixel to unit conversion value.
        /// </summary>
        private const float ScreenToPhysics = 1f / PhysicsToScreenConversionFactor;

        /// <summary>
        /// Converts the given physics unit <paramref name="value"/> to pixel units.
        /// </summary>
        /// <param name="value">The physics value to convert.</param>
        /// <returns>A pixel component as a screen unit of instead of physics unit.</returns>
        public static float ToScreenUnits(this float value) => value * PhysicsToScreenConversionFactor;

        /// <summary>
        /// Converts the given pixel units <paramref name="value"/> to physics units.
        /// </summary>
        /// <param name="value">The pixel units to convert.</param>
        /// <returns>The pixel component in the physics unit instead of screen unit.</returns>
        public static float ToPhysicsUnit(this float value) => value * ScreenToPhysics;

        /// <summary>
        /// Converts all of the given physics <paramref name="values"/> to pixel values.
        /// </summary>
        /// <param name="values">The list of physics values to convert.</param>
        /// <returns>The value in a screen unit.</returns>
        public static float[] ToScreenUnit(this float[] values) => (from p in values select p.ToScreenUnits()).ToArray();

        /// <summary>
        /// Converts the given pixel unit <see cref="Vector2"/> <paramref name="value"/> to a
        /// physics unit <see cref="Vector2"/> value.
        /// </summary>
        /// <param name="value">The pixel unit vector to convert.</param>
        /// <returns>The vector in the physics unit.</returns>
        public static Vector2 ToPhysicsVector(this Vector2 value) => new Vector2(value.X.ToPhysicsUnit(), value.Y.ToPhysicsUnit());
    }
}
