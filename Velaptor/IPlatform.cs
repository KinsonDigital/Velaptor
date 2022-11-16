// <copyright file="IPlatform.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.InteropServices;

namespace Velaptor;

/// <summary>
/// Represents the current platform.
/// </summary>
public interface IPlatform
{
    /// <summary>
    /// Gets the current platform of the system.
    /// </summary>
    OSPlatform CurrentPlatform { get; }

    /// <summary>
    /// Gets a value indicating whether or not the system is a 64 bit process.
    /// </summary>
    bool Is64BitProcess { get; }

    /// <summary>
    /// Gets a value indicating whether or not the system is a 32 bit process.
    /// </summary>
    public bool Is32BitProcess { get; }
}
