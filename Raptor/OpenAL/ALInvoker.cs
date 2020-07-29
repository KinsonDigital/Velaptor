using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using OpenToolkit.Audio.OpenAL;
using OpenToolkit.Graphics.OpenGL;

namespace Raptor.OpenAL
{
    [ExcludeFromCodeCoverage]
    internal class ALInvoker : IALInvoker
    {
        public Action<string> ErrorCallback { get; set; }

        public ALError GetError() => AL.GetError();

        public string GetErrorString(ALError param) => AL.GetErrorString(param);

        public int GenBuffer()
        {
            AL.GenBuffer(out var bufferId);

            var error = GetError();

            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));

            return bufferId;
        }

        public int GenSource()
        {
            AL.GenSource(out var sourceId);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));

            return sourceId;
        }

        public ALSourceState GetSourceState(int sid)
        {
            AL.GetSource(sid, ALGetSourcei.SourceState, out var result);

            return (ALSourceState)result;
        }

        public int GetSource(int sid, ALGetSourcei param)
        {
            AL.GetSource(sid, param, out int result);


            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));

            return result;
        }

        public void BufferData<TBuffer>(int bid, ALFormat format, TBuffer[] buffer, int size, int freq) where TBuffer : unmanaged
        {
            AL.BufferData(bid, format, buffer, size, freq);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public void Source(int sid, ALSourcei param, int value)
        {
            AL.Source(sid, param, value);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public bool GetSource(int sid, ALSourceb param)
        {
            AL.GetSource(sid, param, out var result);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));

            return result;
        }

        public void BindBufferToSource(int source, int buffer)
        {
            AL.BindBufferToSource(source, buffer);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public void Source(int sid, ALSourceb param, bool value)
        {
            AL.Source(sid, param, value);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public float GetSource(int sid, ALSourcef param)
        {
            AL.GetSource(sid, param, out var value);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));

            return value;
        }

        public void Source(int sid, ALSourcef param, float value)
        {
            AL.Source(sid, param, value);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public void SourcePlay(int sid)
        {
            AL.SourcePlay(sid);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public void SourcePause(int sid)
        {
            AL.SourcePause(sid);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public void SourceStop(int sid)
        {
            AL.SourceStop(sid);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public void SourceRewind(int sid)
        {
            AL.SourceRewind(sid);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public void DeleteBuffer(int buffer)
        {
            AL.DeleteBuffer(buffer);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public void DeleteSource(int source)
        {
            AL.DeleteSource(source);

            var error = GetError();
            if (error != ALError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(ALError), error));
        }

        public IList<string> GetString(ALDevice device, AlcGetStringList param)
        {
            var result = ALC.GetString(param);
            var error = ALC.GetError(device);

            if (error != AlcError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(AlcError), error));

            return result;
        }

        public ALDevice GetContextsDevice(ALContext context)
        {
            var deviceResult = ALC.GetContextsDevice(context);

            var error = ALC.GetError(deviceResult);
            if (error != AlcError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(AlcError), error));

            return deviceResult;
        }

        public bool MakeContextCurrent(ALContext context)
        {
            var result = false;

            // If the context is null, then the attempt is to destroy the context
            if (context == ALContext.Null)
            {
                result = ALC.MakeContextCurrent(context);

                if (!result) ErrorCallback?.Invoke("Issue destroying context.");
            }
            else
            {
                result = ALC.MakeContextCurrent(context);

                var device = GetContextsDevice(context);

                var error = ALC.GetError(device);
                if (error != AlcError.NoError)
                {
                    ErrorCallback?.Invoke(Enum.GetName(typeof(AlcError), error));
                }
                else
                {
                    // Throw error if the context could not be made current
                    if (!result)
                        ErrorCallback?.Invoke($"Context with handle '{context.Handle}' could not be made current.");
                }
            }

            return result;
        }

        public void DestroyContext(ALContext context)
        {
            var device = GetContextsDevice(context);

            ALC.DestroyContext(context);
            var error = ALC.GetError(device);

            if (error != AlcError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(AlcError), error));
        }

        public bool CloseDevice(ALDevice device)
        {
            var closeResult = ALC.CloseDevice(device);
            var error = ALC.GetError(device);

            if (error != AlcError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(AlcError), error));

            return closeResult;
        }

        public ALDevice OpenDevice(string? deviceName)
        {
            var deviceResult = ALC.OpenDevice(deviceName);
            var error = ALC.GetError(deviceResult);

            if (error != AlcError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(AlcError), error));

            return deviceResult;
        }

        public ALContext CreateContext(ALDevice device, ALContextAttributes attributes)
        {
            var contextResult = ALC.CreateContext(device, attributes);
            var error = ALC.GetError(device);

            if (error != AlcError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(AlcError), error));

            return contextResult;
        }

        public string GetString(ALDevice device, AlcGetString param)
        {
            var result = ALC.GetString(device, param);
            var error = ALC.GetError(device);

            if (error != AlcError.NoError) ErrorCallback?.Invoke(Enum.GetName(typeof(AlcError), error));

            return result;
        }
    }
}
