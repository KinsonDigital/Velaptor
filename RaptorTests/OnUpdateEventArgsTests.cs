using Raptor;
using Moq;
using Xunit;
using System;

namespace RaptorTests
{
    /// <summary>
    /// Unit tests to test the <see cref="OnUpdateEventArgs"/> class.
    /// </summary>
    public class OnUpdateEventArgsTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoking_SetsEngineTime()
        {

            //Arrange
            var expected = new FrameTime()
            {
                ElapsedTime = new TimeSpan(1, 2, 3, 4, 5),
                TotalTime = new TimeSpan(6, 7, 8, 9, 10)
            };

            var frameTime = new FrameTime()
            {
                ElapsedTime = new TimeSpan(1, 2, 3, 4, 5),
                TotalTime = new TimeSpan(6, 7, 8, 9, 10)
            };

            var eventArgs = new OnUpdateEventArgs(frameTime);

            //Act
            var actual = eventArgs.EngineTime;

            //Assert
            Assert.Equal(expected, actual);
        }
        #endregion

    }
}
