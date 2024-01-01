// <copyright file="MouseBenchmarks.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable
#pragma warning disable SA1129
#pragma warning disable SA1600
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace MousePerf;

using System.Drawing;
using BenchmarkDotNet.Attributes;
using Velaptor.Factories;
using Velaptor.Input;

[MemoryDiagnoser]
public class MouseBenchmarks
{
    private readonly Mouse mouse;
    private MouseState mouseState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseBenchmarks"/> class.
    /// </summary>
    public MouseBenchmarks()
    {
        IReactableFactory factoryFake = new ReactableFactoryFake();
        this.mouse = new Mouse(factoryFake);
    }

    [Params(MouseButton.LeftButton, MouseButton.RightButton, MouseButton.MiddleButton)]
    public MouseButton MouseButton { get; set; }

    [Params(true, false)]
    public bool IsDown { get; set; }

    [IterationSetup]
    public void IterationSetup()
    {
        this.mouseState = new MouseState(
            new Point(10, 20),
            MouseButton == MouseButton.LeftButton,
            MouseButton == MouseButton.RightButton,
            MouseButton == MouseButton.MiddleButton,
            MouseScrollDirection.ScrollDown,
            30);
    }

    [Benchmark(Description = $"{nameof(MouseState)}.{nameof(GetPosition)}")]
    public void GetPosition() => this.mouseState.GetPosition();

    [Benchmark(Description = $"{nameof(MouseState.GetX)}", OperationsPerInvoke = 1_000_000_000)]
    public void GetX() => this.mouseState.GetX();

    [Benchmark(Description = $"{nameof(MouseState.GetY)}", OperationsPerInvoke = 1_000_000_000)]
    public void GetY() => this.mouseState.GetY();

    [Benchmark(Description = $"{nameof(MouseState.IsButtonDown)}", OperationsPerInvoke = 1_000_000_000)]
    public void IsButtonDown() => this.mouseState.IsButtonDown(MouseButton);

    [Benchmark(Description = $"{nameof(MouseState.IsButtonUp)}", OperationsPerInvoke = 1_000_000_000)]
    public void IsButtonUp() => this.mouseState.IsButtonUp(MouseButton);

    [Benchmark(Description = $"{nameof(MouseState.IsLeftButtonDown)}", OperationsPerInvoke = 1_000_000_000)]
    public void IsLeftButtonDown() => this.mouseState.IsLeftButtonDown();

    [Benchmark(Description = $"{nameof(MouseState.IsLeftButtonUp)}", OperationsPerInvoke = 1_000_000_000)]
    public void IsLeftButtonUp() => this.mouseState.IsLeftButtonUp();

    [Benchmark(Description = $"{nameof(MouseState.IsMiddleButtonDown)}", OperationsPerInvoke = 1_000_000_000)]
    public void IsMiddleButtonDown() => this.mouseState.IsMiddleButtonDown();

    [Benchmark(Description = $"{nameof(MouseState.IsMiddleButtonUp)}", OperationsPerInvoke = 1_000_000_000)]
    public void IsMiddleButtonUp() => this.mouseState.IsMiddleButtonUp();

    [Benchmark(Description = $"{nameof(MouseState.IsRightButtonDown)}", OperationsPerInvoke = 1_000_000_000)]
    public void IsRightButtonDown() => this.mouseState.IsRightButtonDown();

    [Benchmark(Description = $"{nameof(MouseState.IsRightButtonUp)}", OperationsPerInvoke = 1_000_000_000)]
    public void IsRightButtonUp() => this.mouseState.IsRightButtonUp();

    [Benchmark(Description = $"{nameof(MouseState.GetButtonState)}", OperationsPerInvoke = 1_000_000_000)]
    public void GetButtonState() => this.mouseState.GetButtonState(MouseButton);

    [Benchmark(Description = $"{nameof(MouseState.AnyButtonsDown)}", OperationsPerInvoke = 1_000_000_000)]
    public void AnyButtonsDown() => this.mouseState.AnyButtonsDown();

    [Benchmark(Description = $"{nameof(MouseState.GetScrollWheelValue)}", OperationsPerInvoke = 1_000_000_000)]
    public void GetScrollWheelValue() => this.mouseState.GetScrollWheelValue();

    [Benchmark(Description = $"{nameof(MouseState.GetScrollDirection)}", OperationsPerInvoke = 1_000_000_000)]
    public void GetScrollDirection() => this.mouseState.GetScrollDirection();

    [Benchmark(Description = $"{nameof(Mouse.GetState)}", OperationsPerInvoke = 1_000_000_000)]
    public void GetState() => this.mouse.GetState();
}
