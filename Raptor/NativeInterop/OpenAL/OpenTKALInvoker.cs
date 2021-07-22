// <copyright file="OpenTKALInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.NativeInterop.OpenAL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using OpenTK.Audio.OpenAL;
    using RaptorALFormat = ALFormat;
    using RaptorALSourcei = ALSourcei;
    using RaptorALSourceb = ALSourceb;
    using RaptorALSourcef = ALSourcef;
    using RaptorALSourceState = ALSourceState;

    using RaptorAlcGetStringList = AlcGetStringList;
    using TKALSourcei = OpenTK.Audio.OpenAL.ALSourcei;
    using TKALSourceb = OpenTK.Audio.OpenAL.ALSourceb;
    using TKALSourcef = OpenTK.Audio.OpenAL.ALSourcef;
    using TKALFormat = OpenTK.Audio.OpenAL.ALFormat;
    using TKAlcGetStringList = OpenTK.Audio.OpenAL.AlcGetStringList;

    // TODO: Contert all IntPtr values to nint

    /// <summary>
    /// Invokes OpenAL functions.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class OpenTKALInvoker : IALInvoker
    {
        private IntPtr devicePtr;

        /// <inheritdoc/>
        public event Action<string>? ErrorCallback;

        /// <inheritdoc/>
        public void BufferData<TBuffer>(uint bid, RaptorALFormat format, TBuffer[] buffer, int size, int freq)
            where TBuffer : unmanaged
        {
            AL.BufferData((int)bid, (TKALFormat)format, buffer, freq);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public bool CloseDevice(IntPtr device)
        {
            var alDevice = new ALDevice(device);

            var closeResult = ALC.CloseDevice(alDevice);
            var error = ALC.GetError(alDevice);
            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            if (closeResult)
            {
                this.devicePtr = IntPtr.Zero;
            }

            return closeResult;
        }

        /// <inheritdoc/>
        public IntPtr CreateContext(IntPtr device, ALContextAttributes attributes)
        {
            var alDevice = new ALDevice(device);

            var contextResult = ALC.CreateContext(alDevice, attributes);
            var error = ALC.GetError(alDevice);

            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return contextResult.Handle;
        }

        /// <inheritdoc/>
        public void DeleteBuffer(uint buffer)
        {
            AL.DeleteBuffer((int)buffer);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void DeleteSource(uint source)
        {
            AL.DeleteSource((int)source);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void DestroyContext(IntPtr context)
        {
            var alContext = new ALContext(context);
            var device = new ALDevice(GetContextsDevice(alContext));

            ALC.DestroyContext(alContext);
            var error = ALC.GetError(device);
            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public uint GenBuffer()
        {
            var result = (uint)AL.GenBuffer();

            var error = GetError();

            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public uint GenSource()
        {
            var result = (uint)AL.GenSource();

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public bool GetSource(uint sid, RaptorALSourceb param)
        {
            AL.GetSource((int)sid, (TKALSourceb)param, out var result);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public float GetSource(uint sid, RaptorALSourcef param)
        {
            AL.GetSource((int)sid, (TKALSourcef)param, out var value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);

            return value;
        }

        /// <inheritdoc/>
        public RaptorALSourceState GetSourceState(uint sid)
        {
            AL.GetSource((int)sid, ALGetSourcei.SourceState, out var result);

            return (RaptorALSourceState)result;
        }

        /// <inheritdoc/>
        public IList<string> GetListOfDevices()
        {
            var device = new ALDevice(this.devicePtr);

            var result = ALC.GetString(device, TKAlcGetStringList.AllDevicesSpecifier);
            var error = ALC.GetError(device);
            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return result;
        }

        /// <inheritdoc/>
        public bool MakeContextCurrent(IntPtr context)
        {
            bool result;

            var alContext = new ALContext(context);

            // If the context is null, then the attempt is to destroy the context
            if (alContext == ALContext.Null)
            {
                result = ALC.MakeContextCurrent(alContext);

                if (!result)
                {
                    InvokeErrorIfTrue(true, "Issue destroying the context.");
                }
            }
            else
            {
                result = ALC.MakeContextCurrent(alContext);

                var device = new ALDevice(GetContextsDevice(alContext));

                var error = ALC.GetError(device);

                if (result)
                {
                    var errorMessage = Enum.GetName(typeof(AlcError), error);
                    InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);
                }
                else
                {
                    // Throw an error that the context could not be made current
                    InvokeErrorIfTrue(true, $"Context with handle '{context}' could not be made current.");
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public IntPtr OpenDevice(string? deviceName)
        {
            var deviceResult = ALC.OpenDevice(deviceName);
            var error = ALC.GetError(deviceResult);

            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            this.devicePtr = deviceResult.Handle;

            return deviceResult;
        }

        /// <inheritdoc/>
        public void Source(uint sid, RaptorALSourcei param, int value)
        {
            AL.Source((int)sid, (TKALSourcei)param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void Source(uint sid, RaptorALSourceb param, bool value)
        {
            AL.Source((int)sid, (TKALSourceb)param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void Source(uint sid, RaptorALSourcef param, float value)
        {
            AL.Source((int)sid, (TKALSourcef)param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void SourcePause(uint sid)
        {
            AL.SourcePause((int)sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void SourcePlay(uint sid)
        {
            AL.SourcePlay((int)sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void SourceRewind(uint sid)
        {
            AL.SourceRewind((int)sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <inheritdoc/>
        public void SourceStop(uint sid)
        {
            AL.SourceStop((int)sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(ALError), error);

            InvokeErrorIfTrue(error != ALError.NoError, errorMessage);
        }

        /// <summary>
        ///     Error support. Obtain the most recent error generated in the AL state machine.
        ///     When an error is detected by AL, a flag is set and the error code is recorded.
        ///     Further errors, if they occur, do not affect this recorded code. When alGetError
        ///     is called, the code is returned and the flag is cleared, so that a further error
        ///     will again record its code.
        /// </summary>
        /// <returns>
        ///     The first error that occurred. can be used with AL.GetString. Returns an OpenAL enum
        ///     representing the error state. When an OpenAL error occurs, the error state is
        ///     set and will not be changed until the error state is retrieved using alGetError.
        ///     Whenever alGetError is called, the error state is cleared and the last state
        ///     (the current state when the call was made) is returned. To isolate error detection
        ///     to a specific portion of code, alGetError should be called before the isolated
        ///     section to clear the current error state.
        /// </returns>
        private ALError GetError() => AL.GetError();

        /// <summary>
        /// This function retrieves a context's device pointer.
        /// </summary>
        /// <param name="context">A pointer to a context.</param>
        /// <returns>Returns a pointer to the specified context's device.</returns>
        private ALDevice GetContextsDevice(ALContext context)
        {
            var deviceResult = ALC.GetContextsDevice(context);

            var error = ALC.GetError(deviceResult);
            var errorMessage = Enum.GetName(typeof(AlcError), error);

            InvokeErrorIfTrue(error != AlcError.NoError, errorMessage);

            return deviceResult;
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
