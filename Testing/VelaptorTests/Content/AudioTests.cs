// <copyright file="AudioTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1202

namespace VelaptorTests.Content;

using System;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using CASL;
using FluentAssertions;
using Helpers;
using NSubstitute;
using Velaptor;
using Velaptor.Content;
using Velaptor.ReactableData;
using Xunit;
using Audio = Velaptor.Content.Audio;
using ICASLAudio = CASL.IAudio;

/// <summary>
/// Tests the <see cref="Velaptor.Content.Audio"/> class.
/// </summary>
public class AudioTests : TestsBase
{
    private readonly IPushReactable<DisposeAudioData> mockDisposeReactable;
    private readonly ICASLAudio mockCASLAudio;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioTests"/> class.
    /// </summary>
    public AudioTests()
    {
        this.mockDisposeReactable = Substitute.For<IPushReactable<DisposeAudioData>>();
        this.mockCASLAudio = Substitute.For<ICASLAudio>();
    }

    #region Constructor Tests
    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullDisposeReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Audio(null, this.mockCASLAudio, 1);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'disposeReactable')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullInternalAudioParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Audio(this.mockDisposeReactable, null, 1);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'internalAudio')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WhenInvoked_CorrectlySetsIdProperty()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest(123);

        var actual = sut.Id;

        // Assert
        actual.Should().Be(123);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Volume_WhenSettingValueWhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.Volume = 123;

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void Volume_WhenGettingValueWhileDisposed_ReturnsZero()
    {
        // Arrange
        this.mockCASLAudio.Volume.Returns(123);
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.Volume;

        // Assert
        actual.Should().Be(0f);
    }

    [Fact]
    public void Volume_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Volume = 123;

        // Assert
        sut.Volume.Should().Be(123);
    }

    [Fact]
    public void Position_WhenGettingValueWhileDisposed_ReturnsEmptyTimeSpan()
    {
        // Arrange
        this.mockCASLAudio.Position.Returns(new AudioTime(0.123f));
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.Position;

        // Assert
        actual.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void Position_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockCASLAudio.Position.Returns(new AudioTime(0.123f));
        var expected = new TimeSpan(0, 0, 0, 0, 123);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Position;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Length_WhenGettingValueWhileDisposed_ReturnsEmptyTimeSpan()
    {
        // Arrange
        this.mockCASLAudio.Length.Returns(new AudioTime(0.123f));
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.Length;

        // Assert
        actual.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void Length_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockCASLAudio.Length.Returns(new AudioTime(0.123f));
        var expected = new TimeSpan(0, 0, 0, 0, 123);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Length;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void IsLooping_WhenSettingValueWhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.IsLooping = true;

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void IsLooping_WhenGettingValueWhileDisposed_ReturnsFalse()
    {
        // Arrange
        this.mockCASLAudio.IsLooping.Returns(true);
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.IsLooping;

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsLooping_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.IsLooping = true;

        // Assert
        sut.IsLooping.Should().BeTrue();
    }

    [Fact]
    public void IsPlaying_WhenGettingValueWhileDisposed_ReturnsFalse()
    {
        // Arrange
        this.mockCASLAudio.State.Returns(AudioState.Playing);
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.IsPlaying;

        // Assert
        actual.Should().BeFalse();
    }

    [Theory]
    [InlineData(AudioState.Playing, true)]
    [InlineData(AudioState.Paused, false)]
    [InlineData(AudioState.Stopped, false)]
    internal void IsPlaying_WhenGettingValue_ReturnsCorrectResult(AudioState state, bool expected)
    {
        // Arrange
        this.mockCASLAudio.State.Returns(state);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.IsPlaying;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void IsPaused_WhenGettingValueWhileDisposed_ReturnsFalse()
    {
        // Arrange
        this.mockCASLAudio.State.Returns(AudioState.Paused);
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.IsPaused;

        // Assert
        actual.Should().BeFalse();
    }

    [Theory]
    [InlineData(AudioState.Playing, false)]
    [InlineData(AudioState.Paused, true)]
    [InlineData(AudioState.Stopped, false)]
    internal void IsPaused_WhenGettingValue_ReturnsCorrectResult(AudioState state, bool expected)
    {
        // Arrange
        this.mockCASLAudio.State.Returns(state);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.IsPaused;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void IsStopped_WhenGettingValueWhileDisposed_ReturnsTrue()
    {
        // Arrange
        this.mockCASLAudio.State.Returns(AudioState.Playing);
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.IsStopped;

        // Assert
        actual.Should().BeTrue();
    }

    [Theory]
    [InlineData(AudioState.Playing, false)]
    [InlineData(AudioState.Paused, false)]
    [InlineData(AudioState.Stopped, true)]
    internal void IsStopped_WhenGettingValue_ReturnsCorrectResult(AudioState state, bool expected)
    {
        // Arrange
        this.mockCASLAudio.State.Returns(state);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.IsStopped;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void BufferType_WhenGettingValueWhileDisposed_ReturnsAsFull()
    {
        // Arrange
        this.mockCASLAudio.BufferType.Returns(BufferType.Stream);
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.BufferType;

        // Assert
        actual.Should().Be(AudioBuffer.Full);
    }

    [Theory]
    [InlineData(BufferType.Full, AudioBuffer.Full)]
    [InlineData(BufferType.Stream, AudioBuffer.Stream)]
    internal void BufferType_WhenGettingValue_ReturnsCorrectResult(BufferType bufferType, AudioBuffer expected)
    {
        // Arrange
        this.mockCASLAudio.BufferType.Returns(bufferType);
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.BufferType;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void PlaySpeed_WhenSettingValueWhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.PlaySpeed = 123;

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void PlaySpeed_WhenGettingValueWhileDisposed_ReturnsZero()
    {
        // Arrange
        this.mockCASLAudio.PlaySpeed.Returns(123);
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.PlaySpeed;

        // Assert
        actual.Should().Be(0);
    }

    [Fact]
    public void PlaySpeed_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.PlaySpeed = 123;

        // Assert
        sut.PlaySpeed.Should().Be(123);
    }

    [Fact]
    public void Name_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockCASLAudio.Name.Returns("test-name");
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Name;

        // Assert
        actual.Should().Be("test-name");
    }

    [Fact]
    public void Name_WhenGettingValueWhileDisposed_ReturnsEmpty()
    {
        // Arrange
        this.mockCASLAudio.Name.Returns("test-name");
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.Name;

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void FilePath_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockCASLAudio.FilePath.Returns("test-path");
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.FilePath;

        // Assert
        actual.Should().Be("test-path");
    }

    [Fact]
    public void FilePath_WhenGettingValueWhileDisposed_ReturnsEmpty()
    {
        // Arrange
        this.mockCASLAudio.FilePath.Returns("test-path");
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var actual = sut.FilePath;

        // Assert
        actual.Should().BeEmpty();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void FastForward_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.FastForward(123);

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void FastForward_WhenInvoked_MovesAudioPositionForward()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.FastForward(123);

        // Assert
        this.mockCASLAudio.Received(1).FastForward(123);
    }

    [Fact]
    public void Pause_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.Pause();

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void Pause_WhenInvoked_PausesAudio()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Pause();

        // Assert
        this.mockCASLAudio.Received(1).Pause();
    }

    [Fact]
    public void Play_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.Play();

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void Play_WhenInvoked_PlaysAudio()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Play();

        // Assert
        this.mockCASLAudio.Received(1).Play();
    }

    [Fact]
    public void Rewind_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.FastForward(123);

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void Rewind_WhenInvoked_MovesAudioPositionBackwards()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Rewind(123);

        // Assert
        this.mockCASLAudio.Received(1).Rewind(123);
    }

    [Fact]
    public void SetTimePosition_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.SetTimePosition(123);

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void SetTimePosition_WhenInvoked_SetsAudioToCorrectPosition()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.SetTimePosition(123);

        // Assert
        this.mockCASLAudio.Received(1).SetTimePosition(123);
    }

    [Fact]
    public void Stop_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Dispose();

        // Act
        var act = () => sut.Stop();

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage("Cannot access a disposed object.\nObject name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void Stop_WhenInvoked_StopsAudio()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Stop();

        // Assert
        this.mockCASLAudio.Received(1).Reset();
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfAudio()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockCASLAudio.Received(1).Dispose();
    }
    #endregion

    #region Reactable Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void DisposeReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Assert
        this.mockDisposeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<DisposeAudioData>>();

                subscription.Id.Should().Be(PushNotifications.AudioDisposedId);
                subscription.Name.Should().Be($"Audio.ctor() - {PushNotifications.AudioDisposedId}");
            });

        // Act
        _ = CreateSystemUnderTest();
    }

    [Fact]
    public void DisposeReactable_WhenSendingDisposeNotification_DisposesOfAudio()
    {
        // Arrange
        IReceiveSubscription<DisposeAudioData>? subscription = null;
        this.mockDisposeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()))
            .Do(callInfo =>
            {
                subscription = callInfo.Arg<IReceiveSubscription<DisposeAudioData>>();
            });

        _ = CreateSystemUnderTest(123);

        // Act
        subscription.OnReceive(new DisposeAudioData { AudioId = 123 });
        subscription.OnReceive(new DisposeAudioData { AudioId = 123 });

        // Assert
        this.mockCASLAudio.Received(1).Dispose();
    }

    [Fact]
    public void DisposeReactable_WhenSendingDisposeNotificationWithIncorrectId_DoesNotDisposesOfAudio()
    {
        // Arrange
        IReceiveSubscription<DisposeAudioData>? subscription = null;
        this.mockDisposeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()))
            .Do(callInfo =>
            {
                subscription = callInfo.Arg<IReceiveSubscription<DisposeAudioData>>();
            });

        _ = CreateSystemUnderTest(123);

        // Act
        subscription.OnReceive(new DisposeAudioData { AudioId = 456 });

        // Assert
        this.mockCASLAudio.DidNotReceive().Dispose();
    }

    [Fact]
    public void DisposeReactable_WhenDisposingOfReactable_InvokesUnsubscriber()
    {
        // Arrange
        IReceiveSubscription<DisposeAudioData>? subscription = null;
        var mockUnsubscriber = Substitute.For<IDisposable>();

        this.mockDisposeReactable.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()).Returns(mockUnsubscriber);
        this.mockDisposeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()))
            .Do(callInfo =>
            {
                subscription = callInfo.Arg<IReceiveSubscription<DisposeAudioData>>();
            });

        _ = CreateSystemUnderTest(123);

        // Act
        subscription.OnUnsubscribe();

        // Assert
        mockUnsubscriber.Received(1).Dispose();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Audio"/> for the purpose of testing.
    /// </summary>
    /// <param name="audioId">The audio id used for testing.</param>
    /// <returns>The instance to test.</returns>
    private Audio CreateSystemUnderTest(uint audioId = 1)
        => new (this.mockDisposeReactable, this.mockCASLAudio, audioId);
}
