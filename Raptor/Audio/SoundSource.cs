using System;
using System.Collections.Generic;
using System.Text;

namespace Raptor.Audio
{
    internal struct SoundSource
    {
        public int SourceId;

        public int BufferId;

        public float TotalTime; // In seconds

        public string FileName;

        public int SampleRate;
    }
}
