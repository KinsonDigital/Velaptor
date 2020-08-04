// <copyright file="OggSoundDecoder.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    /// <summary>
    /// Decodes OGG audio data files.
    /// </summary>
    internal class OggSoundDecoder : ISoundDecoder<float>
    {
        private readonly IAudioDataStream<float> audioDataStream;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="OggSoundDecoder"/> class.
        /// </summary>
        /// <param name="dataStream">Streams the audio data from the file as floats.</param>
        public OggSoundDecoder(IAudioDataStream<float> dataStream) => this.audioDataStream = dataStream;

        /// <summary>
        /// Loads ogg audio data from an ogg file using the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The file name/path to the ogg file.</param>
        /// <returns>The sound and related audio data.</returns>
        public SoundData<float> LoadData(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("The param must not be null or empty.", nameof(fileName));

            if (Path.GetExtension(fileName) != ".ogg")
                throw new ArgumentException("The file name must have an ogg file extension.", nameof(fileName));

            var result = default(SoundData<float>);

            this.audioDataStream.Filename = fileName;

            result.SampleRate = this.audioDataStream.SampleRate;
            result.Channels = this.audioDataStream.Channels;

            var dataResult = new List<float>();

            var buffer = new float[this.audioDataStream.Channels * this.audioDataStream.SampleRate];

            while (this.audioDataStream.ReadSamples(buffer, 0, buffer.Length) > 0)
            {
                dataResult.AddRange(buffer);
            }

            result.TotalSeconds = dataResult.Count / 4f / this.audioDataStream.SampleRate;

            switch (this.audioDataStream.Channels)
            {
                case 1:
                    result.Format = AudioFormat.Mono32Float;
                    break;
                case 2:
                    result.Format = AudioFormat.StereoFloat32;
                    break;
            }

            result.BufferData = new ReadOnlyCollection<float>(dataResult);

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
        /// <param name="disposing">True to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                    this.audioDataStream.Dispose();

                this.isDisposed = true;
            }
        }
    }
}
