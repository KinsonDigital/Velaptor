// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RaptorTests", AllInternalsVisible = true)]

#pragma warning disable SA1116 // Split parameters should start on line after declaration
namespace Raptor
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Raptor.Audio;
    using Raptor.Content;
    using Raptor.Graphics;
    using Raptor.Input;
    using Raptor.NativeInterop;
    using Raptor.OpenAL;
    using Raptor.OpenGL;
    using Raptor.Services;
    using SimpleInjector;

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
                {
                    SetupContainer();
                }

                return IoCContainer;
            }
        }

        /// <summary>
        /// Sets up the IoC container.
        /// </summary>
        private static void SetupContainer()
        {
            SetupOpenGL();

            SetupServices();

            SetupContent();

            IoCContainer.Register<IFreeTypeInvoker, FreeTypeInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IKeyboardInput<KeyCode, KeyboardState>, Keyboard>(Lifestyle.Singleton);
            IoCContainer.Register<IMouseInput<MouseButton, MouseState>, Mouse>(Lifestyle.Singleton);

            isInitialized = true;
        }

        /// <summary>
        /// Setup container registration related to OpenTK.
        /// </summary>
        private static void SetupOpenGL()
        {
            IoCContainer.Register(() => FileSystem.File, Lifestyle.Singleton);
            IoCContainer.Register(() => FileSystem.Directory, Lifestyle.Singleton);
            IoCContainer.Register<IPlatform, Platform>(Lifestyle.Singleton);
            IoCContainer.Register<IGLInvoker, GLInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IALInvoker, ALInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGLFWInvoker, GLFWInvoker>(Lifestyle.Singleton);

            IoCContainer.Register<GLFWMonitors>();

            IoCContainer.Register<IGPUBuffer, GPUBuffer<VertexData>>(Lifestyle.Singleton);

            IoCContainer.Register<IShaderProgram, ShaderProgram>(Lifestyle.Singleton);

            IoCContainer.Register<ISpriteBatch, SpriteBatch>(Lifestyle.Singleton);

            IoCContainer.Register<IGameWindowFacade, GameWindowFacade>(Lifestyle.Singleton, suppressDisposal: true);

            SetupAudio();
        }

        /// <summary>
        /// Setup container registration related to audio.
        /// </summary>
        private static void SetupAudio()
        {
            IoCContainer.Register<IAudioDeviceManager, AudioDeviceManager>(Lifestyle.Singleton);

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
            IoCContainer.Register<IImageService, ImageService>(Lifestyle.Singleton);
            IoCContainer.Register<IEmbeddedResourceLoaderService, EmbeddedResourceLoaderService>(Lifestyle.Singleton);
            IoCContainer.Register<ISystemMonitorService, SystemMonitorService>(Lifestyle.Singleton);
            IoCContainer.Register<IFontAtlasService, FontAtlasService>(Lifestyle.Singleton);

            IoCContainer.Register<ITaskService, TaskService>();
            IoCContainer.SuppressDisposableTransientWarning<ITaskService>();
        }

        /// <summary>
        /// Setup container registration related to content.
        /// </summary>
        private static void SetupContent() => IoCContainer.Register<AtlasTexturePathResolver>();
    }
}
