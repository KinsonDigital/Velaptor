// <copyright file="ControlBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI;

using System;
using System.Collections.Generic;
using System.Drawing;
using Moq;
using Velaptor.Input;
using Velaptor.UI;
using Fakes;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="ControlBase"/> class.
/// </summary>
public class ControlBaseTests
{
    private readonly Mock<IAppInput<MouseState>> mockMouseInput;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlBaseTests"/> class.
    /// </summary>
    public ControlBaseTests() => this.mockMouseInput = new Mock<IAppInput<MouseState>>();

    /// <summary>
    /// Gets the mouse test data.
    /// </summary>
    public static IEnumerable<object[]> MouseData =>
        new List<object[]>
        {
            //              xPos    yPos    mouseDown   expectedAlpha   expectedRed     expectedGreen       expectedBlue
            new object[] {   75,     75,      true,          255,            190,           190,                190 },
            new object[] {   75,     75,      false,         255,            230,           230,                230 },
            new object[] {   200,    75,      true,          255,            255,           255,                255 },
            new object[] {   200,    75,      false,         255,            255,           255,                255 },
        };

    #region Prop Tests
    [Fact]
    public void Name_WithDefaultValue_ReturnsCorrectResult()
    {
        // Act
        var sut = CreateSystemUnderTest();

        // Arrange
        var actual = sut.Name;

        // Act
        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void Name_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Name = "test-name";

        // Arrange
        var actual = sut.Name;

        // Assert
        Assert.Equal("test-name", actual);
    }

    [Fact]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new Point(11, 22);
        var sut = CreateSystemUnderTest();
        sut.Position = new Point(11, 22);

        // Act
        var actual = sut.Position;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Left_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 11;
        var sut = CreateSystemUnderTest();
        sut.Left = 11;

        // Act
        var actualLeft = sut.Left;
        var actualPosX = sut.Position.X;

        // Assert
        Assert.Equal(expected, actualLeft);
        Assert.Equal(expected, actualPosX);
    }

    [Fact]
    public void Right_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 30;
        var sut = CreateSystemUnderTest();
        sut.Width = 20u;

        // Act
        sut.Right = 30;
        var actualRight = sut.Right;
        var actualPosX = sut.Position.X;

        // Assert
        Assert.Equal(expected, actualRight);
        Assert.Equal(20, actualPosX);
    }

    [Fact]
    public void Top_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 20;
        var sut = CreateSystemUnderTest();

        // Act
        sut.Top = 20;
        var actualTop = sut.Top;

        // Assert
        Assert.Equal(expected, actualTop);
    }

    [Fact]
    public void Bottom_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 30;
        var sut = CreateSystemUnderTest();
        sut.Height = 20;
        sut.Bottom = 30;

        // Act
        var actualBottom = sut.Bottom;
        var positionY = sut.Position.Y;

        // Assert
        Assert.Equal(expected, actualBottom);
        Assert.Equal(20, positionY);
    }

    [Fact]
    public void Width_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Width = 11;
        var actual = sut.Width;

        // Assert
        Assert.Equal(11u, actual);
    }

    [Fact]
    public void Height_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Height = 11;
        var actual = sut.Height;

        // Assert
        Assert.Equal(11u, actual);
    }

    [Fact]
    public void Visible_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Visible;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Visible_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Visible = false;
        var actual = sut.Visible;

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Enabled_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Enabled;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Enabled_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Enabled = false;
        var actual = sut.Enabled;

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void MouseDownColor_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var control = CreateSystemUnderTest();

        // Act
        var actual = control.MouseDownColor;

        // Assert
        Assert.Equal(Color.FromArgb(255, 190, 190, 190), actual);
    }

    [Fact]
    public void MouseHoverColor_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var control = CreateSystemUnderTest();

        // Act
        var actual = control.MouseHoverColor;

        // Assert
        Assert.Equal(Color.FromArgb(255, 230, 230, 230), actual);
    }

    [Fact]
    public void MouseDownColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var control = CreateSystemUnderTest();

        // Act
        control.MouseDownColor = Color.FromArgb(11, 22, 33, 44);
        var actual = control.MouseDownColor;

        // Assert
        Assert.Equal(Color.FromArgb(11, 22, 33, 44), actual);
    }

    [Fact]
    public void MouseHoverColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var control = CreateSystemUnderTest();

        // Act
        control.MouseHoverColor = Color.FromArgb(11, 22, 33, 44);
        var actual = control.MouseHoverColor;

        // Assert
        Assert.Equal(Color.FromArgb(11, 22, 33, 44), actual);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void LoadContent_WhenInvokedBeforeAndAfterLoadingContent_SetContentAsLoaded()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var beforeLoad = sut.IsLoaded;
        sut.LoadContent();
        var afterLoad = sut.IsLoaded;

        Assert.False(beforeLoad);
        Assert.True(afterLoad);
    }

    [Theory]
    [MemberData(nameof(MouseData))]
    public void Update_WhenLeftMouseButtonIsDownOverControl_SetsTintColor(
        int xPos,
        int yPos,
        bool mouseDown,
        byte alpha,
        byte red,
        byte green,
        byte blue)
    {
        // Arrange
        var sut = new ControlBaseFake(this.mockMouseInput.Object)
        {
            Position = new Point(50, 50),
            Width = 100,
            Height = 100,
        };

        var mouseState = default(MouseState);
        mouseState.SetPosition(xPos, yPos);
        mouseState.SetButtonState(MouseButton.LeftButton, mouseDown);

        this.mockMouseInput.Setup(m => m.GetState())
            .Returns(mouseState);

        sut.LoadContent();

        // Act
        sut.Update(default);

        // Assert
        Assert.Equal(alpha, sut.TintColorValue.A);
        Assert.Equal(red, sut.TintColorValue.R);
        Assert.Equal(green, sut.TintColorValue.G);
        Assert.Equal(blue, sut.TintColorValue.B);
    }

    [Fact]
    public void Update_WhenMouseMovesOverCtrl_RaisesMouseMoveEvent()
    {
        // Arrange
        var mouseStateWhenNotOverControl = default(MouseState);
        mouseStateWhenNotOverControl.SetPosition(75, 75);

        var mouseStateWhenOverControl = default(MouseState);
        mouseStateWhenOverControl.SetPosition(80, 80);

        // Set the mouse position before moving it over the control
        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseStateWhenNotOverControl);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        sut.LoadContent();

        // Act
        sut.Update(default);

        // Set current mouse position
        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseStateWhenOverControl);

        // Assert
        Assert.Raises<MousePositionEventArgs>(
            e => sut.MouseMove += e,
            e => sut.MouseMove -= e,
            () =>
            {
                sut.Update(default);
            });
    }

    [Fact]
    public void Update_WithMouseButtonDownAndOverCtrl_RaisesMouseDownEvent()
    {
        // Arrange
        var mouseState = default(MouseState);
        mouseState.SetPosition(75, 75);
        mouseState.SetButtonState(MouseButton.LeftButton, true);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseState);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        sut.LoadContent();

        // Act
        sut.Update(default);

        // Assert
        Assert.Raises<EventArgs>(
            e => sut.MouseDown += e,
            e => sut.MouseDown -= e,
            () =>
            {
                sut.Update(default);
            });
    }

    [Fact]
    public void Update_WithMouseButtonDownThenUpOverCtrl_RaisesMouseUpAndClickEvent()
    {
        // Arrange
        var mouseButtonDownOverControlState = default(MouseState);
        mouseButtonDownOverControlState.SetPosition(75, 75);
        mouseButtonDownOverControlState.SetButtonState(MouseButton.LeftButton, true);
        var mouseButtonUpOverControlState = default(MouseState);
        mouseButtonUpOverControlState.SetPosition(75, 75);
        mouseButtonUpOverControlState.SetButtonState(MouseButton.LeftButton, false);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseButtonDownOverControlState);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        var clickInvoked = false;

        void CtrlClicked(object? sender, EventArgs e)
        {
            clickInvoked = true;
        }

        void MouseUp(object? sender, EventArgs e)
        {
            Assert.False(clickInvoked, $"The '{nameof(ControlBase.MouseUp)}' event must be invoked before '{nameof(ControlBase.Click)}'");
        }

        sut.Click += CtrlClicked;
        sut.MouseUp += MouseUp;

        sut.LoadContent();

        // Act
        sut.Update(default);

        // Set left mouse button as up which is a full click
        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseButtonUpOverControlState);

        // Assert
        Assert.Raises<EventArgs>(
            e => sut.MouseUp += e,
            e => sut.MouseUp -= e,
            () =>
            {
                sut.Update(default);
            });
    }

    [Fact]
    public void Update_WhenContentIsNotLoaded_DoesNotUpdateCtrl()
    {
        // Arrange
        var mouseState = default(MouseState);
        mouseState.SetPosition(75, 75);
        mouseState.SetButtonState(MouseButton.LeftButton, true);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseState);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        // Act & Assert
        AssertExtensions.DoesNotRaise<EventArgs>(
            e => sut.MouseDown += e,
            e => sut.MouseDown -= e,
            () => sut.Update(default));
    }

    [Fact]
    public void Update_WhenDisabled_DoesNotUpdateCtrl()
    {
        // Arrange
        var mouseState = default(MouseState);
        mouseState.SetPosition(75, 75);
        mouseState.SetButtonState(MouseButton.LeftButton, true);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseState);

        var sut = CreateSystemUnderTest();
        sut.Enabled = false;
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        sut.LoadContent();

        // Act & Assert
        AssertExtensions.DoesNotRaise<EventArgs>(
            e => sut.MouseDown += e,
            e => sut.MouseDown -= e,
            () => sut.Update(default));
    }

    [Fact]
    public void UnloadContent_WhenInvoked_SetsControlAsUnloaded()
    {
        // Arrange
        var control = CreateSystemUnderTest();

        // Act
        control.UnloadContent();
        var actual = control.IsLoaded;

        // Assert
        Assert.False(actual);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ControlBase"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ControlBaseFake CreateSystemUnderTest()
        => new ControlBaseFake(this.mockMouseInput.Object);
}
