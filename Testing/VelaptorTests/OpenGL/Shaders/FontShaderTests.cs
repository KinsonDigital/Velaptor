// <copyright file="FontShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using System.Linq;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="FontShader"/> class.
/// </summary>
public class FontShaderTests
{
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderLoaderService> mockShaderLoader;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchSizeUnsubscriber;
    private IReceiveReactor? glInitReactor;
    private IReceiveReactor<BatchSizeData>? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontShaderTests"/> class.
    /// </summary>
    public FontShaderTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();
        this.mockShaderLoader = new Mock<IShaderLoaderService>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Returns<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId || reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return new Mock<IDisposable>().Object;
                }

                Assert.Fail("Unrecognized event id.");
                return null;
            })
            .Callback<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
            });

        this.mockBatchSizeUnsubscriber = new Mock<IDisposable>();

        var mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Returns<IReceiveReactor<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                return this.mockBatchSizeUnsubscriber.Object;
            })
            .Callback<IReceiveReactor<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.batchSizeReactor = reactor;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable()).Returns(mockBatchSizeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontShader(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockShaderLoader.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsNameProp()
    {
        // Arrange
        var customAttributes = Attribute.GetCustomAttributes(typeof(FontShader));
        var containsAttribute = customAttributes.Any(i => i is ShaderNameAttribute);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        containsAttribute
            .Should()
            .BeTrue($"the '{nameof(ShaderNameAttribute)}' is required on a shader implementation to set the shader name.");
        sut.Name.Should().Be("Font");
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
        this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "fontTexture")).Returns(uniformLocation);
        this.mockGL.Setup(m => m.GetProgram(shaderId, GLProgramParameterName.LinkStatus)).Returns(status);

        var shader = CreateSystemUnderTest();

        this.glInitReactor?.OnReceive();

        // Act
        shader.Use();

        // Assert
        this.mockGL.VerifyOnce(m => m.ActiveTexture(GLTextureUnit.Texture1));
        this.mockGL.VerifyOnce(m => m.Uniform1(uniformLocation, 1));
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void BatchSizeReactable_WithUnsubscribeNotification_UnsubscribesFromReactable()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnUnsubscribe();
        this.batchSizeReactor.OnUnsubscribe();

        // Assert
        this.mockBatchSizeUnsubscriber.VerifyOnce(m => m.Dispose());
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingReactableNotification_SetsBatchSize()
    {
        // Arrange
        var batchSizeData = new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Font };

        var shader = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(batchSizeData);
        var actual = shader.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontShader CreateSystemUnderTest() =>
        new FontShader(this.mockGL.Object,
            this.mockGLService.Object,
            this.mockShaderLoader.Object,
            this.mockReactableFactory.Object);
}
