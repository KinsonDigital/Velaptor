// <copyright file="MP3SoundDecoder.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Audio
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Decodes MP3 audio data files.
    /// </summary>
    internal class MP3SoundDecoder : ISoundDecoder<byte>
    {
        private readonly IAudioDataStream<byte> audioDataStream;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MP3SoundDecoder"/> class.
        /// </summary>
        /// <param name="dataStream">Streams the audio data from the file as bytes.</param>
        public MP3SoundDecoder(IAudioDataStream<byte> dataStream) => this.audioDataStream = dataStream;

        /// <summary>
        /// Loads mp3 audio data from an mp3 file using the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The file name/path to the mp3 file.</param>
        /// <returns>The sound and related audio data.</returns>
        public SoundData<byte> LoadData(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("The param must not be null or empty.", nameof(fileName));

            if (Path.GetExtension(fileName) != ".mp3")
                throw new ArgumentException("The file name must have an mp3 file extension.", nameof(fileName));

            SoundData<byte> result = default;

            this.audioDataStream.Filename = fileName;

            result.SampleRate = this.audioDataStream.SampleRate;
            result.Channels = this.audioDataStream.Channels;

            var dataResult = new List<byte>();

            const byte bitsPerSample = 16;
            const byte bytesPerSample = bitsPerSample / 8;

            var buffer = new byte[this.audioDataStream.Channels * this.audioDataStream.SampleRate * bytesPerSample];

            while (this.audioDataStream.ReadSamples(buffer, 0, buffer.Length) > 0)
            {
                dataResult.AddRange(buffer);
            }

            // This calculate is also not completely accurate.  It comes out to 1 second longer
            // then the sound actually is.  This might not be the case every time due to depending on the values in the calculation.
            result.TotalSeconds = dataResult.Count / 4f / this.audioDataStream.SampleRate;

            result.Format = this.audioDataStream.Format;

            result.BufferData = dataResult.ToArray();

            return result;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True if the managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.audioDataStream.Dispose();
                }

                this.isDisposed = true;
            }
        }
    }
}
