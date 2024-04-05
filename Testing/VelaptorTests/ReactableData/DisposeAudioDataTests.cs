// <copyright file="DisposeAudioDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ReactableData;

using FluentAssertions;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="DisposeAudioData"/> struct.
/// </summary>
public class DisposeAudioDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange & Act
        var sut = new DisposeAudioData
        {
            AudioId = 123,
        };

        // Assert
        sut.AudioId.Should().Be(123);
    }
    #endregion
}
