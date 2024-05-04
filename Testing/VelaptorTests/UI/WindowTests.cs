// <copyright file="WindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Threading.Tasks;
using Fakes;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Scene;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="Window"/> class.
/// </summary>
public class WindowTests
{
    private readonly Mock<IWindow> mockWindow;
    private readonly Mock<ISceneManager> mockSceneManager;
    private readonly Mock<IBatcher> mockBatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowTests"/> class.
    /// </summary>
    public WindowTests()
    {
        this.mockSceneManager = new Mock<ISceneManager>();
        this.mockBatcher = new Mock<IBatcher>();

        this.mockWindow = new Mock<IWindow>();
        this.mockWindow.SetupProperty(p => p.Initialize);
        this.mockWindow.SetupProperty(p => p.Update);
        this.mockWindow.SetupProperty(p => p.Draw);
        this.mockWindow.SetupProperty(p => p.WinResize);
        this.mockWindow.SetupProperty(p => p.Uninitialize);
        this.mockWindow.SetupGet(p => p.SceneManager).Returns(this.mockSceneManager.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWindowParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WindowFake(null, this.mockBatcher.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'window')");
    }

    [Fact]
    public void Ctor_WhenInvoked_AutoPropsAreDefaultTrue()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.AutoSceneLoading.Should().BeTrue();
        sut.AutoSceneRendering.Should().BeTrue();
        sut.AutoSceneUnloading.Should().BeTrue();
        sut.AutoSceneUpdating.Should().BeTrue();
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Initialize_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        void Expected()
        {
            // This is only used for setting the property
        }

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize = Expected;
        var actual = sut.Initialize;

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeSameAs(actual);
    }

    [Fact]
    public void Update_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        void Expected(FrameTime a)
        {
            // This is only used for setting the property
        }

        var sut = CreateSystemUnderTest();

        // Act
        sut.Update = Expected;
        var actual = sut.Update;

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeSameAs(actual);
    }

    [Fact]
    public void Draw_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        void Expected(FrameTime a)
        {
            // This is only used for setting the property
        }

        var sut = CreateSystemUnderTest();

        // Act
        sut.Draw = Expected;
        var actual = sut.Draw;

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeSameAs(actual);
    }

    [Fact]
    public void WinResize_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        void Expected(SizeU a)
        {
            // This is only used for setting the property
        }

        var sut = CreateSystemUnderTest();

        // Act
        sut.WinResize = Expected;
        var actual = sut.WinResize;

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeSameAs(actual);
    }

    [Fact]
    public void Uninitialize_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        void Expected()
        {
            // This is only used for setting the property
        }

        var sut = CreateSystemUnderTest();

        // Act
        sut.Uninitialize = Expected;
        var actual = sut.Uninitialize;

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeSameAs(actual);
    }

    [Fact]
    public void Title_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Title = "test-title";
        _ = sut.Title;

        // Assert
        this.mockWindow.VerifySet(p => p.Title = "test-title", Times.Once());
        this.mockWindow.VerifyGet(p => p.Title, Times.Once());
    }

    [Fact]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Position = new Vector2(11, 22);
        _ = sut.Position;

        // Assert
        this.mockWindow.VerifySet(p => p.Position = new Vector2(11, 22), Times.Once());
        this.mockWindow.VerifyGet(p => p.Position, Times.Once());
    }

    [Fact]
    public void Width_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Width = 1234;
        _ = sut.Width;

        // Assert
        this.mockWindow.VerifySet(p => p.Width = 1234, Times.Once());
        this.mockWindow.VerifyGet(p => p.Width, Times.Once());
    }

    [Fact]
    public void Height_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Height = 1234;

        // Act
        _ = sut.Height;

        // Assert
        this.mockWindow.VerifySet(p => p.Height = 1234, Times.Once());
        this.mockWindow.VerifyGet(p => p.Height, Times.Once());
    }

    [Fact]
    public void AutoClearBuffer_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.AutoClearBuffer = true;
        _ = sut.AutoClearBuffer;

        // Assert
        this.mockWindow.VerifySet(p => p.AutoClearBuffer = true, Times.Once());
        this.mockWindow.VerifyGet(p => p.AutoClearBuffer, Times.Once());
    }

    [Fact]
    public void AutoSceneLoading_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var defaultValue = sut.AutoSceneLoading;

        // Act
        sut.AutoSceneLoading = !sut.AutoSceneLoading;

        // Assert
        sut.AutoSceneLoading.Should().Be(!defaultValue);
    }

    [Fact]
    public void AutoSceneUnloading_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var defaultValue = sut.AutoSceneUnloading;

        // Act
        sut.AutoSceneUnloading = !sut.AutoSceneUnloading;

        // Assert
        sut.AutoSceneUnloading.Should().Be(!defaultValue);
    }

    [Fact]
    public void AutoSceneUpdating_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var defaultValue = sut.AutoSceneUpdating;

        // Act
        sut.AutoSceneUpdating = !sut.AutoSceneUpdating;

        // Assert
        sut.AutoSceneUpdating.Should().Be(!defaultValue);
    }

    [Fact]
    public void AutoSceneRendering_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var defaultValue = sut.AutoSceneRendering;

        // Act
        sut.AutoSceneRendering = !sut.AutoSceneRendering;

        // Assert
        sut.AutoSceneRendering.Should().Be(!defaultValue);
    }

    [Fact]
    public void MouseCursorVisible_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.MouseCursorVisible = true;
        _ = sut.MouseCursorVisible;

        // Assert
        this.mockWindow.VerifySet(p => p.MouseCursorVisible = true, Times.Once());
        this.mockWindow.VerifyGet(p => p.MouseCursorVisible, Times.Once());
    }

    [Fact]
    public void UpdateFrequency_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.UpdateFrequency = 1234;
        _ = sut.UpdateFrequency;

        // Assert
        this.mockWindow.VerifySet(p => p.UpdateFrequency = 1234, Times.Once());
        this.mockWindow.VerifyGet(p => p.UpdateFrequency, Times.Once());
    }

    [Fact]
    public void WindowState_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.WindowState = StateOfWindow.FullScreen;
        _ = sut.WindowState;

        // Assert
        this.mockWindow.VerifySet(p => p.WindowState = StateOfWindow.FullScreen, Times.Once());
        this.mockWindow.VerifyGet(p => p.WindowState, Times.Once());
    }

    [Fact]
    public void TypeOfBorder_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.TypeOfBorder = WindowBorder.Resizable;
        _ = sut.TypeOfBorder;

        // Assert
        this.mockWindow.VerifySet(p => p.TypeOfBorder = WindowBorder.Resizable, Times.Once());
        this.mockWindow.VerifyGet(p => p.TypeOfBorder, Times.Once());
    }

    [Fact]
    public void Initialized_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockWindow.SetupGet(p => p.Initialized).Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Initialized;

        // Assert
        actual.Should().BeTrue();
    }
    #endregion

    #region Method tests
    [Fact]
    public void Show_WhenInvoked_ShowsWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Show();

        // Assert
        this.mockWindow.Verify(m => m.Show(), Times.Once());
    }

    [Fact]
    public void Close_WhenInvoked_ClosesWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Close();

        // Assert
        this.mockWindow.VerifyOnce(m => m.Close());
    }

    [Fact]
    public async Task ShowAsync_WhenInvoked_ShowsInternalWindow()
    {
        // Arrange
        this.mockWindow.Setup(m => m.ShowAsync(null, null))
            .Returns(Task.Run(() => { }));
        var sut = CreateSystemUnderTest();

        // Act
        await sut.ShowAsync();

        // Assert
        this.mockWindow.Verify(m => m.ShowAsync(null, null), Times.Once);
    }

    [Fact]
    public void OnDraw_WhenAutoRenderingIsEnabled_RenderScenesAndManipulatesBatch()
    {
        // Arrange
        this.mockSceneManager.SetupGet(p => p.TotalScenes).Returns(1);

        var sut = CreateSystemUnderTest();
        sut.AutoSceneRendering = true;

        // Act
        sut.OnDraw(default);

        // Assert
        this.mockBatcher.VerifyOnce(m => m.Begin());
        this.mockSceneManager.VerifyOnce(m => m.Render());
        this.mockBatcher.VerifyOnce(m => m.End());
    }

    [Fact]
    public void OnDraw_WhenAutoRenderingIsDisabled_DoesNotRenderScenes()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoSceneRendering = false;

        // Act
        sut.OnDraw(default);

        // Assert
        this.mockBatcher.VerifyNever(m => m.Clear());
        this.mockBatcher.VerifyNever(m => m.Begin());
        this.mockSceneManager.VerifyNever(m => m.Render());
        this.mockBatcher.VerifyNever(m => m.End());
    }

    [Fact]
    public void OnDraw_WhenAutoRenderingIsNotDisabledWithNoScenes_ShouldNotRenderScenesOrManipulateBatches()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoSceneRendering = true;

        // Act
        sut.OnDraw(default);

        // Assert
        this.mockBatcher.VerifyNever(m => m.Clear());
        this.mockBatcher.VerifyNever(m => m.Begin());
        this.mockSceneManager.VerifyNever(m => m.Render());
        this.mockBatcher.VerifyNever(m => m.End());
    }

    [Fact]
    [SuppressMessage("csharpsquid", "S3966", Justification = "Disposing twice is required for testing.")]
    public void Dispose_WhenInvoked_DisposesOfMangedResources()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockWindow.Verify(m => m.Dispose(), Times.Once());
    }
    #endregion

    /// <summary>
    /// Creates an instance of <see cref="WindowFake"/> for the purpose
    /// of testing the abstract <see cref="Window"/> class.
    /// </summary>
    /// <returns>The instance used for testing.</returns>
    private WindowFake CreateSystemUnderTest() => new (this.mockWindow.Object, this.mockBatcher.Object);
}
