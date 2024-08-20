// <copyright file="AudioFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Factories;

using System;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using FluentAssertions;
using NSubstitute;
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
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable<DisposeAudioData> mockDisposeSoundReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioFactoryTests"/> class.
    /// </summary>
    public AudioFactoryTests()
    {
        this.mockDisposeSoundReactable = Substitute.For<IPushReactable<DisposeAudioData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeAudioReactable().Returns(this.mockDisposeSoundReactable);
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

        this.mockDisposeSoundReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()))
            .Do(callInfo =>
            {
                subscription = callInfo.Arg<IReceiveSubscription<DisposeAudioData>>();
            });

        this.mockDisposeSoundReactable.When(x => x.Push(Arg.Any<Guid>(), Arg.Any<DisposeAudioData>()))
            .Do(callInfo =>
            {
                var notificationId = callInfo.Arg<Guid>();
                var data = callInfo.Arg<DisposeAudioData>();

                notificationId.Should().Be(PushNotifications.AudioDisposedId);
                data.AudioId.Should().Be(1u);
            });

        var sut = CreateSystemUnderTest();
        this.mockDisposeSoundReactable.Push(PushNotifications.AudioDisposedId, new DisposeAudioData { AudioId = 1u });

        // Act
        subscription.OnReceive(new DisposeAudioData { AudioId = 1u });

        // Assert
        sut.LoadedAudio.Should().BeEmpty();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="AudioFactory"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private AudioFactory CreateSystemUnderTest() => new (this.mockReactableFactory);
}
