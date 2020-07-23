using System;
using System.Collections.Generic;
using System.Text;

namespace Raptor.Audio
{
    // TODO: Improve this exception by adding constructor for custom message and also provide default message using device name
    public class AudioDeviceDoesNotExistException : Exception
    {
    }
}
