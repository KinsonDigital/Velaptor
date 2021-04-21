// <copyright file="SoundData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Audio
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Holds data related to a single sound.
    /// </summary>
    /// <typeparam name="T">The type of buffer data of the sound.</typeparam>
    public struct SoundData<T> : IEquatable<SoundData<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundData{T}"/> struct.
        /// </summary>
        /// <param name="bufferData">The buffer data.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="channels">The total number of channels.</param>
        /// <param name="format">The audio format.</param>
        /// <param name="totalSeconds">The total number of seconds of the sound.</param>
        public SoundData(T[] bufferData, int sampleRate, int channels, AudioFormat format, float totalSeconds)
            : this()
        {
            BufferData = new ReadOnlyCollection<T>(bufferData);
            SampleRate = sampleRate;
            Channels = channels;
            Format = format;
            TotalSeconds = totalSeconds;
        }

        /// <summary>
        /// Gets or sets the buffer.
        /// </summary>
        public ReadOnlyCollection<T> BufferData { get; set; }

        /// <summary>
        /// Gets or sets the rate that samples are read in the audio file.
        /// </summary>
        /// <remarks>
        ///     This would the be frequency in Hz of how many samples are read per second.
        /// </remarks>
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets the total number of channels.
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// Gets or sets the audio format of the audio file.
        /// </summary>
        public AudioFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the total number of seconds of the audio file.
        /// </summary>
        public float TotalSeconds { get; set; }

        /// <summary>
        /// Returns a value indicating if the left operand is equal to the right operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns><see langword="true"/> if both operands are equal.</returns>
        public static bool operator ==(SoundData<T> left, SoundData<T> right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating if the left operand is not equal to the right operator.
        /// </summary>
        /// <param name="left">The left side of the operator.</param>
        /// <param name="right">The right side of the operator.</param>
        /// <returns><see langword="true"/> if both operands are not equal.</returns>
        public static bool operator !=(SoundData<T> left, SoundData<T> right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is SoundData<T> data))
            {
                return false;
            }

            return Equals(data);
        }

        /// <inheritdoc/>
        public bool Equals(SoundData<T> other)
            => Enumerable.SequenceEqual<T>(other.BufferData.ToArray(), BufferData.ToArray()) &&
            other.Channels == Channels &&
            other.Format == Format &&
            other.SampleRate == SampleRate &&
            other.TotalSeconds == TotalSeconds;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
            => HashCode.Combine(
                BufferData,
                Channels,
                Format,
                SampleRate,
                TotalSeconds);
    }
}
