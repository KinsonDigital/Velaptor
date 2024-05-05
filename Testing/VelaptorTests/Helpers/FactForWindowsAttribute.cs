// <copyright file="FactForWindowsAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System.Runtime.InteropServices;
using Xunit;

/// <summary>
/// <inheritdoc cref="FactAttribute"/>.
/// <para>
///     Test is only executed for the <c>Windows</c> platform.
/// </para>
/// </summary>
internal sealed class FactForWindowsAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactForWindowsAttribute"/> class.
    /// </summary>
    public FactForWindowsAttribute()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        Skip = $"Only executed on {OSPlatform.Windows}.";
    }
}
