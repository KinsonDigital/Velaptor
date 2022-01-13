// <copyright file="Platform.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1724 // The type name conflicts in whole or in part with the namespace name
namespace Velaptor
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the current platform.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Platform : IPlatform
    {
        private static readonly OSPlatform[] Platforms =
        {
            OSPlatform.Windows,
            OSPlatform.OSX,
            OSPlatform.Linux,
            OSPlatform.FreeBSD,
        };

        /// <inheritdoc/>
        public bool Is64BitProcess => Environment.Is64BitProcess;

        /// <inheritdoc/>
        public bool Is32BitProcess => !Is64BitProcess;

        /// <inheritdoc/>
        public OSPlatform CurrentPlatform
        {
            get
            {
                if (Platforms is null)
                {
                    throw new InvalidOperationException($"The '{nameof(IPlatform)}' implementation has not created all possible platforms.");
                }

                foreach (var platform in Platforms)
                {
                    if (RuntimeInformation.IsOSPlatform(platform))
                    {
                        return platform;
                    }
                }

                throw new NotSupportedException("The current platform is not supported.");
            }
        }
    }
}
