using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using Silk.NET.OpenAL;
using SilkAL = Silk.NET.OpenAL.AL;
using SilkALC = Silk.NET.OpenAL.ALContext;
using VelaptorALFormat = Velaptor.NativeInterop.OpenAL.ALFormat;
using VelaptorALSourcei = Velaptor.NativeInterop.OpenAL.ALSourcei;
using VelaptorALSourceb = Velaptor.NativeInterop.OpenAL.ALSourceb;
using VelaptorALSourcef = Velaptor.NativeInterop.OpenAL.ALSourcef;
using VelaptorALSourceState = Velaptor.NativeInterop.OpenAL.ALSourceState;

using SilkALSourcei = Silk.NET.OpenAL.GetSourceInteger;
using SilkALSourceb = Silk.NET.OpenAL.SourceBoolean;
using SilkALSourcef = Silk.NET.OpenAL.SourceFloat;
using SilkGetContextString = Silk.NET.OpenAL.GetContextString;

using TKALContext = OpenTK.Audio.OpenAL.ALContext;
using Silk.NET.OpenAL.Extensions.EXT;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Velaptor.NativeInterop.OpenAL
{
    internal class SilkALInvoker : IALInvoker
    {
        private readonly SilkAL al;
        private readonly SilkALC alc;
        private readonly MP3Format mp3FormatExtension;
        private readonly VorbisFormat oggFormatExtension;

        public event Action<string>? ErrorCallback;

        public SilkALInvoker()
        {
            this.al = SilkAL.GetApi();
            this.alc = SilkALC.GetApi();

            var mp3ExtensionPresent = this.al.IsExtensionPresent("AL_EXT_MP3");

            if (al.IsExtensionPresent("AL_EXT_float") || al.IsExtensionPresent("AL_EXT_float32"))
            {

            }

            var floatFormat = this.al.GetExtension<FloatFormat>();

            this.mp3FormatExtension = this.al.GetExtension<MP3Format>();
            this.oggFormatExtension = this.al.GetExtension<VorbisFormat>();
        }

        public void BufferData<TBuffer>(uint bid, VelaptorALFormat format, TBuffer[] buffer, int size, int freq)
            where TBuffer : unmanaged
        {
            switch (format)
            {
                case VelaptorALFormat.Mono8:
                    throw new Exception("Nope");
                case VelaptorALFormat.Mono16:
                    throw new Exception("Nope");
                case VelaptorALFormat.Stereo8:
                    throw new Exception("Nope");
                case VelaptorALFormat.Stereo16:
                    this.mp3FormatExtension.BufferData(bid, MP3BufferFormat.MP3, buffer, freq);
                    break;
                case VelaptorALFormat.MonoFloat32:
                    throw new Exception("Nope");
                case VelaptorALFormat.StereoFloat32:
                    this.oggFormatExtension.BufferData(bid, VorbisBufferFormat.Vorbis, buffer, freq);
                    break;
                default:
                    throw new Exception("Nope");
            }
        }

        public bool CloseDevice(IntPtr device)
        {
            unsafe
            {
                var devicePtr = (Device*)device;

                var closeResult = this.alc.CloseDevice(devicePtr);

                var error = this.alc.GetError(devicePtr);
                var errorMessage = Enum.GetName(typeof(ContextError), error);

                InvokeErrorIfTrue(error != ContextError.NoError, errorMessage);

                return closeResult;
            }
        }

        public IntPtr CreateContext(IntPtr device, ALContextAttributes attributes)
        {
            unsafe
            {
                var alDevice = (Device*)device;

                // TODO: Hopefully the attribute list of an empty int[] pointer will work

                var contextResult = this.alc.CreateContext(alDevice, (int*)IntPtr.Zero);
                var error = this.alc.GetError(alDevice);

                var errorMessage = Enum.GetName(typeof(ContextError), error);

                InvokeErrorIfTrue(error != ContextError.NoError, errorMessage);

                return (IntPtr)contextResult;
            }
        }

        public void DeleteBuffer(uint buffer)
        {
            this.al.DeleteBuffer(buffer);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
        }

        public void DeleteSource(uint source)
        {
            this.al.DeleteSource(source);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
        }

        public void DestroyContext(IntPtr context)
        {
            unsafe
            {
                var unmanagedContextPtr = (Context*)context;

                var device = GetContextsDevice(unmanagedContextPtr);

                this.alc.DestroyContext(unmanagedContextPtr);
                var error = this.alc.GetError(device);
                var errorMessage = Enum.GetName(typeof(ContextError), error);

                InvokeErrorIfTrue(error != ContextError.NoError, errorMessage);
            }
        }

        public uint GenBuffer()
        {
            var result = this.al.GenBuffer();

            var error = GetError();

            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);

            return result;
        }

        public uint GenSource()
        {
            var result = this.al.GenSource();

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);

            return result;
        }

        public bool GetSource(uint sid, VelaptorALSourceb param)
        {
            this.al.GetSourceProperty(sid, (SilkALSourceb)param, out var result);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);

            return result;
        }

        public float GetSource(uint sid, VelaptorALSourcef param)
        {
            this.al.GetSourceProperty(sid, (SilkALSourcef)param, out var value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);

            return value;
        }

        public VelaptorALSourceState GetSourceState(uint sid)
        {
            this.al.GetSourceProperty(sid, SilkALSourcei.SourceType, out var result);

            return (VelaptorALSourceState)result;
        }

        public IList<string> GetListOfDevices()
        {
            unsafe
            {
                var alDevice = (Device*)IntPtr.Zero;

                // The value here should be a string of names that is delmited with NULL terminated characters
                var result = this.alc.GetContextProperty(alDevice, SilkGetContextString.DeviceSpecifier);

                // Parse the string above
                Debugger.Break();

                var error = this.alc.GetError(alDevice);
                var errorMessage = Enum.GetName(typeof(ContextError), error);

                InvokeErrorIfTrue(error != ContextError.NoError, errorMessage);

                // TODO: Fix this
                return new string[0];
            }
        }

        public bool MakeContextCurrent(IntPtr context)
        {
            bool result;

            unsafe
            {
                var alContext = (Context*)context;

                // If the context is null, then the attempt is to destroy the context
                if (context == IntPtr.Zero)
                {
                    result = this.alc.MakeContextCurrent(alContext);

                    if (!result)
                    {
                        InvokeErrorIfTrue(true, "Issue destroying the context.");
                    }
                }
                else
                {
                    result = this.alc.MakeContextCurrent(alContext);

                    var device = GetContextsDevice(alContext);

                    var error = this.alc.GetError(device);

                    if (result)
                    {
                        var errorMessage = Enum.GetName(typeof(ContextError), error);
                        InvokeErrorIfTrue(error != ContextError.NoError, errorMessage);
                    }
                    else
                    {
                        // Throw an error that the context could not be made current
                        InvokeErrorIfTrue(true, $"Context with handle '{context}' could not be made current.");
                    }
                }
            }

            return result;
        }

        public IntPtr OpenDevice(string? deviceName)
        {
            unsafe
            {
                var deviceResult = this.alc.OpenDevice(deviceName);
                var error = this.alc.GetError(deviceResult);

                var errorMessage = Enum.GetName(typeof(ContextError), error);

                InvokeErrorIfTrue(error != ContextError.NoError, errorMessage);

                // TODO: If this casting does not work, use Marshal.PtrToStructure<T>()
                return (IntPtr)deviceResult;
            }
        }

        public void Source(uint sid, VelaptorALSourcei param, int value)
        {
            this.al.SetSourceProperty(sid, (SourceInteger)param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
        }

        public void Source(uint sid, VelaptorALSourceb param, bool value)
        {
            this.al.SetSourceProperty(sid, (SilkALSourceb)param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
        }

        public void Source(uint sid, VelaptorALSourcef param, float value)
        {
            this.al.SetSourceProperty(sid, (SilkALSourcef)param, value);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
        }

        public void SourcePause(uint sid)
        {
            this.al.SourcePause(sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
        }

        public void SourcePlay(uint sid)
        {
            this.al.SourcePlay(sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
        }

        public void SourceRewind(uint sid)
        {
            this.al.SourceRewind(sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
        }

        public void SourceStop(uint sid)
        {
            this.al.SourceStop(sid);

            var error = GetError();
            var errorMessage = Enum.GetName(typeof(AudioError), error);

            InvokeErrorIfTrue(error != AudioError.NoError, errorMessage);
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
        private AudioError GetError() => this.al.GetError();

        /// <summary>
        /// This function retrieves a context's device pointer.
        /// </summary>
        /// <param name="context">A pointer to a context.</param>
        /// <returns>Returns a pointer to the specified context's device.</returns>
        private unsafe Device* GetContextsDevice(Context* context)
        {
            var deviceResult = this.alc.GetContextsDevice(context);

            var error = this.alc.GetError(deviceResult);
            var errorMessage = Enum.GetName(typeof(ContextError), error);

            InvokeErrorIfTrue(error != ContextError.NoError, errorMessage);

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
