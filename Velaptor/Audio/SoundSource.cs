// <copyright file="SoundSource.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Audio
{
    /// <summary>
    /// Represents a single OpenAL sound source.
    /// </summary>
    internal struct SoundSource
    {
        /// <summary>
        /// The OpenAL id of the sound source.
        /// </summary>
        public uint SourceId;

        /// <summary>
        /// The total number of seconds of the sound.
        /// </summary>
        public float TotalSeconds;

        /// <summary>
        /// The sample rate of the sound.
        /// </summary>
        /// <remarks>
        ///     This would the be frequency in Hz of how many samples are read per second.
        /// </remarks>
        public int SampleRate;
    }
}
