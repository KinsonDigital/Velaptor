// <copyright file="FontShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using System.Linq;
using Carbonate;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
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
    private readonly Mock<IShaderLoaderService<uint>> mockShaderLoader;
    private readonly Mock<IReactable> mockReactable;
    private IReactor? glInitReactor;
    private IReactor? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontShaderTests"/> class.
    /// </summary>
    public FontShaderTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();
        this.mockShaderLoader = new Mock<IShaderLoaderService<uint>>();

        this.mockReactable = new Mock<IReactable>();
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.EventId == NotificationIds.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }

                if (reactor.EventId == NotificationIds.BatchSizeSetId)
                {
                    this.batchSizeReactor = reactor;
                }
            });
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
                null);
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
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 123u });

        var shader = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnNext(mockMessage.Object);
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
        expectedMsg += $" for subscription ID '{NotificationIds.BatchSizeSetId}'.";

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => this.batchSizeReactor.OnNext(mockMessage.Object);

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
        const uint shaderId = 78;
        const int uniformLocation = 1234;
        this.mockGL.Setup(m => m.CreateProgram()).Returns(shaderId);
        this.mockGL.Setup(m => m.GetUniformLocation(shaderId, "fontTexture"))
            .Returns(uniformLocation);
        const int status = 1;
        this.mockGL.Setup(m
                => m.GetProgram(shaderId, GLProgramParameterName.LinkStatus))
            .Returns(status);

        var shader = CreateSystemUnderTest();

        this.glInitReactor?.OnNext();

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
            this.mockReactable.Object);
}
