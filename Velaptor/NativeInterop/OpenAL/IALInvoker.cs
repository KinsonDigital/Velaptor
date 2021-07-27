// <copyright file="IALInvoker.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.OpenAL
{
    using System;
    using System.Collections.Generic;
    using OpenTK.Audio.OpenAL;
    using VelaptorALFormat = ALFormat;
    using VelaptorALSourceState = ALSourceState;

    /// <summary>
    /// Invokes OpenAL functions.
    /// </summary>
    internal interface IALInvoker
    {
        /// <summary>
        /// Occurs when an OpenAL error occurs.
        /// </summary>
        event Action<string>? ErrorCallback;

        /// <summary>
        ///     This function fills a buffer with audio buffer. All the predefined formats are
        ///     PCM buffer, but this function may be used by extensions to load other buffer
        ///     types as well.
        /// </summary>
        /// <typeparam name="TBuffer">The type of the data buffer.</typeparam>
        /// <param name="bid">Buffer Handle/Name to be filled with buffer.</param>
        /// <param name="format">
        ///     Format type from among the following: ALFormat.Mono8,
        ///     ALFormat.Mono16, ALFormat.Stereo8, ALFormat.Stereo16.
        /// </param>
        /// <param name="buffer">The audio buffer.</param>
        /// <param name="size">The size of the audio buffer in bytes.</param>
        /// <param name="freq">The frequency of the audio buffer.</param>
        void BufferData<TBuffer>(uint bid, VelaptorALFormat format, TBuffer[] buffer, int size, int freq)
            where TBuffer : unmanaged;

        /// <summary>
        /// This function closes a device by name.
        /// </summary>
        /// <param name="device">A pointer to an opened device.</param>
        /// <returns>
        ///     <see langword="true"/> will be returned on success or <see langword="false"/> on failure. Closing a device will fail
        ///     if the device contains any contexts or buffers.
        /// </returns>
        bool CloseDevice(IntPtr device);

        /// <summary>
        /// This function creates a context using a specified device.
        /// </summary>
        /// <param name="device">A pointer to a device.</param>
        /// <param name="attributes">The ALContext attributes to request.</param>
        /// <returns>Returns a pointer to the new context (NULL on failure).</returns>
        /// <remarks>
        ///     The attribute list can be NULL, or a zero terminated list of integer pairs composed
        ///     of valid ALC attribute tokens and requested values.
        /// </remarks>
        IntPtr CreateContext(IntPtr device, ALContextAttributes attributes);

        /// <summary>
        ///     This function deletes one buffer only, freeing the resources used by the buffer.
        ///     Buffers which are attached to a source can not be deleted. See AL.Source (ALSourcei)
        ///     and AL.SourceUnqueueBuffers for information on how to detach a buffer from a
        ///     source.
        /// </summary>
        /// <param name="buffer">Pointer to a buffer name identifying the buffer to be deleted.</param>
        void DeleteBuffer(uint buffer);

        /// <summary>
        /// This function deletes one source only.
        /// </summary>
        /// <param name="source">Pointer to a source name identifying the source to be deleted.</param>
        void DeleteSource(uint source);

        /// <summary>
        /// This function destroys a context.
        /// </summary>
        /// <param name="context">A pointer to the new context.</param>
        void DestroyContext(IntPtr context);

        /// <summary>
        ///     This function generates one buffer only, which contain audio data (see AL.BufferData).
        ///     References to buffers are uint values, which are used wherever a buffer reference
        ///     is needed (in calls such as AL.DeleteBuffers, AL.Source with parameter ALSourcei,
        ///     AL.SourceQueueBuffers, and AL.SourceUnqueueBuffers).
        /// </summary>
        /// <returns>Pointer to an uint value which will store the name of the new buffer.</returns>
        uint GenBuffer();

        /// <summary>
        /// This function generates one source only.
        /// </summary>
        /// <returns>Pointer to an int value which will store the name of the new source.</returns>
        uint GenSource();

        /// <summary>
        /// This function opens a device by name.
        /// </summary>
        /// <param name="deviceName">A null-terminated string describing a device.</param>
        /// <returns>
        ///     Returns a pointer to the opened device. The return
        ///     value will be NULL if there is an error.
        /// </returns>
        IntPtr OpenDevice(string? deviceName);

        /// <summary>
        /// This function makes a specified context the current context.
        /// </summary>
        /// <param name="context">A pointer to the new context.</param>
        /// <returns>Returns <see langword="true"/> on success, or <see langword="false"/> on failure.</returns>
        bool MakeContextCurrent(IntPtr context);

        /// <summary>
        /// This function retrieves a bool property of a source.
        /// </summary>
        /// <param name="sid">Source name whose attribute is being retrieved.</param>
        /// <param name="param">The name of the attribute to get: ALSourceb.SourceRelative, Looping.</param>
        /// <returns>A pointer to the bool value being retrieved.</returns>
        bool GetSource(uint sid, ALSourceb param);

        /// <summary>
        /// This function retrieves a floating-point property of a source.
        /// </summary>
        /// <param name="sid">Source name whose attribute is being retrieved.</param>
        /// <param name="param">
        ///     The name of the attribute to set: ALSourcef.Pitch, Gain, MinGain, MaxGain, MaxDistance,
        ///     RolloffFactor, ConeOuterGain, ConeInnerAngle, ConeOuterAngle, SecOffset, ReferenceDistance,
        ///     EfxAirAbsorptionFactor, EfxRoomRolloffFactor, EfxConeOuterGainHighFrequency.
        /// </param>
        /// <returns>A pointer to the floating-point value being retrieved.</returns>
        float GetSource(uint sid, ALSourcef param);

        /// <summary>
        /// This function retrieves an integer property of a source.
        /// </summary>
        /// <param name="sid">Source name whose attribute is being retrieved.</param>
        /// <returns>The source state.</returns>
        VelaptorALSourceState GetSourceState(uint sid);

        /// <summary>
        /// Returns a list of all of the current audio devices in the system.
        /// </summary>
        /// <returns>The list of devices.</returns>
        IList<string> GetListOfDevices();

        /// <summary>
        /// This function sets an integer property of a source.
        /// </summary>
        /// <param name="sid">Source name whose attribute is being set.</param>
        /// <param name="param">
        ///     The name of the attribute to set: ALSourcei.SourceRelative, ConeInnerAngle, ConeOuterAngle,
        ///     Looping, Buffer, SourceState.
        /// </param>
        /// <param name="value">The value to set the attribute to.</param>
        void Source(uint sid, ALSourcei param, int value);

        /// <summary>
        /// This function sets an bool property of a source.
        /// </summary>
        /// <param name="sid">Source name whose attribute is being set.</param>
        /// <param name="param">The name of the attribute to set: ALSourceb.SourceRelative, Looping.</param>
        /// <param name="value">The value to set the attribute to.</param>
        void Source(uint sid, ALSourceb param, bool value);

        /// <summary>
        /// This function sets a floating-point property of a source.
        /// </summary>
        /// <param name="sid">Source name whose attribute is being set.</param>
        /// <param name="param">
        ///     The name of the attribute to set: ALSourcef.Pitch, Gain, MinGain, MaxGain, MaxDistance,
        ///     RolloffFactor, ConeOuterGain, ConeInnerAngle, ConeOuterAngle, SecOffset, ReferenceDistance,
        ///     EfxAirAbsorptionFactor, EfxRoomRolloffFactor, EfxConeOuterGainHighFrequency.
        /// </param>
        /// <param name="value">The value to set the attribute to.</param>
        void Source(uint sid, ALSourcef param, float value);

        /// <summary>
        ///     This function plays, replays or resumes a source. The playing source will have
        ///     it's state changed to ALSourceState.Playing. When called on a source which is
        ///     already playing, the source will restart at the beginning. When the attached
        ///     buffer(s) are done playing, the source will progress to the ALSourceState.Stopped
        ///     state.
        /// </summary>
        /// <param name="sid">The name of the source to be played.</param>
        void SourcePlay(uint sid);

        /// <summary>
        ///     This function pauses a source. The paused source will have its state changed
        ///     to ALSourceState.Paused.
        /// </summary>
        /// <param name="sid">The name of the source to be paused.</param>
        void SourcePause(uint sid);

        /// <summary>
        ///     This function stops a source. The stopped source will have it's state changed
        ///     to ALSourceState.Stopped.
        /// </summary>
        /// <param name="sid">The name of the source to be stopped.</param>
        void SourceStop(uint sid);

        /// <summary>
        ///     This function stops the source and sets its state to ALSourceState.Initial.
        /// </summary>
        /// <param name="sid">The name of the source to be rewound.</param>
        void SourceRewind(uint sid);
    }
}
