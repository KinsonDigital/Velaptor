// <copyright file="OggAudioDataStream.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Audio
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using NVorbis;
    using Velaptor.Exceptions;
    using Velaptor.NativeInterop.OpenAL;

    /// <summary>
    /// Streams ogg audio data from a ogg file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class OggAudioDataStream : IAudioDataStream<float>
    {
        private VorbisReader? vorbisReader;
        private string? fileName;
        private bool isDisposed;

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <remarks>
        ///     The <see cref="ReadSamples(float[], int, int)"/> method will
        ///     return 0 samples used if this is null or empty.
        /// </remarks>
        public string Filename
        {
            get => this.fileName is null ? string.Empty : this.fileName;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new StringNullOrEmptyException();
                }

                if (!File.Exists(value))
                {
                    throw new FileNotFoundException($"The file '{value}' was not found or does not exist", value);
                }

                if (this.vorbisReader is null)
                {
                    this.vorbisReader = new VorbisReader(value);
                }
                else
                {
                    if (value != this.fileName)
                    {
                        this.vorbisReader.Dispose();
                        this.vorbisReader = new VorbisReader(value);
                    }
                }

                this.fileName = value;
            }
        }

        /// <inheritdoc/>
        public int Channels => string.IsNullOrEmpty(this.fileName) ? 0 : this.vorbisReader?.Channels ?? 0;

        /// <inheritdoc/>
        public ALFormat Format
        {
            get
            {
                if (string.IsNullOrEmpty(this.fileName) || this.vorbisReader is null)
                {
                    return default;
                }

                return Channels == 1 ? ALFormat.MonoFloat32 : ALFormat.StereoFloat32;
            }
        }

        /// <inheritdoc/>
        public int SampleRate => string.IsNullOrEmpty(this.fileName) ? 0 : this.vorbisReader?.SampleRate ?? 0;

        /// <inheritdoc/>
        public int ReadSamples(float[] buffer, int offset, int count)
        {
            if (string.IsNullOrEmpty(Filename))
            {
                throw new StringNullOrEmptyException();
            }

            return this.vorbisReader?.ReadSamples(buffer, offset, count) ?? 0;
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
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.vorbisReader?.Dispose();
                }

                this.isDisposed = true;
            }
        }
    }
}
