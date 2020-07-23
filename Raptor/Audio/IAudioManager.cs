using System;

namespace Raptor.Audio
{
    internal interface IAudioManager
    {
        event EventHandler? DeviceChanged;

        Guid CreateSoundID(string fileName);

        Guid CreateSoundID(string fileName, Guid guid);

        bool IsSoundLooping(Guid soundId);

        void SetLooping(Guid soundId, bool value);

        void SetVolume(Guid soundId, float value);

        float GetVolume(Guid soundId);

        float GetTimePosition(Guid soundId);

        void SetSoundTimePosition(Guid soundId, float seconds);

        void UploadOggData(SoundStats<float> data, Guid soundId);

        void UploadMp3Data(SoundStats<byte> data, Guid soundId);

        void UnloadSoundData(Guid soundId, bool preserveSound = false);

        string[] DeviceNames { get; }

        bool IsInitialized { get; }

        void ChangeDevice(string name);

        void Init(string name = null);

        void PlaySound(Guid soundId);

        void PauseSound(Guid soundId);

        void StopSound(Guid soundId);

        void ResetSound(Guid soundId);

        void OnDeviceChanged();
    }
}
