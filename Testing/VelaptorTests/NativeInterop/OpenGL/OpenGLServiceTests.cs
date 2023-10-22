// <copyright file="OpenGLServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.OpenGL;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Helpers;
using Moq;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Xunit;
using FluentAssertions;

/// <summary>
/// Tests the <see cref="OpenGLService"/> class.
/// </summary>
public class OpenGLServiceTests
{
    private readonly Mock<IGLInvoker> mockGLInvoker;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenGLServiceTests"/> class.
    /// </summary>
    public OpenGLServiceTests()
    {
        this.mockGLInvoker = new Mock<IGLInvoker>();

        unsafe
        {
            this.mockGLInvoker.Setup(m => m.GetInteger(GLGetPName.Viewport, It.IsAny<int[]>()))
                .Callback<GLGetPName, int[]>((_, data) =>
                {
                    fixed (int* dataPtr = data)
                    {
                        dataPtr[0] = 11;
                        dataPtr[1] = 22;
                        dataPtr[2] = 33;
                        dataPtr[3] = 44;
                    }
                });
        }
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLInvokerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new OpenGLService(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'glInvoker')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void IsVBOBound_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.BindVBO(123u);
        var isBound = service.IsVBOBound;
        service.UnbindVBO();
        var isUnbound = service.IsVBOBound;

        // Assert
        isBound.Should().BeTrue();
        isUnbound.Should().BeFalse();
    }

    [Fact]
    public void IsEBOBound_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.BindEBO(123u);
        var isBound = service.IsEBOBound;
        service.UnbindEBO();
        var isUnbound = service.IsEBOBound;

        // Assert
        isBound.Should().BeTrue();
        isUnbound.Should().BeFalse();
    }

    [Fact]
    public void IsVAOBound_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.BindVAO(123u);
        var isBound = service.IsVAOBound;
        service.UnbindVAO();
        var isUnbound = service.IsVAOBound;

        // Assert
        isBound.Should().BeTrue();
        isUnbound.Should().BeFalse();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetViewPortSize_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateService();

        // Act
        var actual = service.GetViewPortSize();

        // Assert
        actual.Should().BeEquivalentTo(new Size(33, 44));
    }

    [Fact]
    public void SetViewPortSize_WhenInvoked_SetsViewPort()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.SetViewPortSize(new Size(55, 66));

        // Assert
        this.mockGLInvoker.Verify(m => m.Viewport(11, 22, 55, 66));
    }

    [Fact]
    public void GetViewPortPosition_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateService();

        // Act
        var actual = service.GetViewPortPosition();

        // Assert
        actual.Should().BeEquivalentTo(new Vector2(11, 22));
    }

    [Fact]
    public void BindVBO_WhenInvoked_BindsVertexBufferObject()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.BindVBO(123u);

        // Assert
        this.mockGLInvoker.Verify(m => m.BindBuffer(GLBufferTarget.ArrayBuffer, 123u), Times.Once);
    }

    [Fact]
    public void UnbindVBO_WhenInvoked_UnbindsVertexBufferObject()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.UnbindVBO();

        // Assert
        this.mockGLInvoker.Verify(m => m.BindBuffer(GLBufferTarget.ArrayBuffer, 0u), Times.Once);
    }

    [Fact]
    public void BindEBO_WhenInvoked_BindsElementBufferObject()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.BindEBO(123u);

        // Assert
        this.mockGLInvoker.Verify(m => m.BindBuffer(GLBufferTarget.ElementArrayBuffer, 123u), Times.Once);
    }

    [Fact]
    public void UnbindEBO_WithBoundVAO_ThrowsException()
    {
        // Arrange
        var service = CreateService();
        service.BindVAO(123u);

        // Act
        var act = service.UnbindEBO;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("The VAO object must be unbound before unbinding an EBO object.");
    }

    [Fact]
    public void UnbindEBO_WhenInvoked_UnbindsElementBufferObject()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.UnbindEBO();

        // Assert
        this.mockGLInvoker.Verify(m => m.BindBuffer(GLBufferTarget.ElementArrayBuffer, 0u), Times.Once);
    }

    [Fact]
    public void BindVAO_WhenInvoked_BindsVertexArrayObject()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.BindVAO(123u);

        // Assert
        this.mockGLInvoker.Verify(m => m.BindVertexArray(123u), Times.Once);
    }

    [Fact]
    public void UnbindVAO_WhenInvoked_UnbindsVertexArrayObject()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.UnbindVAO();

        // Assert
        this.mockGLInvoker.Verify(m => m.BindVertexArray(0u), Times.Once);
    }

    [Fact]
    public void BindTexture2D_WhenInvoked_BindsTexture()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.BindTexture2D(123u);

        // Assert
        this.mockGLInvoker.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, 123u), Times.Once);
    }

    [Fact]
    public void UnbindTexture2D_WhenInvoked_UnbindsTexture()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.UnbindTexture2D();

        // Assert
        this.mockGLInvoker.Verify(m => m.BindTexture(GLTextureTarget.Texture2D, 0u), Times.Once);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    public void ProgramLinkedSuccessfully_WhenSuccessful_ReturnsCorrectResult(int linkStatus, bool expected)
    {
        // Arrange
        this.mockGLInvoker.Setup(m =>
                m.GetProgram(123, GLProgramParameterName.LinkStatus))
            .Returns(linkStatus);
        var service = CreateService();

        // Act
        var actual = service.ProgramLinkedSuccessfully(123);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(0, false)]
    public void ShaderCompiledSuccessfully_WhenInvoked_ReturnsCorrectResult(int compileStatus, bool expected)
    {
        // Arrange
        this.mockGLInvoker.Setup(m =>
                m.GetShader(123, GLShaderParameter.CompileStatus))
            .Returns(compileStatus);
        var service = CreateService();

        // Act
        var actual = service.ShaderCompiledSuccessfully(123);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void BeginGroup_WhenInvoked_CreatesDebugGroup()
    {
        // Arrange
        const string label = "test-label";
        var service = CreateService();

        // Act
        service.BeginGroup(label);

        // Assert
        this.mockGLInvoker.Verify(m =>
            m.PushDebugGroup(GLDebugSource.DebugSourceApplication, 100, (uint)label.Length, label), Times.Once);
    }

    [Fact]
    public void EndGroup_WhenInvoked_EndsDebugGroup()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.EndGroup();

        // Assert
        this.mockGLInvoker.Verify(m =>
            m.PopDebugGroup(), Times.Once);
    }

    [Fact]
    public void LabelShader_WhenInvoked_LabelsShader()
    {
        // Arrange
        const string label = "test-label";
        var service = CreateService();

        // Act
        service.LabelShader(123, label);

        // Assert
        this.mockGLInvoker.Verify(m => m.ObjectLabel(GLObjectIdentifier.Shader, 123, (uint)label.Length, label));
    }

    [Fact]
    public void LabelShaderProgram_WhenInvoked_LabelsShaderProgram()
    {
        // Arrange
        const string label = "test-label";
        var service = CreateService();

        // Act
        service.LabelShaderProgram(123, label);

        // Assert
        this.mockGLInvoker.Verify(m => m.ObjectLabel(GLObjectIdentifier.Program, 123, (uint)label.Length, label));
    }

    [Theory]
    [InlineData("", "NOT SET VAO")]
    [InlineData(null, "NOT SET VAO")]
    [InlineData("test-label", "test-label VAO")]
    public void LabelVertexArray_WhenInvoked_LabelsVertexArray(string label, string expected)
    {
        // Arrange
        var service = CreateService();

        // Act
        service.LabelVertexArray(123, label);

        // Assert
        this.mockGLInvoker.Verify(m =>
            m.ObjectLabel(GLObjectIdentifier.VertexArray, 123, (uint)expected.Length, expected));
    }

    [Fact]
    public void LabelBuffer_WithInvalidBufferType_ThrowsException()
    {
        // Arrange
        const int invalidValue = 123;
        var expected = $"The value of argument 'bufferType' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(OpenGLBufferType)}'. (Parameter 'bufferType')";

        var service = CreateService();

        // Act
        var act = () => service.LabelBuffer(It.IsAny<uint>(), It.IsAny<string>(), (OpenGLBufferType)invalidValue);

        // Assert
        act.Should().Throw<InvalidEnumArgumentException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData("", (int)OpenGLBufferType.VertexBufferObject, "NOT SET VBO")]
    [InlineData(null, (int)OpenGLBufferType.VertexBufferObject, "NOT SET VBO")]
    [InlineData("test-label", (int)OpenGLBufferType.VertexBufferObject, "test-label VBO")]
    [InlineData("", (int)OpenGLBufferType.IndexArrayObject, "NOT SET EBO")]
    [InlineData(null, (int)OpenGLBufferType.IndexArrayObject, "NOT SET EBO")]
    [InlineData("test-label", (int)OpenGLBufferType.IndexArrayObject, "test-label EBO")]
    public void LabelBuffer_WhenInvoked_LabelsVertexArray(string label, int bufferTypeNumericalValue, string expected)
    {
        // Arrange
        var bufferType = (OpenGLBufferType)bufferTypeNumericalValue;
        var service = CreateService();

        // Act
        service.LabelBuffer(123, label, bufferType);

        // Assert
        this.mockGLInvoker.Verify(m =>
            m.ObjectLabel(GLObjectIdentifier.Buffer, 123, (uint)expected.Length, expected));
    }

    [Theory]
    [InlineData("", "NOT SET")]
    [InlineData(null, "NOT SET")]
    [InlineData("test-label", "test-label")]
    public void LabelTexture_WhenInvoked_LabelsTexture(string label, string expected)
    {
        // Arrange
        var service = CreateService();

        // Act
        service.LabelTexture(123, label);

        // Assert
        this.mockGLInvoker.Verify(m =>
            m.ObjectLabel(GLObjectIdentifier.Texture, 123, (uint)expected.Length, expected));
    }

    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="OpenGLService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private OpenGLService CreateService() => new (this.mockGLInvoker.Object);
}
