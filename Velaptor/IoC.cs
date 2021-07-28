// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VelaptorTests", AllInternalsVisible = true)]

#pragma warning disable SA1116 // Split parameters should start on line after declaration
namespace Velaptor
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Velaptor.Content;
    using Velaptor.Graphics;
    using Velaptor.Input;
    using Velaptor.NativeInterop;
    using Velaptor.NativeInterop.FreeType;
    using Velaptor.Observables;
    using Velaptor.OpenGL;
    using Velaptor.Services;
    using SimpleInjector;

    /// <summary>
    /// Provides dependency injection for the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class IoC
    {
        private static readonly FileSystem FileSystem = new ();
        private static readonly Container IoCContainer = new ();
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
            IoCContainer.Register<IFreeTypeExtensions, FreeTypeExtensions>(Lifestyle.Singleton);

            IoCContainer.Register<IKeyboardInput<KeyCode, KeyboardState>, Keyboard>(Lifestyle.Singleton);
            IoCContainer.Register<IMouseInput<MouseButton, MouseState>, Mouse>(Lifestyle.Singleton);

            IoCContainer.Register<OpenGLInitObservable>(Lifestyle.Singleton);
            IoCContainer.Register<OpenGLContextObservable>(Lifestyle.Singleton);

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

#if OPENTK
            SetupInteropWithOpenTK();
#elif SILK
            SetupInteropWithSILK();
#endif

            IoCContainer.Register<IGLInvokerExtensions, GLInvokerExtensions>(Lifestyle.Singleton);

            IoCContainer.Register<GLFWMonitors>(suppressDisposal: true);

            IoCContainer.Register<IGPUBuffer, GPUBuffer<VertexData>>(Lifestyle.Singleton);

            IoCContainer.Register<IShaderProgram, ShaderProgram>(Lifestyle.Singleton);

            IoCContainer.Register<ISpriteBatch, SpriteBatch>(Lifestyle.Singleton);
        }

#pragma warning disable IDE0051 // Remove unused private members
        /// <summary>
        /// Sets up all of the OpenGL interop with the SILK library.
        /// </summary>
        private static void SetupInteropWithSILK()
        {
            IoCContainer.Register<IGLInvoker, SilkGLInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGLFWInvoker, SilkGLFWInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGameWindowFacade, SilkWindowFacade>(Lifestyle.Singleton, suppressDisposal: true);
        }

        /// <summary>
        /// Sets up all of the OpenGL interop with the OpenTK library.
        /// </summary>
        private static void SetupInteropWithOpenTK()
        {
            IoCContainer.Register<IGLInvoker, OpenTKGLInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGLFWInvoker, OpenTKGLFWInvoker>(Lifestyle.Singleton);
            IoCContainer.Register<IGameWindowFacade, OpenTKWindowFacade>(Lifestyle.Singleton, suppressDisposal: true);
        }
#pragma warning restore IDE0051 // Remove unused private members

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

            IoCContainer.Register<IBatchManagerService, BatchManagerService>(Lifestyle.Singleton);
        }

        /// <summary>
        /// Setup container registration related to content.
        /// </summary>
        private static void SetupContent() => IoCContainer.Register<AtlasTexturePathResolver>();
    }
}
