// <copyright file="ImGuiFacadeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.ImGui;

using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using Shouldly;
using Velaptor.NativeInterop.ImGui;
using Velaptor.NativeInterop.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="ImGuiFacade"/> class.
/// </summary>
public class ImGuiFacadeTests
{
    private readonly IImGuiManager mockImGuiManager;
    private readonly IImGuiService mockImGuiService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImGuiFacadeTests"/> class.
    /// </summary>
    public ImGuiFacadeTests()
    {
        this.mockImGuiManager = Substitute.For<IImGuiManager>();
        this.mockImGuiService = Substitute.For<IImGuiService>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullImGuiMangerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImGuiFacade(null, this.mockImGuiService);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'imGuiManager')");
    }

    [Fact]
    public void Ctor_WithNullImGuiServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ImGuiFacade(this.mockImGuiManager, null);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'imGuiService')");
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
    public void Update_WhenInvokedForTheFirstTime_BuildsFontAtlasWithDummyTextureId()
    {
        // Arrange
        this.mockImGuiService.GetTexDataAsRGBA32().Returns((new byte[] { 10, 20, 30 }, 100, 200));
        var sut = CreateSystemUnderTest();

        // Act
        sut.Update(0.5);

        // Assert
        this.mockImGuiService.Received(1).GetTexDataAsRGBA32();
        this.mockImGuiService.Received(1).SetTexID(1u);  // dummy ID so IsBuilt() passes
        this.mockImGuiService.Received(1).ClearTexData();
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
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe(expected);
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
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe(expected);
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
    /// Creates a new instance of <see cref="ImGuiFacade"/> for testing.
    /// </summary>
    private ImGuiFacade CreateSystemUnderTest()
        => new (this.mockImGuiManager, this.mockImGuiService);
}
