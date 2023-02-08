// <copyright file="ViewPortSizeDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using FluentAssertions;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="ViewPortSizeData"/> struct.
/// </summary>
public class ViewPortSizeDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropValuesToCorrectResult()
    {
        // Arrange & Act
        var sut = new ViewPortSizeData
        {
            Width = 1,
            Height = 2,
        };

        // Assert
        sut.Width.Should().Be(1);
        sut.Height.Should().Be(2);
    }
    #endregion
}
