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
        private static readonly FileSystem FileSystem;
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
            IoCContainer.Register<IImageFileService, ImageFileService>();
            IoCContainer.Register<ILoader<ITexture>, TextureLoader>();
            IoCContainer.Register<ILoader<ISound>, SoundLoader>();
            IoCContainer.Register<IALInvoker, ALInvoker>();

            // Register the proper data stream to be the implementation if the consumer is a certain decoder
            IoCContainer.RegisterConditional<IAudioDataStream<float>, OggAudioDataStream>(context =>
            {
                return !context.HasConsumer || context.Consumer.ImplementationType == typeof(OggSoundDecoder);
            });
            SuppressDisposableTransientWarning<IAudioDataStream<float>>();

            IoCContainer.RegisterConditional<IAudioDataStream<byte>, Mp3AudioDataStream>(context =>
            {
                return !context.HasConsumer || context.Consumer.ImplementationType == typeof(MP3SoundDecoder);
            });
            SuppressDisposableTransientWarning<IAudioDataStream<byte>>();

            IoCContainer.Register<ISoundDecoder<float>, OggSoundDecoder>();
            SuppressDisposableTransientWarning<ISoundDecoder<float>>();

            IoCContainer.Register<ISoundDecoder<byte>, MP3SoundDecoder>();
            SuppressDisposableTransientWarning<ISoundDecoder<byte>>();

            IoCContainer.Register<IContentSource, ContentSource>();
            IoCContainer.Register<IContentLoader, ContentLoader>();
            IoCContainer.Register<ILoader<AtlasRegionRectangle[]>, AtlasDataLoader<AtlasRegionRectangle>>();

            IoCContainer.Register<IGPUBuffer>(() =>
            {
                return new GPUBuffer<VertexData>(IoCContainer.GetInstance<IGLInvoker>());
            });
            SuppressDisposableTransientWarning<IGPUBuffer>();

            IoCContainer.Register<IShaderProgram>(() =>
            {
                return new ShaderProgram(IoCContainer.GetInstance<IGLInvoker>(), IoCContainer.GetInstance<IFile>());
            });
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
