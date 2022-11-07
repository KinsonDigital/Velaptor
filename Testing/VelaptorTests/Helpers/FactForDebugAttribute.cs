// <copyright file="FactForDebugAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Xunit;

namespace VelaptorTests.Helpers;

/// <summary>
/// <inheritdoc cref="FactAttribute"/>.
/// <para>
///     Only executed if the build is a <c>Debug</c> build.
/// </para>
/// </summary>
public sealed class FactForDebugAttribute : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactForDebugAttribute"/> class.
    /// </summary>
    public FactForDebugAttribute()
    {
#if !DEBUG
            Skip = "Only executed with debug builds.";
#endif
    }
}
