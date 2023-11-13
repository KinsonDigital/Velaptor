// <copyright file="CachedValueTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
namespace VelaptorTests;

using System;
using FluentAssertions;
using Moq;
using Velaptor;
using Xunit;

public class CachedValueTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGetterWhenNotCachingParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new CachedValue<int>(123, null, _ => { });
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'getterWhenNotCaching')");
    }

    [Fact]
    public void Ctor_WithNullSetterWhenNotCachingParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new CachedValue<int>(123, () => 0, null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'setterWhenNotCaching')");
    }

    [Fact]
    public void Ctor_WhenInvokedWithCachingOn_SetsValueInternally()
    {
        // Arrange
        var externalSystemValue = 0;
        var cachedValue = new CachedValue<int>(
            1234,
            () => externalSystemValue,
            value => externalSystemValue = value,
            true);

        // Act
        var actual = cachedValue.GetValue();

        // Assert
        actual.Should().Be(1234);
        externalSystemValue.Should().Be(0);
    }

    [Fact]
    public void Ctor_WhenInvokedWithCachingOff_SetsValueExternally()
    {
        // Arrange
        var externalSystemValue = 0;
        var cachedValue = new CachedValue<int>(
            1234,
            () => externalSystemValue,
            value => externalSystemValue = value,
            false);

        // Act
        var actual = cachedValue.GetValue();

        // Assert
        actual.Should().Be(1234);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void IsCaching_WhenGoingFromCachingOffToOn_ReturnsCorrectResult()
    {
        // Arrange
        var externalSystemValue = 0;
        var cachedValue = new CachedValue<int>(
            1234,
            () => externalSystemValue,
            value => externalSystemValue = value,
            false)
        {
            // Act
            IsCaching = true,
        };

        var actual = cachedValue.GetValue();

        // Assert
        actual.Should().Be(1234);
    }

    [Fact]
    public void IsCaching_WhenGoingFromCachingOnToOff_ReturnsCorrectResult()
    {
        // Arrange
        var externalSystemValue = 0;
        var cachedValue = new CachedValue<int>(
            1234,
            () => externalSystemValue,
            value => externalSystemValue = value,
            true)
        {
            // Act
            IsCaching = false,
        };

        var actual = cachedValue.GetValue();

        // Assert
        actual.Should().Be(1234);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetValue_WhileCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var cachedValue = new CachedValue<int>(
            1234,
            It.IsAny<int>,
            _ => { });

        // Act
        var actual = cachedValue.GetValue();

        // Assert
        actual.Should().Be(1234);
    }

    [Fact]
    public void GetValue_WhileNotCachingValue_ReturnsCorrectResult()
    {
        // Arrange
        var cachedValue = new CachedValue<int>(
            5678,
            () => 1234,
            _ => { })
        {
            IsCaching = false,
        };

        // Act
        var actual = cachedValue.GetValue();

        // Assert
        actual.Should().Be(1234);
    }

    [Fact]
    public void SetValue_WhileCaching_ReturnsCorrectResult()
    {
        // Arrange
        var cachedValue = new CachedValue<int>(
            5678,
            () => 0,
            _ => { });

        // Act
        cachedValue.SetValue(1234);
        cachedValue.SetValue(5678);
        var actual = cachedValue.GetValue();

        // Assert
        actual.Should().Be(5678);
    }

    [Fact]
    public void SetValue_WhileNotCachingValue_InvokesOnResolve()
    {
        // Arrange
        var actual = 0;
        var cachedValue = new CachedValue<int>(
            5678,
            () => 0,
            value => actual = value)
        {
            IsCaching = false,
        };

        // Act
        cachedValue.SetValue(1234);

        // Assert
        actual.Should().Be(1234);
    }
    #endregion
}
