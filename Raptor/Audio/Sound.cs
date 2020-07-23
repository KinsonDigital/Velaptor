// <copyright file="Sound.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
namespace Raptor.Audio
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Enumeration;
    using OpenToolkit.Audio.OpenAL;
    using Raptor.Factories;
    using Raptor.OpenAL;

    /// <summary>
    /// A single sound that can be played, paused etc.
    /// </summary>
    public class Sound : ISound
    {
        private const string IsDisposedExceptionMessage = "The sound is disposed.  You must create another sound instance.";
        private readonly IAudioManager audioManager;
        private readonly ISoundDecoder<float> oggDecoder;
        private readonly ISoundDecoder<byte> mp3Decoder;
        private readonly string fileName;
        private Guid soundId;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="fileName">The path/name to the sound file to load.</param>
        [ExcludeFromCodeCoverage]
        public Sound(string fileName)
        {
            this.fileName = fileName;

            this.oggDecoder = new OggSoundDecoder();
            this.mp3Decoder = new MP3SoundDecoder();

            this.audioManager = AudioDeviceManagerFactory.CreateManager();
            this.audioManager.DeviceChanged += AudioManager_DeviceChanged;

            Init(fileName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="audioManager">Manages audio related operations.</param>
        /// <param name="fileName">The path/name to the sound file to load.</param>
        /// <param name="oggDecoder">Decodes OGG audio files.</param>
        /// <param name="mp3Decoder">Decodes MP3 audio files.</param>
        internal Sound(IAudioManager audioManager, string fileName, ISoundDecoder<float> oggDecoder, ISoundDecoder<byte> mp3Decoder)
        {
            this.fileName = fileName;
            this.oggDecoder = oggDecoder;
            this.mp3Decoder = mp3Decoder;

            this.audioManager = audioManager;
            this.audioManager.DeviceChanged += AudioManager_DeviceChanged;

            Init(fileName);
        }

        /// <inheritdoc/>
        public string Name => Path.GetFileNameWithoutExtension(this.fileName);

        /// <inheritdoc/>
        public float Volume
        {
            get
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                return this.audioManager.GetVolume(this.soundId);
            }
            set
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                this.audioManager.SetVolume(this.soundId, value);
            }
        }

        /// <inheritdoc/>
        public float CurrentTimePosition
        {
            get
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                return this.audioManager.GetTimePosition(this.soundId);
            }
        }

        /// <inheritdoc/>
        public bool IsLooping
        {
            get
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                return this.audioManager.IsSoundLooping(this.soundId);
            }
            set
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                this.audioManager.SetLooping(this.soundId, value);
            }
        }

        /// <inheritdoc/>
        public void PlaySound()
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            this.audioManager.PlaySound(this.soundId);
        }

        /// <inheritdoc/>
        public void PauseSound()
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            this.audioManager.PauseSound(this.soundId);
        }

        /// <inheritdoc/>
        public void StopSound()
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            this.audioManager.StopSound(this.soundId);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            this.audioManager.ResetSound(this.soundId);
        }

        /// <inheritdoc/>
        public void SetTimePosition(float seconds)
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            this.audioManager.SetTimePosition(this.soundId, seconds);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
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
                this.audioManager.UnloadSoundData(this.soundId);
                this.audioManager.DeviceChanged -= AudioManager_DeviceChanged;
                this.isDisposed = true;
            }
        }

        private void Init(string fileName)
        {
            if (this.soundId == Guid.Empty)
                this.soundId = this.audioManager.CreateSoundID(fileName);
            else
                this.audioManager.CreateSoundID(fileName, this.soundId);

            var extension = Path.GetExtension(this.fileName);

            switch (extension)
            {
                case ".ogg":
                    var oggData = this.oggDecoder.LoadData(this.fileName);

                    this.audioManager.UploadOggData(oggData, this.soundId);
                    break;
                case ".mp3":
                    var mp3Data = this.mp3Decoder.LoadData(this.fileName);

                    this.audioManager.UploadMp3Data(mp3Data, this.soundId);

                    break;
                default:
                    throw new Exception($"The file extension '{extension}' is not supported file type.");
            }
        }

        /// <summary>
        /// Invoked when the audio device has been changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains various event related information.</param>
        private void AudioManager_DeviceChanged(object? sender, EventArgs e) => Init(this.fileName);
    }
}
