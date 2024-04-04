// <copyright file="SoundFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Factories;

using System;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="SoundFactory"/> class.
/// </summary>
public class SoundFactoryTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable<DisposeSoundData>> mockDisposeSoundReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFactoryTests"/> class.
    /// </summary>
    public SoundFactoryTests()
    {
        this.mockDisposeSoundReactable = new Mock<IPushReactable<DisposeSoundData>>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateDisposeSoundReactable())
            .Returns(this.mockDisposeSoundReactable.Object);
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

    #region Reactable Tests
    [Fact]
    public void DisposeSoundReactable_WithDisposeNotification_RemovesSoundReference()
    {
        // Arrange
        IReceiveSubscription<DisposeSoundData>? subscription = null;

        this.mockDisposeSoundReactable
            .Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<DisposeSoundData>>()))
            .Callback<IReceiveSubscription<DisposeSoundData>>((subscriptionParam) =>
            {
                subscriptionParam.Should().NotBeNull();
                subscriptionParam.Id.Should().Be(PushNotifications.SoundDisposedId);
                subscriptionParam.Name.Should().Be("SoundFactoryTests.Ctor - SoundDisposedId");

                subscription = subscriptionParam;
            });

        this.mockDisposeSoundReactable
            .Setup(m => m.Push(It.IsAny<Guid>(), It.IsAny<DisposeSoundData>()))
            .Callback((Guid eventId, in DisposeSoundData data) =>
            {
                data.SoundId.Should().Be(1);
                eventId.Should().Be(PushNotifications.SoundDisposedId);
            });

        var sut = CreateSystemUnderTest();
        this.mockDisposeSoundReactable.Object.Push(PushNotifications.SoundDisposedId, new DisposeSoundData { SoundId = 1 });

        // Act
        subscription.OnReceive(new DisposeSoundData { SoundId = 1 });

        // Assert
        sut.Sounds.Should().BeEmpty();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SoundFactory"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SoundFactory CreateSystemUnderTest() => new (this.mockReactableFactory.Object);
}
