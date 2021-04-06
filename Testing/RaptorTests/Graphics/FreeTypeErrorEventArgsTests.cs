// <copyright file="FreeTypeErrorEventArgsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.NativeInterop
{
    using Raptor.NativeInterop;
    using Xunit;

    public class FreeTypeErrorEventArgsTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsErrorMessageProperty()
        {
            // Act
            var eventArgs = new FreeTypeErrorEventArgs("test-message");

            // Assert
            Assert.Equal("test-message", eventArgs.ErrorMessage);
        }
        #endregion
    }
}
