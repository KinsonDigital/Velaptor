// <copyright file="DisposeTextureDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using FluentAssertions;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="DisposeTextureData"/> struct.
/// </summary>
public class DisposeTextureDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange & Act
        var sut = new DisposeTextureData
        {
            TextureId = 123,
        };

        // Assert
        sut.TextureId.Should().Be(123);
    }
    #endregion
}
