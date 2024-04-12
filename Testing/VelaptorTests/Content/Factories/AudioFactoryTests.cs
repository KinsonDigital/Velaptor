// <copyright file="AudioFactoryTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="AudioFactory"/> class.
/// </summary>
public class AudioFactoryTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable<DisposeAudioData>> mockDisposeSoundReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioFactoryTests"/> class.
    /// </summary>
    public AudioFactoryTests()
    {
        this.mockDisposeSoundReactable = new Mock<IPushReactable<DisposeAudioData>>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateDisposeAudioReactable())
            .Returns(this.mockDisposeSoundReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new AudioFactory(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
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
        IReceiveSubscription<DisposeAudioData>? subscription = null;

        this.mockDisposeSoundReactable
            .Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<DisposeAudioData>>()))
            .Callback<IReceiveSubscription<DisposeAudioData>>((subscriptionParam) =>
            {
                subscriptionParam.Should().NotBeNull();
                subscriptionParam.Id.Should().Be(PushNotifications.AudioDisposedId);
                subscriptionParam.Name.Should().Be($"AudioFactory.ctor() - {PushNotifications.AudioDisposedId}");

                subscription = subscriptionParam;
            });

        this.mockDisposeSoundReactable
            .Setup(m => m.Push(It.IsAny<Guid>(), It.IsAny<DisposeAudioData>()))
            .Callback((Guid eventId, in DisposeAudioData data) =>
            {
                data.AudioId.Should().Be(1);
                eventId.Should().Be(PushNotifications.AudioDisposedId);
            });

        var sut = CreateSystemUnderTest();
        this.mockDisposeSoundReactable.Object.Push(PushNotifications.AudioDisposedId, new DisposeAudioData { AudioId = 1 });

        // Act
        subscription.OnReceive(new DisposeAudioData { AudioId = 1 });

        // Assert
        sut.LoadedAudio.Should().BeEmpty();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="AudioFactory"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AudioFactory CreateSystemUnderTest() => new (this.mockReactableFactory.Object);
}
