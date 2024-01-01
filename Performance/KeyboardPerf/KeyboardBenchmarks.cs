// <copyright file="KeyboardBenchmarks.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1600

// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable
#pragma warning disable SA1129
#pragma warning disable SA1124
#pragma warning disable SA1514
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace KeyboardPerf;

using BenchmarkDotNet.Attributes;
using Velaptor.Input;
using Velaptor.Services;

/// <summary>
/// Measures performance of the <see cref="KeyboardState"/> class.
/// </summary>
[MemoryDiagnoser]
public class KeyboardBenchmarks
{
    private readonly KeyboardState keyboardState;
    private Keyboard keyboard;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardBenchmarks"/> class.
    /// </summary>
    public KeyboardBenchmarks()
    {
        this.keyboardState = new KeyboardState();
        SetupKeyboardState();
        SetupKeyboard();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.IsKeyDown)}")]
    public void IsKeyDown()
    {
        this.keyboardState.IsKeyDown(KeyCode.A);
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.IsKeyUp)}")]
    public void IsKeyUp()
    {
        this.keyboardState.IsKeyUp(KeyCode.A);
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.SetKeyState)}")]
    public void SetKeyState()
    {
        this.keyboardState.SetKeyState(KeyCode.B, true);
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.KeyToChar)}")]
    public void KeyToChar()
    {
        this.keyboardState.KeyToChar(KeyCode.A);
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.GetDownKeys)}")]
    public void GetDownKeys()
    {
        this.keyboardState.GetDownKeys();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.AnyAltKeysDown)}")]
    public void AnyAltKeysDown()
    {
        this.keyboardState.AnyAltKeysDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.AnyCtrlKeysDown)}")]
    public void AnyCtrlKeysDown()
    {
        this.keyboardState.AnyCtrlKeysDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.AnyShiftKeysDown)}")]
    public void AnyShiftKeysDown()
    {
        this.keyboardState.AnyShiftKeysDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.AnyNumpadNumberKeysDown)}")]
    public void AnyNumpadNumberKeysDown()
    {
        this.keyboardState.AnyNumpadNumberKeysDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.AnyStandardNumberKeysDown)}")]
    public void AnyStandardNumberKeysDown()
    {
        this.keyboardState.AnyStandardNumberKeysDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.IsLeftAltKeyDown)}")]
    public void IsLeftAltKeyDown()
    {
        this.keyboardState.IsLeftAltKeyDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.IsLeftCtrlKeyDown)}")]
    public void IsLeftCtrlKeyDown()
    {
        this.keyboardState.IsLeftCtrlKeyDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.IsLeftShiftKeyDown)}")]
    public void IsLeftShiftKeyDown()
    {
        this.keyboardState.IsLeftShiftKeyDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.IsRightAltKeyDown)}")]
    public void IsRightAltKeyDown()
    {
        this.keyboardState.IsRightAltKeyDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.IsRightCtrlKeyDown)}")]
    public void IsRightCtrlKeyDown()
    {
        this.keyboardState.IsRightCtrlKeyDown();
    }

    [Benchmark(Description = $"{nameof(KeyboardState)}.{nameof(KeyboardState.IsRightShiftKeyDown)}")]
    public void IsRightShiftKeyDown()
    {
        this.keyboardState.IsRightShiftKeyDown();
    }

    [Benchmark(Description = $"{nameof(Keyboard)}.{nameof(Keyboard.GetState)}")]
    public void GetState()
    {
        this.keyboard.GetState();
    }

    #region Setup
    /// <summary>
    /// Sets up the keyboard state object for analysis.
    /// </summary>
    private void SetupKeyboardState()
    {
        var keys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

        // NOTE: This is used for worse case scenario for checking if a single key is in the down position
        var midIndex = keys.Length / 2;

        for (var i = 0; i < keys.Length; i++)
        {
            KeyCode key = keys[i];
            this.keyboardState.SetKeyState(key, i == midIndex);
        }
    }

    /// <summary>
    /// Sets up the keyboard object for analysis.
    /// </summary>
    private void SetupKeyboard()
    {
        var reactable = new PushReactableFake();
        var keyboardDataStore = new KeyboardDataService(reactable);
        this.keyboard = new Keyboard(keyboardDataStore);
    }
    #endregion
}
