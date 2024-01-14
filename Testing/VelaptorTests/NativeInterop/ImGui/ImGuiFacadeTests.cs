// <copyright file="ImGuiFacadeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.ImGui;

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using Velaptor.NativeInterop.ImGui;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="ImGuiFacade"/> class.
/// </summary>
public class ImGuiFacadeTests
{
    private readonly IGLInvoker mockGlInvoker;
    private readonly IImGuiManager mockImGuiManager;
    private readonly IImGuiService mockImGuiService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImGuiFacadeTests"/> class.
    /// </summary>
    public ImGuiFacadeTests()
    {
        this.mockGlInvoker = Substitute.For<IGLInvoker>();
        this.mockImGuiManager = Substitute.For<IImGuiManager>();
        this.mockImGuiService = Substitute.For<IImGuiService>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGlInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImGuiFacade(
                null,
                this.mockImGuiManager,
                this.mockImGuiService);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'glInvoker')");
    }

    [Fact]
    public void Ctor_WithNullImGuiMangerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImGuiFacade(
                this.mockGlInvoker,
                null,
                this.mockImGuiService);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'imGuiManager')");
    }

    [Fact]
    public void Ctor_WithNullImGuiServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImGuiFacade(
                this.mockGlInvoker,
                this.mockImGuiManager,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'imGuiService')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Update_WhenInvoked_SetsUpImGuiAndUpdatesImGui()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Update(0.5);
        sut.Update(0.5);

        // Assert
        this.mockImGuiService.Received(1).ClearFonts();
        this.mockImGuiService.Received(1).AddEmbeddedFont(22);
        this.mockImGuiService.Received(1).DisableIniFile();
    }

    [Fact]
    public void Update_WhenInvokedForTheFirstTime_RebuildsFontAtlas()
    {
        // Arrange
        const uint textureId = 123;
        var expectedPixelData = new byte[] { 10, 20, 30 };
        const int expectedWidth = 100;
        const int expectedHeight = 200;
        this.mockGlInvoker.GenTexture().Returns(_ => textureId);
        this.mockImGuiService.GetTexDataAsRGBA32().Returns((expectedPixelData, expectedWidth, expectedHeight));
        var sut = CreateSystemUnderTest();

        // Act
        sut.Update(0.5);

        // Assert
        // Assert that the texture data was retrieved
        this.mockImGuiService.Received(1).GetTexDataAsRGBA32();

        // Assert that the texture data was uploaded to the GPU
        this.mockGlInvoker.Received(1).GenTexture();
        this.mockGlInvoker.Received(1).PixelStore(GLPixelStoreParameter.UnpackAlignment, 1);
        this.mockGlInvoker.Received(1).BindTexture(GLTextureTarget.Texture2D, textureId);
        this.mockGlInvoker.Received(1).TexImage2D<byte>(
            GLTextureTarget.Texture2D,
            0,
            GLInternalFormat.Rgba,
            expectedWidth,
            expectedHeight,
            0,
            GLPixelFormat.Rgba,
            GLPixelType.UnsignedByte,
            expectedPixelData);
        this.mockGlInvoker.Received(1).TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureMinFilter,
            GLTextureMinFilter.Linear);
        this.mockGlInvoker.Received(1).TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureMagFilter,
            GLTextureMagFilter.Linear);
        this.mockGlInvoker.Received(1).BindTexture(GLTextureTarget.Texture2D, 0);

        // Assert that the texture ID has been set
        this.mockImGuiService.Received(1).SetTexID(textureId);

        // Assert that the temporary texture data was cleared
        this.mockImGuiService.ClearTexData();
    }

    [Fact]
    public void Render_WhenUpdateMethodHasNotBeenInvokedFirst_ThrowsException()
    {
        // Arrange
        const string updateMethod = $"{nameof(ImGuiFacade)}.{nameof(ImGuiFacade.Update)}";
        const string renderMethod = $"{nameof(ImGuiFacade)}.{nameof(ImGuiFacade.Render)}";
        const string expected = $"The '{updateMethod}' method must be invoked before the '{renderMethod}' method.";

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render();

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Render_WhenUpdateMethodHasBeenInvokedFirstTimeButNotSecondTime_ThrowsException()
    {
        // Arrange
        const string updateMethod = $"{nameof(ImGuiFacade)}.{nameof(ImGuiFacade.Update)}";
        const string renderMethod = $"{nameof(ImGuiFacade)}.{nameof(ImGuiFacade.Render)}";
        const string expected = $"The '{updateMethod}' method must be invoked before the '{renderMethod}' method.";

        var sut = CreateSystemUnderTest();
        sut.Update(0.5);
        sut.Render();

        // Act
        var act = () => sut.Render();

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Render_WhenInvokedAfterUpdateMethodIsInvoked_PerformsImGuiRender()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Update(0.5);

        // Act
        sut.Render();

        // Assert
        this.mockImGuiManager.Received(1).Render();
    }

    [Fact]
    [SuppressMessage("csharpsquid", "S3966", Justification = "Disposing twice is required for testing.")]
    public void Dispose_WhenInvoked_DisposesOfResources()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockImGuiManager.Received(1).Dispose();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ImGuiFacade"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ImGuiFacade CreateSystemUnderTest()
        => new (this.mockGlInvoker, this.mockImGuiManager, this.mockImGuiService);
}
