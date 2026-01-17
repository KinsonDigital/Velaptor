// <copyright file="AudioFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Carbonate.OneWay;
using ReactableData;
using Velaptor.Factories;
using CASLAudio = CASL.Audio;

/// <summary>
/// Creates audio based on the audio file at a location.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
internal sealed class AudioFactory : IAudioFactory
{
    private readonly IPushReactable<DisposeAudioData> disposeReactable;
    private uint currentId = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioFactory"/> class.
    /// </summary>
    public AudioFactory()
    {
        var reactableFactory = IoC.Container.GetInstance<IReactableFactory>();
        this.disposeReactable = reactableFactory.CreateDisposeAudioReactable();
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the CASL library.")]
    public IAudio Create(string filePath, AudioBuffer bufferType)
    {
        var audioBufferType = bufferType switch
        {
            AudioBuffer.Full => CASL.BufferType.Full,
            AudioBuffer.Stream => CASL.BufferType.Stream,
            _ => throw new InvalidEnumArgumentException(nameof(bufferType), (int)bufferType, typeof(AudioBuffer)),
        };

        var newId = ++this.currentId;
        var caslAudio = new CASLAudio(filePath, audioBufferType);

        return new Audio(this.disposeReactable, caslAudio, newId);
    }
}
