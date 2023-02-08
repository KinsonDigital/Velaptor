// <copyright file="FrameTimeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using System;
using FluentAssertions;
using Velaptor;
using Xunit;

public class FrameTimeTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsPropsToCorrectValues()
    {
        // Arrange & Arrange
        var time = new FrameTime
        {
            TotalTime = new TimeSpan(1, 2, 3, 4),
            ElapsedTime = new TimeSpan(5, 6, 7, 8),
        };

        // Assert
        time.TotalTime.Should().Be(new TimeSpan(1, 2, 3, 4));
        time.ElapsedTime.Should().Be(new TimeSpan(5, 6, 7, 8));
    }
    #endregion
}
