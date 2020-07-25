using System;
using System.Collections.Generic;
using System.Text;

namespace Raptor.Audio
{
    internal struct SoundState
    {
        public int SourceId;

        public float TimePosition;

        public float TotalSeconds;

        public PlaybackState PlaybackState;
    }
}
