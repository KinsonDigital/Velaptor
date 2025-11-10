// <copyright file="AudioTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1202

namespace VelaptorTests.Content;

using System;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using CASL;
using Helpers;
using NSubstitute;
using Shouldly;
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
    private const uint AudioId = 123;
    private readonly IPushReactable<DisposeAudioData> mockDisposeReactable;
    private readonly ICASLAudio mockCASLAudio;
    private IReceiveSubscription<DisposeAudioData>? mockDisposeReactableSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioTests"/> class.
    /// </summary>
    public AudioTests()
    {
        this.mockDisposeReactable = Substitute.For<IPushReactable<DisposeAudioData>>();
        this.mockDisposeReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()))
            .Do((callInfo) =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<DisposeAudioData>>();

                if (subscription.Id == PushNotifications.AudioDisposedId)
                {
                    this.mockDisposeReactableSubscription = subscription;
                }
            });

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
            _ = new Audio(null, this.mockCASLAudio, AudioId);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'disposeReactable')");
    }

    [Fact]
    [Trait("Category", Ctor)]
    public void Ctor_WithNullInternalAudioParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Audio(this.mockDisposeReactable, null, AudioId);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'internalAudio')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SetsAudioId()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Id;

        // Assert
        actual.ShouldBe(AudioId);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Volume_WhenSettingValueWhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act && Assert
        var exception = Should.Throw<ObjectDisposedException>(() => sut.Volume = 123);
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void Volume_WhenGettingValueWhileDisposed_ReturnsZero()
    {
        // Arrange
        this.mockCASLAudio.Volume.Returns(123);
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.Volume;

        // Assert
        actual.ShouldBe(0f);
    }

    [Fact]
    public void Volume_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Volume = 123;

        // Assert
        sut.Volume.ShouldBe(123);
    }

    [Fact]
    public void Position_WhenGettingValueWhileDisposed_ReturnsEmptyTimeSpan()
    {
        // Arrange
        this.mockCASLAudio.Position.Returns(new AudioTime(0.123f));
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.Position;

        // Assert
        actual.ShouldBe(TimeSpan.Zero);
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
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Length_WhenGettingValueWhileDisposed_ReturnsEmptyTimeSpan()
    {
        // Arrange
        this.mockCASLAudio.Length.Returns(new AudioTime(0.123f));
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.Length;

        // Assert
        actual.ShouldBe(TimeSpan.Zero);
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
        actual.ShouldBe(expected);
    }

    [Fact]
    public void IsLooping_WhenSettingValueWhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act && Assert
        var exception = Should.Throw<ObjectDisposedException>(() => sut.IsLooping = true);

        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void IsLooping_WhenGettingValueWhileDisposed_ReturnsFalse()
    {
        // Arrange
        this.mockCASLAudio.IsLooping.Returns(true);
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.IsLooping;

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void IsLooping_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.IsLooping = true;

        // Assert
        sut.IsLooping.ShouldBeTrue();
    }

    [Fact]
    public void IsPlaying_WhenGettingValueWhileDisposed_ReturnsFalse()
    {
        // Arrange
        this.mockCASLAudio.State.Returns(AudioState.Playing);
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.IsPlaying;

        // Assert
        actual.ShouldBeFalse();
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
        actual.ShouldBe(expected);
    }

    [Fact]
    public void IsPaused_WhenGettingValueWhileDisposed_ReturnsFalse()
    {
        // Arrange
        this.mockCASLAudio.State.Returns(AudioState.Paused);
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.IsPaused;

        // Assert
        actual.ShouldBeFalse();
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
        actual.ShouldBe(expected);
    }

    [Fact]
    public void IsStopped_WhenGettingValueWhileDisposed_ReturnsTrue()
    {
        // Arrange
        this.mockCASLAudio.State.Returns(AudioState.Playing);
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.IsStopped;

        // Assert
        actual.ShouldBeTrue();
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
        actual.ShouldBe(expected);
    }

    [Fact]
    public void BufferType_WhenGettingValueWhileDisposed_ReturnsAsFull()
    {
        // Arrange
        this.mockCASLAudio.BufferType.Returns(BufferType.Stream);
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.BufferType;

        // Assert
        actual.ShouldBe(AudioBuffer.Full);
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
        actual.ShouldBe(expected);
    }

    [Fact]
    public void PlaySpeed_WhenSettingValueWhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act && Assert
        var exception = Should.Throw<ObjectDisposedException>(() => sut.PlaySpeed = 123);
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
    }

    [Fact]
    public void PlaySpeed_WhenGettingValueWhileDisposed_ReturnsZero()
    {
        // Arrange
        this.mockCASLAudio.PlaySpeed.Returns(123);
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.PlaySpeed;

        // Assert
        actual.ShouldBe(0);
    }

    [Fact]
    public void PlaySpeed_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.PlaySpeed = 123;

        // Assert
        sut.PlaySpeed.ShouldBe(123);
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
        actual.ShouldBe("test-name");
    }

    [Fact]
    public void Name_WhenGettingValueWhileDisposed_ReturnsEmpty()
    {
        // Arrange
        this.mockCASLAudio.Name.Returns("test-name");
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.Name;

        // Assert
        actual.ShouldBeEmpty();
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
        actual.ShouldBe("test-path");
    }

    [Fact]
    public void FilePath_WhenGettingValueWhileDisposed_ReturnsEmpty()
    {
        // Arrange
        this.mockCASLAudio.FilePath.Returns("test-path");
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var actual = sut.FilePath;

        // Assert
        actual.ShouldBeEmpty();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void FastForward_WhileDisposed_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var act = () => sut.FastForward(123);

        // Assert
        var exception = act.ShouldThrow<ObjectDisposedException>();
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
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
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var act = () => sut.Pause();

        // Assert
        var exception = act.ShouldThrow<ObjectDisposedException>();
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
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
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var act = () => sut.Play();

        // Assert
        var exception = act.ShouldThrow<ObjectDisposedException>();
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
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
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var act = () => sut.FastForward(123);

        // Assert
        var exception = act.ShouldThrow<ObjectDisposedException>();
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
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
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var act = () => sut.SetTimePosition(123);

        // Assert
        var exception = act.ShouldThrow<ObjectDisposedException>();
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
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
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

        // Act
        var act = () => sut.Stop();

        // Assert
        var exception = act.ShouldThrow<ObjectDisposedException>();
        exception.Message.ShouldBe($"Cannot access a disposed object.{Environment.NewLine}Object name: 'Velaptor.Content.Audio'.");
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
        _ = CreateSystemUnderTest();

        // Act
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });
        this.mockDisposeReactableSubscription.OnReceive(new  DisposeAudioData { AudioId = AudioId });

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
        this.mockDisposeReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<DisposeAudioData>>();

                subscription.Id.ShouldBe(PushNotifications.AudioDisposedId);
                subscription.Name.ShouldBe($"Audio.ctor() - {PushNotifications.AudioDisposedId}");
            });

        // Act
        _ = CreateSystemUnderTest();
    }

    [Fact]
    public void DisposeReactable_WhenDisposingOfReactable_InvokesUnsubscriber()
    {
        // Arrange
        var mockUnsubscriber = Substitute.For<IDisposable>();

        this.mockDisposeReactable.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()).Returns(mockUnsubscriber);
        this.mockDisposeReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription<DisposeAudioData>>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<DisposeAudioData>>();

                if (subscription.Id == PushNotifications.AudioDisposedId)
                {
                    this.mockDisposeReactableSubscription = subscription;
                }
            });

        _ = CreateSystemUnderTest();

        // Act
        this.mockDisposeReactableSubscription.OnUnsubscribe();

        // Assert
        mockUnsubscriber.Received(1).Dispose();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="Audio"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Audio CreateSystemUnderTest()
        => new (this.mockDisposeReactable, this.mockCASLAudio, AudioId);
}
