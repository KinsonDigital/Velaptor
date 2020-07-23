using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Audio.OpenAL;

namespace Raptor.OpenAL
{
    internal class ALCInvoker : IALCInvoker
    {
        public Action<string> ErrorCallback { get; set; }

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

        public ALDevice OpenDevice(string devicename)
        {
            var deviceResult = ALC.OpenDevice(devicename);
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
