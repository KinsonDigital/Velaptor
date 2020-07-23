using System;
using System.Collections.Generic;
using System.Text;

namespace Raptor.Audio
{
    public interface IAudioDevices : IDisposable
    {
        AudioDeviceManager Instance { get; }
    }
}
