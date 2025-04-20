// <copyright file="PushNotificationExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions;

using System;
using Shouldly;
using Velaptor.Exceptions;
using Xunit;

/// <summary>
/// Tests the <see cref="PushNotificationException"/> class.
/// </summary>
public class PushNotificationExceptionTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithNoParam_CorrectlySetsExceptionMessage()
    {
        // Act
        var exception = new PushNotificationException();

        // Assert
        exception.Message.ShouldBe("There was an issue with the push notification.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new PushNotificationException("test-message");

        // Assert
        exception.Message.ShouldBe("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new PushNotificationException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.ShouldBe("inner-exception");
        deviceException.Message.ShouldBe("test-exception");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSubscriberSourceAndSubscriptionID_ThrowsException()
    {
        // Arrange
        const string subscriberSrc = "test-source";
        var subscriptionId = Guid.NewGuid();

        var expected = $"There was an issue with the '{subscriberSrc}' subscription source for subscription ID '{subscriptionId}'.";

        // Act
        var deviceException = new PushNotificationException(subscriberSrc, subscriptionId);

        // Assert
        deviceException.Message.ShouldBe(expected);
        deviceException.InnerException.ShouldBeNull();
    }
    #endregion
}
