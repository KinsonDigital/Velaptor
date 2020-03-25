using Moq;
using Xunit;
using Raptor.Plugins;
using Raptor;

namespace RaptorTests
{
    /// <summary>
    /// Unit tests to test the <see cref="OnRenderEventArgs"/> class.
    /// </summary>
    public class OnRenderEventArgsTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoking_SetsRendererProp()
        {
            //Arrange
            var mockRenderer = new Mock<IRenderer>();
            var expected = mockRenderer.Object;
            var eventArgs = new OnRenderEventArgs(mockRenderer.Object);

            //Act
            var actual = eventArgs.Renderer;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion


        #region Prop Tests
        [Fact]
        public void Renderer_WhenGettingAndSettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var mockRendererForCtor = new Mock<IRenderer>();
            var eventArgs = new OnRenderEventArgs(mockRendererForCtor.Object);
            var mockRendererForProp = new Mock<IRenderer>();
            var expected = mockRendererForProp.Object;

            //Act
            eventArgs.Renderer = mockRendererForProp.Object;
            var actual = eventArgs.Renderer;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
