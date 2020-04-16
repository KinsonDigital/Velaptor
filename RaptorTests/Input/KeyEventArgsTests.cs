using Xunit;
using Raptor.Input;

namespace RaptorTests.Input
{
    /// <summary>
    /// Unit tests to test the <see cref="KeyEventArgs"/> class.
    /// </summary>
    public class KeyEventArgsTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoking_SetsKeysProp()
        {
            //Arrange
            var expected = new KeyCode[]
            {
                KeyCode.Left,
                KeyCode.Right
            };

            //Act
            var eventArgs = new KeyEventArgs(new KeyCode[] { KeyCode.Left, KeyCode.Right });
            var actual = eventArgs.Keys;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion


        #region Prop Tests
        [Fact]
        public void Keys_WhenGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var expected = new KeyCode[]
            {
                KeyCode.Up,
                KeyCode.Down
            };

            //Act
            var eventArgs = new KeyEventArgs(new KeyCode[] { KeyCode.Left, KeyCode.Right });
            var actual = eventArgs.Keys;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
