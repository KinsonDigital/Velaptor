using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raptor.NativeInterop.OpenAL
{
    /// <summary>
    /// A list of valid Int32 Source parameters.
    /// </summary>
    internal enum ALSourceInteger
    {
        /// <summary>
        /// Indicate the Buffer to provide sound samples. Type: uint Range: any valid Buffer
        /// </summary>
        Buffer = 4105,
    }

    internal enum ALFormat
    {
        Mono8 = 4352,
        Mono16 = 4353,
        Stereo8 = 4354,
        Stereo16 = 4355,
        MonoFloat32 = 65552,
        StereoFloat32 = 65553,
    }

    /// <summary>
    /// A list of valid Int32 Source parameters.
    /// </summary>
    internal enum ALSourcei
    {
        /// <summary>
        /// Indicate the Buffer to provide sound samples. Type: uint Range: any valid Buffer Handle.
        /// </summary>
        Buffer = 4105,
    }

    /// <summary>
    /// A list of valid 8-bit boolean Source/GetSource parameters.
    /// </summary>
    internal enum ALSourceb
    {
        /// <summary>
        /// Indicate whether the Source is looping. Type: bool Range: [True, False] Default: False.
        /// </summary>
        Looping = 4103,
    }

    /// <summary>
    /// A list of valid 32-bit Float Source/GetSource parameters.
    /// </summary>
    internal enum ALSourcef
    {
        /// <summary>
        /// Indicate the gain (volume amplification) applied. Type: float. Range: [0.0f - ? ]
        ///  A value of 1.0 means un-attenuated/unchanged. Each division by 2 equals an
        /// attenuation of -6dB. Each multiplicaton with 2 equals an amplification of +6dB.
        /// A value of 0.0f is meaningless with respect to a logarithmic scale; it is interpreted
        /// as zero volume - the channel is effectively disabled.
        /// </summary>
        Gain = 4106,

        /// <summary>
        /// The playback position, expressed in seconds.
        /// </summary>
        SecOffset = 4132,
    }

    /// <summary>
    /// Defines available parameters for OpenTK.Audio.OpenAL.ALC.GetString(OpenTK.Audio.OpenAL.ALDevice,OpenTK.Audio.OpenAL.AlcGetStringList).
    /// </summary>
    internal enum AlcGetStringList
    {
        /// <summary>
        /// The specifier strings for all available devices. ALC_ENUMERATE_ALL_EXT
        /// </summary>
        AllDevicesSpecifier = 4115,
    }

    /// <summary>
    /// Source state information, can be retrieved by AL.Source() with SourceInteger.SourceState.
    /// </summary>
    internal enum ALSourceState
    {
        /// <summary>
        /// Default State when loaded, can be manually set with AL.SourceRewind().
        /// </summary>
        Initial = 4113,

        /// <summary>
        /// The source is currently playing.
        /// </summary>
        Playing,

        /// <summary>
        /// The source has paused playback.
        /// </summary>
        Paused,

        /// <summary>
        /// The source is not playing.
        /// </summary>
        Stopped,
    }

    /// <summary>
    /// Defines available parameters for IContextState.GetContextProperty(Device*,GetContextString).
    /// </summary>
    internal enum GetContextString
    {
        /// <summary>
        /// A list of available context extensions separated by spaces.
        /// </summary>
        Extensions = 4102,

        /// <summary>
        /// Gets the name of the provided device.
        /// </summary>
        DeviceSpecifier = 4101,
    }
}
