// // <copyright file="WgpuWindowTests.cs" company="KinsonDigital">
// // Copyright (c) KinsonDigital. All rights reserved.
// // </copyright>
//
// namespace VelaptorTests.WebGpu;
//
// using System;
// using NSubstitute;
// using Shouldly;
// using Silk.NET.Windowing;
// using Velaptor;
// using Velaptor.Factories;
// using Velaptor.Graphics.Renderers;
// using Velaptor.NativeInterop.GLFW;
// using Velaptor.Scene;
// using Velaptor.Services;
// using Velaptor.Telemetry;
// using Velaptor.WebGpu;
// using Xunit;
//
// /// <summary>
// /// Tests the <see cref="WgpuWindow"/> class.
// /// </summary>
// public class WgpuWindowTests
// {
//     private readonly ITelemetryService mockTelemetryService;
//     private readonly IWindow mockSilkWindow;
//     private readonly INativeInputFactory mockNativeInputFactory;
//     private readonly IGlfwInvoker mockGlfwInvoker;
//     private readonly ISystemDisplayService mockSystemDisplayService;
//     private readonly IPlatform mockPlatform;
//     private readonly ITaskService mockTaskService;
//     private readonly IStatsWindowService mockStatsWindowServiceService;
//     private readonly ISceneManager mockSceneManager;
//     private readonly IFontRenderer mockFontRenderer;
//     private readonly IReactableFactory mockReactableFactory;
//     private readonly ITimerService mockTimerService;
//
//     /// <summary>
//     /// Initializes a new instance of the <see cref="WgpuWindowTests"/> class.
//     /// </summary>
//     public WgpuWindowTests()
//     {
//         this.mockTelemetryService = Substitute.For<ITelemetryService>();
//         this.mockSilkWindow = Substitute.For<IWindow>();
//         this.mockNativeInputFactory = Substitute.For<INativeInputFactory>();
//         this.mockGlfwInvoker = Substitute.For<IGlfwInvoker>();
//         this.mockSystemDisplayService = Substitute.For<ISystemDisplayService>();
//         this.mockPlatform = Substitute.For<IPlatform>();
//         this.mockTaskService = Substitute.For<ITaskService>();
//         this.mockStatsWindowServiceService = Substitute.For<IStatsWindowService>();
//         this.mockSceneManager = Substitute.For<ISceneManager>();
//         this.mockFontRenderer = Substitute.For<IFontRenderer>();
//         this.mockReactableFactory = Substitute.For<IReactableFactory>();
//         this.mockTimerService = Substitute.For<ITimerService>();
//     }
//
//     #region Ctor Tests
//     [Fact]
//     public void Ctor_WithNullTelemetryServiceParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 null,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'telemetryService')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullSilkWindowParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 null,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'silkWindow')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullNativeInputFactoryParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 null,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'nativeInputFactory')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullGlfwInvokerParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 null,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'glfwInvoker')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullSystemDisplayServiceParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 null,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'systemDisplayService')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullPlatformParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 null,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'platform')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullTaskServiceParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 null,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'taskService')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullStatsWindowServiceServiceParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 null,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'statsWindowServiceService')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullSceneManagerParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 null,
//                 null,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'sceneManager')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullFontRendererParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 null,
//                 this.mockReactableFactory,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'fontRenderer')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullReactableFactoryParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 null,
//                 null,
//                 this.mockTimerService);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
//     }
//
//     [Fact]
//     public void Ctor_WithNullTimerServiceParam_ThrowsException()
//     {
//         // Arrange & Act
//         var act = () =>
//         {
//             _ = new WgpuWindow(
//                 100,
//                 200,
//                 this.mockTelemetryService,
//                 this.mockSilkWindow,
//                 this.mockNativeInputFactory,
//                 this.mockGlfwInvoker,
//                 this.mockSystemDisplayService,
//                 this.mockPlatform,
//                 this.mockTaskService,
//                 this.mockStatsWindowServiceService,
//                 this.mockSceneManager,
//                 this.mockFontRenderer,
//                 this.mockReactableFactory,
//                 null);
//         };
//
//         // Assert
//         act.ShouldThrow<ArgumentNullException>()
//             .Message.ShouldBe("Value cannot be null. (Parameter 'timerService')");
//     }
//     #endregion
//
//     #region Prop Tests
//
//     #endregion
//
//     #region Method Tests
//
//     #endregion
// }
