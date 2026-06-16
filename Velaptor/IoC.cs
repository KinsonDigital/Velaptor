// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;
using Batching;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Content;
using Content.Factories;
using Content.Fonts.Services;
using Factories;
using Graphics;
using Graphics.Renderers;
using Hardware.Services;
using Input;
using NativeInterop.FreeType;
using NativeInterop.GLFW;
using NativeInterop.ImGui;
using NativeInterop.Services;
using NativeInterop.WebGPU;
using OpenGL.Batching;
using ReactableData;
using Scene;
using Services;
using Silk.NET.OpenGL;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Telemetry;
using UI;
using Velaptor.WebGPU;
using WgpuGraphicsDevice = Velaptor.WebGPU.GraphicsDevice;
using WgpuGraphicsSurface = Velaptor.WebGPU.GraphicsSurface;
using WgpuFrame = Velaptor.WebGPU.Frame;
using WgpuTextureBindGroupRegistry = Velaptor.WebGPU.TextureBindGroupRegistry;

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
    /// Gets the (inversion of control) container used to get instances of objects.
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
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }

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

        SetupWebGPU();

        SetupBuffers();

        SetupRendering();

        SetupFactories();

        SetupServices();

        SetupContent();

        SetupReactables();

        IoCContainer.Register<ITelemetryClient, TelemetryClient>(Lifestyle.Singleton);
        IoCContainer.Register<ISceneManager, SceneManager>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<TextureBatchItem>>, RenderItemComparer<TextureBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<FontGlyphBatchItem>>, RenderItemComparer<FontGlyphBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<ShapeBatchItem>>, RenderItemComparer<ShapeBatchItem>>(Lifestyle.Singleton);
        IoCContainer.Register<IComparer<RenderItem<LineBatchItem>>, RenderItemComparer<LineBatchItem>>(Lifestyle.Singleton);

        IoCContainer.Register<IBatcher, WgpuBatcher>(Lifestyle.Singleton);
        IoCContainer.Register<IBatchingManager, BatchingManager>(Lifestyle.Singleton);
        IoCContainer.Register<IAppInput<KeyboardState>, Keyboard>(Lifestyle.Singleton);
        IoCContainer.Register<IAppInput<MouseState>, Mouse>(Lifestyle.Singleton);
        IoCContainer.Register<IKeyboardDataService, KeyboardDataService>(Lifestyle.Singleton);
        IoCContainer.RegisterSingleton(() => new HttpClient { Timeout = TimeSpan.FromSeconds(5) });

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
            var wgpu = IoCContainer.GetInstance<IWGPUInvoker>();
            var reactableFactory = IoCContainer.GetInstance<IReactableFactory>();
            var pipeline = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsTexturePipeline>();
            var buffer = IoCContainer.GetInstance<Velaptor.WebGPU.Buffers.FontGpuBuffer>();
            var frame = IoCContainer.GetInstance<WgpuFrame>();
            var bindGroupRegistry = IoCContainer.GetInstance<WgpuTextureBindGroupRegistry>();
            var batchManager = IoCContainer.GetInstance<IBatchingManager>();

            return new Velaptor.WebGPU.Renderers.FontRenderer(
                wgpu,
                reactableFactory,
                pipeline,
                buffer,
                frame,
                bindGroupRegistry,
                batchManager);
        }, Lifestyle.Singleton);

        IoCContainer.Register<ITextureRenderer>(
            () =>
        {
            var wgpu = IoCContainer.GetInstance<IWGPUInvoker>();
            var reactableFactory = IoCContainer.GetInstance<IReactableFactory>();
            var pipeline = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsTexturePipeline>();
            var buffer = IoCContainer.GetInstance<Velaptor.WebGPU.Buffers.TextureGpuBuffer>();
            var frame = IoCContainer.GetInstance<WgpuFrame>();
            var bindGroupRegistry = IoCContainer.GetInstance<WgpuTextureBindGroupRegistry>();
            var batchManager = IoCContainer.GetInstance<IBatchingManager>();

            return new Velaptor.WebGPU.Renderers.TextureRenderer(
                wgpu,
                reactableFactory,
                pipeline,
                buffer,
                frame,
                bindGroupRegistry,
                batchManager);
        }, Lifestyle.Singleton);

        IoCContainer.Register<ILineRenderer>(
            () =>
        {
            var wgpu = IoCContainer.GetInstance<IWGPUInvoker>();
            var reactableFactory = IoCContainer.GetInstance<IReactableFactory>();
            var pipeline = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsLinePipeline>();
            var buffer = IoCContainer.GetInstance<Velaptor.WebGPU.Buffers.LineGpuBuffer>();
            var frame = IoCContainer.GetInstance<WgpuFrame>();
            var batchManager = IoCContainer.GetInstance<IBatchingManager>();

            return new Velaptor.WebGPU.Renderers.LineRenderer(
                wgpu,
                reactableFactory,
                pipeline,
                buffer,
                frame,
                batchManager);
        }, Lifestyle.Singleton);

        IoCContainer.Register<IShapeRenderer>(
            () =>
        {
            var wgpu = IoCContainer.GetInstance<IWGPUInvoker>();
            var reactableFactory = IoCContainer.GetInstance<IReactableFactory>();
            var pipeline = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsShapePipeline>();
            var buffer = IoCContainer.GetInstance<Velaptor.WebGPU.Buffers.ShapeGpuBuffer>();
            var frame = IoCContainer.GetInstance<WgpuFrame>();
            var batchManager = IoCContainer.GetInstance<IBatchingManager>();

            return new Velaptor.WebGPU.Renderers.ShapeRenderer(
                wgpu,
                reactableFactory,
                pipeline,
                buffer,
                frame,
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

        IoCContainer.Register<IWGPUInvoker, WGPUInvoker>(Lifestyle.Singleton);
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
    /// Sets up the container registration related to the WebGPU backend.
    /// </summary>
    private static void SetupWebGPU()
    {
        IoCContainer.Register<Velaptor.WebGPU.GraphicsDevice>(Lifestyle.Singleton);
        IoCContainer.Register<IGraphicsDevice>(() => IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>(), Lifestyle.Singleton);

        IoCContainer.Register<Velaptor.WebGPU.GraphicsSurface>(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();
            var window = IoCContainer.GetInstance<IWindowFactory>().CreateSilkWindow();

            return new Velaptor.WebGPU.GraphicsSurface(gd, window);
        }, Lifestyle.Singleton);

        IoCContainer.Register<WgpuFrame>(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();
            var surface = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsSurface>();

            return new WgpuFrame(gd, surface);
        }, Lifestyle.Singleton);

        IoCContainer.Register<WgpuTextureBindGroupRegistry>(Lifestyle.Singleton);

        // Texture pipeline
        IoCContainer.Register<Velaptor.WebGPU.GraphicsTexturePipeline>(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();
            var surface = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsSurface>();
            var shader = new Velaptor.WebGPU.GraphicsShader(
                IoCContainer.GetInstance<IEmbeddedResourceLoaderService<string>>(),
                IoCContainer.GetInstance<IPath>(),
                "texture");

            return new Velaptor.WebGPU.GraphicsTexturePipeline(gd, surface, shader);
        }, Lifestyle.Singleton);

        // Shape pipeline
        IoCContainer.Register<Velaptor.WebGPU.GraphicsShapePipeline>(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();
            var surface = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsSurface>();
            var shader = new Velaptor.WebGPU.GraphicsShader(
                IoCContainer.GetInstance<IEmbeddedResourceLoaderService<string>>(),
                IoCContainer.GetInstance<IPath>(),
                "shape");

            return new Velaptor.WebGPU.GraphicsShapePipeline(gd, surface, shader);
        }, Lifestyle.Singleton);

        // Line pipeline
        IoCContainer.Register<Velaptor.WebGPU.GraphicsLinePipeline>(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();
            var surface = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsSurface>();
            var shader = new Velaptor.WebGPU.GraphicsShader(
                IoCContainer.GetInstance<IEmbeddedResourceLoaderService<string>>(),
                IoCContainer.GetInstance<IPath>(),
                "line");

            return new Velaptor.WebGPU.GraphicsLinePipeline(gd, surface, shader);
        }, Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to the GPU buffers.
    /// </summary>
    private static void SetupBuffers()
    {
        // GPU buffers must be pre-sized to match the batching manager's initial capacity.
        // WebGPU records draw commands (SetVertexBuffer, SetIndexBuffer, DrawIndexed) into
        // the command encoder; QueueWriteBuffer executes immediately.  If the buffer resizes
        // mid-render-pass (Allocate disposes old handles and creates new ones), previously
        // recorded commands reference disposed handles, causing a native crash on submit.
        const uint gpuBufferInitialCapacity = 1000;

        IoCContainer.Register(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();

            return new Velaptor.WebGPU.Buffers.TextureGpuBuffer(gd, gpuBufferInitialCapacity);
        }, Lifestyle.Singleton);

        IoCContainer.Register(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();

            return new Velaptor.WebGPU.Buffers.FontGpuBuffer(gd, gpuBufferInitialCapacity);
        }, Lifestyle.Singleton);

        IoCContainer.Register(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();

            return new Velaptor.WebGPU.Buffers.ShapeGpuBuffer(gd, gpuBufferInitialCapacity);
        }, Lifestyle.Singleton);

        IoCContainer.Register(
            () =>
        {
            var gd = IoCContainer.GetInstance<Velaptor.WebGPU.GraphicsDevice>();

            return new Velaptor.WebGPU.Buffers.LineGpuBuffer(gd, gpuBufferInitialCapacity);
        }, Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to factories.
    /// </summary>
    private static void SetupFactories()
    {
        IoCContainer.Register<IWindowFactory, SilkWindowFactory>(Lifestyle.Singleton);
        IoCContainer.Register<INativeInputFactory, NativeInputFactory>(Lifestyle.Singleton);
        IoCContainer.Register<ITextureFactory, TextureFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IAudioFactory, AudioFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IFontFactory, FontFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IAtlasDataFactory, AtlasDataFactory>(Lifestyle.Singleton);
        IoCContainer.Register<IRenderMediator, RenderMediator>(Lifestyle.Singleton);
        IoCContainer.Register<IPathResolverFactory, PathResolverFactory>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to services.
    /// </summary>
    private static void SetupServices()
    {
        IoCContainer.Register<IAppService, AppService>(Lifestyle.Singleton);
        IoCContainer.Register<IGpuService, GpuService>(Lifestyle.Singleton);
        IoCContainer.Register<ICpuService, CpuService>(Lifestyle.Singleton);
        IoCContainer.Register<ITelemetryService, TelemetryService>(Lifestyle.Singleton);
        IoCContainer.Register<IConsoleService, ConsoleService>(Lifestyle.Singleton);
        IoCContainer.Register<IDateTimeService, DateTimeService>(Lifestyle.Singleton);
        IoCContainer.Register<IConsoleLoggerService, ConsoleLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<IFileLoggerService, FileLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<IEventLoggerService, EventLoggerService>(Lifestyle.Singleton);
        IoCContainer.Register<ILoggingService, LoggingService>(Lifestyle.Singleton);
        IoCContainer.Register<IAppSettingsService, AppSettingsService>(Lifestyle.Singleton);
        IoCContainer.Register<IImageService, ImageService>(Lifestyle.Singleton);
        IoCContainer.Register<IEmbeddedResourceLoaderService<string>, TextResourceLoaderService>(Lifestyle.Singleton);
        IoCContainer.Register<ISystemDisplayService, SystemDisplayService>(Lifestyle.Singleton);
        IoCContainer.Register<IFontAtlasService, FontAtlasService>(Lifestyle.Singleton);
        IoCContainer.Register<IJsonService, JSONService>(Lifestyle.Singleton);
        IoCContainer.Register<IEmbeddedResourceLoaderService<Stream?>, EmbeddedFontResourceService>(Lifestyle.Singleton);
        IoCContainer.Register<IFreeTypeService, FreeTypeService>(Lifestyle.Singleton);
        IoCContainer.Register<IStopWatchWrapper, StopWatchWrapper>(Lifestyle.Singleton);
        IoCContainer.Register<ITimerService, TimerService>(Lifestyle.Singleton);
        IoCContainer.Register<IStatsWindowService, StatsWindowService>(Lifestyle.Singleton);
        IoCContainer.Register<IDotnetService, DotnetService>(Lifestyle.Singleton);

        IoCContainer.Register<IFontStatsService>(
            () => new FontStatsService(
                IoCContainer.GetInstance<IFreeTypeService>(),
                IoCContainer.GetInstance<IPathResolverFactory>().CreateFontPathResolver(),
                IoCContainer.GetInstance<IDirectory>(),
                IoCContainer.GetInstance<IPath>()), Lifestyle.Singleton);

        IoCContainer.Register<ITaskService, TaskService>(Lifestyle.Singleton);
    }

    /// <summary>
    /// Sets up the container registration related to content.
    /// </summary>
    private static void SetupContent()
    {
        IoCContainer.Register<IImageLoader, ImageLoader>(Lifestyle.Singleton);
        IoCContainer.Register<AtlasTexturePathResolver>(Lifestyle.Singleton);
        IoCContainer.Register<IContentLoaderFactory, ContentLoaderFactory>(Lifestyle.Singleton);
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


