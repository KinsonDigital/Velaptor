// <copyright file="WindowSizeDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using Shouldly;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="WindowSizeData"/> class.
/// </summary>
public class WindowSizeDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange & Act
        var sut = new WindowSizeData { Width = 11, Height = 22 };

        // Assert
        sut.Width.ShouldBe(11u);
        sut.Height.ShouldBe(22u);
    }
    #endregion
}
