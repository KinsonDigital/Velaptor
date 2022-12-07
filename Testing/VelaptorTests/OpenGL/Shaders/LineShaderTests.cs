// <copyright file="LineShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Shaders;

using System;
using System.Linq;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Services;
using Velaptor.OpenGL.Shaders;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Xunit;

public class LineShaderTests
{
    private readonly Mock<IReactable<BatchSizeData>> mockBatchSizeReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineShaderTests"/> class.
    /// </summary>
    public LineShaderTests() => this.mockBatchSizeReactable = new Mock<IReactable<BatchSizeData>>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullBatchSizeReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineShader(
                new Mock<IGLInvoker>().Object,
                new Mock<IOpenGLService>().Object,
                new Mock<IShaderLoaderService<uint>>().Object,
                new Mock<IReactable<GLInitData>>().Object,
                null,
                new Mock<IReactable<ShutDownData>>().Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'batchSizeReactable')");
    }

    [Fact]
    public void Ctor_WhenReceivingBatchSizeNotification_SetsBatchSize()
    {
        // Arrange
        IReactor<BatchSizeData>? reactor = null;

        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<BatchSizeData>>()))
            .Callback<IReactor<BatchSizeData>>(reactorObj =>
            {
                if (reactorObj is null)
                {
                    Assert.True(false, "Batch size reactor object cannot be null for test.");
                }

                reactor = reactorObj;
            });

        var shader = CreateSystemUnderTest();

        // Act
        reactor.OnNext(new BatchSizeData(123u));
        var actual = shader.BatchSize;

        // Assert
        actual.Should().Be(123u);
    }

    [Fact]
    public void Ctor_WhenEndingNotifications_Unsubscribes()
    {
        // Arrange
        IReactor<BatchSizeData>? reactor = null;
        var mockUnsubscriber = new Mock<IDisposable>();

        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<BatchSizeData>>()))
            .Callback<IReactor<BatchSizeData>>(reactorObj =>
            {
                if (reactorObj is null)
                {
                    Assert.True(false, "Batch size reactor object cannot be null for test.");
                }

                reactor = reactorObj;
            })
            .Returns<IReactor<BatchSizeData>>(_ => mockUnsubscriber.Object);

        _ = CreateSystemUnderTest();

        // Act
        reactor.OnCompleted();
        reactor.OnCompleted();

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
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineShader CreateSystemUnderTest()
        => new (new Mock<IGLInvoker>().Object,
            new Mock<IOpenGLService>().Object,
            new Mock<IShaderLoaderService<uint>>().Object,
            new Mock<IReactable<GLInitData>>().Object,
            this.mockBatchSizeReactable.Object,
            new Mock<IReactable<ShutDownData>>().Object);
}
