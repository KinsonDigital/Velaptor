// <copyright file="FactForProduction.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using Xunit;

/// <summary>
/// Attribute that is applied to a method to indicate that it is a fact that should be run by the test runner for production builds only.
/// <para>
///     Test is only executed in <c>Production</c> builds.
/// </para>
/// </summary>
public sealed class FactForProduction : FactAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FactForProduction"/> class.
    /// </summary>
    public FactForProduction()
    {
#if DEBUG || DEBUG_CONSOLE
        Skip = "Only executed in production builds.";
#endif
    }
}

