// <copyright file="ShaderNameAttributeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System;
using FluentAssertions;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="ShaderNameAttribute"/> class.
/// </summary>
public class ShaderNameAttributeTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new ShaderNameAttribute(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WithEmptyParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => _ = new ShaderNameAttribute(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsProperty()
    {
        // Arrange & Act
        var attribute = new ShaderNameAttribute("test-name");

        // Assert
        attribute.Name.Should().BeEquivalentTo("test-name");
    }
    #endregion
}
