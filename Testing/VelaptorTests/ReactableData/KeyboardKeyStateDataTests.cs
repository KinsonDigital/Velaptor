// <copyright file="KeyboardKeyStateDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using Shouldly;
using Velaptor.Input;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="KeyboardKeyStateData"/> struct.
/// </summary>
public class KeyboardKeyStateDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange &  Act
        var sut = new KeyboardKeyStateData
        {
            Key = KeyCode.V,
            IsDown = true,
        };

        // Assert
        sut.Key.ShouldBe(KeyCode.V);
        sut.IsDown.ShouldBeTrue();
    }
    #endregion
}
