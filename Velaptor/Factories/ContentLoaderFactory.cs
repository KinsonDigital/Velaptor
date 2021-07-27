// <copyright file="ContentLoaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Velaptor.Audio;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop;
    using Velaptor.NativeInterop.OpenAL;
    using Velaptor.Services;

    /// <summary>
    /// Creates instances of a content loader.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ContentLoaderFactory
    {
        private static IContentLoader? contentLoader;
        private static ILoader<ITexture>? textureLoader;
        private static ILoader<IAtlasData>? atlasLoader;
        private static ILoader<ISound>? soundLoader;
        private static ILoader<IFont>? fontLoader;

        /// <summary>
        /// Creates a single instance of a content loader.
        /// </summary>
        /// <returns>A framework content loader implementation.</returns>
        public static IContentLoader CreateContentLoader()
        {
            if (contentLoader is null)
            {
                contentLoader = new ContentLoader(
                    CreateTextureLoader(),
                    CreateSoundLoader(),
                    CreateTextureAtlasLoader(),
                    CreateFontLoader());
            }

            return contentLoader;
        }

        /// <summary>
        /// Creates a loader that loads textures from disk.
        /// </summary>
        /// <returns>A loader for loading textures.</returns>
        public static ILoader<ITexture> CreateTextureLoader()
        {
            if (textureLoader is null)
            {
                var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
                var imageService = IoC.Container.GetInstance<IImageService>();
                var texturePathResolver = new TexturePathResolver(IoC.Container.GetInstance<IDirectory>());

                textureLoader = new TextureLoader(
                    glInvoker,
                    imageService,
                    texturePathResolver);
            }

            return textureLoader;
        }

        /// <summary>
        /// Creates a loader for loading atlas data from disk.
        /// </summary>
        /// <returns>A loader for loading texture atlas data.</returns>
        public static ILoader<IAtlasData> CreateTextureAtlasLoader()
        {
            if (atlasLoader is null)
            {
                var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
                var atlasDataPathResolver = new AtlasJSONDataPathResolver(IoC.Container.GetInstance<IDirectory>());

                atlasLoader = new AtlasLoader(
                    glInvoker,
                    IoC.Container.GetInstance<IImageService>(),
                    atlasDataPathResolver,
                    IoC.Container.GetInstance<IFile>());
            }

            return atlasLoader;
        }

        /// <summary>
        /// Creates a loader that loads sounds from disk.
        /// </summary>
        /// <returns>A loader for loading sound data.</returns>
        public static ILoader<ISound> CreateSoundLoader()
        {
            if (soundLoader is null)
            {
                var alInvoker = IoC.Container.GetInstance<IALInvoker>();
                var audioManager = AudioDeviceManagerFactory.CreateDeviceManager();
                var soundPathResolver = new SoundPathResolver(IoC.Container.GetInstance<IDirectory>());
                var oggDecoder = IoC.Container.GetInstance<ISoundDecoder<float>>();
                var mp3Decoder = IoC.Container.GetInstance<ISoundDecoder<byte>>();

                soundLoader = new SoundLoader(
                    alInvoker,
                    audioManager,
                    soundPathResolver,
                    oggDecoder,
                    mp3Decoder);
            }

            return soundLoader;
        }

        /// <summary>
        /// Creates a loader that loads fonts from disk for rendering test.
        /// </summary>
        /// <returns>A loader for loading sound data.</returns>
        public static ILoader<IFont> CreateFontLoader()
        {
            if (fontLoader is null)
            {
                var glInvoker = IoC.Container.GetInstance<IGLInvoker>();
                var freeTypeInvoker = IoC.Container.GetInstance<IFreeTypeInvoker>();
                var fontPathResolver = new FontPathResolver(IoC.Container.GetInstance<IDirectory>());
                var fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();

                fontLoader = new FontLoader(
                    glInvoker,
                    freeTypeInvoker,
                    fontAtlasService,
                    fontPathResolver,
                    IoC.Container.GetInstance<IFile>(),
                    IoC.Container.GetInstance<IImageService>());
            }

            return fontLoader;
        }
    }
}
