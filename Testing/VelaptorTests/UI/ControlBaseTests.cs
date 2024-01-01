// <copyright file="ControlBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI;

using System;
using System.Drawing;
using Fakes;
using FluentAssertions;
using Moq;
using Velaptor.Input;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="ControlBase"/> class.
/// </summary>
public class ControlBaseTests
{
    private readonly Mock<IAppInput<KeyboardState>> mockKeyboardInput;
    private readonly Mock<IAppInput<MouseState>> mockMouseInput;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlBaseTests"/> class.
    /// </summary>
    public ControlBaseTests()
    {
        this.mockKeyboardInput = new Mock<IAppInput<KeyboardState>>();
        this.mockMouseInput = new Mock<IAppInput<MouseState>>();
    }

    /// <summary>
    /// Gets the mouse test data.
    /// </summary>
    public static TheoryData<int, int, bool, byte, byte, byte, byte> MouseData =>
        new ()
        {
            // xPos    yPos    mouseDown   expectedAlpha   expectedRed     expectedGreen       expectedBlue
            // ReSharper disable MultipleSpaces
            {   75,     75,      true,          255,            190,           190,                190 },
            {   75,     75,      false,         255,            230,           230,                230 },
            {   200,    75,      true,          255,            255,           255,                255 },
            {   200,    75,      false,         255,            255,           255,                255 },
            // ReSharper restore MultipleSpaces
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
        actual.Should().BeEmpty();
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
        actual.Should().Be("test-name");
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
        actual.Should().Be(expected);
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
        actualLeft.Should().Be(expected);
        actualPosX.Should().Be(expected);
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
        actualRight.Should().Be(expected);
        actualPosX.Should().Be(20);
    }

    [Fact]
    public void Top_WhenSettingValue_PropsReturnCorrectResults()
    {
        // Arrange
        const int expected = 20;
        var sut = CreateSystemUnderTest();

        // Act
        sut.Top = 20;
        var actual = sut.Top;

        // Assert
        actual.Should().Be(expected);
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
        var actualPosY = sut.Position.Y;

        // Assert
        actualBottom.Should().Be(expected);
        actualPosY.Should().Be(20);
    }

    [Fact]
    public void Width_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Width = 11;

        // Act
        var actual = sut.Width;

        // Assert
        actual.Should().Be(11u);
    }

    [Fact]
    public void HalfWidth_WhenSettingWidth_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Width = 11;

        // Act
        var actual = sut.HalfWidth;

        // Assert
        actual.Should().Be(5u);
    }

    [Fact]
    public void Height_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Height = 11;

        // Act
        var actual = sut.Height;

        // Assert
        actual.Should().Be(11u);
    }

    [Fact]
    public void HalfHeight_WhenSettingHeight_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Height = 11;

        // Act
        var actual = sut.HalfHeight;

        // Assert
        actual.Should().Be(5u);
    }

    [Fact]
    public void Visible_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Visible;

        // Assert
        actual.Should().BeTrue();
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
        actual.Should().BeFalse();
    }

    [Fact]
    public void Enabled_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Enabled;

        // Assert
        actual.Should().BeTrue();
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
        actual.Should().BeFalse();
    }

    [Fact]
    public void MouseDownColor_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.MouseDownColor;

        // Assert
        actual.Should().Be(Color.FromArgb(255, 190, 190, 190));
    }

    [Fact]
    public void MouseHoverColor_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.MouseHoverColor;

        // Assert
        actual.Should().Be(Color.FromArgb(255, 230, 230, 230));
    }

    [Fact]
    public void MouseDownColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.MouseDownColor = Color.FromArgb(11, 22, 33, 44);
        var actual = sut.MouseDownColor;

        // Assert
        Assert.Equal(Color.FromArgb(11, 22, 33, 44), actual);
    }

    [Fact]
    public void MouseHoverColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.MouseHoverColor = Color.FromArgb(11, 22, 33, 44);
        var actual = sut.MouseHoverColor;

        // Assert
        actual.Should().Be(Color.FromArgb(11, 22, 33, 44));
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

        beforeLoad.Should().BeFalse();
        afterLoad.Should().BeTrue();
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
        var sut = new ControlBaseFake(this.mockKeyboardInput.Object, this.mockMouseInput.Object)
        {
            Position = new Point(50, 50),
            Width = 100,
            Height = 100,
        };

        var mouseState = new MouseState(
            new Point(xPos, yPos),
            mouseDown,
            false,
            false,
            MouseScrollDirection.ScrollDown,
            0);

        this.mockMouseInput.Setup(m => m.GetState())
            .Returns(mouseState);

        sut.LoadContent();

        // Act
        sut.Update(default);

        // Assert
        sut.TintColorValue.A.Should().Be(alpha);
        sut.TintColorValue.R.Should().Be(red);
        sut.TintColorValue.G.Should().Be(green);
        sut.TintColorValue.B.Should().Be(blue);
    }

    [Fact]
    public void Update_WhenMouseMovesOverCtrl_RaisesMouseMoveEvent()
    {
        // Arrange
        var mouseStateWhenNotOverControl = new MouseState(
            new Point(80, 80),
            false,
            false,
            false,
            MouseScrollDirection.ScrollDown,
            0);

        var mouseStateWhenOverControl = default(MouseState);

        // Set the mouse position before moving it over the control
        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseStateWhenNotOverControl);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        sut.LoadContent();
        var monitor = sut.Monitor();

        // Act
        sut.Update(default);

        // Set current mouse position
        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseStateWhenOverControl);

        // Assert
        monitor.Should().Raise(nameof(ControlBase.MouseMove));
    }

    [Fact]
    public void Update_WithMouseButtonDownAndOverCtrl_RaisesMouseDownEvent()
    {
        // Arrange
        var mouseState = new MouseState(
            new Point(75, 75),
            true,
            false,
            false,
            MouseScrollDirection.ScrollDown,
            0);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseState);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        sut.LoadContent();
        var monitor = sut.Monitor();

        // Act
        sut.Update(default);

        // Assert
        monitor.Should().Raise(nameof(ControlBase.MouseDown));
    }

    [Fact]
    public void Update_WithMouseButtonDownThenUpOverCtrl_RaisesMouseUpAndClickEvent()
    {
        // Arrange
        var mouseButtonDownOverControlState = new MouseState(
            new Point(75, 75),
            true,
            false,
            false,
            MouseScrollDirection.ScrollDown,
            0);
        var mouseButtonUpOverControlState = new MouseState(
            new Point(75, 75),
            false,
            false,
            false,
            MouseScrollDirection.ScrollDown,
            0);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseButtonDownOverControlState);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        var clickEventInvoked = false;

        // ReSharper disable InconsistentNaming
        void Control_Clicked(object? sender, EventArgs e)
        {
            clickEventInvoked = true;
        }

        void Control_MouseUp(object? sender, EventArgs e)
        {
            const string becauseMsg =
                $"The '{nameof(ControlBase.MouseUp)}' event must be invoked before the '{nameof(ControlBase.Click)}' event";
            clickEventInvoked.Should().BeFalse(becauseMsg);
        }

        // ReSharper restore InconsistentNaming
        sut.Click += Control_Clicked;
        sut.MouseUp += Control_MouseUp;

        sut.LoadContent();
        var monitor = sut.Monitor();

        // Act
        sut.Update(default);

        // Set left mouse button as up which is a full click
        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseButtonUpOverControlState);

        // Invoked update again after mouse button has been lifted
        sut.Update(default);

        // Assert
        monitor.Should().Raise(nameof(ControlBase.MouseUp));
        monitor.Should().Raise(nameof(ControlBase.Click));
    }

    [Fact]
    public void Update_WhenContentIsNotLoaded_DoesNotUpdateCtrl()
    {
        // Arrange
        var mouseState = new MouseState(
            new Point(75, 75),
            true,
            false,
            false,
            MouseScrollDirection.ScrollDown,
            0);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseState);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        var monitor = sut.Monitor();

        // Act
        sut.Update(default);

        // Assert
        monitor.Should().NotRaise(nameof(ControlBase.MouseDown));
    }

    [Fact]
    public void Update_WhenDisabled_DoesNotUpdateCtrl()
    {
        // Arrange
        var mouseState = new MouseState(
            new Point(75, 75),
            true,
            false,
            false,
            MouseScrollDirection.ScrollDown,
            0);

        this.mockMouseInput.Setup(m => m.GetState()).Returns(mouseState);

        var sut = CreateSystemUnderTest();
        sut.Enabled = false;
        sut.Position = new Point(50, 50);
        sut.Width = 100;
        sut.Height = 100;

        var monitor = sut.Monitor();

        sut.LoadContent();

        // Act
        sut.Update(default);

        // Assert
        monitor.Should().NotRaise(nameof(ControlBase.MouseDown));
    }

    [Fact]
    public void UnloadContent_WhenInvoked_SetsControlAsUnloaded()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.UnloadContent();
        var actual = sut.IsLoaded;

        // Assert
        actual.Should().BeFalse();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ControlBase"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ControlBaseFake CreateSystemUnderTest()
        => new (this.mockKeyboardInput.Object, this.mockMouseInput.Object);
}
