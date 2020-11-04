// <copyright file="SystemMonitor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Hardware
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Holds information about a single monitor in the system.
    /// </summary>
    public class SystemMonitor : IEquatable<SystemMonitor>
    {
        private readonly IPlatform platform;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemMonitor"/> class.
        /// </summary>
        /// <param name="platform">The current platform.</param>
        public SystemMonitor(IPlatform platform) => this.platform = platform;

        /// <summary>
        /// Gets or sets a value indicating whether the monitor is the main monitor in the system.
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// Gets or sets the width in screen coordinates in the current video mode.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height in screen coordinates in the current video mode.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the bit depth of the red color channel in the current video mode.
        /// </summary>
        public int RedBitDepth { get; set; }

        /// <summary>
        /// Gets or sets the bit depth of the green color channel in the current video mode.
        /// </summary>
        public int GreenBitDepth { get; set; }

        /// <summary>
        /// Gets or sets the bit depth of the blue color channel in the current video mode.
        /// </summary>
        public int BlueBitDepth { get; set; }

        /// <summary>
        /// Gets or sets The refresh rate in Hz in the current video mode.
        /// </summary>
        public int RefreshRate { get; set; }

        /// <summary>
        /// Gets or sets the scale of the monitor on the horizontal axis.
        /// </summary>
        public float HorizontalScale { get; set; }

        /// <summary>
        /// Gets or sets the scale of the monitor on the vertical axis.
        /// </summary>
        public float VerticalScale { get; set; }

        /// <summary>
        /// Gets the approximate dpi of the monitor on the horizontal axis.
        /// </summary>
        public float HorizontalDPI => GetPlatformDefaultDpi() * HorizontalScale;

        /// <summary>
        /// Gets the approximate dpi of the monitor on the vertical axis.
        /// </summary>
        public float VerticalDPI => GetPlatformDefaultDpi() * VerticalScale;

        /// <summary>
        /// Returns a value indicating if the left operand is equal to the right operand.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if both operands are equal.</returns>
        public static bool operator ==(SystemMonitor left, SystemMonitor right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating if the left operand is not equal to the right operand.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if both operands are not equal.</returns>
        public static bool operator !=(SystemMonitor left, SystemMonitor right) => !(left == right);

        /// <inheritdoc/>
        public bool Equals(SystemMonitor other)
            => IsMain == other.IsMain &&
            RedBitDepth == other.RedBitDepth &&
            GreenBitDepth == other.GreenBitDepth &&
            BlueBitDepth == other.BlueBitDepth &&
            Width == other.Width &&
            Height == other.Height &&
            RefreshRate == other.RefreshRate &&
            HorizontalScale == other.HorizontalScale &&
            VerticalScale == other.VerticalScale;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is SystemMonitor monitor))
                return false;

            return Equals(monitor);
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
            => HashCode.Combine(IsMain, RedBitDepth, GreenBitDepth, BlueBitDepth, Width) +
            HashCode.Combine(Height, RefreshRate, HorizontalScale, VerticalScale);

        /// <summary>
        /// Gets the default DPI value of the current platform.
        /// </summary>
        /// <returns>The current platform's DPI setting.</returns>
        [ExcludeFromCodeCoverage]
        private float GetPlatformDefaultDpi() => this.platform.CurrentPlatform == OSPlatform.OSX ? 72f : 96f;
    }
}
