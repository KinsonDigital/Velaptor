// <copyright file="FontShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using System.Linq;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using NSubstitute;
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
/// Tests the <see cref="FontShader"/> class.
/// </summary>
public class FontShaderTests
{
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IShaderLoaderService mockShaderLoader;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable<BatchSizeData> mockBatchSizeReactable;
    private readonly IDisposable batchSizeUnsubscriber;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontShaderTests"/> class.
    /// </summary>
    public FontShaderTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGLService = Substitute.For<IOpenGLService>();
        this.mockShaderLoader = Substitute.For<IShaderLoaderService>();

        this.batchSizeUnsubscriber = Substitute.For<IDisposable>();

        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable.Subscribe(Arg.Any<IReceiveSubscription>()).Returns(_ => Substitute.For<IDisposable>());

        mockPushReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription>();
                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
            });

        this.mockBatchSizeReactable = Substitute.For<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Subscribe(Arg.Any<IReceiveSubscription<BatchSizeData>>())
            .Returns(_ => this.batchSizeUnsubscriber);
        this.mockBatchSizeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<BatchSizeData>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription<BatchSizeData>>();
                reactor.Should().NotBeNull("It is required for unit testing.");
                this.batchSizeReactor = reactor;
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockPushReactable);
        this.mockReactableFactory.CreateBatchSizeReactable().Returns(this.mockBatchSizeReactable);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new FontShader(
                this.mockGL,
                this.mockGLService,
                this.mockShaderLoader,
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

        this.mockGL.CreateProgram().Returns(shaderId);
        this.mockGL.GetUniformLocation(shaderId, "fontTexture").Returns(uniformLocation);
        this.mockGL.GetProgram(shaderId, GLProgramParameterName.LinkStatus).Returns(status);

        var shader = CreateSystemUnderTest();

        this.glInitReactor?.OnReceive();

        // Act
        shader.Use();

        // Assert
        this.mockGL.Received(1).ActiveTexture(GLTextureUnit.Texture1);
        this.mockGL.Received(1).Uniform1(uniformLocation, 1);
    }
    #endregion

    #region Indirect Tests
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

    [Fact]
    public void BatchSizeReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        this.mockBatchSizeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<BatchSizeData>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription<BatchSizeData>>();
                reactor.Should().NotBeNull("It is required for unit testing.");
                this.batchSizeReactor = reactor;
                reactor.Name.Should().Be($"FontShader.ctor() - {PushNotifications.BatchSizeChangedId}");
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
        this.batchSizeUnsubscriber.Received(1).Dispose();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontShader CreateSystemUnderTest() =>
        new (this.mockGL,
            this.mockGLService,
            this.mockShaderLoader,
            this.mockReactableFactory);
}
