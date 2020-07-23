using System;
using System.Collections.Generic;
using System.Text;

namespace Raptor.Audio
{
    public interface ISound : IDisposable
    {
        public bool IsLooping { get; set; }

        public float Volume { get; set; }

        public float CurrentTimePosition { get; }

        public void Play();

        public void Pause();

        public void Stop();

        public void Reset();

        //TODO: Create overloads that take TimeSpan, milliseconds, minutes, and combination of those
        public void SetTimePosition(float seconds);
    }
}
