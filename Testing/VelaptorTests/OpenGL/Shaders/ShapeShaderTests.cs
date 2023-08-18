// <copyright file="ShapeShaderTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="ShapeShader"/> class.
/// </summary>
public class ShapeShaderTests
{
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderLoaderService> mockShaderLoader;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchSizeUnsubscriber;
    private IReceiveReactor<BatchSizeData>? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeShaderTests"/> class.
    /// </summary>
    public ShapeShaderTests()
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
            _ = new ShapeShader(
                new Mock<IGLInvoker>().Object,
                new Mock<IOpenGLService>().Object,
                new Mock<IShaderLoaderService>().Object,
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
        var customAttributes = Attribute.GetCustomAttributes(typeof(ShapeShader));
        var containsAttribute = customAttributes.Any(i => i is ShaderNameAttribute);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        containsAttribute
            .Should()
            .BeTrue($"the '{nameof(ShaderNameAttribute)}' is required on a shader implementation to set the shader name.");
        sut.Name.Should().Be("Shape");
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void BatchSizeReactable_WhenReceivingBatchSizeNotification_SetsBatchSize()
    {
        // Arrange
        var batchSizeData = new BatchSizeData { BatchSize = 123, TypeOfBatch = BatchType.Rect };

        var shader = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(batchSizeData);
        var actual = shader.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }

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
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ShapeShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ShapeShader CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockGLService.Object,
            this.mockShaderLoader.Object,
            this.mockReactableFactory.Object);
}
