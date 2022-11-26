// <copyright file="MousePositionEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI;

using System.Drawing;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="MousePositionEventArgs"/> class.
/// </summary>
public class MousePositionEventArgsTests
{
    [Fact]
    public void Ctor_WhenInvoked_ConstructsInstance()
    {
        // Arrange
        var expected = new Point(11, 22);
        var eventArgs = new MousePositionEventArgs(new Point(11, 22));

        // Act
        var actual = eventArgs.MousePosition;

        // Assert
        Assert.Equal(expected, actual);
    }
}
