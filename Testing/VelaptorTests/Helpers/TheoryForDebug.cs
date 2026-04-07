// <copyright file="TheoryForDebug.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using Xunit;

/// <summary>
/// Marks a test method as being a data theory for debug builds only.
/// Data theories are tests that are fed various bits of data from a data source, mapping to parameters on the test method.
/// If the data source contains multiple rows, then the test method is executed multiple times (once with each data row).
/// Data is provided by attributes which derive from DataAttribute (notably, InlineDataAttribute and MemberDataAttribute).
/// <para>
///     Test is only executed in <c>Debug</c> builds.
/// </para>
/// </summary>
public sealed class TheoryForDebug : TheoryAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TheoryForDebug"/> class.
    /// </summary>
    public TheoryForDebug()
    {
#if RELEASE
        Skip = "Only executed in debug builds.";
#endif
    }
}
