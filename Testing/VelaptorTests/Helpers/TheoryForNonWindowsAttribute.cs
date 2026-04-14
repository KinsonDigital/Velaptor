// <copyright file="TheoryForNonWindowsAttribute.cs" company="KinsonDigital">
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
public sealed class TheoryForNonWindowsAttribute : TheoryAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TheoryForNonWindowsAttribute"/> class.
    /// </summary>
    public TheoryForNonWindowsAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        Skip = $"Only executed on Non-{OSPlatform.Windows}.";
    }
}
