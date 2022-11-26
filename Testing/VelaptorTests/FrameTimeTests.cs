// <copyright file="FrameTimeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using System;
using Velaptor;
using Xunit;

public class FrameTimeTests
{
    #region  Prop Tests
    [Fact]
    public void TotalTime_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var time = default(FrameTime);
        var expected = new TimeSpan(0, 0, 0, 123);

        // Act
        time.TotalTime = new TimeSpan(0, 0, 0, 123);

        // Assert
        Assert.Equal(expected, time.TotalTime);
    }

    [Fact]
    public void ElapsedTime_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var time = default(FrameTime);
        var expected = new TimeSpan(0, 0, 0, 123);

        // Act
        time.ElapsedTime = new TimeSpan(0, 0, 0, 123);

        // Assert
        Assert.Equal(expected, time.ElapsedTime);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData(123, 456, true)]
    [InlineData(111, 456, false)]
    [InlineData(123, 444, false)]
    [InlineData(654, 987, false)]
    public void Equals_WithFrameTimeParam_ReturnsCorrectResult(int totalSeconds, int totalElapsedSeconds, bool expected)
    {
        // Arrange
        var timeA = default(FrameTime);
        timeA.TotalTime = new TimeSpan(0, 0, 0, 123);
        timeA.ElapsedTime = new TimeSpan(0, 0, 0, 456);

        var timeB = default(FrameTime);
        timeB.TotalTime = new TimeSpan(0, 0, 0, totalSeconds);
        timeB.ElapsedTime = new TimeSpan(0, 0, 0, totalElapsedSeconds);

        // Act
        var actual = timeA.Equals(timeB);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Equals_WithNullObjParam_ReturnsFalse()
    {
        // Arrange
        var timeA = default(FrameTime);
        timeA.TotalTime = new TimeSpan(0, 0, 0, 123);
        timeA.ElapsedTime = new TimeSpan(0, 0, 0, 456);

        object? timeB = null;

        // Act
        var actual = timeA.Equals(timeB);

        // Assert
        Assert.False(actual);
    }

    [Theory]
    [InlineData(123, 456, true)]
    [InlineData(111, 456, false)]
    [InlineData(123, 444, false)]
    [InlineData(654, 987, false)]
    public void Equals_WithObjectParam_ReturnsCorrectResult(int totalSeconds, int totalElapsedSeconds, bool expected)
    {
        // Arrange
        var timeA = default(FrameTime);
        timeA.TotalTime = new TimeSpan(0, 0, 0, 123);
        timeA.ElapsedTime = new TimeSpan(0, 0, 0, 456);

        object timeB = new FrameTime
        {
            TotalTime = new TimeSpan(0, 0, 0, totalSeconds),
            ElapsedTime = new TimeSpan(0, 0, 0, totalElapsedSeconds),
        };

        // Act
        var actual = timeA.Equals(timeB);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(123, 456, true)]
    [InlineData(111, 456, false)]
    [InlineData(123, 444, false)]
    [InlineData(654, 987, false)]
    public void EqualsOperator_WithFrameTimeParam_ReturnsCorrectResult(
        int totalSeconds,
        int totalElapsedSeconds,
        bool expected)
    {
        // Arrange
        var timeA = default(FrameTime);
        timeA.TotalTime = new TimeSpan(0, 0, 0, 123);
        timeA.ElapsedTime = new TimeSpan(0, 0, 0, 456);

        var timeB = default(FrameTime);
        timeB.TotalTime = new TimeSpan(0, 0, 0, totalSeconds);
        timeB.ElapsedTime = new TimeSpan(0, 0, 0, totalElapsedSeconds);

        // Act
        var actual = timeA == timeB;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(123, 456, false)]
    [InlineData(111, 456, true)]
    [InlineData(123, 444, true)]
    [InlineData(654, 987, true)]
    public void NotEqualsEquals_WithFrameTimeParam_ReturnsCorrectResult(
        int totalSeconds,
        int totalElapsedSeconds,
        bool expected)
    {
        // Arrange
        var timeA = default(FrameTime);
        timeA.TotalTime = new TimeSpan(0, 0, 0, 123);
        timeA.ElapsedTime = new TimeSpan(0, 0, 0, 456);

        var timeB = default(FrameTime);
        timeB.TotalTime = new TimeSpan(0, 0, 0, totalSeconds);
        timeB.ElapsedTime = new TimeSpan(0, 0, 0, totalElapsedSeconds);

        // Act
        var actual = timeA != timeB;

        // Assert
        Assert.Equal(expected, actual);
    }
    #endregion
}
