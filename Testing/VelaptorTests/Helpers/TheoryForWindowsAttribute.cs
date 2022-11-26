// <copyright file="TheoryForWindowsAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System.Runtime.InteropServices;
using Xunit;

/// <summary>
/// <inheritdoc cref="TheoryAttribute"/>.
/// <para>
///     Test is only executed for the <c>Windows</c> platform.
/// </para>
/// </summary>
public sealed class TheoryForWindowsAttribute : TheoryAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TheoryForWindowsAttribute"/> class.
    /// </summary>
    public TheoryForWindowsAttribute()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        Skip = $"Only executed on {OSPlatform.Windows}.";
    }
}
