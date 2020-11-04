// <copyright file="IPlatform.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the current platform.
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// Gets the current platform of the system.
        /// </summary>
        OSPlatform CurrentPlatform { get; }
    }
}
