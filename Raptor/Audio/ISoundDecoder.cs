using System;
using System.Collections.Generic;
using System.Text;

namespace Raptor.Audio
{
    public interface ISoundDecoder<T>
    {
        SoundStats<T> LoadData(string fileName);
    }
}
