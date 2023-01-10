// <copyright file="LineShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using System.Linq;
using Carbonate;
using Carbonate.Core;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;
using Velaptor.ReactableData;
using Xunit;

public class LineShaderTests
{
    private readonly Mock<IPushReactable> mockReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineShaderTests"/> class.
    /// </summary>
    public LineShaderTests() => this.mockReactable = new Mock<IPushReactable>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineShader(
                new Mock<IGLInvoker>().Object,
                new Mock<IOpenGLService>().Object,
                new Mock<IShaderLoaderService<uint>>().Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
    }

    [Fact]
    public void Ctor_WhenReceivingBatchSizeNotification_SetsBatchSize()
    {
        // Arrange
        IReceiveReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");

                reactor = reactorObj;
            });

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 123u });

        var shader = CreateSystemUnderTest();

        // Act
        reactor.OnReceive(mockMessage.Object);
        var actual = shader.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }

    [Fact]
    public void Ctor_WhenEndingNotifications_Unsubscribes()
    {
        // Arrange
        IReceiveReactor? reactor = null;
        var mockUnsubscriber = new Mock<IDisposable>();

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactorObj =>
            {
                if (reactorObj is null)
                {
                    Assert.True(false, "Batch size reactor object cannot be null for test.");
                }

                reactor = reactorObj;
            })
            .Returns<IReceiveReactor>(_ => mockUnsubscriber.Object);

        _ = CreateSystemUnderTest();

        // Act
        reactor.OnUnsubscribe();
        reactor.OnUnsubscribe();

        // Assert
        mockUnsubscriber.VerifyOnce(m => m.Dispose());
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsNameProp()
    {
        // Arrange
        var customAttributes = Attribute.GetCustomAttributes(typeof(LineShader));
        var containsAttribute = customAttributes.Any(i => i is ShaderNameAttribute);

        // Act
        var sut = CreateSystemUnderTest();

        // Assert
        containsAttribute
            .Should()
            .BeTrue($"the '{nameof(ShaderNameAttribute)}' is required on a shader implementation to set the shader name.");
        sut.Name.Should().Be("Line");
    }

    [Fact]
    public void Ctor_WhenBatchSizeNotificationHasAnIssue_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(LineShader)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{NotificationIds.BatchSizeSetId}'.";

        IReceiveReactor? reactor = null;

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");

                reactor = reactorObj;
            });

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => reactor.OnReceive(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expectedMsg);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineShader CreateSystemUnderTest()
        => new (new Mock<IGLInvoker>().Object,
            new Mock<IOpenGLService>().Object,
            new Mock<IShaderLoaderService<uint>>().Object,
            this.mockReactable.Object);
}
