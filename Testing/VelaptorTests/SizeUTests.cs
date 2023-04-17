// <copyright file="SizeUTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using FluentAssertions;
using Velaptor;
using Xunit;

/// <summary>
/// Tests the <see cref="SizeU"/> struct.
/// </summary>
public class SizeUTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange & Act
        var size = new SizeU(11u, 22u);

        // Assert
        size.Width.Should().Be(11u);
        size.Height.Should().Be(22u);
    }
    #endregion
}
