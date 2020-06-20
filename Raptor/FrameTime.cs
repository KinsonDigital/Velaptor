// <copyright file="FrameTime.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Holds timing information for a game loop.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public struct FrameTime : IEquatable<FrameTime>
    {
        /// <summary>
        /// Gets or sets the total time that has passed.
        /// </summary>
        public TimeSpan TotalTime { get; set; }

        /// <summary>
        /// Gets or sets the total time that has passed for the current frame.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        public static bool operator ==(FrameTime left, FrameTime right) => left.Equals(right);

        public static bool operator !=(FrameTime left, FrameTime right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is FrameTime frameTime))
                return false;

            return Equals(frameTime);
        }

        /// <inheritdoc/>
        public bool Equals(FrameTime other)
            => other.TotalTime == TotalTime && other.ElapsedTime == ElapsedTime;

        /// <inheritdoc/>
        public override int GetHashCode() => ElapsedTime.GetHashCode();
    }
}
