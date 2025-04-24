// <copyright file="ViewPortSizeDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using Shouldly;
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
        sut.Width.ShouldBe(1u);
        sut.Height.ShouldBe(2u);
    }
    #endregion
}
