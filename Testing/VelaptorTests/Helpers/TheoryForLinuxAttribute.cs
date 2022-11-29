// <copyright file="TheoryForLinuxAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System.Runtime.InteropServices;
using Xunit;

/// <summary>
/// <inheritdoc cref="TheoryAttribute"/>.
/// <para>
///     Test is only executed for the <c>Linux</c> platform.
/// </para>
/// </summary>
public sealed class TheoryForLinuxAttribute : TheoryAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TheoryForLinuxAttribute"/> class.
    /// </summary>
    public TheoryForLinuxAttribute()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return;
        }

        Skip = $"Only executed on {OSPlatform.Linux}.";
    }
}
