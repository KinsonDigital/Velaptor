// <copyright file="ShaderProgramTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Fakes;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Exceptions;
using Velaptor.OpenGL.Services;
using Xunit;

/// <summary>
/// Initializes a new instance of <see cref="ShaderProgramTests"/>.
/// </summary>
public class ShaderProgramTests
{
    private const string VertShaderSrc = "vert-sut-src";
    private const string FragShaderSrc = "frag-sut-src";
    private const string ShaderName = "UNKNOWN";
    private const uint VertexShaderId = 1234u;
    private const uint FragShaderId = 5678u;
    private const uint ShaderProgramId = 1928u;
    private readonly Mock<IShaderLoaderService> mockShaderLoader;
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockGLInitUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderProgramTests"/> class.
    /// </summary>
    public ShaderProgramTests()
    {
        this.mockShaderLoader = new Mock<IShaderLoaderService>();

        // Sets up the vertex sut file mock.
        this.mockShaderLoader.Setup(m
                => m.LoadVertSource(ShaderName))
            .Returns(() => VertShaderSrc);

        // Sets up the fragment sut file mock.
        this.mockShaderLoader.Setup(m
                => m.LoadFragSource(ShaderName))
            .Returns(() => FragShaderSrc);

        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();

        const int getShaderStatusCode = 1;
        const int getProgramStatusCode = 1;
        this.mockGL.Setup(m => m.CreateShader(GLShaderType.VertexShader)).Returns(VertexShaderId);
        this.mockGL.Setup(m => m.CreateShader(GLShaderType.FragmentShader)).Returns(FragShaderId);
        this.mockGL.Setup(m => m.GetShader(It.IsAny<uint>(), GLShaderParameter.CompileStatus))
            .Returns(getShaderStatusCode);
        this.mockGL.Setup(m => m.GetProgram(It.IsAny<uint>(), GLProgramParameterName.LinkStatus))
            .Returns(getProgramStatusCode);
        this.mockGL.Setup(m => m.CreateProgram()).Returns(ShaderProgramId);

        this.mockGLInitUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Returns<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    return this.mockGLInitUnsubscriber.Object;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                return null;
            })
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    this.glInitReactor = reactor;
                }
                else if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    this.shutDownReactor = reactor;
                }
                else
                {
                    Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                }
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ShaderProgramFake(
                null,
                this.mockGLService.Object,
                this.mockShaderLoader.Object,
                this.mockReactableFactory.Object);
        }, "The parameter must not be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ShaderProgramFake(
                this.mockGL.Object,
                null,
                this.mockShaderLoader.Object,
                this.mockReactableFactory.Object);
        }, "The parameter must not be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullShaderLoaderServiceParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ShaderProgramFake(
                this.mockGL.Object,
                this.mockGLService.Object,
                null,
                this.mockReactableFactory.Object);
        }, "The parameter must not be null. (Parameter 'shaderLoaderService')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ShaderProgramFake(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockShaderLoader.Object,
                null);
        }, "The parameter must not be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Name_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Name;

        // Assert
        actual.Should().Be("UNKNOWN");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ReactorInit_WhenInvoked_LoadsShaderSourceCode()
    {
        // Arrange
        CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockShaderLoader.Verify(m
            => m.LoadVertSource(ShaderName), Times.Once);
        this.mockShaderLoader.Verify(m
            => m.LoadFragSource(ShaderName), Times.Once);
    }

    [Fact]
    public void ReactorInit_WhenInvokedSecondTime_DoesNotCreateShaderProgram()
    {
        // Arrange
        CreateSystemUnderTest();
        this.glInitReactor.OnReceive();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockShaderLoader.Verify(m
            => m.LoadVertSource(It.IsAny<string>()), Times.Once);
        this.mockShaderLoader.Verify(m
            => m.LoadFragSource(It.IsAny<string>()), Times.Once);
        this.mockGL.Verify(m => m.CreateShader(It.IsAny<GLShaderType>()), Times.Exactly(2));
        this.mockGL.Verify(m => m.CreateProgram(), Times.Once());
        this.mockGL.Verify(m => m.AttachShader(It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
        this.mockGL.Verify(m => m.LinkProgram(It.IsAny<uint>()), Times.Once());
        this.mockGL.Verify(m => m.DetachShader(It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
        this.mockGL.Verify(m => m.DeleteShader(It.IsAny<uint>()), Times.Exactly(2));
    }

    [Fact]
    public void ReactorInit_WhenInvoked_SuccessfullyCreatesVertexShader()
    {
        // Arrange
        CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        // Verify the creation of the vertex sut
        this.mockGL.Verify(m => m.CreateShader(GLShaderType.VertexShader), Times.Once());
        this.mockGL.Verify(m => m.ShaderSource(VertexShaderId, VertShaderSrc), Times.Once());
        this.mockGL.Verify(m => m.CompileShader(VertexShaderId), Times.Once());

        // Verify the creation of the fragment sut
        this.mockGL.Verify(m => m.CreateShader(GLShaderType.FragmentShader), Times.Once());
        this.mockGL.Verify(m => m.ShaderSource(FragShaderId, FragShaderSrc), Times.Once());
        this.mockGL.Verify(m => m.CompileShader(FragShaderId), Times.Once());
    }

    [Fact]
    public void ReactorInit_WhenInvoked_SuccessfullyCreatesProgram()
    {
        // Arrange
        CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGL.Verify(m => m.CreateProgram(), Times.Once());
        this.mockGL.Verify(m => m.AttachShader(ShaderProgramId, VertexShaderId), Times.Once());
        this.mockGL.Verify(m => m.AttachShader(ShaderProgramId, FragShaderId), Times.Once());
        this.mockGL.Verify(m => m.LinkProgram(ShaderProgramId), Times.Once());
    }

    [Fact]
    public void ReactorInit_WhenInvoked_DestroysVertexAndFragmentShader()
    {
        // Arrange
        CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGL.Verify(m => m.DetachShader(ShaderProgramId, VertexShaderId), Times.Once());
        this.mockGL.Verify(m => m.DeleteShader(VertexShaderId), Times.Once());
        this.mockGL.Verify(m => m.DetachShader(ShaderProgramId, FragShaderId), Times.Once());
        this.mockGL.Verify(m => m.DeleteShader(FragShaderId), Times.Once());
    }

    [Fact]
    public void ReactorInit_WithVertexShaderCompileIssue_ThrowsException()
    {
        // Arrange
        const int statusCode = 0;
        this.mockGL.Setup(m => m.GetShader(VertexShaderId, GLShaderParameter.CompileStatus))
            .Returns(statusCode);
        this.mockGL.Setup(m => m.GetShaderInfoLog(VertexShaderId)).Returns("Vertex Shader Compile Error");

        CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ShaderCompileException>(() =>
        {
            this.glInitReactor.OnReceive();
        }, $"Error compiling vertex shader '{ShaderName}' with shader ID '{VertexShaderId}'.{Environment.NewLine}Vertex Shader Compile Error");
    }

    [Fact]
    public void ReactorInit_WithFragmentShaderCompileIssue_ThrowsException()
    {
        // Arrange
        const int statusCode = 0;
        this.mockGL.Setup(m => m.GetShader(FragShaderId, GLShaderParameter.CompileStatus))
            .Returns(statusCode);
        this.mockGL.Setup(m => m.GetShaderInfoLog(FragShaderId)).Returns("Fragment Shader Compile Error");

        CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ShaderCompileException>(() =>
        {
            this.glInitReactor.OnReceive();
        }, $"Error compiling fragment shader '{ShaderName}' with shader ID '{FragShaderId}'.{Environment.NewLine}Fragment Shader Compile Error");
    }

    [Fact]
    public void ReactorInit_WithProgramLinkingIssue_ThrowsException()
    {
        // Arrange
        const int statusCode = 0;
        this.mockGL.Setup(m => m.GetProgram(ShaderProgramId, GLProgramParameterName.LinkStatus))
            .Returns(statusCode);
        this.mockGL.Setup(m => m.GetProgramInfoLog(ShaderProgramId)).Returns("Program Linking Error");

        CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ShaderLinkException>(() =>
        {
            this.glInitReactor.OnReceive();
        }, $"Error linking shader with ID '{ShaderProgramId}'{Environment.NewLine}Program Linking Error");
    }

    [Fact]
    public void Use_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var program = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ShaderNotInitializedException>(() =>
        {
            program.Use();
        }, "The shader has not been initialized.");
    }

    [Fact]
    public void Use_WhenInvoked_SetsProgramForUse()
    {
        // Arrange
        var program = CreateSystemUnderTest();
        this.glInitReactor.OnReceive();

        // Act
        program.Use();

        // Assert
        this.mockGL.Verify(m => m.UseProgram(ShaderProgramId), Times.Once());
    }

    [Fact]
    public void WithShutDownNotification_DisposesOfShaderProgram()
    {
        // Arrange
        CreateSystemUnderTest();
        this.glInitReactor.OnReceive();

        // Act
        this.shutDownReactor?.OnReceive();
        this.shutDownReactor?.OnReceive();

        // Assert
        this.mockGLInitUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockGL.Verify(m => m.DeleteProgram(ShaderProgramId), Times.Once);
    }

    [Fact]
    public void PushReactable_WhenOnCompleteIsInvoked_UnsubscribesFromReactable()
    {
        // Arrange
        CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnUnsubscribe();

        // Assert
        this.mockGLInitUnsubscriber.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates an instance of <see cref="ShaderProgramFake"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test with.</returns>
    private ShaderProgramFake CreateSystemUnderTest()
        => new (
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockShaderLoader.Object,
            this.mockReactableFactory.Object);
}
