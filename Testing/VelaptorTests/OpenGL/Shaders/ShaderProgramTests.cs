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
using NSubstitute;
using Velaptor;
using Velaptor.Factories;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
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
    private readonly IShaderLoaderService mockShaderLoader;
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IDisposable mockGLInitUnsubscriber;
    private readonly IDisposable mockShutDownUnsubscriber;
    private readonly IPushReactable mockPushReactable;
    private IReceiveSubscription? glInitReactor;
    private IReceiveSubscription? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderProgramTests"/> class.
    /// </summary>
    public ShaderProgramTests()
    {
        this.mockShaderLoader = Substitute.For<IShaderLoaderService>();

        // Sets up the vertex sut file mock.
        this.mockShaderLoader.LoadVertSource(ShaderName).Returns(VertShaderSrc);

        // Sets up the fragment sut file mock.
        this.mockShaderLoader.LoadFragSource(ShaderName).Returns(FragShaderSrc);

        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGLService = Substitute.For<IOpenGLService>();

        const int getShaderStatusCode = 1;
        const int getProgramStatusCode = 1;
        this.mockGL.CreateShader(GLShaderType.VertexShader).Returns(VertexShaderId);
        this.mockGL.CreateShader(GLShaderType.FragmentShader).Returns(FragShaderId);
        this.mockGL.GetShader(Arg.Any<uint>(), GLShaderParameter.CompileStatus).Returns(getShaderStatusCode);
        this.mockGL.GetProgram(Arg.Any<uint>(), GLProgramParameterName.LinkStatus).Returns(getProgramStatusCode);
        this.mockGL.CreateProgram().Returns(ShaderProgramId);

        this.mockGLInitUnsubscriber = Substitute.For<IDisposable>();
        this.mockShutDownUnsubscriber = Substitute.For<IDisposable>();

        this.mockPushReactable = Substitute.For<IPushReactable>();
        this.mockPushReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription>();

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

        this.mockPushReactable.Subscribe(Arg.Any<IReceiveSubscription>())
            .Returns(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription>();
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    return this.mockGLInitUnsubscriber;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                return null;
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(this.mockPushReactable);
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
                this.mockGLService,
                this.mockShaderLoader,
                this.mockReactableFactory);
        }, "Value cannot be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ShaderProgramFake(
                this.mockGL,
                null,
                this.mockShaderLoader,
                this.mockReactableFactory);
        }, "Value cannot be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullShaderLoaderServiceParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ShaderProgramFake(
                this.mockGL,
                this.mockGLService,
                null,
                this.mockReactableFactory);
        }, "Value cannot be null. (Parameter 'shaderLoaderService')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new ShaderProgramFake(
                this.mockGL,
                this.mockGLService,
                this.mockShaderLoader,
                null);
        }, "Value cannot be null. (Parameter 'reactableFactory')");
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
        this.mockShaderLoader.Received(1).LoadVertSource(ShaderName);
        this.mockShaderLoader.Received(1).LoadFragSource(ShaderName);
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
        this.mockShaderLoader.Received(1).LoadVertSource(Arg.Any<string>());
        this.mockShaderLoader.Received(1).LoadFragSource(Arg.Any<string>());
        this.mockGL.Received(2).CreateShader(Arg.Any<GLShaderType>());
        this.mockGL.Received(1).CreateProgram();
        this.mockGL.Received(2).AttachShader(Arg.Any<uint>(), Arg.Any<uint>());
        this.mockGL.Received(1).LinkProgram(Arg.Any<uint>());
        this.mockGL.Received(2).DetachShader(Arg.Any<uint>(), Arg.Any<uint>());
        this.mockGL.Received(2).DeleteShader(Arg.Any<uint>());
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
        this.mockGL.Received(1).CreateShader(GLShaderType.VertexShader);
        this.mockGL.Received(1).ShaderSource(VertexShaderId, VertShaderSrc);
        this.mockGL.Received(1).CompileShader(VertexShaderId);

        // Verify the creation of the fragment sut
        this.mockGL.Received(1).CreateShader(GLShaderType.FragmentShader);
        this.mockGL.Received(1).ShaderSource(FragShaderId, FragShaderSrc);
        this.mockGL.Received(1).CompileShader(FragShaderId);
    }

    [Fact]
    public void ReactorInit_WhenInvoked_SuccessfullyCreatesProgram()
    {
        // Arrange
        CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGL.Received(1).CreateProgram();
        this.mockGL.Received(1).AttachShader(ShaderProgramId, VertexShaderId);
        this.mockGL.Received(1).AttachShader(ShaderProgramId, FragShaderId);
        this.mockGL.Received(1).LinkProgram(ShaderProgramId);
    }

    [Fact]
    public void ReactorInit_WhenInvoked_DestroysVertexAndFragmentShader()
    {
        // Arrange
        CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnReceive();

        // Assert
        this.mockGL.Received(1).DetachShader(ShaderProgramId, VertexShaderId);
        this.mockGL.Received(1).DeleteShader(VertexShaderId);
        this.mockGL.Received(1).DetachShader(ShaderProgramId, FragShaderId);
        this.mockGL.Received(1).DeleteShader(FragShaderId);
    }

    [Fact]
    public void ReactorInit_WithVertexShaderCompileIssue_ThrowsException()
    {
        // Arrange
        const int statusCode = 0;
        this.mockGL.GetShader(VertexShaderId, GLShaderParameter.CompileStatus).Returns(statusCode);
        this.mockGL.GetShaderInfoLog(VertexShaderId).Returns("Vertex Shader Compile Error");

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
        this.mockGL.GetShader(FragShaderId, GLShaderParameter.CompileStatus).Returns(statusCode);
        this.mockGL.GetShaderInfoLog(FragShaderId).Returns("Fragment Shader Compile Error");

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
        this.mockGL.GetProgram(ShaderProgramId, GLProgramParameterName.LinkStatus).Returns(statusCode);
        this.mockGL.GetProgramInfoLog(ShaderProgramId).Returns("Program Linking Error");

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
        this.mockGL.Received(1).UseProgram(ShaderProgramId);
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
        this.mockGL.Received(1).DeleteProgram(ShaderProgramId);
    }
    #endregion

    #region Reactable Tests
    [Fact]
    public void PushReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Act & Assert
        this.mockPushReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription>();
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.GLInitializedId)
                {
                    reactor.Name.Should().Be($"ShaderProgram.UNKNOWN() - {PushNotifications.GLInitializedId}");
                }
                else if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    reactor.Name.Should().Be($"ShaderProgram.UNKNOWN() - {PushNotifications.SystemShuttingDownId}");
                }
                else
                {
                    Assert.Fail($"The event ID '{reactor.Id}' is not recognized or accounted for in the unit test.");
                }
            });

        _ = CreateSystemUnderTest();
    }

    [Fact]
    public void PushReactable_WhenUnsubscribingGlInit_Unsubscribes()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.glInitReactor.OnUnsubscribe();

        // Assert
        this.mockGLInitUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    public void PushReactable_WhenUnsubscribingShutdown_Unsubscribes()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.shutDownReactor.OnUnsubscribe();

        // Assert
        this.mockShutDownUnsubscriber.Received(1).Dispose();
    }
    #endregion

    /// <summary>
    /// Creates an instance of <see cref="ShaderProgramFake"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test with.</returns>
    private ShaderProgramFake CreateSystemUnderTest()
        => new (
            this.mockGL,
            this.mockGLService,
            this.mockShaderLoader,
            this.mockReactableFactory);
}
