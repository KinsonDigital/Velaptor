using System;
using System.Collections.Generic;
using System.Text;

namespace Raptor.Audio
{
    internal struct SoundState
    {
        public Guid SoundID;

        public float TimePosition;

        public PlaybackState PlaybackState;
    }
}
