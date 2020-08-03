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
    using OpenToolkit.Audio.OpenAL;
    using Raptor.Content;
    using Raptor.Factories;
    using Raptor.OpenAL;

    /// <summary>
    /// A single sound that can be played, paused etc.
    /// </summary>
    public class Sound : ISound
    {
        private const string IsDisposedExceptionMessage = "The sound is disposed.  You must create another sound instance.";
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
        private int sampleRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class.
        /// </summary>
        /// <param name="name">The name of the content item to load.</param>
        [ExcludeFromCodeCoverage]
        // TODO: Add static functionality to ContentLoader that can be used internally  in this
        // class to be able to load the path to the sound content by using the name only.  The name should not
        // have an extension.
        public Sound(string name)
        {
            this.name = name;

            this.alInvoker = new ALInvoker
            {
                ErrorCallback = ErrorCallback,
            };

            this.oggDecoder = IoC.Container.GetInstance<ISoundDecoder<float>>();
            this.mp3Decoder = IoC.Container.GetInstance<ISoundDecoder<byte>>();

            this.audioManager = AudioDeviceManagerFactory.CreateManager();
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
        public string ContentName => Path.GetFileNameWithoutExtension(this.name);

        /// <inheritdoc/>
        public float Volume
        {
            get
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                // Get the current volume between 0.0 and 1.0
                var volume = this.alInvoker.GetSource(this.srcId, ALSourcef.Gain);

                // Change the range to be between 0 and 100
                return volume * 100f;
            }
            set
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

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
        public float TimePosition
        {
            get
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                var sampleOffset = this.alInvoker.GetSource(this.srcId, ALGetSourcei.SampleOffset);

                return sampleOffset / (float)this.sampleRate;
            }
        }

        /// <inheritdoc/>
        public bool IsLooping
        {
            get
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                return this.alInvoker.GetSource(srcId, ALSourceb.Looping);
            }
            set
            {
                if (this.isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                this.alInvoker.Source(this.srcId, ALSourceb.Looping, value);
            }
        }

        /// <inheritdoc/>
        public void PlaySound()
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            alInvoker.SourcePlay(this.srcId);
        }

        /// <inheritdoc/>
        public void PauseSound()
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            this.alInvoker.SourcePause(this.srcId);
        }

        /// <inheritdoc/>
        public void StopSound()
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            this.alInvoker.SourceStop(this.srcId);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            this.alInvoker.SourceRewind(this.srcId);
        }

        /// <inheritdoc/>
        public void SetTimePosition(float seconds)
        {
            if (this.isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            // Prevent negative number
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
                UnloadSoundData();
                this.audioManager.DeviceChanged -= AudioManager_DeviceChanged;

                if (!(this.alInvoker is null))
                    this.alInvoker.ErrorCallback -= ErrorCallback;

                this.isDisposed = true;
            }
        }

        private void Init()
        {
            if (!this.audioManager.IsInitialized)
                this.audioManager.InitDevice();

            (this.srcId, this.bufferId) = this.audioManager.InitSound();

            var fileName = this.contentSource.GetContentPath(ContentType.Sounds, this.name);

            var extension = Path.GetExtension(fileName);

            switch (extension)
            {
                case ".ogg":
                    var oggData = this.oggDecoder.LoadData(fileName);

                    this.totalSeconds = oggData.TotalSeconds;
                    this.sampleRate = oggData.SampleRate;

                    UploadOggData(oggData);
                    break;
                case ".mp3":
                    var mp3Data = this.mp3Decoder.LoadData(fileName);

                    this.totalSeconds = mp3Data.TotalSeconds;
                    this.sampleRate = mp3Data.SampleRate;

                    UploadMp3Data(mp3Data);
                    break;
                default:
                    throw new Exception($"The file extension '{extension}' is not supported file type.");
            }
        }

        private void UploadOggData(SoundData<float> data)
        {
            SoundSource soundSrc;
            soundSrc.SourceId = this.srcId;
            soundSrc.TotalSeconds = data.TotalSeconds;
            soundSrc.SampleRate = data.SampleRate;

            this.alInvoker.BufferData(this.bufferId,
                            MapFormat(data.Format),
                            data.BufferData.ToArray(),
                            data.BufferData.Count * sizeof(float),
                            data.SampleRate);

            // Bind the buffer to the source
            this.alInvoker.Source(this.srcId, ALSourcei.Buffer, this.bufferId);

            this.audioManager.UpdateSoundSource(soundSrc);
        }

        private void UploadMp3Data(SoundData<byte> data)
        {
            SoundSource soundSrc;
            soundSrc.SourceId = this.srcId;
            soundSrc.TotalSeconds = data.TotalSeconds;
            soundSrc.SampleRate = data.SampleRate;

            this.alInvoker.BufferData(this.bufferId,
                            MapFormat(data.Format),
                            data.BufferData.ToArray(),
                            data.BufferData.Count,
                            data.SampleRate);

            // Bind the buffer to the source
            this.alInvoker.Source(this.srcId, ALSourcei.Buffer, this.bufferId);

            //TODO: Call audiomanager update sound source
        }

        private ALFormat MapFormat(AudioFormat format) => format switch
        {
            AudioFormat.Mono8 => ALFormat.Mono8,
            AudioFormat.Mono16 => ALFormat.Mono16,
            AudioFormat.Mono32Float => ALFormat.MonoFloat32Ext,
            AudioFormat.Stereo8 => ALFormat.Stereo8,
            AudioFormat.Stereo16 => ALFormat.Stereo16,
            AudioFormat.StereoFloat32 => ALFormat.StereoFloat32Ext,
            _ => throw new Exception("Invalid or unknown audio format."),
        };

        private void UnloadSoundData()
        {
            // TODO: Add isDiposed check and throw exception
            // TODO: Check if the sourceId exists first and if not, throw an exception

            this.alInvoker.DeleteSource(this.srcId);
            this.alInvoker.DeleteBuffer(this.bufferId);
        }

        /// <summary>
        /// Invoked when the audio device has been changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains various event related information.</param>
        private void AudioManager_DeviceChanged(object? sender, EventArgs e) => Init();

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
