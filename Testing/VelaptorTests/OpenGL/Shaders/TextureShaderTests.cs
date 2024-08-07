// <copyright file="TextureShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureShader"/> class.
/// </summary>
public class TextureShaderTests
{
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderLoaderService> mockShaderLoader;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IDisposable> batchSizeUnsubscriber;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureShaderTests"/> class.
    /// </summary>
    public TextureShaderTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();
        this.mockShaderLoader = new Mock<IShaderLoaderService>();

        this.batchSizeUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Returns<IReceiveSubscription>(_ => new Mock<IDisposable>().Object)
            .Callback<IReceiveSubscription>(reactor =>
            {
                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
            });

        this.mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<BatchSizeData>>()))
            .Returns(() => this.batchSizeUnsubscriber.Object)
            .Callback<IReceiveSubscription<BatchSizeData>>(reactor => this.batchSizeReactor = reactor);

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable()).Returns(this.mockBatchSizeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureShader(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockShaderLoader.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsNameProp()
    {
        // Arrange
        var customAttributes = Attribute.GetCustomAttributes(typeof(TextureShader));
        var containsAttribute = Array.Exists(customAttributes, i => i is ShaderNameAttribute);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        containsAttribute
            .Should()
            .BeTrue($"the '{nameof(ShaderNameAttribute)}' is required on a shader implementation to set the shader name.");
        sut.Name.Should().Be("Texture");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Use_WhenInvoked_SetsShaderAsUsed()
    {
        // Arrange
        const uint shaderId = 78;
        const int uniformLocation = 1234;
        const int status = 1;

        this.mockGL.Setup(m => m.CreateProgram()).Returns(shaderId);
        this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "mainTexture")).Returns(uniformLocation);
        this.mockGL.Setup(m => m.GetProgram(shaderId, GLProgramParameterName.LinkStatus)).Returns(status);

        var shader = CreateSystemUnderTest();

        this.glInitReactor?.OnReceive();

        // Act
        shader.Use();

        // Assert
        this.mockGL.VerifyOnce(m => m.ActiveTexture(GLTextureUnit.Texture0));
        this.mockGL.VerifyOnce(m => m.Uniform1(uniformLocation, 0));
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void BatchSizeReactable_WhenReceivingReactableNotification_SetsBatchSize()
    {
        // Arrange
        var batchSizeData = new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Texture };

        var shader = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(batchSizeData);
        var actual = shader.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }

    [Fact]
    public void BatchSizeReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<BatchSizeData>>()))
            .Callback<IReceiveSubscription<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("It is required for unit testing.");
                this.batchSizeReactor = reactor;
                reactor.Name.Should().Be($"TextureShader.ctor() - {PushNotifications.BatchSizeChangedId}");
            });

        _ = CreateSystemUnderTest();
    }

    [Fact]
    public void BatchSizeReactable_WhenUnsubscribingGlInit_Unsubscribes()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnUnsubscribe();

        // Assert
        this.batchSizeUnsubscriber.Verify(m => m.Dispose(), Times.Once);
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
            this.mockReactableFactory.Object);
}
