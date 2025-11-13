// <copyright file="IAudioLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// Loads audio data.
/// </summary>
internal interface IAudioLoader : IUnloader<IAudio>
{
    /// <summary>
    /// Gets the total number of cached textures.
    /// </summary>
    int TotalCachedItems { get; }

#pragma warning disable SA1629 // Documentation text should end with a period.
    /// <summary>
    /// Loads the audio with the given name.
    /// </summary>
    /// <param name="audioPathOrName">The full file path or name of the audio to load.</param>
    /// <param name="bufferType">The type of buffer to use.</param>
    /// <returns>The loaded audio.</returns>
    /// <code>
    /// audioLoader.Load("my-atlas", AudioBuffer.Full);
    /// // or
    /// audioLoader.Load("C:/content/my-atlas.png", AudioBuffer.Stream);
    /// </code>
    IAudio Load(string audioPathOrName, AudioBuffer bufferType);
#pragma warning restore SA1629 // Documentation text should end with a period.
}
