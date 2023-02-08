// <copyright file="DisposeSoundDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using FluentAssertions;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="DisposeSoundData"/> struct.
/// </summary>
public class DisposeSoundDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange & Act
        var sut = new DisposeSoundData
        {
            SoundId = 123,
        };

        // Assert
        sut.SoundId.Should().Be(123);
    }
    #endregion
}
