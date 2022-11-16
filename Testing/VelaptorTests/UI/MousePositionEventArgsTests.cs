// <copyright file="MousePositionEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Drawing;
using Velaptor.UI;
using Xunit;

namespace VelaptorTests.UI;

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
