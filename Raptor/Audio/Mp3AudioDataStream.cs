// <copyright file="Mp3AudioDataStream.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Audio
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using MP3Sharp;
    using Raptor.Exceptions;

    /// <summary>
    /// Streams mp3 audio data from a mp3 file.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Mp3AudioDataStream : IAudioDataStream<byte>
    {
        // NOTE: the Mp3Sharp decoder library only deals with 16bit mp3 files.  Which is 99% of what is used now days anyways
        private MP3Stream? mp3Reader;
        private string? fileName;
        private bool isDisposed;

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <remarks>
        ///     The <see cref="ReadSamples(byte[], int, int)"/> method will
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
                    throw new FileNotFoundException($"The file '{value}' was not found or does not exist.");
                }

                if (this.mp3Reader is null)
                {
                    this.mp3Reader = new MP3Stream(value);
                }
                else
                {
                    if (value != this.fileName)
                    {
                        this.mp3Reader.Dispose();
                        this.mp3Reader = new MP3Stream(value);
                    }
                }

                this.fileName = value;
            }
        }

        /// <inheritdoc/>
        public int Channels => string.IsNullOrEmpty(this.fileName) ? 0 : this.mp3Reader?.ChannelCount ?? 0;

        /// <inheritdoc/>
        public AudioFormat Format
        {
            get
            {
                if (string.IsNullOrEmpty(this.fileName) || this.mp3Reader is null)
                {
                    return default;
                }

                return this.mp3Reader.Format == SoundFormat.Pcm16BitMono
                    ? AudioFormat.Mono16
                    : AudioFormat.Stereo16;
            }
        }

        /// <inheritdoc/>
        public int SampleRate => string.IsNullOrEmpty(this.fileName) ? 0 : this.mp3Reader?.Frequency ?? 0;

        /// <inheritdoc/>
        public int ReadSamples(byte[] buffer, int offset, int count)
        {
            if (string.IsNullOrEmpty(Filename))
            {
                throw new StringNullOrEmptyException();
            }

            return this.mp3Reader?.Read(buffer, offset, count) ?? 0;
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
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.mp3Reader?.Dispose();
                }

                this.isDisposed = true;
            }
        }
    }
}
