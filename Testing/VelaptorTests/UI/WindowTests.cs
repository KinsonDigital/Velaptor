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
using NSubstitute;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Scene;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="Window"/> class.
/// </summary>
public class WindowTests : TestsBase
{
    private readonly IWindow mockWindow;
    private readonly ISceneManager mockSceneManager;
    private readonly IBatcher mockBatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowTests"/> class.
    /// </summary>
    public WindowTests()
    {
        this.mockSceneManager = Substitute.For<ISceneManager>();
        this.mockBatcher = Substitute.For<IBatcher>();

        this.mockWindow = Substitute.For<IWindow>();
        this.mockWindow.SceneManager.Returns(this.mockSceneManager);
    }

    #region Constructor Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullWindowParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new WindowFake(null, this.mockBatcher);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'window')");
    }

    [Fact]
    [Trait("Category", Ctor)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    public void Title_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Title = "test-title";
        _ = sut.Title;

        // Assert
        this.mockWindow.Received(1).Title = "test-title";
        _ = this.mockWindow.Received(1).Title;
    }

    [Fact]
    [Trait("Category", Prop)]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Position = new Vector2(11, 22);
        _ = sut.Position;

        // Assert
        this.mockWindow.Received(1).Position = new Vector2(11, 22);
        _ = this.mockWindow.Received(1).Position;
    }

    [Fact]
    [Trait("Category", Prop)]
    public void Width_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Width = 1234;
        _ = sut.Width;

        // Assert
        this.mockWindow.Received(1).Width = 1234;
        _ = this.mockWindow.Received(1).Width;
    }

    [Fact]
    [Trait("Category", Prop)]
    public void Height_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Height = 1234;

        // Act
        _ = sut.Height;

        // Assert
        this.mockWindow.Received(1).Height = 1234;
        this.mockWindow.Received(1).Height = 1234;
    }

    [Fact]
    [Trait("Category", Prop)]
    public void AutoClearBuffer_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.AutoClearBuffer = true;
        _ = sut.AutoClearBuffer;

        // Assert
        this.mockWindow.Received(1).AutoClearBuffer = true;
        _ = this.mockWindow.Received(1).AutoClearBuffer;
    }

    [Fact]
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
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
    [Trait("Category", Prop)]
    public void MouseCursorVisible_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.MouseCursorVisible = true;
        _ = sut.MouseCursorVisible;

        // Assert
        this.mockWindow.Received(1).MouseCursorVisible = true;
        _ = this.mockWindow.Received(1).MouseCursorVisible;
    }

    [Fact]
    [Trait("Category", Prop)]
    public void UpdateFrequency_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.UpdateFrequency = 1234;
        _ = sut.UpdateFrequency;

        // Assert
        this.mockWindow.Received(1).UpdateFrequency = 1234;
        _ = this.mockWindow.Received(1).UpdateFrequency;
    }

    [Fact]
    [Trait("Category", Prop)]
    public void WindowState_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.WindowState = StateOfWindow.FullScreen;
        _ = sut.WindowState;

        // Assert
        this.mockWindow.Received(1).WindowState = StateOfWindow.FullScreen;
        _ = this.mockWindow.Received(1).WindowState;
    }

    [Fact]
    [Trait("Category", Prop)]
    public void TypeOfBorder_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.TypeOfBorder = WindowBorder.Resizable;
        _ = sut.TypeOfBorder;

        // Assert
        this.mockWindow.Received(1).TypeOfBorder = WindowBorder.Resizable;
        _ = this.mockWindow.Received(1).TypeOfBorder;
    }

    [Fact]
    [Trait("Category", Prop)]
    public void SceneManager_WhenGettingValue_IsExpectedObject()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.SceneManager.Should().BeSameAs(this.mockSceneManager);
    }

    [Fact]
    [Trait("Category", Prop)]
    public void Fps_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockWindow.Fps.Returns(123);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Fps;

        // Assert
        actual.Should().Be(123);
    }

    [Fact]
    [Trait("Category", Prop)]
    public void Initialized_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockWindow.Initialized.Returns(true);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Initialized;

        // Assert
        actual.Should().BeTrue();
    }
    #endregion

    #region Method tests
    [Fact]
    [Trait("Category", Method)]
    public void Show_WhenInvoked_ShowsWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Show();

        // Assert
        this.mockWindow.Received(1).Show();
    }

    [Fact]
    [Trait("Category", Method)]
    public void Close_WhenInvoked_ClosesWindow()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Close();

        // Assert
        this.mockWindow.Received(1).Close();
    }

    [Fact]
    [Trait("Category", Method)]
    public async Task ShowAsync_WhenInvoked_ShowsInternalWindow()
    {
        // Arrange
        this.mockWindow.ShowAsync().Returns(Task.Run(() => { }));
        var sut = CreateSystemUnderTest();

        // Act
        await sut.ShowAsync();

        // Assert
        await this.mockWindow.Received(1).ShowAsync();
    }

    [Fact]
    [Trait("Category", Method)]
    public void OnDraw_WhenAutoRenderingIsEnabled_RenderScenesAndManipulatesBatch()
    {
        // Arrange
        this.mockSceneManager.TotalScenes.Returns(1);

        var sut = CreateSystemUnderTest();
        sut.AutoSceneRendering = true;

        // Act
        sut.OnDraw(default);

        // Assert
        this.mockBatcher.Received(1).Begin();
        this.mockSceneManager.Received(1).Render();
        this.mockBatcher.Received(1).End();
    }

    [Fact]
    [Trait("Category", Method)]
    public void OnDraw_WhenAutoRenderingIsDisabled_DoesNotRenderScenes()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoSceneRendering = false;

        // Act
        sut.OnDraw(default);

        // Assert
        this.mockBatcher.DidNotReceive().Clear();
        this.mockBatcher.DidNotReceive().Begin();
        this.mockSceneManager.DidNotReceive().Render();
        this.mockBatcher.DidNotReceive().End();
    }

    [Fact]
    [Trait("Category", Method)]
    public void OnDraw_WhenAutoRenderingIsNotDisabledWithNoScenes_ShouldNotRenderScenesOrManipulateBatches()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoSceneRendering = true;

        // Act
        sut.OnDraw(default);

        // Assert
        this.mockBatcher.DidNotReceive().Clear();
        this.mockBatcher.DidNotReceive().Begin();
        this.mockSceneManager.DidNotReceive().Render();
        this.mockBatcher.DidNotReceive().End();
    }

    [Fact]
    [Trait("Category", Method)]
    public void OnUnload_WithAutoSceneUnloadingDisabled_DoesNotInvokeManagerUnloadContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoSceneUnloading = false;

        // Act
        sut.OnUnload();

        // Assert
        this.mockSceneManager.DidNotReceive().UnloadContent();
    }

    [Fact]
    [Trait("Category", Method)]
    public void OnUnload_WithAutoSceneUnloadingEnabled_InvokesManagerUnloadContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoSceneUnloading = true;

        // Act
        sut.OnUnload();

        // Assert
        this.mockSceneManager.Received(1).UnloadContent();
    }

    [Fact]
    [Trait("Category", Method)]
    [SuppressMessage("csharpsquid", "S3966", Justification = "Disposing twice is required for testing.")]
    public void Dispose_WhenInvoked_DisposesOfMangedResources()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockWindow.Received(1).Dispose();
    }
    #endregion

    /// <summary>
    /// Creates an instance of <see cref="WindowFake"/> for the purpose
    /// of testing the abstract <see cref="Window"/> class.
    /// </summary>
    /// <returns>The instance used for testing.</returns>
    private WindowFake CreateSystemUnderTest() => new (this.mockWindow, this.mockBatcher);
}
