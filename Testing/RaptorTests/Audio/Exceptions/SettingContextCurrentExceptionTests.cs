// <copyright file="SettingContextCurrentExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Audio.Exceptions
{
    using System;
    using Raptor.Audio.Exceptions;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SettingContextCurrentException"/> class.
    /// </summary>
    public class SettingContextCurrentExceptionTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvokedWithNoParam_CorrectlySetsMessage()
        {
            // Act
            var exception = new SettingContextCurrentException();

            // Assert
            Assert.Equal("There was an issue setting the audio context as the current context.", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMesage()
        {
            // Act
            var exception = new SettingContextCurrentException("test-message");

            // Assert
            Assert.Equal("test-message", exception.Message);
        }

        [Fact]
        public void Ctor_WhenInvokedWithMessageAndDeviceNameParams_CorrectlySetsMessage()
        {
            // Act
            var innerException = new Exception("inner-exception");
            var exception = new SettingContextCurrentException("outer-exception", innerException);

            // Assert
            Assert.Equal("outer-exception", exception.Message);
            Assert.Equal("inner-exception", exception.InnerException?.Message);
        }
        #endregion
    }
}
