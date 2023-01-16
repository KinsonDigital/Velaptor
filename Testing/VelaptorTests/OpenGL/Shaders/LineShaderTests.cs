// <copyright file="LineShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using System.Linq;
using Carbonate.Core;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Factories;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;
using Velaptor.ReactableData;
using Xunit;

public class LineShaderTests
{
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchSizeUnsubscriber;
    private IReceiveReactor<BatchSizeData>? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineShaderTests"/> class.
    /// </summary>
    public LineShaderTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();
        this.mockShaderLoader = new Mock<IShaderLoaderService<uint>>();

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
        this.mockReactableFactory.Setup(m => m.CreateNoDataReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable()).Returns(mockBatchSizeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineShader(
                new Mock<IGLInvoker>().Object,
                new Mock<IOpenGLService>().Object,
                new Mock<IShaderLoaderService<uint>>().Object,
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
        var customAttributes = Attribute.GetCustomAttributes(typeof(LineShader));
        var containsAttribute = customAttributes.Any(i => i is ShaderNameAttribute);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        containsAttribute
            .Should()
            .BeTrue($"the '{nameof(ShaderNameAttribute)}' is required on a shader implementation to set the shader name.");
        sut.Name.Should().Be("Line");
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
    public void BatchSizeReactable_WhenNotificationHasAnIssue_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(LineShader)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{PushNotifications.BatchSizeSetId}'.";

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => this.batchSizeReactor.OnReceive(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expectedMsg);
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingBatchSizeNotification_SetsBatchSize()
    {
        // Arrange
        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 123u });

        var shader = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(mockMessage.Object);
        var actual = shader.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineShader CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockGLService.Object,
            this.mockShaderLoader.Object,
            this.mockReactableFactory.Object);
}
