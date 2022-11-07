// <copyright file="ControlBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using Moq;
using Velaptor.Input;
using Velaptor.UI;
using VelaptorTests.Fakes;
using VelaptorTests.Helpers;
using Xunit;

namespace VelaptorTests.UI;

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
        var ctrlBase = CreateBase();

        // Arrange
        var actual = ctrlBase.Name;

        // Act
        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void Name_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var ctrlBase = CreateBase();
        ctrlBase.Name = "test-name";

        // Arrange
        var actual = ctrlBase.Name;

        // Assert
        Assert.Equal("test-name", actual);
    }

    [Fact]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new Point(11, 22);
        var ctrlBase = CreateBase();
        ctrlBase.Position = new Point(11, 22);

        // Act
        var actual = ctrlBase.Position;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Left_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 11;
        var ctrlBase = CreateBase();
        ctrlBase.Left = 11;

        // Act
        var actualLeft = ctrlBase.Left;
        var actualPosX = ctrlBase.Position.X;

        // Assert
        Assert.Equal(expected, actualLeft);
        Assert.Equal(expected, actualPosX);
    }

    [Fact]
    public void Right_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 30;
        var ctrlBase = CreateBase();
        ctrlBase.Width = 20u;

        // Act
        ctrlBase.Right = 30;
        var actualRight = ctrlBase.Right;
        var actualPosX = ctrlBase.Position.X;

        // Assert
        Assert.Equal(expected, actualRight);
        Assert.Equal(20, actualPosX);
    }

    [Fact]
    public void Top_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 20;
        var ctrlBase = CreateBase();

        // Act
        ctrlBase.Top = 20;
        var actualTop = ctrlBase.Top;

        // Assert
        Assert.Equal(expected, actualTop);
    }

    [Fact]
    public void Bottom_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 30;
        var ctrlBase = CreateBase();
        ctrlBase.Height = 20;
        ctrlBase.Bottom = 30;

        // Act
        var actualBottom = ctrlBase.Bottom;
        var positionY = ctrlBase.Position.Y;

        // Assert
        Assert.Equal(expected, actualBottom);
        Assert.Equal(20, positionY);
    }

    [Fact]
    public void Width_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var ctrlBase = CreateBase();

        // Act
        ctrlBase.Width = 11;
        var actual = ctrlBase.Width;

        // Assert
        Assert.Equal(11u, actual);
    }

    [Fact]
    public void Height_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var ctrlBase = CreateBase();

        // Act
        ctrlBase.Height = 11;
        var actual = ctrlBase.Height;

        // Assert
        Assert.Equal(11u, actual);
    }

    [Fact]
    public void Visible_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var ctrlBase = CreateBase();

        // Act
        var actual = ctrlBase.Visible;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Visible_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var ctrlBase = CreateBase();

        // Act
        ctrlBase.Visible = false;
        var actual = ctrlBase.Visible;

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Enabled_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var ctrlBase = CreateBase();

        // Act
        var actual = ctrlBase.Enabled;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Enabled_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var ctrlBase = CreateBase();

        // Act
        ctrlBase.Enabled = false;
        var actual = ctrlBase.Enabled;

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void MouseDownColor_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var control = CreateBase();

        // Act
        var actual = control.MouseDownColor;

        // Assert
        Assert.Equal(Color.FromArgb(255, 190, 190, 190), actual);
    }

    [Fact]
    public void MouseHoverColor_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var control = CreateBase();

        // Act
        var actual = control.MouseHoverColor;

        // Assert
        Assert.Equal(Color.FromArgb(255, 230, 230, 230), actual);
    }

    [Fact]
    public void MouseDownColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var control = CreateBase();

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
        var control = CreateBase();

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
        var ctrlBase = CreateBase();

        // Act
        var beforeLoad = ctrlBase.IsLoaded;
        ctrlBase.LoadContent();
        var afterLoad = ctrlBase.IsLoaded;

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
        var ctrlBase = new ControlBaseFake(this.mockMouseInput.Object)
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

        ctrlBase.LoadContent();

        // Act
        ctrlBase.Update(default);

        // Assert
        Assert.Equal(alpha, ctrlBase.TintColorValue.A);
        Assert.Equal(red, ctrlBase.TintColorValue.R);
        Assert.Equal(green, ctrlBase.TintColorValue.G);
        Assert.Equal(blue, ctrlBase.TintColorValue.B);
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

        var ctrlBase = CreateBase();
        ctrlBase.Position = new Point(50, 50);
        ctrlBase.Width = 100;
        ctrlBase.Height = 100;

        ctrlBase.LoadContent();

        // Act
        ctrlBase.Update(default);

        // Set current mouse position
        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseStateWhenOverControl);

        // Assert
        Assert.Raises<MousePositionEventArgs>(
            (e) => ctrlBase.MouseMove += e,
            (e) => ctrlBase.MouseMove -= e,
            () =>
            {
                ctrlBase.Update(default);
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

        var ctrlBase = CreateBase();
        ctrlBase.Position = new Point(50, 50);
        ctrlBase.Width = 100;
        ctrlBase.Height = 100;

        ctrlBase.LoadContent();

        // Act
        ctrlBase.Update(default);

        // Assert
        Assert.Raises<EventArgs>(
            (e) => ctrlBase.MouseDown += e,
            (e) => ctrlBase.MouseDown -= e,
            () =>
            {
                ctrlBase.Update(default);
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

        var ctrlBase = CreateBase();
        ctrlBase.Position = new Point(50, 50);
        ctrlBase.Width = 100;
        ctrlBase.Height = 100;

        var clickInvoked = false;

        void CtrlClicked(object? sender, EventArgs e)
        {
            clickInvoked = true;
        }

        void MouseUp(object? sender, EventArgs e)
        {
            Assert.False(clickInvoked, $"The '{nameof(ControlBase.MouseUp)}' event must be invoked before '{nameof(ControlBase.Click)}'");
        }

        ctrlBase.Click += CtrlClicked;
        ctrlBase.MouseUp += MouseUp;

        ctrlBase.LoadContent();

        // Act
        ctrlBase.Update(default);

        // Set left mouse button as up which is a full click
        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseButtonUpOverControlState);

        // Assert
        Assert.Raises<EventArgs>(
            (e) => ctrlBase.MouseUp += e,
            (e) => ctrlBase.MouseUp -= e,
            () =>
            {
                ctrlBase.Update(default);
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

        var ctrlBase = CreateBase();
        ctrlBase.Position = new Point(50, 50);
        ctrlBase.Width = 100;
        ctrlBase.Height = 100;

        // Act & Assert
        AssertExtensions.DoesNotRaise<EventArgs>(
            e => ctrlBase.MouseDown += e,
            e => ctrlBase.MouseDown -= e,
            () => ctrlBase.Update(default));
    }

    [Fact]
    public void Update_WhenDisabled_DoesNotUpdateCtrl()
    {
        // Arrange
        var mouseState = default(MouseState);
        mouseState.SetPosition(75, 75);
        mouseState.SetButtonState(MouseButton.LeftButton, true);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseState);

        var ctrlBase = CreateBase();
        ctrlBase.Enabled = false;
        ctrlBase.Position = new Point(50, 50);
        ctrlBase.Width = 100;
        ctrlBase.Height = 100;

        ctrlBase.LoadContent();

        // Act & Assert
        AssertExtensions.DoesNotRaise<EventArgs>(
            e => ctrlBase.MouseDown += e,
            e => ctrlBase.MouseDown -= e,
            () => ctrlBase.Update(default));
    }

    [Fact]
    public void UnloadContent_WhenInvoked_SetsControlAsUnloaded()
    {
        // Arrange
        var control = CreateBase();

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
    private ControlBaseFake CreateBase()
        => new ControlBaseFake(this.mockMouseInput.Object);
}
