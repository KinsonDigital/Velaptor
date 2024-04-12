// <copyright file="KeyboardTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Input;

using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Velaptor.Input;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="Keyboard"/> class.
/// </summary>
public class KeyboardTests
{
    private readonly IKeyboardDataService mockKeyDataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardTests"/> class.
    /// </summary>
    public KeyboardTests() => this.mockKeyDataService = Substitute.For<IKeyboardDataService>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullKeyboardDataStoreParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Keyboard(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'keyboardDataService')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetState_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new Dictionary<KeyCode, bool> { { KeyCode.K, true }, };

        this.mockKeyDataService.GetKeyStates().Returns(expected);

        var sut = CreateSystemUnderTest();

        // Act
        var state = sut.GetState();
        var actual = state.KeyStates;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Keyboard"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Keyboard CreateSystemUnderTest() => new (this.mockKeyDataService);
}
