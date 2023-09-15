// <copyright file="SoundFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Factories;

using System;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using FluentAssertions;
using Moq;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="SoundFactory"/> class.
/// </summary>
public class SoundFactoryTests
{
    private readonly Mock<IDisposable> mockDisposeSoundUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveSubscription? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFactoryTests"/> class.
    /// </summary>
    public SoundFactoryTests()
    {
        this.mockDisposeSoundUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Returns<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                return this.mockShutDownUnsubscriber.Object;
            })
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.shutDownReactor = reactor;
            });

        var mockDisposeSoundReactable = new Mock<IPushReactable<DisposeSoundData>>();
        mockDisposeSoundReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<DisposeSoundData>>()))
            .Returns<IReceiveSubscription<DisposeSoundData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                return this.mockDisposeSoundUnsubscriber.Object;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();

        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateDisposeSoundReactable()).Returns(mockDisposeSoundReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new SoundFactory(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetNewId_WhenInvoked_AddsSoundIdAndPathToList()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.GetNewId("test-file");

        // Assert
        actual.Should().Be(1);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SoundFactory"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SoundFactory CreateSystemUnderTest()
        => new (this.mockReactableFactory.Object);
}
