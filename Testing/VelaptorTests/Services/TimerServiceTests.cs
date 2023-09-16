// <copyright file="TimerServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using FluentAssertions;
using NSubstitute;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="TimerService"/> class.
/// </summary>
public class TimerServiceTests
{
    private readonly IStopWatchWrapper mockStopWatchWrapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerServiceTests"/> class.
    /// </summary>
    public TimerServiceTests() => this.mockStopWatchWrapper = Substitute.For<IStopWatchWrapper>();

    #region Method Tests
    [Fact]
    public void Start_WhenInvoked_StartsTheTimer()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Start();

        // Assert
        this.mockStopWatchWrapper.Received().Start();
    }

    [Fact]
    public void Stop_WithTimerRunning_StopsTheTimer()
    {
        // Arrange
        this.mockStopWatchWrapper.IsRunning.Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Stop();

        // Assert
        this.mockStopWatchWrapper.Received().Stop();
    }

    [Fact]
    public void Stop_WithTimerNotRunning_DoesNotStopTimerOrRecordData()
    {
        // Arrange
        this.mockStopWatchWrapper.IsRunning.Returns(false);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Stop();

        // Assert
        this.mockStopWatchWrapper.DidNotReceive().Start();
        _ = this.mockStopWatchWrapper.DidNotReceive().Elapsed;
    }

    [Fact]
    public void Stop_WhenInvoked_AddsSampleToSamplesList()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.mockStopWatchWrapper.IsRunning.Returns(true);
        this.mockStopWatchWrapper.Elapsed.Returns(new TimeSpan(0, 0, 0, 0, 100));
        sut.Start();

        // Act
        sut.Stop();

        // Assert
        Assert.Equal(100, sut.MillisecondsPassed);
    }

    [Fact]
    public void Stop_WhenStartingAndStopping_CorrectlyRecordsAndReturnsAverageTime()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.mockStopWatchWrapper.IsRunning.Returns(true);

        // Act
        // Record sample 1
        this.mockStopWatchWrapper.Elapsed.Returns(new TimeSpan(0, 0, 0, 0, 100));
        sut.Stop();

        // Record sample 2
        this.mockStopWatchWrapper.Elapsed.Returns(new TimeSpan(0, 0, 0, 0, 1000));
        sut.Stop();

        // Assert
        sut.MillisecondsPassed.Should().Be(550);
    }

    [Fact]
    public void Stop_WhenRecordingMoreThanTotalSamples_StartsRecordingBackAtBeginning()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.mockStopWatchWrapper.IsRunning.Returns(true);
        this.mockStopWatchWrapper.Elapsed.Returns(new TimeSpan(0, 0, 0, 0, 1000));

        // Make sure that every single sample in the total 1000 sample range
        // is recorded.
        for (var i = 1; i <= 1000; i++)
        {
            sut.Stop();
        }

        // This should be the first sample spot
        this.mockStopWatchWrapper.Elapsed.Returns(new TimeSpan(0, 0, 0, 0, 10));

        // Act
        sut.Stop();

        // Assert
        sut.MillisecondsPassed.Should().Be(999.01f);
    }

    [Fact]
    public void Reset_WhenInvoked_ResetsTheTimer()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Reset();

        // Assert
        this.mockStopWatchWrapper.Received().Reset();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TimerService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TimerService CreateSystemUnderTest() => new (this.mockStopWatchWrapper);
}
