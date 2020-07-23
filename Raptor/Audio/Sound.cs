// <copyright file="Sound.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
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
        private readonly IAudioDeviceManager audioManager;
        private readonly ISoundDecoder<float> oggDecoder;
        private readonly ISoundDecoder<byte> mp3Decoder;
        private readonly string fileName;
        private Guid soundId;
        private bool isDisposed;

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

        internal Sound(IAudioDeviceManager audioManager, string fileName, ISoundDecoder<float> oggDecoder, ISoundDecoder<byte> mp3Decoder)
        {
            this.audioManager = audioManager;
            this.fileName = fileName;
            this.oggDecoder = oggDecoder;
            this.mp3Decoder = mp3Decoder;

            Init(fileName);
        }

        public string Name => Path.GetFileNameWithoutExtension(this.fileName);

        //TODO: Check that this is working
        public bool IsLooping
        {
            get
            {
                if (isDisposed)
                    throw new Exception("Object already disposed");

                return this.audioManager.IsSoundRepeating(this.soundId);
            }
            set
            {
                if (isDisposed)
                    throw new Exception("Object already disposed");

                this.audioManager.RepeatSound(this.soundId, value);
            }
        }

        public float Volume
        {
            get
            {
                if (isDisposed)
                    throw new Exception("Object already disposed");

                return this.audioManager.GetVolume(this.soundId);
            }
            set
            {
                if (isDisposed)
                    throw new Exception("Object already disposed");

                this.audioManager.SetVolume(this.soundId, value);
            }
        }

        public float CurrentTimePosition
        {
            get
            {
                if (isDisposed)
                    throw new Exception("Object already disposed");

                return this.audioManager.GetTimePosition(this.soundId);
            }
        }

        public void Play()
        {
            if (isDisposed)
                throw new Exception("Object already disposed");

            this.audioManager.PlaySound(this.soundId);
        }

        public void Pause()
        {
            if (isDisposed)
                throw new Exception("Object already disposed");

            this.audioManager.PauseSound(this.soundId);
        }

        public void Stop()
        {
            if (isDisposed)
                throw new Exception("Object already disposed");

            this.audioManager.StopSound(this.soundId);
        }

        public void Reset()
        {
            if (isDisposed)
                throw new Exception("Object already disposed");

            this.audioManager.ResetSound(this.soundId);
        }

        //TODO: Create overloads that take TimeSpan, milliseconds, minutes, and combination of those
        public void SetTimePosition(float seconds)
        {
            if (isDisposed)
                throw new Exception("Object already disposed");

            this.audioManager.SetSoundTimePosition(this.soundId, seconds);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                //AL.Source(_sourceId, ALSourcei.Buffer, 0);

                // Delete the buffer
                //AL.SourceUnqueueBuffers(_sourceId, new[] { _bufferId });

                this.audioManager.UnloadSoundData(this.soundId);
                this.audioManager.DeviceChanged -= AudioManager_DeviceChanged;

                //if (this.audioDevices.CurrentContext != ALContext.Null)
                //{
                //    ALC.MakeContextCurrent(ALContext.Null);
                //    ALC.DestroyContext(_context);
                //}

                //_context = ALContext.Null;

                //if (_device != IntPtr.Zero)
                //    ALC.CloseDevice(_device);

                //_device = ALDevice.Null;

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
                    throw new Exception($"The file extension '{extension}' is not supported.");
            }
        }

        private void AudioManager_DeviceChanged(object? sender, EventArgs e)
        {
            Init(this.fileName);
        }
    }
}
