// <copyright file="ALInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenAL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using OpenTK.Audio.OpenAL;

    /// <summary>
    /// Invokes OpenAL functions.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class ALInvoker : IALInvoker
    {
        /// <inheritdoc/>
        public event Action<string>? ErrorCallback;

        /// <inheritdoc/>
        public ALError GetError() => AL.GetError();

        /// <inheritdoc/>
        public ALContext CreateContext(ALDevice device, ALContextAttributes attributes)
        {
            var contextResult = ALC.CreateContext(device, attributes);
            var error = ALC.GetError(device);

            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return contextResult;
        }

        /// <inheritdoc/>
        public ALDevice OpenDevice(string? deviceName)
        {
            var deviceResult = ALC.OpenDevice(deviceName);
            var error = ALC.GetError(deviceResult);

            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return deviceResult;
        }

        /// <inheritdoc/>
        public bool MakeContextCurrent(ALContext context)
        {
            bool result;

            // If the context is null, then the attempt is to destroy the context
            if (context == ALContext.Null)
            {
                result = ALC.MakeContextCurrent(context);

                if (!result)
                {
                    InvokeErrorIfTrue(true, "Issue destroying the context.");
                }
            }
            else
            {
                result = ALC.MakeContextCurrent(context);

                var device = GetContextsDevice(context);

                var error = ALC.GetError(device);

                if (result)
                {
                    var errorMessage = Enum.GetName(typeof(AlcError), error);
                    InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);
                }
                else
                {
                    // Throw an error that the context could not be made current
                    InvokeErrorIfTrue(true, $"Context with handle '{context.Handle}' could not be made current.");
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public int GenBuffer()
        {
            var result = AL.GenBuffer();

            var error = GetError();

            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public int GenSource()
        {
            var result = AL.GenSource();

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public string GetErrorString(ALError param) => AL.GetErrorString(param);

        /// <inheritdoc/>
        public int GetSource(int sid, ALGetSourcei param)
        {
            AL.GetSource(sid, param, out var result);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public bool GetSource(int sid, ALSourceb param)
        {
            AL.GetSource(sid, param, out var result);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public float GetSource(int sid, ALSourcef param)
        {
            AL.GetSource(sid, param, out var value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return value;
        }

        /// <inheritdoc/>
        public ALSourceState GetSourceState(int sid)
        {
            AL.GetSource(sid, ALGetSourcei.SourceState, out var result);

            return (ALSourceState)result;
        }

        /// <inheritdoc/>
        public ALDevice GetContextsDevice(ALContext context)
        {
            var deviceResult = ALC.GetContextsDevice(context);

            var error = ALC.GetError(deviceResult);
            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return deviceResult;
        }

        /// <inheritdoc/>
        public string GetString(ALDevice device, AlcGetString param)
        {
            var result = ALC.GetString(device, param);
            var error = ALC.GetError(device);

            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public IList<string> GetString(ALDevice device, AlcGetStringList param)
        {
            var result = ALC.GetString(param);
            var error = ALC.GetError(device);
            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public void BufferData<TBuffer>(int bid, ALFormat format, TBuffer[] buffer, int size, int freq)
            where TBuffer : unmanaged
        {
            AL.BufferData(bid, format, buffer, freq);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void BindBufferToSource(int source, int buffer)
        {
            AL.BindBufferToSource(source, buffer);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void Source(int sid, ALSourcei param, int value)
        {
            AL.Source(sid, param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void Source(int sid, ALSourceb param, bool value)
        {
            AL.Source(sid, param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void Source(int sid, ALSourcef param, float value)
        {
            AL.Source(sid, param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void SourcePlay(int sid)
        {
            AL.SourcePlay(sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void SourcePause(int sid)
        {
            AL.SourcePause(sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void SourceStop(int sid)
        {
            AL.SourceStop(sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void SourceRewind(int sid)
        {
            AL.SourceRewind(sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public bool CloseDevice(ALDevice device)
        {
            var closeResult = ALC.CloseDevice(device);
            var error = ALC.GetError(device);
            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return closeResult;
        }

        /// <inheritdoc/>
        public void DeleteBuffer(int buffer)
        {
            AL.DeleteBuffer(buffer);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void DeleteSource(int source)
        {
            AL.DeleteSource(source);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void DestroyContext(ALContext context)
        {
            var device = GetContextsDevice(context);

            ALC.DestroyContext(context);
            var error = ALC.GetError(device);
            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);
        }

        /// <summary>
        /// Invokes the error callback if the <paramref name="shouldInvoke"/> is true.
        /// </summary>
        /// <param name="shouldInvoke">If true, invokes the error callback.</param>
        /// <param name="errorMessage">The error message.</param>
        private void InvokeErrorIfTrue(bool shouldInvoke, string? errorMessage)
        {
            if (shouldInvoke)
            {
                ErrorCallback?.Invoke(string.IsNullOrEmpty(errorMessage) ? "OpenAL" : errorMessage);
            }
        }
    }
}
