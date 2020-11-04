// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1116 // Split parameters should start on line after declaration
namespace Raptor
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Raptor.Audio;
    using Raptor.Content;
    using Raptor.Graphics;
    using Raptor.OpenAL;
    using Raptor.OpenGL;
    using Raptor.Services;
    using SimpleInjector;
    using SimpleInjector.Diagnostics;

    /// <summary>
    /// Provides dependency injection for the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class IoC
    {
        private static readonly FileSystem FileSystem = new FileSystem();
        private static readonly Container IoCContainer = new Container();
        private static bool isInitialized;

        /// <summary>
        /// Gets the inversion of control container used to get instances of objects.
        /// </summary>
        public static Container Container
        {
            get
            {
                if (!isInitialized)
                    SetupContainer();

                return IoCContainer;
            }
        }

        /// <summary>
        /// Sets up the IoC container.
        /// </summary>
        private static void SetupContainer()
        {
            SetupOpenTK();

            SetupServices();

            SetupContent();

            isInitialized = true;
        }

        /// <summary>
        /// Setup container registration related to OpenTK.
        /// </summary>
        private static void SetupOpenTK()
        {
            IoCContainer.Register<IPlatform, Platform>(Lifestyle.Singleton);
            IoCContainer.Register<IGLInvoker, GLInvoker>(Lifestyle.Singleton);
            IoCContainer.Register(() => FileSystem.File);
            IoCContainer.Register(() => FileSystem.Directory);
            IoCContainer.Register<IALInvoker, ALInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGLFWInvoker, GLFWInvoker>(Lifestyle.Singleton);

            IoCContainer.Register<GLFWMonitors>();

            IoCContainer.Register<IGPUBuffer>(() =>
            {
                return new GPUBuffer<VertexData>(IoCContainer.GetInstance<IGLInvoker>());
            }, true);

            IoCContainer.Register<IShaderProgram, ShaderProgram>(true);

            IoCContainer.Register<ISpriteBatch>(() =>
            {
                return new SpriteBatch(IoCContainer.GetInstance<IGLInvoker>(), IoCContainer.GetInstance<IShaderProgram>(), IoCContainer.GetInstance<IGPUBuffer>());
            }, true);

            SetupAudio();
        }

        /// <summary>
        /// Setup container registration related to audio.
        /// </summary>
        private static void SetupAudio()
        {
            // Register the proper data stream to be the implementation if the consumer is a certain decoder
            IoCContainer.RegisterConditional<IAudioDataStream<float>, OggAudioDataStream>(context =>
            {
                return !context.HasConsumer || context.Consumer.ImplementationType == typeof(OggSoundDecoder);
            }, true);

            IoCContainer.RegisterConditional<IAudioDataStream<byte>, Mp3AudioDataStream>(context =>
            {
                return !context.HasConsumer || context.Consumer.ImplementationType == typeof(MP3SoundDecoder);
            }, true);

            IoCContainer.Register<ISoundDecoder<float>, OggSoundDecoder>(true);
            IoCContainer.Register<ISoundDecoder<byte>, MP3SoundDecoder>(true);
        }

        /// <summary>
        /// Setup container registration related to services.
        /// </summary>
        private static void SetupServices()
        {
            IoCContainer.Register<IImageFileService, ImageFileService>();
            IoCContainer.Register<IEmbeddedResourceLoaderService, EmbeddedResourceLoaderService>();
            IoCContainer.Register<ISystemMonitorService, SystemMonitorService>();
        }

        /// <summary>
        /// Setup container registration related to content.
        /// </summary>
        private static void SetupContent()
        {
            IoCContainer.Register<ILoader<ITexture>>(() =>
            {
                return new TextureLoader(
                    IoCContainer.GetInstance<IGLInvoker>(),
                    IoCContainer.GetInstance<IImageFileService>(),
                    new GraphicsContentSource(IoCContainer.GetInstance<IDirectory>()));
            });

            IoCContainer.Register<IContentLoader>(() =>
            {
                return new ContentLoader(Container.GetInstance<ILoader<ITexture>>(), Container.GetInstance<ILoader<ISound>>());
            });

            IoCContainer.Register<ILoader<ISound>>(() =>
            {
                return new SoundLoader(
                    IoCContainer.GetInstance<IALInvoker>(),
                    AudioDeviceManager.GetInstance(Container.GetInstance<IALInvoker>()),
                    new SoundContentSource(IoCContainer.GetInstance<IDirectory>()),
                    IoCContainer.GetInstance<ISoundDecoder<float>>(),
                    IoCContainer.GetInstance<ISoundDecoder<byte>>());
            });

            IoCContainer.Register<ILoader<AtlasRegionRectangle[]>>(() =>
            {
                return new AtlasDataLoader<AtlasRegionRectangle>(new AtlasContentSource(IoCContainer.GetInstance<IDirectory>()), IoCContainer.GetInstance<IFile>());
            });
        }
    }
}
