// <copyright file="MouseMoveEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI;

using System.Drawing;
using FluentAssertions;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="MouseMoveEventArgs"/> class.
/// </summary>
public class MouseMoveEventArgsTests
{
    [Fact]
    public void Ctor_WhenInvoked_ConstructsInstance()
    {
        // Arrange
        var expectedGlobalPos = new Point(11, 22);
        var expectedLocalPos = new Point(33, 44);
        var eventArgs = new MouseMoveEventArgs(expectedGlobalPos, expectedLocalPos);

        // Act
        var actualGlobalPos = eventArgs.GlobalPos;
        var actualLocalPos = eventArgs.LocalPos;

        // Assert
        actualGlobalPos.Should().Be(expectedGlobalPos);
        actualLocalPos.Should().Be(expectedLocalPos);
    }
}
