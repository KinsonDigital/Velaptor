using System;
using System.Collections.Generic;
using OpenToolkit.Audio.OpenAL;

namespace Raptor.OpenAL
{
    internal interface IALCInvoker
    {
        Action<string> ErrorCallback { get; set; }

        bool CloseDevice(ALDevice device);
        ALContext CreateContext(ALDevice device, ALContextAttributes attributes);
        void DestroyContext(ALContext context);
        IList<string> GetString(ALDevice device, AlcGetStringList param);
        string GetString(ALDevice device, AlcGetString param);
        bool MakeContextCurrent(ALContext context);
        ALDevice OpenDevice(string devicename);
    }
}
