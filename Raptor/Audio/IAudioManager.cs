using System;

namespace Raptor.Audio
{
    internal interface IAudioManager
    {
        event EventHandler? DeviceChanged;

        string[] DeviceNames { get; }

        bool IsInitialized { get; }

        void Init(string name = null);

        Guid CreateSoundID(string fileName);

        Guid CreateSoundID(string fileName, Guid guid);

        void UploadOggData(SoundStats<float> data, Guid soundId);

        void UploadMp3Data(SoundStats<byte> data, Guid soundId);

        void PlaySound(Guid soundId);

        void PauseSound(Guid soundId);

        void StopSound(Guid soundId);

        void ResetSound(Guid soundId);

        bool IsSoundLooping(Guid soundId);

        void SetLooping(Guid soundId, bool value);

        float GetVolume(Guid soundId);

        void SetVolume(Guid soundId, float value);

        float GetTimePosition(Guid soundId);

        void SetTimePosition(Guid soundId, float seconds);

        void ChangeDevice(string name);

        void UnloadSoundData(Guid soundId, bool preserveSound = false);
    }
}
