// <copyright file="TimerServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using FluentAssertions;
using NSubstitute;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="TimerService"/> class.
/// </summary>
public class TimerServiceTests
{
    private const long Frequency = 10_000_000;
    private readonly IStopWatchWrapper mockStopWatchWrapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimerServiceTests"/> class.
    /// </summary>
    public TimerServiceTests()
    {
        this.mockStopWatchWrapper = Substitute.For<IStopWatchWrapper>();
        this.mockStopWatchWrapper.Frequency.Returns(Frequency);
    }

    #region Method Tests
    [Fact]
    public void StartAndStop_WhenSamplesAreNotFull_CalculatesMillisecondsPassed()
    {
        // Arrange
        var isStarted = false;
        this.mockStopWatchWrapper.GetTimestamp().Returns(_ =>
        {
            var result = isStarted ? 320_000L : 160_000L;
            isStarted = true;

            return result;
        });
        var sut = CreateSystemUnderTest();

        // Act
        sut.Start();
        sut.Stop();

        // Assert
        this.mockStopWatchWrapper.Received(2).GetTimestamp();
        sut.MillisecondsPassed.Should().Be(16);
    }

    [Fact]
    public void StartAndStop_WhenSamplesAreFull_CalculatesMillisecondsPassed()
    {
        // Arrange
        var isStarted = false;
        this.mockStopWatchWrapper.GetTimestamp().Returns(_ =>
        {
            var result = isStarted ? 320_000L : 160_000L;
            isStarted = !isStarted;

            return result;
        });
        var sut = CreateSystemUnderTest();

        // Act
        for (var i = 0; i < 1000; i++)
        {
            sut.Start();
            sut.Stop();
        }

        // Assert
        this.mockStopWatchWrapper.Received(2000).GetTimestamp();
        sut.MillisecondsPassed.Should().Be(16);
    }

    [Fact]
    public void Reset_WhenInvoked_ResetsTheTimer()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Start();
        sut.Stop();

        // Act
        sut.Reset();

        // Assert
        sut.MillisecondsPassed.Should().Be(0);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TimerService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TimerService CreateSystemUnderTest() => new (this.mockStopWatchWrapper);
}
