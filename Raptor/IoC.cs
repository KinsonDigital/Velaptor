// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

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
            IoCContainer.Register<IGLInvoker, GLInvoker>(Lifestyle.Singleton);
            IoCContainer.Register(() => FileSystem.File);
            IoCContainer.Register(() => FileSystem.Directory);
            IoCContainer.Register<IALInvoker, ALInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGLFWInvoker, GLFWInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IImageFileService, ImageFileService>();
            IoCContainer.Register<IEmbeddedResourceLoaderService, EmbeddedResourceLoaderService>();
            IoCContainer.Register<ISystemMonitorService, SystemMonitorService>();
            IoCContainer.Register<GLFWMonitors>();

            // Register the proper data stream to be the implementation if the consumer is a certain decoder
            IoCContainer.RegisterConditional<IAudioDataStream<float>, OggAudioDataStream>(context =>
            {
                return !context.HasConsumer || context.Consumer.ImplementationType == typeof(OggSoundDecoder);
            });
            SuppressDisposableTransientWarning<IAudioDataStream<float>>(); // TODO: Look into removing the warning suppressions

            IoCContainer.RegisterConditional<IAudioDataStream<byte>, Mp3AudioDataStream>(context =>
            {
                return !context.HasConsumer || context.Consumer.ImplementationType == typeof(MP3SoundDecoder);
            });
            SuppressDisposableTransientWarning<IAudioDataStream<byte>>();

            IoCContainer.Register<ISoundDecoder<float>, OggSoundDecoder>();
            SuppressDisposableTransientWarning<ISoundDecoder<float>>();

            IoCContainer.Register<ISoundDecoder<byte>, MP3SoundDecoder>();
            SuppressDisposableTransientWarning<ISoundDecoder<byte>>();

            IoCContainer.Register<ILoader<ITexture>>(() =>
            {
                return new TextureLoader(
                    IoCContainer.GetInstance<IGLInvoker>(),
                    IoCContainer.GetInstance<IImageFileService>(),
                    new GraphicsContentSource(IoCContainer.GetInstance<IDirectory>()));
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

            // TODO: If using Container works, convert all of the IoCContainer references to Container
            IoCContainer.Register<IContentLoader>(() =>
            {
                return new ContentLoader(Container.GetInstance<ILoader<ITexture>>(), Container.GetInstance<ILoader<ISound>>());
            });

            IoCContainer.Register<IGPUBuffer>(() =>
            {
                return new GPUBuffer<VertexData>(IoCContainer.GetInstance<IGLInvoker>());
            });
            SuppressDisposableTransientWarning<IGPUBuffer>();

            IoCContainer.Register<IShaderProgram, ShaderProgram>();

            SuppressDisposableTransientWarning<IShaderProgram>();

            IoCContainer.Register<ISpriteBatch>(() =>
            {
                return new SpriteBatch(IoCContainer.GetInstance<IGLInvoker>(), IoCContainer.GetInstance<IShaderProgram>(), IoCContainer.GetInstance<IGPUBuffer>());
            });
            SuppressDisposableTransientWarning<ISpriteBatch>();

            isInitialized = true;
        }

        /// <summary>
        /// Suppresses SimpleInjector diagnostic warnings related to disposing of objects when they
        /// inherit from <see cref="IDisposable"/>.
        /// </summary>
        /// <typeparam name="T">The type to suppress against.</typeparam>
        private static void SuppressDisposableTransientWarning<T>()
        {
            /*NOTE:
             * The suppression of the SimpleInjector warning of DiagnosticType.DisposableTransientComponent is for
             * classes that are disposable.  This tells simple injector that the disposing of the object will be
             * handled manually by the application/library instead of by simple injector.
             */

            var spriteBatchRegistration = IoCContainer.GetRegistration(typeof(T))?.Registration;
            spriteBatchRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");
        }
    }
}
