// <copyright file="WindowTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI;

using System;
using System.Numerics;
using System.Threading.Tasks;
using Fakes;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Content;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="Window"/> class.
/// </summary>
public class WindowTests
{
    private readonly Mock<IWindow> mockWindow;
    private readonly Mock<IContentLoader> mockContentLoader;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowTests"/> class.
    /// </summary>
    public WindowTests()
    {
        this.mockContentLoader = new Mock<IContentLoader>();

        this.mockWindow = new Mock<IWindow>();
        this.mockWindow.SetupGet(p => p.ContentLoader).Returns(this.mockContentLoader.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWindowParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new WindowFake(null);
        }, "The parameter must not be null. (Parameter 'window')");
    }
    #endregion

    #region Prop Tests
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
    public void ContentLoader_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.ContentLoader = this.mockContentLoader.Object;
        _ = sut.ContentLoader;

        // Assert
        this.mockWindow.VerifySet(p => p.ContentLoader = this.mockContentLoader.Object, Times.Once());
        this.mockWindow.VerifyGet(p => p.ContentLoader, Times.Once());
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
        Assert.True(actual);
    }
    #endregion

    #region Method tests
    [Fact]
    public void Ctor_WhenUsingOverloadWithWindowAndLoaderWithNullWindow_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new WindowFake(null);
        }, "The parameter must not be null. (Parameter 'window')");
    }

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
    public async void ShowAsync_WhenInvoked_ShowsInternalWindow()
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
    private WindowFake CreateSystemUnderTest() => new (this.mockWindow.Object);
}
