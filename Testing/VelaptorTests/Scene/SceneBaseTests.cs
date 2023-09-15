// <copyright file="SceneBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Scene;

using System;
using System.Drawing;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using Fakes;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.ReactableData;
using Velaptor.Scene;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="SceneBase"/> class.
/// </summary>
public class SceneBaseTests
{
    private readonly Mock<IContentLoader> mockContentLoader;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockUnsubscriber;
    private IReceiveSubscription<WindowSizeData>? winSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneBaseTests"/> class.
    /// </summary>
    public SceneBaseTests()
    {
        this.mockContentLoader = new Mock<IContentLoader>();
        this.mockUnsubscriber = new Mock<IDisposable>();

        var mockWinSizeReactable = new Mock<IPushReactable<WindowSizeData>>();
        mockWinSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<WindowSizeData>>()))
            .Callback<IReceiveSubscription<WindowSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit tests.");
                reactor.Name.Should().Be($"SceneBase.Init - {nameof(PushNotifications.WindowSizeChangedId)}");

                this.winSizeReactor = reactor;
            })
            .Returns<IReceiveSubscription<WindowSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit tests.");
                return this.mockUnsubscriber.Object;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateWindowSizeReactable())
            .Returns(mockWinSizeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullContentLoaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SceneBaseFake(null, this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'contentLoader')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SceneBaseFake(this.mockContentLoader.Object, null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_PropAreCorrectDefaultValues()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.Name.Should().BeEmpty();
        sut.Id.Should().NotBe(Guid.Empty);
        sut.IsLoaded.Should().BeFalse();
        sut.WindowSize.Should().BeEquivalentTo(default(SizeU));
        sut.ContentLoader.Should().NotBeNull();
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Name_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act
        var sut = new SceneBaseFake(this.mockContentLoader.Object, this.mockReactableFactory.Object)
        {
            Name = "test-value",
        };

        // Assert
        sut.Name.Should().Be("test-value");
    }

    [Fact]
    public void Controls_WhenInvoked_LoadsContent()
    {
        // Arrange
        var ctrlA = new Mock<IControl>();
        ctrlA.Name = nameof(ctrlA);

        var ctrlB = new Mock<IControl>();
        ctrlB.Name = nameof(ctrlB);

        var sut = CreateSystemUnderTest();
        sut.AddControl(ctrlA.Object);
        sut.AddControl(ctrlB.Object);

        // Act
        var actual = sut.Controls;

        // Assert
        actual.Should().HaveCount(2);
        actual[0].Should().BeSameAs(ctrlA.Object);
        actual[1].Should().BeSameAs(ctrlB.Object);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void AddControl_WhenInvoked_AddsControl()
    {
        // Arrange
        var mockCtrl = new Mock<IControl>();

        var sut = CreateSystemUnderTest();

        // Act
        sut.AddControl(mockCtrl.Object);

        // Assert
        sut.Controls.Should().HaveCount(1);
        sut.Controls[0].Should().BeSameAs(mockCtrl.Object);
    }

    [Fact]
    public void RemoveControl_WhenInvoked_AddsControl()
    {
        // Arrange
        var mockCtrl = new Mock<IControl>();

        var sut = CreateSystemUnderTest();
        sut.AddControl(mockCtrl.Object);

        // Act
        sut.RemoveControl(mockCtrl.Object);

        // Assert
        sut.Controls.Should().BeEmpty();
    }

    [Fact]
    public void LoadContent_WhenInvoked_LoadsContent()
    {
        // Arrange
        var ctrlA = new Mock<IControl>();
        var ctrlB = new Mock<IControl>();

        var sut = CreateSystemUnderTest();
        sut.AddControl(ctrlA.Object);
        sut.AddControl(ctrlB.Object);

        // Act
        sut.LoadContent();

        // Assert
        ctrlA.VerifyOnce(m => m.LoadContent());
        ctrlB.VerifyOnce(m => m.LoadContent());
        sut.IsLoaded.Should().BeTrue();
    }

    [Fact]
    public void UnloadContent_WhenContentHasBeenLoaded_UnloadsContent()
    {
        // Arrange
        var ctrlA = new Mock<IControl>();
        var ctrlB = new Mock<IControl>();

        var sut = CreateSystemUnderTest();
        sut.AddControl(ctrlA.Object);
        sut.AddControl(ctrlB.Object);

        sut.LoadContent();

        // Act
        sut.UnloadContent();

        // Assert
        ctrlA.VerifyOnce(m => m.UnloadContent());
        ctrlB.VerifyOnce(m => m.UnloadContent());
        sut.Controls.Should().BeEmpty();
        sut.IsLoaded.Should().BeFalse();
    }

    [Fact]
    public void UnloadContent_WhenContentHasNotBeenLoaded_DoesNotUnloadContent()
    {
        // Arrange
        var ctrlA = new Mock<IControl>();

        var sut = CreateSystemUnderTest();
        sut.AddControl(ctrlA.Object);

        // Act
        sut.UnloadContent();

        // Assert
        ctrlA.VerifyNever(m => m.UnloadContent());
        sut.Controls.Should().HaveCount(1);
        sut.IsLoaded.Should().BeFalse();
    }

    [Fact]
    public void Update_WhenContentHasNotBeenLoaded_DoesNotUpdateControls()
    {
        // Arrange
        var mockCtrlA = new Mock<IControl>();
        var mockCtrlB = new Mock<IControl>();

        var sut = CreateSystemUnderTest();
        sut.AddControl(mockCtrlA.Object);
        sut.AddControl(mockCtrlB.Object);

        // Act
        sut.Update(default);

        // Assert
        mockCtrlA.VerifyNever(m => m.Update(It.IsAny<FrameTime>()));
        mockCtrlB.VerifyNever(m => m.Update(It.IsAny<FrameTime>()));
    }

    [Fact]
    public void Update_WhenContentHasBeenLoaded_UpdatesControls()
    {
        // Arrange
        var mockCtrlA = new Mock<IControl>();
        var mockCtrlB = new Mock<IControl>();

        var sut = CreateSystemUnderTest();
        sut.AddControl(mockCtrlA.Object);
        sut.AddControl(mockCtrlB.Object);

        sut.LoadContent();

        var frameTime = new FrameTime
        {
            TotalTime = new TimeSpan(1, 2, 3, 4, 5),
            ElapsedTime = new TimeSpan(6, 7, 8, 9, 10),
        };

        // Act
        sut.Update(frameTime);

        // Assert
        mockCtrlA.VerifyOnce(m => m.Update(frameTime));
        mockCtrlB.VerifyOnce(m => m.Update(frameTime));
    }

    [Fact]
    public void Render_WhenContentHasNotBeenLoaded_DoesNotRenderControls()
    {
        // Arrange
        var mockCtrlA = new Mock<IControl>();
        var mockCtrlB = new Mock<IControl>();

        var sut = CreateSystemUnderTest();
        sut.AddControl(mockCtrlA.Object);
        sut.AddControl(mockCtrlB.Object);

        // Act
        sut.Render();

        // Assert
        mockCtrlA.VerifyNever(m => m.Render());
        mockCtrlB.VerifyNever(m => m.Render());
    }

    [Fact]
    public void Render_WhenContentHasBeenLoaded_RendersControls()
    {
        // Arrange
        var mockCtrlA = new Mock<IControl>();
        var mockCtrlB = new Mock<IControl>();

        var sut = CreateSystemUnderTest();
        sut.AddControl(mockCtrlA.Object);
        sut.AddControl(mockCtrlB.Object);

        sut.LoadContent();

        // Act
        sut.Render();

        // Assert
        mockCtrlA.VerifyOnce(m => m.Render());
        mockCtrlB.VerifyOnce(m => m.Render());
    }
    #endregion

    #region Reactable Tests
    [Fact]
    public void WinSizeReactable_WithWindowSizeNotification_SetsWindowSize()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.winSizeReactor.OnReceive(new WindowSizeData { Width = 100, Height = 200 });

        // Assert
        sut.WindowSize.Should().BeEquivalentTo(new SizeU { Width = 100, Height = 200 });
        sut.WindowCenter.Should().BeEquivalentTo(new Point { X = 50, Y = 100 });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SceneBaseFake"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SceneBaseFake CreateSystemUnderTest()
        => new (this.mockContentLoader.Object, this.mockReactableFactory.Object);
}
