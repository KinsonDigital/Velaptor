// <copyright file="Platform.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1724 // The type name conflicts in whole or in part with the namespace name
namespace Raptor
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
        private static OSPlatform[]? platforms;

        /// <summary>
        /// Initializes a new instance of the <see cref="Platform"/> class.
        /// </summary>
        public Platform()
        {
            if (!(platforms is null))
            {
                return;
            }

            platforms = new OSPlatform[]
            {
                OSPlatform.Windows,
                OSPlatform.OSX,
                OSPlatform.Linux,
                OSPlatform.FreeBSD,
            };
        }

        /// <inheritdoc/>
        public OSPlatform CurrentPlatform
        {
            get
            {
                if (platforms is null)
                {
                    throw new InvalidOperationException($"The '{nameof(IPlatform)}' implementation has not created all possible platforms.");
                }

                foreach (var platform in platforms)
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
