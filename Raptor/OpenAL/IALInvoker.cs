using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Audio.OpenAL;

namespace Raptor.OpenAL
{
    internal interface IALInvoker
    {
        Action<string> ErrorCallback { get; set; }

        ALError GetError();

        string GetErrorString(ALError param);

        int GenBuffer();

        int GenSource();

        void BufferData<TBuffer>(int bid, ALFormat format, TBuffer[] buffer, int size, int freq) where TBuffer : unmanaged;

        void BindBufferToSource(int source, int buffer);

        void Source(int sid, ALSourcei param, int value);

        bool GetSource(int sid, ALSourceb param);

        ALSourceState GetSourceState(int sid);

        int GetSource(int sid, ALGetSourcei param);

        void Source(int sid, ALSourceb param, bool value);

        float GetSource(int sid, ALSourcef param);

        void Source(int sid, ALSourcef param, float value);

        void SourcePlay(int sid);

        void SourcePause(int sid);

        void SourceStop(int sid);

        void SourceRewind(int sid);

        void DeleteBuffer(int buffer);

        void DeleteSource(int source);

        bool CloseDevice(ALDevice device);

        ALContext CreateContext(ALDevice device, ALContextAttributes attributes);

        void DestroyContext(ALContext context);

        IList<string> GetString(ALDevice device, AlcGetStringList param);

        string GetString(ALDevice device, AlcGetString param);

        bool MakeContextCurrent(ALContext context);

        ALDevice OpenDevice(string devicename);
    }
}
