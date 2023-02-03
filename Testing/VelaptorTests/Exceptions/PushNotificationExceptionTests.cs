// <copyright file="PushNotificationExceptionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Exceptions;

using System;
using FluentAssertions;
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
        exception.Message.Should().Be("There was an issue with the push notification.");
    }

    [Fact]
    public void Ctor_WhenInvokedWithSingleMessageParam_CorrectlySetsMessage()
    {
        // Act
        var exception = new PushNotificationException("test-message");

        // Assert
        exception.Message.Should().Be("test-message");
    }

    [Fact]
    public void Ctor_WhenInvokedWithMessageAndInnerException_ThrowsException()
    {
        // Arrange
        var innerException = new Exception("inner-exception");

        // Act
        var deviceException = new PushNotificationException("test-exception", innerException);

        // Assert
        deviceException.InnerException.Message.Should().Be("inner-exception");
        deviceException.Message.Should().Be("test-exception");
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
        deviceException.Message.Should().Be(expected);
        deviceException.InnerException.Should().BeNull();
    }
    #endregion
}
