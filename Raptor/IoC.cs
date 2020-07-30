// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System.Diagnostics.CodeAnalysis;
    using FileIO.Core;
    using FileIO.File;
    using Raptor.Content;
    using Raptor.Graphics;
    using Raptor.OpenGL;
    using Raptor.Audio;
    using SimpleInjector;
    using SimpleInjector.Diagnostics;
    using Raptor.OpenAL;
    using FileIO.Directory;

    /// <summary>
    /// Provides dependency injection for the applcation.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class IoC
    {
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
            IoCContainer.Register<ITextFile, TextFile>();
            IoCContainer.Register<IImageFile, ImageFile>();
            IoCContainer.Register<ILoader<ITexture>, TextureLoader>();
            IoCContainer.Register<ILoader<ISound>, SoundLoader>();
            IoCContainer.Register<IDirectory, Directory>();
            IoCContainer.Register<IALInvoker, ALInvoker>();

            // Register the proper data stream to be the implementation if the consumer is a certain decoder
            IoCContainer.RegisterConditional<IAudioDataStream<float>, OggAudioDataStream>(context =>
            {
                return context.HasConsumer ? context.Consumer.ImplementationType == typeof(OggSoundDecoder) : true;
            });

            var floatAudioDataStreamRegistration = IoCContainer.GetRegistration(typeof(IAudioDataStream<float>))?.Registration;
            floatAudioDataStreamRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            IoCContainer.RegisterConditional<IAudioDataStream<byte>, Mp3AudioDataStream>(context =>
            {
                return context.HasConsumer ? context.Consumer.ImplementationType == typeof(MP3SoundDecoder) : true;
            });

            var byteAudioDataStreamRegistration = IoCContainer.GetRegistration(typeof(IAudioDataStream<byte>))?.Registration;
            byteAudioDataStreamRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            IoCContainer.Register<ISoundDecoder<float>, OggSoundDecoder>();
            var oggDecoderRegistration = IoCContainer.GetRegistration(typeof(ISoundDecoder<float>))?.Registration;
            oggDecoderRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            IoCContainer.Register<ISoundDecoder<byte>, MP3SoundDecoder>();
            var mp3DecoderRegistration = IoCContainer.GetRegistration(typeof(ISoundDecoder<byte>))?.Registration;
            mp3DecoderRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            IoCContainer.Register<IContentSource, ContentSource>();
            IoCContainer.Register<IContentLoader, ContentLoader>();
            IoCContainer.Register<ILoader<AtlasRegionRectangle[]>, AtlasDataLoader<AtlasRegionRectangle>>();

            /*NOTE:
             * The suppression of the SimpleInjector warning of DiagnosticType.DisposableTransientComponent is for
             * classes that are disposable.  This tells simple injector that the disposing of the object will be
             * handled manually by the application/library instead of by simple injector.
             */

            // TODO: Make this instandard registration
            IoCContainer.Register<IGPUBuffer>(() =>
            {
                return new GPUBuffer<VertexData>(IoCContainer.GetInstance<IGLInvoker>());
            });
            var bufferRegistration = IoCContainer.GetRegistration(typeof(IGPUBuffer))?.Registration;
            bufferRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            // TODO: Make this instandard registration
            IoCContainer.Register<IShaderProgram>(() =>
            {
                return new ShaderProgram(IoCContainer.GetInstance<IGLInvoker>(), IoCContainer.GetInstance<ITextFile>());
            });
            var shaderRegistration = IoCContainer.GetRegistration(typeof(IShaderProgram))?.Registration;
            shaderRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            // TODO: Make this instandard registration
            IoCContainer.Register<ISpriteBatch>(() =>
            {
                return new SpriteBatch(IoCContainer.GetInstance<IGLInvoker>(), IoCContainer.GetInstance<IShaderProgram>(), IoCContainer.GetInstance<IGPUBuffer>());
            });
            var spriteBatchRegistration = IoCContainer.GetRegistration(typeof(ISpriteBatch))?.Registration;
            spriteBatchRegistration?.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "Disposing of objects to be disposed of manually by the library.");

            isInitialized = true;
        }
    }
}
