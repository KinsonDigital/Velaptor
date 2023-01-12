// <copyright file="WindowSizeDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using FluentAssertions;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="WindowSizeData"/> class;
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
        sut.Width.Should().Be(11);
        sut.Height.Should().Be(22);
    }
    #endregion
}
