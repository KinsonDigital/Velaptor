// <copyright file="ISoundDecoder.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Audio
{
    using System;

    /// <summary>
    /// Decodes audio data files.
    /// </summary>
    /// <typeparam name="T">The type of audio data being returned.</typeparam>
    internal interface ISoundDecoder<T> : IDisposable
    {
        /// <summary>
        /// Loads audio data from an audio file using the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The file name/path to the audio file.</param>
        /// <returns>The sound and related audio data.</returns>
        SoundData<T> LoadData(string fileName);
    }
}
