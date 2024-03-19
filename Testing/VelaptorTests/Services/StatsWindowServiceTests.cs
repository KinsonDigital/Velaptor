// <copyright file="StatsWindowServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using ImGuiNET;
using NSubstitute;
using Velaptor.Input;
using Velaptor.NativeInterop.ImGui;
using Velaptor.Services;
using Xunit;
#pragma warning disable SA1008
using KeyState = (Velaptor.Input.KeyCode key, bool state);
using KeyboardStates = ((Velaptor.Input.KeyCode key, bool state)[], (Velaptor.Input.KeyCode key, bool state)[]);
#pragma warning restore SA1008

/// <summary>
/// Tests the <see cref="StatsWindowService"/> class.
/// </summary>
public class StatsWindowServiceTests
{
    private readonly IImGuiInvoker mockImGuiInvoker;
    private readonly IAppInput<KeyboardState> mockKeyboard;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatsWindowServiceTests"/> class.
    /// </summary>
    public StatsWindowServiceTests()
    {
        this.mockImGuiInvoker = Substitute.For<IImGuiInvoker>();
        this.mockKeyboard = Substitute.For<IAppInput<KeyboardState>>();
    }

#pragma warning disable SA1514
    #region Test Data
    /// <summary>
    /// Gets the current keyboard state data when updating the service.
    /// </summary>
    // ReSharper disable once UseCollectionExpression
    public static TheoryData<KeyboardStates, bool> AnyModifierKeysDownData =>
        new ()
        {
            // All left modifier keys down
            {
                ([
                    // First update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.LeftAlt, true), (KeyCode.LeftShift, true),
                    (KeyCode.S, true),
                ],
                [
                    // Second update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.LeftAlt, true), (KeyCode.LeftShift, true),
                    (KeyCode.S, false),
                ]),
                true // Expected
            },
            // Right Ctrl down and rest of left modifiers down
            {
                ([
                    // First update keyboard state
                    (KeyCode.RightControl, true), (KeyCode.LeftAlt, true), (KeyCode.LeftShift, true),
                    (KeyCode.S, true),
                ],
                [
                    // Second update keyboard state
                    (KeyCode.RightControl, true), (KeyCode.LeftAlt, true), (KeyCode.LeftShift, true),
                    (KeyCode.S, false),
                ]),
                true // Expected
            },
            // Right Alt down and rest of left modifiers down
            {
                ([
                    // First update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.RightAlt, true), (KeyCode.LeftShift, true),
                    (KeyCode.S, true),
                ],
                [
                    // Second update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.RightAlt, true), (KeyCode.LeftShift, true),
                    (KeyCode.S, false),
                ]),
                true // Expected
            },
            // Right Shift down and rest of left modifiers down
            {
                ([
                    // First update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.LeftAlt, true), (KeyCode.RightShift, true),
                    (KeyCode.S, true),
                ],
                [
                    // Second update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.LeftAlt, true), (KeyCode.RightShift, true),
                    (KeyCode.S, false),
                ]),
                true // Expected
            },
        };

    /// <summary>
    /// Gets the previous keyboard state data when updating the service.
    /// </summary>
    // ReSharper disable once UseCollectionExpression
    public static TheoryData<KeyboardStates, bool> AnyPreviousKeysNotDown =>
        new ()
        {
            // Ctrl Down
            {
                ([
                    // First update keyboard state
                    (KeyCode.LeftControl, false), (KeyCode.LeftAlt, false), (KeyCode.LeftShift, false),
                    (KeyCode.S, false),
                ],
                [
                    // Second update keyboard state
                    (KeyCode.LeftControl, false), (KeyCode.LeftAlt, true), (KeyCode.LeftShift, true),
                    (KeyCode.S, true),
                ]),
                false // Expected
            },
            // Alt Not Down
            {
                ([
                    // First update keyboard state
                    (KeyCode.LeftControl, false), (KeyCode.LeftAlt, false), (KeyCode.LeftShift, false),
                    (KeyCode.S, false),
                ],
                [
                    // Second update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.LeftAlt, false), (KeyCode.LeftShift, true),
                    (KeyCode.S, true),
                ]),
                false // Expected
            },
            // Shift Not Down
            {
                ([
                    // First update keyboard state
                    (KeyCode.LeftControl, false), (KeyCode.LeftAlt, false), (KeyCode.LeftShift, false),
                    (KeyCode.S, false),
                ],
                [
                    // Second update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.LeftAlt, true), (KeyCode.LeftShift, false),
                    (KeyCode.S, true),
                ]),
                false // Expected
            },
            // S Not Down
            {
                ([
                    // First update keyboard state
                    (KeyCode.LeftControl, false), (KeyCode.LeftAlt, false), (KeyCode.LeftShift, false),
                    (KeyCode.S, false),
                ],
                [
                    // Second update keyboard state
                    (KeyCode.LeftControl, true), (KeyCode.LeftAlt, true), (KeyCode.LeftShift, true),
                    (KeyCode.S, false),
                ]),
                false // Expected
            },
        };

    /// <summary>
    /// Gets the data for testing the initialization of the window.
    /// </summary>
    public static TheoryData<Vector2, Vector2, Vector2> InitData =>
        new ()
        {
            {
                new Vector2(100, 200), new Vector2(25, 50), new Vector2(105, 210)
            },
            {
                new Vector2(25, 50), new Vector2(100, 200), new Vector2(87, 60)
            },
        };
    #endregion
#pragma warning restore SA1514

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullImGuiInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new StatsWindowService(null, this.mockKeyboard);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'imGuiInvoker')");
    }

    [Fact]
    public void Ctor_WithNullKeyboardParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new StatsWindowService(this.mockImGuiInvoker, null!);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'keyboard')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Position = new Point(10, 20);

        // Assert
        sut.Position.Should().Be(new Point(10, 20));
    }

    [Fact]
    public void Visible_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var defaultValue = sut.Visible;

        // Act
        sut.Visible = true;

        // Assert
        defaultValue.Should().BeFalse();
        sut.Visible.Should().BeTrue();
    }
    #endregion

    #region Method Tests
    [Theory]
    [MemberData(nameof(AnyModifierKeysDownData))]
    [MemberData(nameof(AnyPreviousKeysNotDown))]
    public void Update_WhenKeyCombinationIsInvoked_CorrectWindowVisibilitySet(KeyboardStates keyboardStateData, bool expected)
    {
        // Arrange
        var (firstUpdateKeyStateData, secondUpdateKeyStateData) = keyboardStateData;

        var firstUpdateKeyState = default(KeyboardState);
        foreach (KeyState firstUpdateDataItem in firstUpdateKeyStateData)
        {
            firstUpdateKeyState.SetKeyState(firstUpdateDataItem.key, firstUpdateDataItem.state);
        }

        var secondUpdateKeyState = default(KeyboardState);
        foreach (KeyState secondUpdateDataItem in secondUpdateKeyStateData)
        {
            secondUpdateKeyState.SetKeyState(secondUpdateDataItem.key, secondUpdateDataItem.state);
        }

        var sut = CreateSystemUnderTest();
        this.mockKeyboard.GetState().Returns(firstUpdateKeyState);

        // Act
        sut.Update(default);

        this.mockKeyboard.GetState().Returns(secondUpdateKeyState);

        sut.Update(default);
        var actual = sut.Visible;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Render_WhenInvokedBeforeInitializationIsInvoked_RendersWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.UpdateFpsStat(123.456789f);
        sut.Visible = true;

        // Act
        sut.Render();

        // Assert
        this.mockImGuiInvoker.Received(5).Begin("Runtime Stats", ImGuiWindowFlags.None);
        this.mockImGuiInvoker.Received(5).Text("FPS: 123.46");
        this.mockImGuiInvoker.Received(5).SetWindowSize(Vector2.Zero);
        this.mockImGuiInvoker.Received(1).SetWindowPos(Vector2.Zero);
        this.mockImGuiInvoker.Received(5).End();
    }

    [Theory]
    [MemberData(nameof(InitData))]
    public void Render_WhenInvokedWithInitialization_InitializesWindow(
        Vector2 winSize,
        Vector2 textSize,
        Vector2 expected)
    {
        // Arrange
        var eventWasRaised = false;
        var style = new ImGuiStyle { WindowPadding = new Vector2(5, 10) };

        MockStyle(style);
        this.mockImGuiInvoker.GetWindowSize().Returns(winSize);
        this.mockImGuiInvoker.CalcTextSize(Arg.Any<string>()).Returns(textSize);

        var sut = CreateSystemUnderTest();
        sut.Visible = true;
        sut.Render();

        sut.Initialized += (_, _) => eventWasRaised = true;

        // Act
        sut.Render();

        // Invoked a second time ot make sure that init is only ran once
        sut.Render();

        // Assert
        this.mockImGuiInvoker.Received(1).GetWindowSize();
        this.mockImGuiInvoker.Received(1).CalcTextSize("Runtime Stats");
        this.mockImGuiInvoker.Received(1).GetStyle();
        this.mockImGuiInvoker.Received(1).SetWindowSize(expected);
        eventWasRaised.Should().BeTrue();
        sut.Size.Should().Be(new Size((int)expected.X, (int)expected.Y));
    }

    [Fact]
    [SuppressMessage("csharpsquid", "S3966", Justification = "Disposing twice is required for testing.")]
    public void Dispose_WhenInvoked_DisposesOfResources()
    {
        // Arrange
        var style = new ImGuiStyle { WindowPadding = new Vector2(5, 10) };
        MockStyle(style);

        var sut = CreateSystemUnderTest();
        var eventWasRaised = false;
        sut.Initialized += (_, _) => eventWasRaised = true;

        // Act
        sut.Dispose();
        sut.Dispose();
        sut.Render();
        sut.Render();

        // Assert
        eventWasRaised.Should().BeFalse();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="StatsWindowService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private StatsWindowService CreateSystemUnderTest() => new (this.mockImGuiInvoker, this.mockKeyboard);

    /// <summary>
    /// Mocks the given <paramref name="style"/>.
    /// </summary>
    /// <param name="style">The style to mock.</param>
    private void MockStyle(ImGuiStyle style)
    {
        ImGuiStylePtr stylePtr;
        unsafe
        {
            stylePtr = new ImGuiStylePtr(&style);
        }

        this.mockImGuiInvoker.GetStyle().Returns(stylePtr);
    }
}
