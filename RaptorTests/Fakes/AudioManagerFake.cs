// <copyright file="AudioMangerFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Fakes
{
    using System;
    using Raptor.Audio;

    public class AudioManagerFake : IAudioManager
    {
        private Guid soundId;

        public AudioManagerFake(Guid soundId) => this.soundId = soundId;

        public string[] DeviceNames => throw new NotImplementedException();

        public bool IsInitialized => throw new NotImplementedException();

        public event EventHandler? DeviceChanged;

        public void ChangeDevice(string name)
        {
            throw new NotImplementedException();
        }

        public Guid CreateSoundID(string fileName)
        {
            return this.soundId;
        }

        public Guid CreateSoundID(string fileName, Guid guid)
        {
            return guid;
        }

        public float GetTimePosition(Guid soundId)
        {
            throw new NotImplementedException();
        }

        public float GetVolume(Guid soundId)
        {
            throw new NotImplementedException();
        }

        public void Init(string name = null)
        {
            throw new NotImplementedException();
        }

        public bool IsSoundLooping(Guid soundId)
        {
            throw new NotImplementedException();
        }

        public void OnDeviceChanged() => DeviceChanged?.Invoke(this, new EventArgs());

        public void PauseSound(Guid soundId)
        {
            throw new NotImplementedException();
        }

        public void PlaySound(Guid soundId)
        {
            throw new NotImplementedException();
        }

        public void ResetSound(Guid soundId)
        {
            throw new NotImplementedException();
        }

        public void SetLooping(Guid soundId, bool value)
        {
            throw new NotImplementedException();
        }

        public void SetSoundTimePosition(Guid soundId, float seconds)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(Guid soundId, float value)
        {
            throw new NotImplementedException();
        }

        public void StopSound(Guid soundId)
        {
            throw new NotImplementedException();
        }

        public void UnloadSoundData(Guid soundId, bool preserveSound = false)
        {
        }

        public void UploadMp3Data(SoundStats<byte> data, Guid soundId)
        {
            throw new NotImplementedException();
        }

        public void UploadOggData(SoundStats<float> data, Guid soundId)
        {
        }
    }
}
