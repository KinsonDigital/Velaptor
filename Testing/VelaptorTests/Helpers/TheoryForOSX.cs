// <copyright file="TheoryForOSX.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using System.Runtime.InteropServices;
using Xunit;

/// <summary>
/// <inheritdoc cref="TheoryAttribute"/>.
/// <para>
///     Test is only executed for the <c>OSX</c> platform.
/// </para>
/// </summary>
public sealed class TheoryForOSX : TheoryAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TheoryForOSX"/> class.
    /// </summary>
    public TheoryForOSX()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return;
        }

        Skip = $"Only executed on {OSPlatform.OSX}.";
    }
}
