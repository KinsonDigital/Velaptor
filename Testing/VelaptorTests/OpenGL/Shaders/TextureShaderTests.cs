// <copyright file="TextureShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureShader"/> class.
/// </summary>
public class TextureShaderTests
{
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
    private readonly Mock<IReactable<GLInitData>> mockGLInitReactable;
    private readonly Mock<IReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IReactable<ShutDownData>> mockShutDownReactable;
    private readonly Mock<IDisposable> mockGLInitUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureShaderTests"/> class.
    /// </summary>
    public TextureShaderTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();
        this.mockShaderLoader = new Mock<IShaderLoaderService<uint>>();
        this.mockGLInitReactable = new Mock<IReactable<GLInitData>>();
        this.mockBatchSizeReactable = new Mock<IReactable<BatchSizeData>>();
        this.mockShutDownReactable = new Mock<IReactable<ShutDownData>>();
        this.mockGLInitUnsubscriber = new Mock<IDisposable>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullBatchSizeReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontShader(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockShaderLoader.Object,
                this.mockGLInitReactable.Object,
                null,
                this.mockShutDownReactable.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'batchSizeReactable')");
    }

    [Fact]
    public void Ctor_WhenReceivingBatchSizeNotification_SetsBatchSize()
    {
        // Arrange
        IReactor<BatchSizeData>? reactor = null;

        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<BatchSizeData>>()))
            .Callback<IReactor<BatchSizeData>>(reactorObj =>
            {
                if (reactorObj is null)
                {
                    Assert.True(false, "Batch size reactor object cannot be null for test.");
                }

                reactor = reactorObj;
            });

        var shader = CreateSystemUnderTest();

        // Act
        reactor.OnNext(new BatchSizeData(123u));
        var actual = shader.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }

    [Fact]
    public void Ctor_WhenEndingNotifications_Unsubscribes()
    {
        // Arrange
        IReactor<BatchSizeData>? reactor = null;
        var mockUnsubscriber = new Mock<IDisposable>();

        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<BatchSizeData>>()))
            .Callback<IReactor<BatchSizeData>>(reactorObj =>
            {
                if (reactorObj is null)
                {
                    Assert.True(false, "Batch size reactor object cannot be null for test.");
                }

                reactor = reactorObj;
            })
            .Returns<IReactor<BatchSizeData>>(_ => mockUnsubscriber.Object);

        _ = CreateSystemUnderTest();

        // Act
        reactor.OnCompleted();
        reactor.OnCompleted();

        // Assert
        mockUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Use_WhenInvoked_SetsShaderAsUsed()
    {
        // Arrange
        IReactor<GLInitData>? glInitReactor = null;
        const uint shaderId = 78;
        const int uniformLocation = 1234;
        this.mockGL.Setup(m => m.CreateProgram()).Returns(shaderId);
        this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "mainTexture"))
            .Returns(uniformLocation);
        const int status = 1;
        this.mockGL.Setup(m
                => m.GetProgram(shaderId, GLProgramParameterName.LinkStatus))
            .Returns(status);

        this.mockGLInitReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<GLInitData>>()))
            .Returns(this.mockGLInitUnsubscriber.Object)
            .Callback<IReactor<GLInitData>>(reactor =>
            {
                if (reactor is null)
                {
                    Assert.True(false, "GL initialization reactable subscription failed.  Reactor is null.");
                }

                glInitReactor = reactor;
            });

        var shader = CreateSystemUnderTest();

        glInitReactor.OnNext(default);

        // Act
        shader.Use();

        // Assert
        this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture0), Times.Once);
        this.mockGL.Verify(m => m.Uniform1(uniformLocation, 0), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureShader CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockGLService.Object,
            this.mockShaderLoader.Object,
            this.mockGLInitReactable.Object,
            this.mockBatchSizeReactable.Object,
            this.mockShutDownReactable.Object);
}
