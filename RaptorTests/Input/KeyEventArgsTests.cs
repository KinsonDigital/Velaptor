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
            var expected = new KeyCodes[]
            {
                KeyCodes.Left,
                KeyCodes.Right
            };

            //Act
            var eventArgs = new KeyEventArgs(new KeyCodes[] { KeyCodes.Left, KeyCodes.Right });
            var actual = eventArgs.Keys;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion


        #region Prop Tests
        [Fact]
        public void Keys_WhenSettingAndGettingValue_ReturnsCorrectValue()
        {
            //Arrange
            var expected = new KeyCodes[]
            {
                KeyCodes.Up,
                KeyCodes.Down
            };

            //Act
            var eventArgs = new KeyEventArgs(new KeyCodes[] { KeyCodes.Left, KeyCodes.Right })
            {
                Keys = new KeyCodes[] { KeyCodes.Up, KeyCodes.Down }
            };
            var actual = eventArgs.Keys;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
