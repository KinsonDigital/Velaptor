using Raptor.OpenGL;
using Moq;
using FileIO.Core;
using OpenToolkit.Graphics.OpenGL4;
using Xunit;
using RaptorTests.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RaptorTests.OpenGL
{
    /// <summary>
    /// Initializes a new instance of <see cref="ShaderProgramTests"/>.
    /// </summary>
    public class ShaderProgramTests
    {
        private readonly Mock<ITextFile> _mockTextFile;
        private readonly Mock<IGLInvoker> _mockGL;
        //TODO: This might have to be used somewhere else to make it work
        private string _vertexShaderPath = $@"shader.vert";
        private string _fragShaderPath = $@"shader.frag";
        private readonly int _vertextShaderID = 1234;
        private readonly int _fragShaderID = 5678;
        private readonly int _shaderProgramID = 1928;
        private readonly int _batchSize = 2;

        public ShaderProgramTests()
        {
            _mockTextFile = new Mock<ITextFile>();

            _mockGL = new Mock<IGLInvoker>();

            int getShaderStatusCode = 1;
            int getProgramStatusCode = 1;
            _mockGL.Setup(m => m.CreateShader(ShaderType.VertexShader)).Returns(_vertextShaderID);
            _mockGL.Setup(m => m.CreateShader(ShaderType.FragmentShader)).Returns(_fragShaderID);
            _mockGL.Setup(m => m.GetShader(It.IsAny<int>(), ShaderParameter.CompileStatus, out getShaderStatusCode));
            _mockGL.Setup(m => m.GetProgram(It.IsAny<int>(), GetProgramParameterName.LinkStatus, out getProgramStatusCode));
            _mockGL.Setup(m => m.CreateProgram()).Returns(_shaderProgramID);

            _mockGL.Setup(m => m.ShaderCompileSuccess(It.IsAny<int>())).Returns(true);
            _mockGL.Setup(m => m.LinkProgramSuccess(It.IsAny<int>())).Returns(true);
        }

        [Fact]
        public void Ctor_WhenInvoked_LoadsShaderSourceCode()
        {
            //Arrange
            _mockTextFile.Setup(m => m.LoadAsLines(It.IsAny<string>())).Returns(() => new[] { "line-1", null });

            //Act
            var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            //Assert
            _mockTextFile.Verify(m => m.LoadAsLines(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesVertexShader()
        {
            //Arrange
            SetupVertexShaderFileMock();
            var expected = "layout(location = 3) in float aTransformIndex;\r\nuniform mat4 uTransform[10];//MODIFIED_DURING_COMPILE_TIME\r\n";

            //Act
            var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            //Assert
            _mockGL.Verify(m => m.CreateShader(ShaderType.VertexShader), Times.Once());
            _mockGL.Verify(m => m.ShaderSource(_vertextShaderID, expected), Times.Once());
            _mockGL.Verify(m => m.CompileShader(_vertextShaderID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesFragmentShader()
        {
            //Arrange
            SetupFragmentShaderFileMock();
            var expected = "in vec2 v_TexCoord;\r\nin vec4 v_TintClr;\r\n";

            //Act
            var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            //Assert
            _mockGL.Verify(m => m.CreateShader(ShaderType.FragmentShader), Times.Once());
            _mockGL.Verify(m => m.ShaderSource(_fragShaderID, expected), Times.Once());
            _mockGL.Verify(m => m.CompileShader(_fragShaderID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_SuccessfullyCreatesProgram()
        {
            //Arrange
            SetupVertexShaderFileMock();
            SetupFragmentShaderFileMock();

            //Act
            var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            //Assert
            _mockGL.Verify(m => m.CreateProgram(), Times.Once());
            _mockGL.Verify(m => m.AttachShader(_shaderProgramID, _vertextShaderID), Times.Once());
            _mockGL.Verify(m => m.AttachShader(_shaderProgramID, _fragShaderID), Times.Once());
            _mockGL.Verify(m => m.LinkProgram(_shaderProgramID), Times.Once());
        }

        [Fact]
        public void Ctor_WhenInvoked_DestroysVertexAndFragmentShader()
        {
            //Arrange
            SetupVertexShaderFileMock();
            SetupFragmentShaderFileMock();

            //Act
            var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            //Assert
            _mockGL.Verify(m => m.DetachShader(_shaderProgramID, _vertextShaderID), Times.Once());
            _mockGL.Verify(m => m.DeleteShader(_vertextShaderID), Times.Once());
            _mockGL.Verify(m => m.DetachShader(_shaderProgramID, _fragShaderID), Times.Once());
            _mockGL.Verify(m => m.DeleteShader(_fragShaderID), Times.Once());
        }

        [Fact]
        public void Ctor_WithShaderCompileIssue_ThrowsException()
        {
            //Arrange
            int statusCode = 0;
            _mockGL.Setup(m => m.GetShader(_vertextShaderID, ShaderParameter.CompileStatus, out statusCode));
            _mockGL.Setup(m => m.GetShaderInfoLog(_vertextShaderID)).Returns("Vertex Shader Compile Error");
            _mockGL.Setup(m => m.ShaderCompileSuccess(_vertextShaderID)).Returns(false);

            SetupVertexShaderFileMock();
            SetupFragmentShaderFileMock();

            //Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            }, $"Error occurred while compiling shader with ID '{_vertextShaderID}'\nVertex Shader Compile Error");
        }

        [Fact]
        public void Ctor_WithProgramLinkingIssue_ThrowsException()
        {
            //Arrange
            int statusCode = 0;
            _mockGL.Setup(m => m.GetProgram(_shaderProgramID, GetProgramParameterName.LinkStatus, out statusCode));
            _mockGL.Setup(m => m.GetProgramInfoLog(_shaderProgramID)).Returns("Program Linking Error");
            _mockGL.Setup(m => m.LinkProgramSuccess(_shaderProgramID)).Returns(false);

            SetupVertexShaderFileMock();
            SetupFragmentShaderFileMock();

            //Act & Assert
            AssertHelpers.ThrowsWithMessage<Exception>(() =>
            {
                var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            }, $"Error occurred while linking program with ID '{_shaderProgramID}'\nProgram Linking Error");
        }

        [Fact]
        public void UseProgram_WhenInvoked_SetsProgramForUse()
        {
            //Arrange
            var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            //Act
            program.UseProgram();

            //Assert
            _mockGL.Verify(m => m.UseProgram(_shaderProgramID), Times.Once());
        }

        [Fact]
        public void Dispose_WithUnmanagedResourcesToDispose_DeletesProgram()
        {
            //Arrange
            var program = new ShaderProgram(_mockGL.Object, _mockTextFile.Object);

            //Act
            program.Dispose();
            program.Dispose();

            //Assert
            _mockGL.Verify(m => m.DeleteProgram(_shaderProgramID), Times.Once());
        }

        /// <summary>
        /// Sets up the vertex shader file mock
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void SetupVertexShaderFileMock()
        {
            _mockTextFile.Setup(m => m.LoadAsLines(_vertexShaderPath)).Returns(() =>
            {
                return new[] { "layout(location = 3) in float aTransformIndex;", "uniform mat4 uTransform[1];//$REPLACE_INDEX" };
            });
        }

        /// <summary>
        /// Sets up the fragment shader file mock.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void SetupFragmentShaderFileMock()
        {
            _mockTextFile.Setup(m => m.LoadAsLines(_fragShaderPath)).Returns(() =>
            {
                return new[] { "in vec2 v_TexCoord;", "in vec4 v_TintClr;" };
            });
        }
    }
}
