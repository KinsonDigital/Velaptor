namespace Raptor.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Data;
#if DEBUG
    using System.Diagnostics;
#endif
    using System.Linq;
    using OpenToolkit.Audio.OpenAL;
    using Raptor.OpenAL;

    // TODO: Rename to AudioDevices
    internal sealed class AudioDeviceManager : IAudioDeviceManager
    {
        private static AudioDeviceManager _instance = new AudioDeviceManager();
        private const string DeviceNamePrefix = "OpenAL Soft on ";// All device names returned are prefixed with this
        private static readonly string IsDisposedExceptionMessage = $"The '{nameof(AudioDeviceManager)}' has already been destroyed.\nInvoked the '{nameof(AudioDeviceManager.InitDevice)}()' to re-setup the device manager.";
        private static Dictionary<int, SoundSource> _soundSources = new Dictionary<int, SoundSource>();
        private static List<SoundState> _continuePlaybackCache = new List<SoundState>();
        private static ALDevice _device;
        private static ALContext _context;
        private static ALContextAttributes? _attributes;
        private static IALInvoker? _alInvoker;
        private static bool isDisposed;
        public event EventHandler? DeviceChanged;

        private AudioDeviceManager() { }

        public bool IsInitialized => !AudioIsNull() &&
            !InvokersAreNull();

        public string[] DeviceNames
        {
            get
            {
                if (isDisposed)
                    throw new Exception(IsDisposedExceptionMessage);

                var result = _alInvoker.GetString(_device, AlcGetStringList.AllDevicesSpecifier)
                    .Select(n => n.Replace(DeviceNamePrefix, "")).ToArray();

                return result;
            }
        }

        public void InitDevice(string name = null)
        {
            if (_device.Handle == IntPtr.Zero)
                _device = _alInvoker.OpenDevice(name == null ? null : $"{DeviceNamePrefix}{name}");

            if (_attributes is null)
                _attributes = new ALContextAttributes();

            if (_context.Handle == IntPtr.Zero)
                _context = _alInvoker.CreateContext(_device, _attributes);

            _alInvoker.MakeContextCurrent(_context);

            isDisposed = false;
        }

        public (int srcId, int bufferId) InitSound()
        {
            if (isDisposed)
                throw new Exception(IsDisposedExceptionMessage);

            Guid newId;

            SoundSource soundSrc;
            soundSrc.SampleRate = 0;
            soundSrc.TotalSeconds = -1f;

            soundSrc.SourceId = _alInvoker.GenSource();
            var bufferId = _alInvoker.GenBuffer();

            _soundSources.Add(soundSrc.SourceId, soundSrc);

            return (soundSrc.SourceId, bufferId);
        }

        public void ChangeDevice(string name)
        {
            // TODO: Add isDiposed check and throw exception
            if (!DeviceNames.Contains(name))
                throw new AudioDeviceDoesNotExistException();

            var availableDevices = DeviceNames;

            CacheSoundSources();

            DestroyDevice();
            InitDevice(name);

            _soundSources.Clear();

            DeviceChanged?.Invoke(this, new EventArgs());

            // Reset all of the states such as if playing or paused and the current time position
            foreach (var cachedState in _continuePlaybackCache)
            {
                // Set the current position of the sound
                SetTimePosition(cachedState.SourceId, cachedState.TimePosition, cachedState.TotalSeconds);

                // Set the state of the sound
                switch (cachedState.PlaybackState)
                {
                    case PlaybackState.Playing:
                        _alInvoker.SourcePlay(cachedState.SourceId);
                        break;
                    case PlaybackState.Paused:
                        _alInvoker.SourceStop(cachedState.SourceId);
                        break;
                }
            }
        }

        public void UpdateSoundSource(SoundSource soundSrc)
        {
            if (!(_soundSources.Keys.Contains(soundSrc.SourceId)))
                throw new Exception("The sound source does not exist");//TODO: Make this better and update unit test

            _soundSources[soundSrc.SourceId] = soundSrc;
        }

        internal static AudioDeviceManager GetInstance(IALInvoker alInvoker)
        // TODO: Add isDiposed check and throw exception
        {
            if (_instance.IsInitialized)
                return _instance;

            if (_alInvoker is null)
            {
                _alInvoker = alInvoker;
                _alInvoker.ErrorCallback = ErrorCallback;
            }
            
            _instance.InitDevice();

            return _instance;
        }

        private void CacheSoundSources()
        {
            // Create a cache of all the songs currently playing and record the current playback position
            // Cache only if the sound was currently playing or paused

            // Guarentee that the cache is clear
            _continuePlaybackCache.Clear();

            foreach (var soundSrcKVP in _soundSources)
            {
                var sourceState = _alInvoker.GetSourceState(soundSrcKVP.Value.SourceId);

                if (sourceState != ALSourceState.Playing && sourceState != ALSourceState.Paused)
                    continue;
                SoundState soundState;
                soundState.SourceId = soundSrcKVP.Value.SourceId;
                soundState.PlaybackState = default;
                soundState.TimePosition = GetCurrentTimePosition(soundSrcKVP.Value.SourceId, soundSrcKVP.Value.SampleRate);
                soundState.TotalSeconds = soundSrcKVP.Value.TotalSeconds;

                if (sourceState == ALSourceState.Playing)
                {
                    soundState.PlaybackState = PlaybackState.Playing;
                }
                else if (sourceState == ALSourceState.Paused)
                {
                    soundState.PlaybackState = PlaybackState.Paused;
                }

                _continuePlaybackCache.Add(soundState);
            }
        }

        private bool AudioIsNull() => _device == ALDevice.Null && _context == ALContext.Null && _attributes is null;

        private bool InvokersAreNull() => _alInvoker is null && _alInvoker is null;

        private void DestroyDevice()
        {
            if (_context != ALContext.Null)
            {
                _alInvoker.MakeContextCurrent(ALContext.Null);
                _alInvoker.DestroyContext(_context);
            }

            _context = ALContext.Null;

            if (_device != ALDevice.Null)
                _alInvoker.CloseDevice(_device);

            _device = ALDevice.Null;

            _attributes = null;
        }

        private float GetCurrentTimePosition(int srcId, float sampleRate)
        {
            var sampleOffset = _alInvoker.GetSource(srcId, ALGetSourcei.SampleOffset);

            return sampleOffset / sampleRate;
        }

        private void SetTimePosition(int srcId, float seconds, float totalSeconds)
        {
            // Prevent negative number
            seconds = seconds < 0f ? 0.0f : seconds;

            seconds = seconds > totalSeconds ? totalSeconds : seconds;

            _alInvoker.Source(srcId, ALSourcef.SecOffset, seconds);
        }

        private static void ErrorCallback(string errorMsg)
        {
            Debugger.Break();
        }

        public void Dispose()
        {
            _soundSources?.Clear();

            _continuePlaybackCache?.Clear();

            _device = ALDevice.Null;
            _context = ALContext.Null;
            _attributes = null;
            _alInvoker = null;
            _alInvoker = null;

            _instance = new AudioDeviceManager();
            isDisposed = true;
        }
    }
}
