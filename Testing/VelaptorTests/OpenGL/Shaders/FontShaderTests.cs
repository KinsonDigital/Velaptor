// <copyright file="FontShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using System.Linq;
using Carbonate;
using FluentAssertions;
using Moq;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Helpers;
using Velaptor;
using Velaptor.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="FontShader"/> class.
/// </summary>
public class FontShaderTests
{
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
    private readonly Mock<IReactable<GLInitData>> mockGLInitReactable;
    private readonly Mock<IDisposable> mockGLInitUnsubscriber;
    private readonly Mock<IReactable> mockReactable;
    private readonly Mock<IReactable<ShutDownData>> mockShutDownReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontShaderTests"/> class.
    /// </summary>
    public FontShaderTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();
        this.mockShaderLoader = new Mock<IShaderLoaderService<uint>>();
        this.mockShutDownReactable = new Mock<IReactable<ShutDownData>>();
        this.mockGLInitReactable = new Mock<IReactable<GLInitData>>();
        this.mockReactable = new Mock<IReactable>();
        this.mockGLInitUnsubscriber = new Mock<IDisposable>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
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
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
    }

    [Fact]
    public void Ctor_WhenReceivingReactableNotification_SetsBatchSize()
    {
        // Arrange
        IReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                if (reactorObj is null)
                {
                    Assert.True(false, "Batch size reactor object cannot be null for test.");
                }

                reactor = reactorObj;
            });

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 123u });

        var shader = CreateSystemUnderTest();

        // Act
        reactor.OnNext(mockMessage.Object);
        var actual = shader.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }

    [Fact]
    public void Ctor_WhenReactableUnsubscribes_InvokesUnsubscriber()
    {
        // Arrange
        IReactor? reactor = null;
        var mockUnsubscriber = new Mock<IDisposable>();

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");

                reactor = reactorObj;
            })
            .Returns<IReactor>(_ => mockUnsubscriber.Object);

        _ = CreateSystemUnderTest();

        // Act
        reactor.OnComplete();
        reactor.OnComplete();

        // Assert
        mockUnsubscriber.VerifyOnce(m => m.Dispose());
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsNameProp()
    {
        // Arrange
        var customAttributes = Attribute.GetCustomAttributes(typeof(RectangleShader));
        var containsAttribute = customAttributes.Any(i => i is ShaderNameAttribute);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        containsAttribute
            .Should()
            .BeTrue($"the '{nameof(ShaderNameAttribute)}' is required on a shader implementation to set the shader name.");
        sut.Name.Should().Be("Font");
    }

    [Fact]
    public void Ctor_WhenBatchSizeNotificationHasAnIssue_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(FontShader)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{NotificationIds.BatchSizeId}'.";

        IReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");

                reactor = reactorObj;
            });

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => reactor.OnNext(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expectedMsg);
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
        this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "fontTexture"))
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

        glInitReactor?.OnNext(default);

        // Act
        shader.Use();

        // Assert
        this.mockGL.Verify(m => m.ActiveTexture(GLTextureUnit.Texture1), Times.Once);
        this.mockGL.Verify(m => m.Uniform1(uniformLocation, 1), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontShader CreateSystemUnderTest()
        => new (this.mockGL.Object,
            this.mockGLService.Object,
            this.mockShaderLoader.Object,
            this.mockGLInitReactable.Object,
            this.mockReactable.Object,
            this.mockShutDownReactable.Object);
}
