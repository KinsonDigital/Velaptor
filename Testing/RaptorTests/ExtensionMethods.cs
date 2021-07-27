// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests
{
    using System.Diagnostics.CodeAnalysis;
    using OpenTK.Mathematics;

    /// <summary>
    /// Provides extensions to various things to help make better code.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns a value indicating whether this <see cref="Vector4"/> is empty.
        /// </summary>
        /// <param name="vector">The vector to check.</param>
        /// <returns><see langword="true"/> if empty.</returns>
        public static bool IsEmpty(this Vector4 vector) =>
            vector.X == 0 &&
            vector.Y == 0 &&
            vector.Z == 0 &&
            vector.W == 0;
    }
}
