using Moq;
using Xunit;
using KDScorpionCore.Graphics;
using KDScorpionCore.Plugins;
using KDScorpionCore;
using System;

namespace KDScorpionCoreTests.Graphics
{
    /// <summary>
    /// Unit tests to test the <see cref="Renderer"/> class.
    /// </summary>
    public class RendererTests : IDisposable
    {
        #region Private Fields
        private Texture _texture;
        private readonly Mock<IDebugDraw> _mockDebugDraw;
        private readonly GameText _gameText;
        #endregion


        #region Constructors
        public RendererTests()
        {
            var mockTexture = new Mock<ITexture>();

            _texture = new Texture(mockTexture.Object);

            var mockText = new Mock<IText>();
            mockText.SetupGet(m => m.Color).Returns(new GameColor(11, 22, 33, 44));

            _mockDebugDraw = new Mock<IDebugDraw>();

            _gameText = new GameText()
            {
                InternalText = mockText.Object
            };
        }
        #endregion


        #region Method Tests
        [Fact]
        public void Render_WhenUsingTextureAndXAndY_InvokesInteralRenderMethod()
        {
            //Arrange
            var mockTexture = new Mock<ITexture>();
            var mockRenderer = new Mock<IRenderer>();

            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Assert
            renderer.Render(_texture, It.IsAny<float>(), It.IsAny<float>());
            mockRenderer.Verify(m => m.Render(_texture.InternalTexture, It.IsAny<float>(), It.IsAny<float>()), Times.Once());
        }


        [Fact]
        public void Render_WhenUsingTextureAndXAndYAndAngle_InvokesInteralRenderMethod()
        {
            //Arrange
            var mockTexture = new Mock<ITexture>();
            var mockRenderer = new Mock<IRenderer>();

            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Assert
            renderer.Render(_texture, It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>());
            mockRenderer.Verify(m => m.Render(_texture.InternalTexture, It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>()), Times.Once());
        }


        [Fact]
        public void Render_WhenUsingTextureAndVector_InvokesInteralRenderMethod()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
           
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.Render(_texture, It.IsAny<Vector>());

            //Assert
            mockRenderer.Verify(m => m.Render(It.IsAny<ITexture>(), It.IsAny<float>(), It.IsAny<float>()), Times.Once());
        }


        [Fact]
        public void Render_WhenUsingGameTextAndXAndY_InternalRenderMethodInvoked()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Assert
            renderer.Render(_gameText, It.IsAny<float>(), It.IsAny<float>());
            mockRenderer.Verify(m => m.Render(_gameText.InternalText, It.IsAny<float>(), It.IsAny<float>()), Times.Once());
        }


        [Fact]
        public void Render_WhenUsingGameTextAndXAndYAndGameColor_InternalRenderMethodInvoked()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.Render(_gameText, It.IsAny<float>(), It.IsAny<float>(), It.IsAny<GameColor>());

            //Assert
            mockRenderer.Verify(m => m.Render(_gameText.InternalText, It.IsAny<float>(), It.IsAny<float>(), It.IsAny<GameColor>()), Times.Once());
        }


        [Fact]
        public void Render_WhenInvokingSixParamOverload_InvokesInternalMethod()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.Render(_texture, It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<GameColor>());

            //Assert
            mockRenderer.Verify(m => m.Render(It.IsAny<ITexture>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<GameColor>()), Times.Once());
        }


        [Fact]
        public void Render_WhenInvokingWithTextPositionAndColor_InvokesInternalRenderMethod()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.Render(_gameText, It.IsAny<Vector>(), It.IsAny<GameColor>());

            //Assert
            mockRenderer.Verify(m => m.Render(It.IsAny<IText>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<GameColor>()), Times.Once());
        }


        [Fact]
        public void Render_WhenInvokingWithTextAndPosition_InvokesInternalRenderMethod()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.Render(_gameText, It.IsAny<Vector>());

            //Assert
            mockRenderer.Verify(m => m.Render(It.IsAny<IText>(), It.IsAny<float>(), It.IsAny<float>()), Times.Once());
        }


        [Fact]
        public void Line_WhenInvoking_InvokesInternalRenderer()
        {
            //Act
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.Line(It.IsAny<Vector>(), It.IsAny<Vector>(), It.IsAny<GameColor>());

            //Assert
            mockRenderer.Verify(m => m.RenderLine(It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<GameColor>()), Times.Once());
        }


        [Fact]
        public void FillRect_WhenInvoking_InvokesInternalRenderer()
        {
            //Act
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.FillRect(It.IsAny<Rect>(), It.IsAny<GameColor>());

            //Assert
            mockRenderer.Verify(m => m.FillRect(It.IsAny<Rect>(), It.IsAny<GameColor>()), Times.Once());
        }


        [Fact]
        public void Start_WhenInvoking_InvokesInternalRendererStart()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.Begin();

            //Assert
            mockRenderer.Verify(m => m.Begin(), Times.Once());
        }


        [Fact]
        public void Stop_WhenInvoking_InvokesInternalRendererStart()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.End();

            //Assert
            mockRenderer.Verify(m => m.End(), Times.Once());
        }


        [Fact]
        public void Clear_WhenInvoking_InvokesInternalClear()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.Clear(It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>(), It.IsAny<byte>());

            //Assert
            mockRenderer.Verify(m => m.Clear(It.IsAny<GameColor>()), Times.Once());
        }


        [Fact]
        public void FillCircle_WhenInvoking_InvokesInternalFillCircle()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.FillCircle(It.IsAny<Vector>(), It.IsAny<float>(), It.IsAny<GameColor>());

            //Assert
            mockRenderer.Verify(m => m.FillCircle(It.IsAny<float>(), It.IsAny<float>(), It.IsAny<float>(), It.IsAny<GameColor>()), Times.Once());
        }


        [Fact]
        public void RenderTextureArea_WhenInvoked_InvokesInternalRenderTextureAreaMethod()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);
            
            //Act
            renderer.RenderTextureArea(_texture, It.IsAny<Rect>(), It.IsAny<Vector>());

            //Assert
            mockRenderer.Verify(m => m.RenderTextureArea(It.IsAny<ITexture>(), It.IsAny<Rect>(), It.IsAny<float>(), It.IsAny<float>()), Times.Once());
        }


        [Fact]
        public void RenderDebugDraw_WhenInvoked_InvokesDebugDrawMethod()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var renderer = new Renderer(mockRenderer.Object, _mockDebugDraw.Object);

            //Act
            renderer.RenderDebugDraw(It.IsAny<IPhysicsBody>(), It.IsAny<GameColor>());

            //Assert
            _mockDebugDraw.Verify(m => m.Draw(It.IsAny<IRenderer>(), It.IsAny<IPhysicsBody>(), It.IsAny<GameColor>()), Times.Once());
        }
        #endregion


        #region Public Methods
        public void Dispose() => _texture = null;
        #endregion
    }
}
