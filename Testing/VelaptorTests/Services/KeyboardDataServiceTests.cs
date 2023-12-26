// <copyright file="KeyboardDataServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using FluentAssertions;
using NSubstitute;
using Velaptor.Input;
using Velaptor.ReactableData;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="KeyboardDataService"/> class.
/// </summary>
public class KeyboardDataServiceTests
{
    private readonly IPushReactable<KeyboardKeyStateData> mockKeyboardDataReactable;
    private readonly IDisposable mockUnsubscriber;
    private IReceiveSubscription<KeyboardKeyStateData>? subscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardDataServiceTests"/> class.
    /// </summary>
    public KeyboardDataServiceTests()
    {
        this.mockUnsubscriber = Substitute.For<IDisposable>();

        this.mockKeyboardDataReactable = Substitute.For<IPushReactable<KeyboardKeyStateData>>();
        this.mockKeyboardDataReactable.Subscribe(Arg.Any<IReceiveSubscription<KeyboardKeyStateData>>())
            .Returns(this.mockUnsubscriber)
            .AndDoes(callInfo => this.subscription = callInfo.Arg<IReceiveSubscription<KeyboardKeyStateData>>());
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullKeyboardDataReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new KeyboardDataService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'keyboardDataReactable')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetKeyStates_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new List<KeyValuePair<KeyCode, bool>>();
        var keys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

        foreach (var key in keys)
        {
            expected.Add(new KeyValuePair<KeyCode, bool>(key, key == KeyCode.V));
        }

        var sut = new KeyboardDataService(this.mockKeyboardDataReactable);
        this.subscription.OnReceive(new KeyboardKeyStateData { Key = KeyCode.V, IsDown = true });

        // Act
        var actual = sut.GetKeyStates();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    [SuppressMessage("csharpsquid", "S3966", Justification = "Disposing twice is required for testing.")]
    public void Dispose_WhenInvoked_DisposesOfService()
    {
        // Arrange
        var sut = new KeyboardDataService(this.mockKeyboardDataReactable);

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockUnsubscriber.Received(1).Dispose();
    }
    #endregion
}
