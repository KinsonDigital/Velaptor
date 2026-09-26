// <copyright file="NativeInputFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable ConvertToLocalFunction
namespace VelaptorTests.Factories;

using System;
using NSubstitute;
using Shouldly;
using Silk.NET.Windowing;
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
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'window')");
    }

    [Fact]
    public void Ctor_WithNonNullWindowFactoryParam_DoesNotThrowException()
    {
        // Arrange & Act
        var act = () => _ = new NativeInputFactory(Substitute.For<IWindow>());

        // Assert
        Should.NotThrow(act);
    }
    #endregion
}
