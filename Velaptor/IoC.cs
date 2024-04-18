// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Batching;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Content;
using Content.Caching;
using Content.Factories;
using Content.Fonts.Services;
using Factories;
using Graphics;
using Graphics.Renderers;
using Input;
using NativeInterop.FreeType;
using NativeInterop.GLFW;
using NativeInterop.ImGui;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using OpenGL.Batching;
using OpenGL.Buffers;
using OpenGL.Services;
using ReactableData;
using Scene;
using Services;
using Silk.NET.OpenGL;
using SimpleInjector;
using SimpleInjector.Lifestyles;

/// <summary>
/// Provides dependency injection for the application.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with the '{nameof(SimpleInjector)}' library.")]
internal static class IoC
{
    private static readonly FileSystem FileSystem = new ();
    private static readonly IFileStreamFactory FileStream = FileSystem.FileStream;
    private static readonly Container IoCContainer = new ();
    private static bool isInitialized;

    /// <summary>
    /// Gets the inversion of control container used to get instances of objects.
    /// </summary>
    public static Container Container
    {
        get
        {
            if (UnitTestDetector.IsRunningFromUnitTest)
            {
                throw new InvalidOperationException("The unit test is invoking the IoC container.  This is not allowed.");
            }

            if (!isInitialized)
            {
                SetupContainer();
            }

            return IoCContainer;
        }
    }

    /// <summary>
    /// Disposes of all registered types that are capable of being disposed.
    /// </summary>
    /// <remarks>
    ///     All transient and singleton types are not disposed of by SimpleInjector.
    ///     This is common practice for DI containers.  Because of this, it is left to
    ///     the developer to dispose of these types.  This method will dispose of all
    ///     registered types that are capable of being disposed.
    /// </remarks>
    public static void DisposeOfRegisteredTypes()
    {
        // Get all the registered types that are capable of being disposed
        var disposableRegistrations = IoCContainer.GetDisposableRegistrations();

        foreach (var regType in disposableRegistrations)
        {
            IoCContainer.DisposeOfType(regType);
        }
    }

    /// <summary>
    /// Sets up the IoC container.
    /// </summary>
    private static void SetupContainer()
    {
        IoCContainer.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

        SetupNativeInterop();

        SetupBuffers();

        SetupRendering();

        SetupCaching();

        SetupFactories();

        SetupServices();

        SetupContent();

        SetupReactables();

        IoCContainer.Register<ISceneManager, SceneManager>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<TextureBatchItem>>, RenderItemComparer<TextureBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<FontGlyphBatchItem>>, RenderItemComparer<FontGlyphBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<ShapeBatchItem>>, RenderItemComparer<ShapeBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<LineBatchItem>>, RenderItemComparer<LineBatchItem>>(Lifestyle.Singleton);

        IoCContainer.Register<IBatcher, Batcher>(Lifestyle.Singleton);
        IoCContainer.Register<IBatchingManager, BatchingManager>(Lifestyle.Singleton);
        IoCContainer.Register<IAppInput<KeyboardState>, Keyboard>(Lifestyle.Singleton);
        IoCContainer.Register<IAppInput<MouseState>, Mouse>(Lifestyle.Singleton);
        IoCContainer.Register<IKeyboardDataService, KeyboardDataService>(Lifestyle.Singleton);

        isInitialized = true;
    }

    /// <summary>
    /// Sets up the various renderers.
    /// </summary>
    private static void SetupRendering()
    {
        IoCContainer.Register<IRenderContext, AvaloniaRenderContext>();
        IoCContainer.Register<IFontRenderer>(
            () =>
        {
            var glInvoker = IoCContainer.GetInstance<IGLInvoker>();
            var reactableFactory = IoCContainer.GetInstance<IReactableFactory>();
            var openGLService = IoCContainer.GetInstance<IOpenGLService>();
            var buffer = IoCContainer.GetInstance<IGpuBuffer<FontGlyphBatchItem>>();
            var shader = IoCContainer.GetInstance<IShaderFactory>().CreateFontShader();
            var batchManager = IoCContainer.GetInstance<IBatchingManager>();

            return new FontRenderer(
                glInvoker,
                reactableFactory,
                openGLService,
                buffer,
                shader,
                batchManager);
        }, Lifestyle.Singleton);

        IoCContainer.Register<ITextureRenderer>(
            () =>
        {
            var glInvoker = IoCContainer.GetInstance<IGLInvoker>();
            var reactableFactory = IoCContainer.GetInstance<IReactableFactory>();
            var openGLService = IoCContainer.GetInstance<IOpenGLService>();
            var buffer = IoCContainer.GetInstance<IGpuBuffer<TextureBatchItem>>();
            var shader = IoCContainer.GetInstance<IShaderFactory>().CreateTextureShader();
            var batchManager = IoCContainer.GetInstance<IBatchingManager>();

            return new TextureRenderer(
                glInvoker,
                reactableFactory,
                openGLService,
                buffer,
                shader,
                batchManager);
        }, Lifestyle.Singleton);

        IoCContainer.Register<ILineRenderer>(
            () =>
        {
            var glInvoker = IoCContainer.GetInstance<IGLInvoker>();
            var reactableFactory = IoCContainer.GetInstance<IReactableFactory>();
            var openGLService = IoCContainer.GetInstance<IOpenGLService>();
            var buffer = IoCContainer.GetInstance<IGpuBuffer<LineBatchItem>>();
            var shader = IoCContainer.GetInstance<IShaderFactory>().CreateLineShader();
            var batchManager = IoCContainer.GetInstance<IBatchingManager>();

            return new LineRenderer(
                glInvoker,
                reactableFactory,
                openGLService,
                buffer,
                shader,
                batchManager);
        }, Lifestyle.Singleton);

        IoCContainer.Register<IShapeRenderer>(
            () =>
        {
            var glInvoker = IoCContainer.GetInstance<IGLInvoker>();
            var reactableFactory = IoCContainer.GetInstance<IReactableFactory>();
            var openGLService = IoCContainer.GetInstance<IOpenGLService>();
            var buffer = IoCContainer.GetInstance<IGpuBuffer<ShapeBatchItem>>();
            var shader = IoCContainer.GetInstance<IShaderFactory>().CreateShapeShader();
            var batchManager = IoCContainer.GetInstance<IBatchingManager>();

            return new ShapeRenderer(
                glInvoker,
                reactableFactory,
                openGLService,
                buffer,
                shader,
                batchManager);
        }, Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to OpenGL.
    /// </summary>
    private static void SetupNativeInterop()
    {
        IoCContainer.Register(() => FileSystem.File, Lifestyle.Singleton);
        IoCContainer.Register(() => FileSystem.Directory, Lifestyle.Singleton);
        IoCContainer.Register(() => FileSystem.Path, Lifestyle.Singleton);
        IoCContainer.Register(() => FileStream, Lifestyle.Singleton);
        IoCContainer.Register<IPlatform, Platform>(Lifestyle.Singleton);

        IoCContainer.Register<IGLInvoker, GLInvoker>(Lifestyle.Singleton);
        IoCContainer.Register<IOpenGLService, OpenGLService>(Lifestyle.Singleton);
        IoCContainer.Register<IGlfwInvoker, GlfwInvoker>(Lifestyle.Singleton);
        IoCContainer.Register<IFreeTypeInvoker, FreeTypeInvoker>(Lifestyle.Singleton);
        IoCContainer.Register<IImGuiInvoker, ImGuiInvoker>(Lifestyle.Singleton);
        IoCContainer.Register<IImGuiManager, ImGuiManager>(Lifestyle.Singleton);
        IoCContainer.Register<IImGuiService, ImGuiService>(Lifestyle.Singleton);
        IoCContainer.Register<IImGuiFacade, ImGuiFacade>(Lifestyle.Singleton);

        IoCContainer.Register<GlfwDisplays>(Lifestyle.Singleton);
        IoCContainer.Register<IDisplays, GlfwDisplays>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to the GPU buffers.
    /// </summary>
    private static void SetupBuffers()
    {
        IoCContainer.Register<IGpuBuffer<TextureBatchItem>, TextureGpuBuffer>(Lifestyle.Singleton);
        IoCContainer.Register<IGpuBuffer<FontGlyphBatchItem>, FontGpuBuffer>(Lifestyle.Singleton);
        IoCContainer.Register<IGpuBuffer<ShapeBatchItem>, ShapeGpuBuffer>(Lifestyle.Singleton);
        IoCContainer.Register<IGpuBuffer<LineBatchItem>, LineGpuBuffer>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to caching.
    /// </summary>
    private static void SetupCaching()
    {
        IoCContainer.Register<IItemCache<string, ITexture>, TextureCache>(Lifestyle.Singleton);
        IoCContainer.Register<IItemCache<string, IAudio>, AudioCache>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to factories.
    /// </summary>
    private static void SetupFactories()
    {
        IoCContainer.Register<IWindowFactory, SilkWindowFactory>(Lifestyle.Singleton);
        IoCContainer.Register<INativeInputFactory, NativeInputFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IAudioFactory, AudioFactory>(Lifestyle.Singleton);
        IoCContainer.Register<ITextureFactory, TextureFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IAtlasDataFactory, AtlasDataFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IShaderFactory, ShaderFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IFontFactory, FontFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderMediator, RenderMediator>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to services.
    /// </summary>
    private static void SetupServices()
    {
        IoCContainer.Register<IAppService, AppService>(Lifestyle.Singleton);
        IoCContainer.Register<IConsoleService, ConsoleService>(Lifestyle.Singleton);
        IoCContainer.Register<IDateTimeService, DateTimeService>(Lifestyle.Singleton);
        IoCContainer.Register<IConsoleLoggerService, ConsoleLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<IFileLoggerService, FileLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<IEventLoggerService, EventLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<ILoggingService, LoggingService>(Lifestyle.Singleton);
        IoCContainer.Register<IAppSettingsService, AppSettingsService>(Lifestyle.Singleton);
        IoCContainer.Register<IImageService, ImageService>(Lifestyle.Singleton);
        IoCContainer.Register<IEmbeddedResourceLoaderService<string>, TextResourceLoaderService>(Lifestyle.Singleton);
        IoCContainer.Register<IShaderLoaderService, TextureShaderResourceLoaderService>(Lifestyle.Singleton);
        IoCContainer.Register<ISystemDisplayService, SystemDisplayService>(Lifestyle.Singleton);
        IoCContainer.Register<IFontAtlasService, FontAtlasService>(Lifestyle.Singleton);
        IoCContainer.Register<IJSONService, JSONService>(Lifestyle.Singleton);
        IoCContainer.Register<IEmbeddedResourceLoaderService<Stream?>, EmbeddedFontResourceService>(Lifestyle.Singleton);
        IoCContainer.Register<IFontService, FontService>(Lifestyle.Singleton);
        IoCContainer.Register<IStopWatchWrapper, StopWatchWrapper>(Lifestyle.Singleton);
        IoCContainer.Register<ITimerService, TimerService>(Lifestyle.Singleton);
        IoCContainer.Register<IStatsWindowService, StatsWindowService>(Lifestyle.Singleton);

        IoCContainer.Register<IFontStatsService>(
            () => new FontStatsService(
                IoCContainer.GetInstance<IFontService>(),
                PathResolverFactory.CreateFontPathResolver(),
                PathResolverFactory.CreateSystemFontPathResolver(),
                IoCContainer.GetInstance<IDirectory>(),
                IoCContainer.GetInstance<IPath>()), Lifestyle.Singleton);

        IoCContainer.Register<ITaskService, TaskService>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to content.
    /// </summary>
    private static void SetupContent()
    {
        IoCContainer.Register<IFontMetaDataParser, FontMetaDataParser>(Lifestyle.Singleton);
        IoCContainer.Register<IImageLoader, ImageLoader>(Lifestyle.Singleton);
        IoCContainer.Register<AtlasTexturePathResolver>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to reactables.
    /// </summary>
    private static void SetupReactables()
    {
        IoCContainer.Register<IReactableFactory, ReactableFactory>(Lifestyle.Singleton);

        // This is used for pushing notifications of events that do not require any data
        IoCContainer.Register<IPushReactable, PushReactable>(Lifestyle.Singleton);

        IoCContainer.Register<IPushReactable<GL>, PushReactable<GL>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<BatchSizeData>, PushReactable<BatchSizeData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<ViewPortSizeData>, PushReactable<ViewPortSizeData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<WindowSizeData>, PushReactable<WindowSizeData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPullReactable<WindowSizeData>, PullReactable<WindowSizeData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<MouseStateData>, PushReactable<MouseStateData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<KeyboardKeyStateData>, PushReactable<KeyboardKeyStateData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<DisposeTextureData>, PushReactable<DisposeTextureData>>(Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<DisposeAudioData>, PushReactable<DisposeAudioData>>(Lifestyle.Singleton);
        IoCContainer.Register(() => IoCContainer.GetInstance<IWindowFactory>().CreateSilkWindow(), Lifestyle.Singleton);
        IoCContainer.Register<IPushReactable<GLObjectsData>, PushReactable<GLObjectsData>>(Lifestyle.Singleton);

        IoCContainer.Register<IBatchPullReactable<TextureBatchItem>, BatchPullReactable<TextureBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IBatchPullReactable<FontGlyphBatchItem>, BatchPullReactable<FontGlyphBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IBatchPullReactable<ShapeBatchItem>, BatchPullReactable<ShapeBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IBatchPullReactable<LineBatchItem>, BatchPullReactable<LineBatchItem>>(Lifestyle.Singleton);

        IoCContainer.Register<IRenderBatchReactable<TextureBatchItem>, RenderBatchReactable<TextureBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderBatchReactable<FontGlyphBatchItem>, RenderBatchReactable<FontGlyphBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderBatchReactable<ShapeBatchItem>, RenderBatchReactable<ShapeBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderBatchReactable<LineBatchItem>, RenderBatchReactable<LineBatchItem>>(Lifestyle.Singleton);
    }
}
