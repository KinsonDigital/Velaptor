using System;
using System.Collections.Generic;
using System.Text;

namespace Raptor.Audio
{
    public interface ISoundDecoder<T>
    {
        SoundData<T> LoadData(string fileName);
    }
}
