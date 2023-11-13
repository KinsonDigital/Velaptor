// <copyright file="NativeInputFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Factories;

using System;
using FluentAssertions;
using Moq;
using Velaptor.Factories;
using Xunit;

/// <summary>
/// Tests the <see cref="NativeInputFactory"/> class.
/// </summary>
public class NativeInputFactoryTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWindowFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new NativeInputFactory(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'windowFactory')");
    }

    [Fact]
    public void Ctor_WithNonNullWindowFactoryParam_DoesNotThrowException()
    {
        // Arrange & Act
        var act = () => _ = new NativeInputFactory(new Mock<IWindowFactory>().Object);

        // Assert
        act.Should().NotThrow();
    }
    #endregion
}
