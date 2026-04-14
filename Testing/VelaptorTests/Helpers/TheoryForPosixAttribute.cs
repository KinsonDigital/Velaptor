// <copyright file="TheoryForPosixAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System.Runtime.InteropServices;
using Xunit;

/// <summary>
/// <inheritdoc cref="TheoryAttribute"/>.
/// <para>
///     Test is only executed for Non-<c>Windows</c> platform.
/// </para>
/// </summary>
public sealed class TheoryForPosixAttribute : TheoryAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TheoryForPosixAttribute"/> class.
    /// </summary>
    public TheoryForPosixAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        Skip = "Only executed on Posix platforms.";
    }
}
