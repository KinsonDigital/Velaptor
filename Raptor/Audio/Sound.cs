// <copyright file="Sound.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
namespace Raptor.Audio
{
    using System;
#if DEBUG
    using System.Diagnostics;
#endif
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using OpenTK.Audio.OpenAL;
    using Raptor.Content;
    using Raptor.Factories;
    using Raptor.OpenAL;

    /// <summary>
    /// A single sound that can be played, paused etc.
    /// </summary>
    public class Sound : ISound
    {
        private const string IsDisposedExceptionMessage = "The sound is disposed.  You must create another sound instance.";

        // NOTE: This warning is ignored due to the implementation of the IAudioManager being a singleton.
        // Disposing of the audio manager when any sound is disposed would cause issues with how the
        // audio manager implementation is suppose to behave.
#pragma warning disable CA2213 // Disposable fields should be disposed
        private readonly IAudioDeviceManager audioManager;
#pragma warning restore CA2213 // Disposable fields should be disposed

        private readonly ISoundDecoder<float> oggDecoder;
        private readonly ISoundDecoder<byte> mp3Decoder;
        private readonly IALInvoker alInvoker;
        private readonly IContentSource contentSource;
        private readonly string name;
        private int srcId;
        private int bufferId;
        private bool isDisposed;
        private float totalSeconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="name">The name of the content item to load.</param>
        [ExcludeFromCodeCoverage]
        public Sound(string name)
        {
            this.name = name;

            this.alInvoker = new ALInvoker
            {
                ErrorCallback = ErrorCallback,
            };

            this.oggDecoder = IoC.Container.GetInstance<ISoundDecoder<float>>();
            this.mp3Decoder = IoC.Container.GetInstance<ISoundDecoder<byte>>();

            this.audioManager = AudioDeviceManagerFactory.CreateDeviceManager();
            this.audioManager.DeviceChanged += AudioManager_DeviceChanged;

            this.contentSource = IoC.Container.GetInstance<IContentSource>();
            Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="name">The name of the content to load.</param>
        /// <param name="alInvoker">Provides access to OpenAL.</param>
        /// <param name="audioManager">Manages audio related operations.</param>
        /// <param name="oggDecoder">Decodes OGG audio files.</param>
        /// <param name="mp3Decoder">Decodes MP3 audio files.</param>
        /// <param name="contentSource">Provides access to the source of content.</param>
        internal Sound(string name, IALInvoker alInvoker, IAudioDeviceManager audioManager, ISoundDecoder<float> oggDecoder, ISoundDecoder<byte> mp3Decoder, IContentSource contentSource)
        {
            this.name = name;

            this.alInvoker = alInvoker;
            this.alInvoker.ErrorCallback = ErrorCallback;

            this.oggDecoder = oggDecoder;
            this.mp3Decoder = mp3Decoder;

            this.audioManager = audioManager;
            this.audioManager.DeviceChanged += AudioManager_DeviceChanged;

            this.contentSource = contentSource;

            Init();
        }

        /// <inheritdoc/>
        public string Name => Path.GetFileNameWithoutExtension(this.name);

        /// <inheritdoc/>
        public float Volume
        {
            get
            {
                if (this.isDisposed)
                {
                    throw new Exception(IsDisposedExceptionMessage);
                }

                // Get the current volume between 0.0 and 1.0
                var volume = this.alInvoker.GetSource(this.srcId, ALSourcef.Gain);

                // Change the range to be between 0 and 100
                return volume * 100f;
            }
            set
            {
                if (this.isDisposed)
                {
                    throw new Exception(IsDisposedExceptionMessage);
                }

                // Make sure that the incoming value stays between 0 and 100
                value = value > 100f ? 100f : value;
                value = value < 0f ? 0f : value;

                // Convert the value to be between 0 and 1.
                // This is the excepted range by OpenAL
                value /= 100f;

                this.alInvoker.Source(this.srcId, ALSourcef.Gain, (float)Math.Round(value, 4));
            }
        }

        /// <inheritdoc/>
        public float TimePositionMilliseconds => TimePositionSeconds * 1000f;

        /// <inheritdoc/>
        public float TimePositionSeconds
        {
            get
            {
                if (this.isDisposed)
                {
                    throw new Exception(IsDisposedExceptionMessage);
                }

                return this.alInvoker.GetSource(this.srcId, ALSourcef.SecOffset);
            }
        }

        /// <inheritdoc/>
        public float TimePositionMinutes => TimePositionSeconds / 60f;

        /// <inheritdoc/>
        public TimeSpan TimePosition
        {
            get
            {
                var seconds = TimePositionSeconds;

                return new TimeSpan(0, 0, (int)seconds);
            }
        }

        /// <inheritdoc/>
        public bool IsLooping
        {
            get
            {
                if (this.isDisposed)
                {
                    throw new Exception(IsDisposedExceptionMessage);
                }

                return this.alInvoker.GetSource(this.srcId, ALSourceb.Looping);
            }
            set
            {
                if (this.isDisposed)
                {
                    throw new Exception(IsDisposedExceptionMessage);
                }

                this.alInvoker.Source(this.srcId, ALSourceb.Looping, value);
            }
        }

        /// <inheritdoc/>
        public void PlaySound()
        {
            if (this.isDisposed)
            {
                throw new Exception(IsDisposedExceptionMessage);
            }

            this.alInvoker.SourcePlay(this.srcId);
        }

        /// <inheritdoc/>
        public void PauseSound()
        {
            if (this.isDisposed)
            {
                throw new Exception(IsDisposedExceptionMessage);
            }

            this.alInvoker.SourcePause(this.srcId);
        }

        /// <inheritdoc/>
        public void StopSound()
        {
            if (this.isDisposed)
            {
                throw new Exception(IsDisposedExceptionMessage);
            }

            this.alInvoker.SourceStop(this.srcId);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (this.isDisposed)
            {
                throw new Exception(IsDisposedExceptionMessage);
            }

            this.alInvoker.SourceRewind(this.srcId);
        }

        /// <inheritdoc/>
        public void SetTimePosition(float seconds)
        {
            if (this.isDisposed)
            {
                throw new Exception(IsDisposedExceptionMessage);
            }

            // Prevent negative numbers
            seconds = seconds < 0f ? 0.0f : seconds;

            seconds = seconds > this.totalSeconds ? this.totalSeconds : seconds;

            this.alInvoker.Source(this.srcId, ALSourcef.SecOffset, seconds);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
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
                if (disposing)
                {
                    this.oggDecoder.Dispose();
                    this.mp3Decoder.Dispose();
                    this.audioManager.DeviceChanged -= AudioManager_DeviceChanged;
                }

                UnloadSoundData();

                this.alInvoker.ErrorCallback -= ErrorCallback;

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Maps the given audio <paramref name="format"/> to the <see cref="ALFormat"/> type equivalent.
        /// </summary>
        /// <param name="format">The format to convert.</param>
        /// <returns>The <see cref="ALFormat"/> result.</returns>
        private static ALFormat MapFormat(AudioFormat format) => format switch
        {
            AudioFormat.Mono8 => ALFormat.Mono8,
            AudioFormat.Mono16 => ALFormat.Mono16,
            AudioFormat.Mono32Float => ALFormat.MonoFloat32Ext,
            AudioFormat.Stereo8 => ALFormat.Stereo8,
            AudioFormat.Stereo16 => ALFormat.Stereo16,
            AudioFormat.StereoFloat32 => ALFormat.StereoFloat32Ext,
            _ => throw new Exception("Invalid or unknown audio format."),
        };

        /// <summary>
        /// Initializes the sound.
        /// </summary>
        private void Init()
        {
            if (!this.audioManager.IsInitialized)
            {
                this.audioManager.InitDevice();
            }

            (this.srcId, this.bufferId) = this.audioManager.InitSound();

            var fileName = this.contentSource.GetContentPath(this.name);

            var extension = Path.GetExtension(fileName);

            switch (extension)
            {
                case ".ogg":
                    var oggData = this.oggDecoder.LoadData(fileName);

                    this.totalSeconds = oggData.TotalSeconds;

                    UploadOggData(oggData);
                    break;
                case ".mp3":
                    var mp3Data = this.mp3Decoder.LoadData(fileName);

                    this.totalSeconds = mp3Data.TotalSeconds;

                    UploadMp3Data(mp3Data);
                    break;
                default:
                    throw new Exception($"The file extension '{extension}' is not supported file type.");
            }
        }

        /// <summary>
        /// Uploads Ogg audio data to the sound card.
        /// </summary>
        /// <param name="data">The ogg related sound data to upload.</param>
        private void UploadOggData(SoundData<float> data)
        {
            SoundSource soundSrc;
            soundSrc.SourceId = this.srcId;
            soundSrc.TotalSeconds = data.TotalSeconds;
            soundSrc.SampleRate = data.SampleRate;

            this.alInvoker.BufferData(
                this.bufferId,
                MapFormat(data.Format),
                data.BufferData.ToArray(),
                data.BufferData.Count * sizeof(float),
                data.SampleRate);

            // Bind the buffer to the source
            this.alInvoker.Source(this.srcId, ALSourcei.Buffer, this.bufferId);

            this.audioManager.UpdateSoundSource(soundSrc);
        }

        /// <summary>
        /// Uploads MP3 audio data to the sound card.
        /// </summary>
        /// <param name="data">The mp3 related sound data to upload.</param>
        private void UploadMp3Data(SoundData<byte> data)
        {
            SoundSource soundSrc;
            soundSrc.SourceId = this.srcId;
            soundSrc.TotalSeconds = data.TotalSeconds;
            soundSrc.SampleRate = data.SampleRate;

            this.alInvoker.BufferData(
                this.bufferId,
                MapFormat(data.Format),
                data.BufferData.ToArray(),
                data.BufferData.Count,
                data.SampleRate);

            // Bind the buffer to the source
            this.alInvoker.Source(this.srcId, ALSourcei.Buffer, this.bufferId);

            // TODO: Call audio manager update sound source
        }

        /// <summary>
        /// Unloads the sound data from the card.
        /// </summary>
        private void UnloadSoundData()
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.srcId <= 0)
            {
                return;
            }

            this.alInvoker.DeleteSource(this.srcId);

            if (this.bufferId != 0)
            {
                this.alInvoker.DeleteBuffer(this.bufferId);
            }
        }

        /// <summary>
        /// Invoked when the audio device has been changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains various event related information.</param>
        private void AudioManager_DeviceChanged(object? sender, EventArgs e) => Init();

        /// <summary>
        /// The callback invoked when an OpenAL error occurs.
        /// </summary>
        /// <param name="errorMsg">The OpenAL message.</param>
        [ExcludeFromCodeCoverage]
        private void ErrorCallback(string errorMsg)
        {
#if DEBUG
#pragma warning disable IDE0022 // Use expression body for methods
            Debugger.Break();
#pragma warning restore IDE0022 // Use expression body for methods
#endif
        }
    }
}
