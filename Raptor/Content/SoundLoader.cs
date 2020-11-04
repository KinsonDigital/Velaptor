// <copyright file="SoundLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Raptor.Audio;
    using Raptor.Audio.Exceptions;
    using Raptor.OpenAL;

    /// <summary>
    /// Loads sound content.
    /// </summary>
    public class SoundLoader : ILoader<ISound>
    {
        private readonly IALInvoker alInvoker;
        private readonly IAudioDeviceManager audioManager;
        private readonly IContentSource contentSource;
        private readonly ISoundDecoder<float> oggDecoder;
        private readonly ISoundDecoder<byte> mp3Decoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoader"/> class.
        /// </summary>
        /// <param name="contentSource">Manages the source of the content.</param>
        /// <param name="oggDecoder">Decodes ogg sound files.</param>
        /// <param name="mp3Decoder">Decodes mp3 sound files.</param>
        [ExcludeFromCodeCoverage]
        public SoundLoader(IContentSource contentSource, ISoundDecoder<float> oggDecoder, ISoundDecoder<byte> mp3Decoder)
        {
            this.contentSource = contentSource;
            this.oggDecoder = oggDecoder;
            this.mp3Decoder = mp3Decoder;
            this.alInvoker = new ALInvoker();
            this.audioManager = AudioDeviceManager.GetInstance(this.alInvoker);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoader"/> class.
        /// </summary>
        /// <param name="alInvoker">Make calls to OpenAL.</param>
        /// <param name="audioManager">Manages the audio devices.</param>
        /// <param name="contentSource">Manages the source of the content.</param>
        /// <param name="oggDecoder">Decodes ogg sound files.</param>
        /// <param name="mp3Decoder">Decodes mp3 sound files.</param>
        internal SoundLoader(IALInvoker alInvoker, IAudioDeviceManager audioManager, IContentSource contentSource, ISoundDecoder<float> oggDecoder, ISoundDecoder<byte> mp3Decoder)
        {
            this.alInvoker = alInvoker;
            this.audioManager = audioManager;
            this.contentSource = contentSource;
            this.oggDecoder = oggDecoder;
            this.mp3Decoder = mp3Decoder;
        }

        /// <summary>
        /// Loads a sound with the given name.
        /// </summary>
        /// <param name="name">The name of the sound content to load.</param>
        /// <returns>The name of the sound content item to load.</returns>
        /// <remarks>
        ///     The content <see cref="name"/> can have or not have the extension of the content file to load.
        ///     Using the file extension has no effect and the content item is matched using the name without the extension.
        ///     The only sounds supported are .ogg and .mp3 files.
        ///     Also, duplicate names are not aloud in the same content source and this is matched against the name without the extension.
        ///
        ///     Example: sound.ogg and sound.mp3 will result in an exception.
        /// </remarks>
        /// <exception cref="UnsupportedSoundTypeException">
        ///     Thrown when a sound file extension other then '.ogg' and '.mp3' are used.
        /// </exception>
        public ISound Load(string name)
        {
            var extension = Path.GetExtension(name);

            if (!new[] { ".ogg", ".mp3" }.Contains(extension))
                throw new UnsupportedSoundTypeException($"The extension '{extension}' is not supported.  Supported audio files are '.ogg' and '.mp3'.");

            return new Sound(
                name,
                this.alInvoker,
                this.audioManager,
                this.oggDecoder,
                this.mp3Decoder,
                this.contentSource);
        }
    }
}
