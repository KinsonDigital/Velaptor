// <copyright file="MouseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Moq;
using Velaptor.Input;
using Velaptor.Reactables.Core;
using VelaptorTests.Helpers;
using Xunit;

namespace VelaptorTests.Input;

/// <summary>
/// Tests the <see cref="Mouse"/> class.
/// </summary>
public class MouseTests
{
    private readonly Mock<IReactable<(int x, int y)>> mockMousePosReactable;
    private readonly Mock<IReactable<(MouseButton button, bool isDown)>> mockMouseBtnReactable;
    private readonly Mock<IReactable<(MouseScrollDirection wheelDirection, int mouseWheelValue)>> mockMouseWheelReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseTests"/> class.
    /// </summary>
    public MouseTests()
    {
        this.mockMousePosReactable = new Mock<IReactable<(int x, int y)>>();
        this.mockMouseBtnReactable = new Mock<IReactable<(MouseButton button, bool isDown)>>();
        this.mockMouseWheelReactable = new Mock<IReactable<(MouseScrollDirection wheelDirection, int mouseWheelValue)>>();
    }

    #region Method Tests
    [Fact]
    public void Ctor_WhenInvoked_SubscribesToReactables()
    {
        // Act & Arrange
        var unused = CreateMouse();

        // Assert
        this.mockMousePosReactable.VerifyOnce(m => m.Subscribe(It.IsAny<Reactor<(int, int)>>()));
        this.mockMouseBtnReactable.VerifyOnce(m => m.Subscribe(It.IsAny<Reactor<(MouseButton, bool)>>()));
        this.mockMouseWheelReactable.VerifyOnce(m => m.Subscribe(It.IsAny<Reactor<(MouseScrollDirection, int)>>()));
    }

    [Fact]
    public void GetState_WhenGettingMousePosition_ReturnsCorrectResult()
    {
        // Arrange
        IReactor<(int, int)>? reactor = null;

        this.mockMousePosReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<(int x, int y)>>()))
            .Callback<IReactor<(int x, int y)>>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var mouse = CreateMouse();
        reactor?.OnNext((11, 22));

        // Act
        var actual = mouse.GetState();

        // Assert
        Assert.Equal(11, actual.GetPosition().X);
        Assert.Equal(22, actual.GetPosition().Y);
    }

    [Theory]
    [InlineData(0)] // Left
    [InlineData(1)] // Right
    [InlineData(2)] // Middle
    public void GetState_WhenGettingMouseButton_ReturnsCorrectResult(int mouseButtonValue)
    {
        // Arrange
        var mouseButton = (MouseButton)mouseButtonValue;
        IReactor<(MouseButton, bool)>? reactor = null;

        this.mockMouseBtnReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<(MouseButton, bool)>>()))
            .Callback<IReactor<(MouseButton, bool)>>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var mouse = CreateMouse();
        reactor?.OnNext((mouseButton, true));

        // Act
        var actual = mouse.GetState();

        // Assert
        Assert.True(actual.GetButtonState(mouseButton));
    }

    [Fact]
    public void GetState_WhenGettingMouseWheelData_ReturnsCorrectResult()
    {
        // Arrange
        IReactor<(MouseScrollDirection, int)>? reactor = null;

        this.mockMouseWheelReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<(MouseScrollDirection, int)>>()))
            .Callback<IReactor<(MouseScrollDirection, int)>>(reactorObj =>
            {
                reactor = reactorObj;
            });

        var mouse = CreateMouse();
        reactor?.OnNext((MouseScrollDirection.ScrollDown, 33));

        // Act
        var actual = mouse.GetState();

        // Assert
        Assert.Equal(MouseScrollDirection.ScrollDown, actual.GetScrollDirection());
        Assert.Equal(33, actual.GetScrollWheelValue());
    }

    [Fact]
    public void Reactable_WhenReactorCompletes_DisposesOfSubscriptions()
    {
        // Arrange
        IReactor<(int, int)>? posReactor = null;
        IReactor<(MouseButton, bool)>? btnReactor = null;
        IReactor<(MouseScrollDirection, int)>? wheelReactor = null;
        var mockMousePosUnsubscriber = new Mock<IDisposable>();
        var mockMouseBtnUnsubscriber = new Mock<IDisposable>();
        var mockMouseWheelUnsubscriber = new Mock<IDisposable>();

        this.mockMousePosReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<(int x, int y)>>()))
            .Returns(mockMousePosUnsubscriber.Object)
            .Callback<IReactor<(int x, int y)>>(reactorObj =>
            {
                posReactor = reactorObj;
            });

        this.mockMouseBtnReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<(MouseButton, bool)>>()))
            .Returns(mockMouseBtnUnsubscriber.Object)
            .Callback<IReactor<(MouseButton, bool)>>(reactorObj =>
            {
                btnReactor = reactorObj;
            });

        this.mockMouseWheelReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<(MouseScrollDirection, int)>>()))
            .Returns(mockMouseWheelUnsubscriber.Object)
            .Callback<IReactor<(MouseScrollDirection, int)>>(reactorObj =>
            {
                wheelReactor = reactorObj;
            });

        var unused = CreateMouse();

        // Act
        posReactor.OnCompleted();
        btnReactor.OnCompleted();
        wheelReactor.OnCompleted();

        // Assert
        mockMousePosUnsubscriber.VerifyOnce(m => m.Dispose());
        mockMouseBtnUnsubscriber.VerifyOnce(m => m.Dispose());
        mockMouseWheelUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Mouse"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Mouse CreateMouse()
        => new Mouse(this.mockMousePosReactable.Object,
            this.mockMouseBtnReactable.Object,
            this.mockMouseWheelReactable.Object);
}
