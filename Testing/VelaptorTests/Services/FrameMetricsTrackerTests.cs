// <copyright file="FrameMetricsTrackerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using Shouldly;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="FrameMetricsTracker"/> class.
/// </summary>
public class FrameMetricsTrackerTests
{
    #region Ctor Tests
    [Fact]
    public void Ctrl_WithHistoryCapacityTooSmall_ThrowsException()
    {
        // Arrange && Act
        var act = () => _ = new FrameMetricsTracker(5);

        // Assert
        act.ShouldThrow<ArgumentOutOfRangeException>()
            .Message.ShouldBe("Capacity must be at least 10 frames. (Parameter 'historyCapacity')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void DisplayUpdateIntervalSeconds_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new FrameMetricsTracker(123);

        // Act
        var actual = sut.DisplayUpdateIntervalSeconds;

        // Assert
        actual.ShouldBe(0.5);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void RecordFrame_WhenRecordingFrames_CalculatesFrameMetrics()
    {
        // Arrange
        var expectedMetrics = new FrameMetrics
        {
            AverageFps = 8,
            AverageFrameTimeMs = 125,
            MaxFrameTimeMs = 200,
            MinFrameTimeMs = 100,
            OnePercentLowFps = 5,
            ZeroPointOnePercentLowFps = 5,
        };

        var sut = new FrameMetricsTracker(10);

        // Act
        // Make sure we record enough frames to fill the buffer
        sut.RecordFrame(0.1);
        sut.RecordFrame(0.2);
        sut.RecordFrame(0.1);
        sut.RecordFrame(0.2);
        sut.RecordFrame(0.1);
        sut.RecordFrame(0.1);
        sut.RecordFrame(0.2);
        sut.RecordFrame(0.1);
        sut.RecordFrame(0.2);
        sut.RecordFrame(0.1);

        // Assert
        sut.CurrentMetrics.ShouldBe(expectedMetrics);
    }

    [Fact]
    public void RecordFrame_WhenDeltaTimeIsTooSmall_UsesMinimumDeltaTime()
    {
        // Arrange
        var expectedMetrics = new FrameMetrics
        {
            AverageFps = 0,
            AverageFrameTimeMs = 0,
            MaxFrameTimeMs = 0,
            MinFrameTimeMs = 0,
            OnePercentLowFps = 0,
            ZeroPointOnePercentLowFps = 0,
        };

        var sut = new FrameMetricsTracker(10);

        // Act
        // Make sure we record enough frames to fill the buffer
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);
        sut.RecordFrame(0.0000001);

        // Assert
        sut.CurrentMetrics.ShouldBe(expectedMetrics);
    }
    #endregion
}
