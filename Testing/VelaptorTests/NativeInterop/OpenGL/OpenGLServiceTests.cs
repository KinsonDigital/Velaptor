// <copyright file="OpenGLServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.OpenGL;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Xunit;
using NSubstitute;
using Shouldly;
using Silk.NET.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.Services;

/// <summary>
/// Tests the <see cref="OpenGLService"/> class.
/// </summary>
public class OpenGLServiceTests
{
    private const int API_ID_RECOMPILE_FRAGMENT_SHADER = 2;
    private const int API_ID_RECOMPILE_VERTEX_SHADER = 131218;
    private readonly IGLInvoker mockGLInvoker;
    private readonly IDotnetService mockDotnetService;
    private readonly ILoggingService mockLoggingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGLServiceTests"/> class.
    /// </summary>
    public OpenGLServiceTests()
    {
        this.mockGLInvoker = Substitute.For<IGLInvoker>();
        this.mockDotnetService = Substitute.For<IDotnetService>();
        this.mockLoggingService = Substitute.For<ILoggingService>();

        this.mockGLInvoker
            .When(x => x.GetInteger(GLGetPName.Viewport, Arg.Any<int[]>()))
            .Do(callInfo =>
            {
                var values = callInfo.ArgAt<int[]>(1);
                values[0] = 11;
                values[1] = 22;
                values[2] = 33;
                values[3] = 44;
            });
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new OpenGLService(null, this.mockDotnetService, this.mockLoggingService);

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act); // Corrected Shouldly syntax
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'glInvoker')");
    }

    [Fact]
    public void Ctor_WithNullDotnetServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new OpenGLService(this.mockGLInvoker, null, this.mockLoggingService);

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'dotnetService')");
    }

    [Fact]
    public void Ctor_WithNullLoggingServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new OpenGLService(this.mockGLInvoker, this.mockDotnetService, null);

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'loggingService')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void IsVBOBound_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.BindVBO(123u);
        var isBound = service.IsVBOBound;
        service.UnbindVBO();
        var isUnbound = service.IsVBOBound;

        // Assert
        isBound.ShouldBeTrue();
        isUnbound.ShouldBeFalse();
    }

    [Fact]
    public void IsEBOBound_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.BindEBO(123u);
        var isBound = service.IsEBOBound;
        service.UnbindEBO();
        var isUnbound = service.IsEBOBound;

        // Assert
        isBound.ShouldBeTrue();
        isUnbound.ShouldBeFalse();
    }

    [Fact]
    public void IsVAOBound_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.BindVAO(123u);
        var isBound = service.IsVAOBound;
        service.UnbindVAO();
        var isUnbound = service.IsVAOBound;

        // Assert
        isBound.ShouldBeTrue();
        isUnbound.ShouldBeFalse();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetViewPortSize_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        var actual = service.GetViewPortSize();

        // Assert
        actual.ShouldBeEquivalentTo(new Size(33, 44));
    }

    [Fact]
    public void SetViewPortSize_WhenInvoked_SetsViewPort()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.SetViewPortSize(new Size(55, 66));

        // Assert
        this.mockGLInvoker.Received(1).Viewport(11, 22, 55, 66);
    }

    [Fact]
    public void GetViewPortPosition_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        var actual = service.GetViewPortPosition();

        // Assert
        actual.ShouldBeEquivalentTo(new Vector2(11, 22));
    }

    [Fact]
    public void BindVBO_WhenInvoked_BindsVertexBufferObject()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.BindVBO(123u);

        // Assert
        this.mockGLInvoker.Received(1).BindBuffer(GLBufferTarget.ArrayBuffer, 123u);
    }

    [Fact]
    public void UnbindVBO_WhenInvoked_UnbindsVertexBufferObject()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.UnbindVBO();

        // Assert
        this.mockGLInvoker.Received(1).BindBuffer(GLBufferTarget.ArrayBuffer, 0u);
    }

    [Fact]
    public void BindEBO_WhenInvoked_BindsElementBufferObject()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.BindEBO(123u);

        // Assert
        this.mockGLInvoker.Received(1).BindBuffer(GLBufferTarget.ElementArrayBuffer, 123u);
    }

    [Fact]
    public void UnbindEBO_WithBoundVAO_ThrowsException()
    {
        // Arrange
        var service = CreateSystemUnderTest();
        service.BindVAO(123u);

        // Act
        var act = service.UnbindEBO;

        // Assert
        var exception = Should.Throw<InvalidOperationException>(act);
        exception.Message.ShouldBe("The VAO object must be unbound before unbinding an EBO object.");
    }

    [Fact]
    public void UnbindEBO_WhenInvoked_UnbindsElementBufferObject()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.UnbindEBO();

        // Assert
        this.mockGLInvoker.Received(1).BindBuffer(GLBufferTarget.ElementArrayBuffer, 0u);
    }

    [Fact]
    public void BindVAO_WhenInvoked_BindsVertexArrayObject()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.BindVAO(123u);

        // Assert
        this.mockGLInvoker.Received(1).BindVertexArray(123u);
    }

    [Fact]
    public void UnbindVAO_WhenInvoked_UnbindsVertexArrayObject()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.UnbindVAO();

        // Assert
        this.mockGLInvoker.Received(1).BindVertexArray(0u);
    }

    [Fact]
    public void BindTexture2D_WhenInvoked_BindsTexture()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.BindTexture2D(123u);

        // Assert
        this.mockGLInvoker.Received(1).BindTexture(GLTextureTarget.Texture2D, 123u);
    }

    [Fact]
    public void UnbindTexture2D_WhenInvoked_UnbindsTexture()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.UnbindTexture2D();

        // Assert
        this.mockGLInvoker.Received(1).BindTexture(GLTextureTarget.Texture2D, 0u);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    public void ProgramLinkedSuccessfully_WhenSuccessful_ReturnsCorrectResult(int linkStatus, bool expected)
    {
        // Arrange
        this.mockGLInvoker.GetProgram(123, GLProgramParameterName.LinkStatus).Returns(linkStatus);
        var service = CreateSystemUnderTest();

        // Act
        var actual = service.ProgramLinkedSuccessfully(123);

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    public void ShaderCompiledSuccessfully_WhenInvoked_ReturnsCorrectResult(int compileStatus, bool expected)
    {
        // Arrange
        this.mockGLInvoker.GetShader(123, GLShaderParameter.CompileStatus).Returns(compileStatus);
        var service = CreateSystemUnderTest();

        // Act
        var actual = service.ShaderCompiledSuccessfully(123);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void BeginGroup_WhenInvoked_CreatesDebugGroup()
    {
        // Arrange
        const string label = "test-label";
        var service = CreateSystemUnderTest();

        // Act
        service.BeginGroup(label);

        // Assert
        this.mockGLInvoker.Received(1).PushDebugGroup(GLDebugSource.DebugSourceApplication, 100, (uint)label.Length, label);
    }

    [Fact]
    public void EndGroup_WhenInvoked_EndsDebugGroup()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.EndGroup();

        // Assert
        this.mockGLInvoker.Received(1).PopDebugGroup();
    }

    [Fact]
    public void LabelShader_WhenInvoked_LabelsShader()
    {
        // Arrange
        const string label = "test-label";
        var service = CreateSystemUnderTest();

        // Act
        service.LabelShader(123, label);

        // Assert
        this.mockGLInvoker.Received(1).ObjectLabel(GLObjectIdentifier.Shader, 123, (uint)label.Length, label);
    }

    [Fact]
    public void LabelShaderProgram_WhenInvoked_LabelsShaderProgram()
    {
        // Arrange
        const string label = "test-label";
        var service = CreateSystemUnderTest();

        // Act
        service.LabelShaderProgram(123, label);

        // Assert
        this.mockGLInvoker.Received(1).ObjectLabel(GLObjectIdentifier.Program, 123, (uint)label.Length, label);
    }

    [Theory]
    [InlineData("", "NOT SET VAO")]
    [InlineData(null, "NOT SET VAO")]
    [InlineData("test-label", "test-label VAO")]
    public void LabelVertexArray_WhenInvoked_LabelsVertexArray(string? label, string expected)
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.LabelVertexArray(123, label);

        // Assert
        this.mockGLInvoker.Received(1).ObjectLabel(GLObjectIdentifier.VertexArray, 123, (uint)expected.Length, expected);
    }

    [Fact]
    public void LabelBuffer_WithInvalidBufferType_ThrowsException()
    {
        // Arrange
        const int invalidValue = 123;
        var expected = $"The value of argument 'bufferType' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(OpenGLBufferType)}'. (Parameter 'bufferType')";

        var service = CreateSystemUnderTest();

        // Act
        var act = () => service.LabelBuffer(default, default, (OpenGLBufferType)invalidValue);

        // Assert
        var exception = Should.Throw<InvalidEnumArgumentException>(act);
        exception.Message.ShouldBe(expected);
    }

    [Theory]
    [InlineData("", (int)OpenGLBufferType.VertexBufferObject, "NOT SET VBO")]
    [InlineData(null, (int)OpenGLBufferType.VertexBufferObject, "NOT SET VBO")]
    [InlineData("test-label", (int)OpenGLBufferType.VertexBufferObject, "test-label VBO")]
    [InlineData("", (int)OpenGLBufferType.IndexArrayObject, "NOT SET EBO")]
    [InlineData(null, (int)OpenGLBufferType.IndexArrayObject, "NOT SET EBO")]
    [InlineData("test-label", (int)OpenGLBufferType.IndexArrayObject, "test-label EBO")]
    public void LabelBuffer_WhenInvoked_LabelsVertexArray(string? label, int bufferTypeNumericalValue, string expected)
    {
        // Arrange
        var bufferType = (OpenGLBufferType)bufferTypeNumericalValue;
        var service = CreateSystemUnderTest();

        // Act
        service.LabelBuffer(123, label, bufferType);

        // Assert
        this.mockGLInvoker.Received(1).ObjectLabel(GLObjectIdentifier.Buffer, 123, (uint)expected.Length, expected);
    }

    [Theory]
    [InlineData("", "NOT SET")]
    [InlineData(null, "NOT SET")]
    [InlineData("test-label", "test-label")]
    public void LabelTexture_WhenInvoked_LabelsTexture(string? label, string expected)
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.LabelTexture(123, label);

        // Assert
        this.mockGLInvoker.Received(1).ObjectLabel(GLObjectIdentifier.Texture, 123, (uint)expected.Length, expected);
    }

    [Fact]
    public void SetupErrorCallback_WhenInvokedTheFirstTime_InvokesIDotnetServiceAndIGLInvokerMethods()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.SetupErrorCallback();

        // Assert
        this.mockDotnetService.Received(1).GcKeepAlive(Arg.Any<DebugProc?>());
        this.mockDotnetService.Received(1).MarshalStringToHGlobalAnsi(string.Empty);
        this.mockGLInvoker.Received(1).DebugMessageCallback(Arg.Any<DebugProc?>(), Arg.Any<nint>());
    }

    [Fact]
    public void SetupErrorCallback_WhenInvokedTheSecondTime_DoesNotPerformAnyAction()
    {
        // Arrange
        var service = CreateSystemUnderTest();

        // Act
        service.SetupErrorCallback();
        service.SetupErrorCallback();

        // Assert
        this.mockDotnetService.Received(1).GcKeepAlive(Arg.Any<DebugProc?>());
    }

    [Fact]
    public void ToOpenGLBytes_WhenInvoked_ReturnsOpenGLBytes()
    {
        // Arrange
        // NOTE: The pixels are in ARGB format and are row major ordering.
        // Row major ordering means top to bottom and left to right.
        // Another way to think of it is one row of pixels at a time from the top to the bottom
        // and each row is one pixel at a time from left to right.
        var pixels = new[,]
        {
            {
                Color.FromArgb(1, 2, 3, 4), // Pixel 0,0
                Color.FromArgb(9, 10, 11, 12), // Pixel 1,0
            },
            {
                Color.FromArgb(5, 6, 7, 8), // Pixel 0,1
                Color.FromArgb(13, 14, 15, 16), // Pixel 1,1
            },
        };
        var expected = new byte[]
        {
            2, 3, 4, 1, // Pixel 0,0
            6, 7, 8, 5, // Pixel 0,1
            10, 11, 12, 9, // Pixel 1,0
            14, 15, 16, 13, // Pixel 1,1
        };
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.ToOpenGLBytes(pixels);

        // Assert
        actual.ShouldBeEquivalentTo(expected);
    }
    #endregion

    #region Indirect Tests
    [Theory]
    [InlineData(API_ID_RECOMPILE_FRAGMENT_SHADER)]
    [InlineData(API_ID_RECOMPILE_VERTEX_SHADER)]
    public void DebugCallback_WhenInvokedComplicationWarnings_DoNotLogError(int shaderId)
    {
        // Arrange
        DebugProc? debugProc = null;
        var glErrorEventExecuted = false;

        this.mockDotnetService.MarshalPtrToStringAnsi(new IntPtr(123)).Returns("test-message");
        this.mockDotnetService.MarshalPtrToStringAnsi(new IntPtr(456)).Returns("user-param");
        this.mockGLInvoker
            .When(x => x.DebugMessageCallback(Arg.Any<DebugProc>(), Arg.Any<nint>()))
            .Do((callInfo) =>
            {
                debugProc = callInfo.Arg<DebugProc>();

                if (debugProc is null)
                {
                    throw new Exception("The 'DebugProc' parameter cannot be null during test set up.");
                }
            });

        var sut = CreateSystemUnderTest();
        sut.GLError += (_, _) => glErrorEventExecuted = true;

        sut.SetupErrorCallback();

        // Act
        debugProc(
            GLEnum.ActiveProgram,
            GLEnum.ActiveTexture,
            shaderId,
            GLEnum.NoError,
            20,
            new IntPtr(123),
            new IntPtr(456));

        // Assert
        this.mockLoggingService.DidNotReceive().Warning(Arg.Any<string?>());
        this.mockLoggingService.DidNotReceive().Error(Arg.Any<string?>());
        glErrorEventExecuted.ShouldBeFalse();
    }

    [Fact]
    public void DebugCallback_WhenInvokedWithNoError_LogsDebugMessage()
    {
        // Arrange
        DebugProc? debugProc = null;
        var expectedMsg = "test-message";
        expectedMsg += $"{Environment.NewLine}\tSrc: ActiveProgram";
        expectedMsg += $"{Environment.NewLine}\tType: ActiveTexture";
        expectedMsg += $"{Environment.NewLine}\tID: 10";
        expectedMsg += $"{Environment.NewLine}\tSeverity: NoError";
        expectedMsg += $"{Environment.NewLine}\tLength: 20";
        expectedMsg += $"{Environment.NewLine}\tUser Param: user-param";

        this.mockDotnetService.MarshalPtrToStringAnsi(new IntPtr(123)).Returns("test-message");
        this.mockDotnetService.MarshalPtrToStringAnsi(new IntPtr(456)).Returns("user-param");
        this.mockGLInvoker
            .When(x => x.DebugMessageCallback(Arg.Any<DebugProc>(), Arg.Any<nint>()))
            .Do((callInfo) =>
            {
                debugProc = callInfo.Arg<DebugProc>();

                if (debugProc is null)
                {
                    throw new Exception("The 'DebugProc' parameter cannot be null during test set up.");
                }
            });

        var sut = CreateSystemUnderTest();
        sut.SetupErrorCallback();

        // Act
        debugProc(
            GLEnum.ActiveProgram,
            GLEnum.ActiveTexture,
            10,
            GLEnum.NoError,
            20,
            new IntPtr(123),
            new IntPtr(456));

        // Assert
        this.mockLoggingService.Received(1).Warning(expectedMsg);
    }

    [Fact]
    public void DebugCallback_WhenInvokedWithErrorNotification_LogsDebugMessage()
    {
        // Arrange
        DebugProc? debugProc = null;
        var glErrorEventExecuted = false;

        this.mockDotnetService.MarshalPtrToStringAnsi(new IntPtr(123)).Returns("test-message");
        this.mockDotnetService.MarshalPtrToStringAnsi(new IntPtr(456)).Returns("user-param");
        this.mockGLInvoker
            .When(x => x.DebugMessageCallback(Arg.Any<DebugProc>(), Arg.Any<nint>()))
            .Do((callInfo) =>
            {
                debugProc = callInfo.Arg<DebugProc>();

                if (debugProc is null)
                {
                    throw new Exception("The 'DebugProc' parameter cannot be null during test set up.");
                }
            });

        var sut = CreateSystemUnderTest();
        sut.GLError += (_, _) => glErrorEventExecuted = true;

        sut.SetupErrorCallback();

        // Act
        debugProc(
            GLEnum.ActiveProgram,
            GLEnum.ActiveTexture,
            10,
            GLEnum.DebugSeverityNotification,
            20,
            new IntPtr(123),
            new IntPtr(456));

        // Assert
        this.mockLoggingService.DidNotReceive().Warning(Arg.Any<string?>());
        glErrorEventExecuted.ShouldBeFalse();
    }

    [Fact]
    public void DebugCallback_WhenInvokedWithError_LogsDebugMessage()
    {
        // Arrange
        DebugProc? debugProc = null;
        var glErrorEventExecuted = false;
        var expectedMsg = "test-message";
        expectedMsg += $"{Environment.NewLine}\tSrc: ActiveProgram";
        expectedMsg += $"{Environment.NewLine}\tType: ActiveTexture";
        expectedMsg += $"{Environment.NewLine}\tID: 10";
        expectedMsg += $"{Environment.NewLine}\tSeverity: DebugTypeError";
        expectedMsg += $"{Environment.NewLine}\tLength: 20";
        expectedMsg += $"{Environment.NewLine}\tUser Param: user-param";

        this.mockDotnetService.MarshalPtrToStringAnsi(new IntPtr(123)).Returns("test-message");
        this.mockDotnetService.MarshalPtrToStringAnsi(new IntPtr(456)).Returns("user-param");
        this.mockGLInvoker
            .When(x => x.DebugMessageCallback(Arg.Any<DebugProc>(), Arg.Any<nint>()))
            .Do((callInfo) =>
            {
                debugProc = callInfo.Arg<DebugProc>();

                if (debugProc is null)
                {
                    throw new Exception("The 'DebugProc' parameter cannot be null during test set up.");
                }
            });

        var sut = CreateSystemUnderTest();
        sut.GLError += (_, _) => glErrorEventExecuted = true;

        sut.SetupErrorCallback();

        // Act
        debugProc(
            GLEnum.ActiveProgram,
            GLEnum.ActiveTexture,
            10,
            GLEnum.DebugTypeError,
            20,
            new IntPtr(123),
            new IntPtr(456));

        // Assert
        this.mockLoggingService.Received(1).Error(expectedMsg);
        glErrorEventExecuted.ShouldBeTrue();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="OpenGLService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private OpenGLService CreateSystemUnderTest() => new (this.mockGLInvoker, this.mockDotnetService, this.mockLoggingService);
}
