// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Helpers;

using Velaptor.Exceptions;

/// <summary>
/// Used for the purpose of testing the <see cref="EnumOutOfRangeException{T}"/>.
/// </summary>
public enum TestEnum
{
    /// <summary>
    /// The first test value.
    /// </summary>
    TestValueA = 1,

    /// <summary>
    /// The second test value.
    /// </summary>
    TestValueB = 2,
}
