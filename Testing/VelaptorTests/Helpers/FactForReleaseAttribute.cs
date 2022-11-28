// <copyright file="FactForReleaseAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using Xunit;

/// <summary>
/// <inheritdoc cref="FactAttribute"/>.
/// <para>
///     Only executed if the build is a <c>Debug</c> build.
/// </para>
/// </summary>
public sealed class FactForReleaseAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactForReleaseAttribute"/> class.
    /// </summary>
    public FactForReleaseAttribute()
    {
#if !RELEASE
        Skip = "Only executed with release builds.";
#endif
    }
}
