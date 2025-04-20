// <copyright file="KeyEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using Shouldly;
using Velaptor.Input;
using Xunit;

/// <summary>
/// Tests the <see cref="KeyEventArgs"/> struct.
/// </summary>
public class KeyEventArgsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropertiesToCorrectValues()
    {
        // Arrange
        var expectedKey = KeyCode.A;

        // Act
        var sut = new KeyEventArgs(expectedKey);

        // Assert
        sut.Key.ShouldBe(expectedKey);
    }
    #endregion
}
