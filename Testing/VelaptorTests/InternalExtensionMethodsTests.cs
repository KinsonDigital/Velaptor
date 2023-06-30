// <copyright file="InternalExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable CS8524

namespace VelaptorTests;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Helpers;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Velaptor;
using Velaptor.ExtensionMethods;
using Velaptor.Graphics;
using Velaptor.Input;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Batching;
using Velaptor.OpenGL.GPUData;
using Xunit;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;
using NETRectF = System.Drawing.RectangleF;
using NETSizeF = System.Drawing.SizeF;

/// <summary>
/// Tests the <see cref="Velaptor.InternalExtensionMethods"/> class.
/// </summary>
public class InternalExtensionMethodsTests
{
    #region Unit Test Data
    // ReSharper disable HeapView.BoxingAllocation

        /// <summary>
    /// Gets the data for testing the <see cref="InternalExtensionMethods.IsLetterKey(KeyCode)"/> method.
    /// </summary>
    public static IEnumerable<object[]> IsLetterKeyTestData =>
        new List<object[]>
        {
            new object[] { KeyCode.A, true },
            new object[] { KeyCode.B, true },
            new object[] { KeyCode.C, true },
            new object[] { KeyCode.D, true },
            new object[] { KeyCode.E, true },
            new object[] { KeyCode.F, true },
            new object[] { KeyCode.G, true },
            new object[] { KeyCode.H, true },
            new object[] { KeyCode.I, true },
            new object[] { KeyCode.J, true },
            new object[] { KeyCode.K, true },
            new object[] { KeyCode.L, true },
            new object[] { KeyCode.M, true },
            new object[] { KeyCode.N, true },
            new object[] { KeyCode.O, true },
            new object[] { KeyCode.P, true },
            new object[] { KeyCode.Q, true },
            new object[] { KeyCode.R, true },
            new object[] { KeyCode.S, true },
            new object[] { KeyCode.T, true },
            new object[] { KeyCode.U, true },
            new object[] { KeyCode.V, true },
            new object[] { KeyCode.W, true },
            new object[] { KeyCode.X, true },
            new object[] { KeyCode.Y, true },
            new object[] { KeyCode.Z, true },
            new object[] { KeyCode.Space, true },
            new object[] { KeyCode.Unknown, false },
            new object[] { KeyCode.Apostrophe, false },
            new object[] { KeyCode.Comma, false },
            new object[] { KeyCode.Minus, false },
            new object[] { KeyCode.Period, false },
            new object[] { KeyCode.Slash, false },
            new object[] { KeyCode.D0, false },
            new object[] { KeyCode.D1, false },
            new object[] { KeyCode.D2, false },
            new object[] { KeyCode.D3, false },
            new object[] { KeyCode.D4, false },
            new object[] { KeyCode.D5, false },
            new object[] { KeyCode.D6, false },
            new object[] { KeyCode.D7, false },
            new object[] { KeyCode.D8, false },
            new object[] { KeyCode.D9, false },
            new object[] { KeyCode.Semicolon, false },
            new object[] { KeyCode.Equal, false },
            new object[] { KeyCode.LeftBracket, false },
            new object[] { KeyCode.Backslash, false },
            new object[] { KeyCode.RightBracket, false },
            new object[] { KeyCode.GraveAccent, false },
            new object[] { KeyCode.Escape, false },
            new object[] { KeyCode.Enter, false },
            new object[] { KeyCode.Tab, false },
            new object[] { KeyCode.Backspace, false },
            new object[] { KeyCode.Insert, false },
            new object[] { KeyCode.Delete, false },
            new object[] { KeyCode.Right, false },
            new object[] { KeyCode.Left, false },
            new object[] { KeyCode.Down, false },
            new object[] { KeyCode.Up, false },
            new object[] { KeyCode.PageUp, false },
            new object[] { KeyCode.PageDown, false },
            new object[] { KeyCode.Home, false },
            new object[] { KeyCode.End, false },
            new object[] { KeyCode.CapsLock, false },
            new object[] { KeyCode.ScrollLock, false },
            new object[] { KeyCode.NumLock, false },
            new object[] { KeyCode.PrintScreen, false },
            new object[] { KeyCode.Pause, false },
            new object[] { KeyCode.F1, false },
            new object[] { KeyCode.F2, false },
            new object[] { KeyCode.F3, false },
            new object[] { KeyCode.F4, false },
            new object[] { KeyCode.F5, false },
            new object[] { KeyCode.F6, false },
            new object[] { KeyCode.F7, false },
            new object[] { KeyCode.F8, false },
            new object[] { KeyCode.F9, false },
            new object[] { KeyCode.F10, false },
            new object[] { KeyCode.F11, false },
            new object[] { KeyCode.F12, false },
            new object[] { KeyCode.F13, false },
            new object[] { KeyCode.F14, false },
            new object[] { KeyCode.F15, false },
            new object[] { KeyCode.F16, false },
            new object[] { KeyCode.F17, false },
            new object[] { KeyCode.F18, false },
            new object[] { KeyCode.F19, false },
            new object[] { KeyCode.F20, false },
            new object[] { KeyCode.F21, false },
            new object[] { KeyCode.F22, false },
            new object[] { KeyCode.F23, false },
            new object[] { KeyCode.F24, false },
            new object[] { KeyCode.F25, false },
            new object[] { KeyCode.KeyPad0, false },
            new object[] { KeyCode.KeyPad1, false },
            new object[] { KeyCode.KeyPad2, false },
            new object[] { KeyCode.KeyPad3, false },
            new object[] { KeyCode.KeyPad4, false },
            new object[] { KeyCode.KeyPad5, false },
            new object[] { KeyCode.KeyPad6, false },
            new object[] { KeyCode.KeyPad7, false },
            new object[] { KeyCode.KeyPad8, false },
            new object[] { KeyCode.KeyPad9, false },
            new object[] { KeyCode.KeyPadDecimal, false },
            new object[] { KeyCode.KeyPadDivide, false },
            new object[] { KeyCode.KeyPadMultiply, false },
            new object[] { KeyCode.KeyPadSubtract, false },
            new object[] { KeyCode.KeyPadAdd, false },
            new object[] { KeyCode.KeyPadEnter, false },
            new object[] { KeyCode.KeyPadEqual, false },
            new object[] { KeyCode.LeftShift, false },
            new object[] { KeyCode.LeftControl, false },
            new object[] { KeyCode.LeftAlt, false },
            new object[] { KeyCode.LeftSuper, false },
            new object[] { KeyCode.RightShift, false },
            new object[] { KeyCode.RightControl, false },
            new object[] { KeyCode.RightAlt, false },
            new object[] { KeyCode.RightSuper, false },
            new object[] { KeyCode.Menu, false },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="InternalExtensionMethods.IsNumberKey(KeyCode)"/> method.
    /// </summary>
    public static IEnumerable<object[]> IsNumberKeyTestData =>
        new List<object[]>
        {
            new object[] { KeyCode.D0, true },
            new object[] { KeyCode.D1, true },
            new object[] { KeyCode.D2, true },
            new object[] { KeyCode.D3, true },
            new object[] { KeyCode.D4, true },
            new object[] { KeyCode.D5, true },
            new object[] { KeyCode.D6, true },
            new object[] { KeyCode.D7, true },
            new object[] { KeyCode.D8, true },
            new object[] { KeyCode.D9, true },
            new object[] { KeyCode.KeyPad0, true },
            new object[] { KeyCode.KeyPad1, true },
            new object[] { KeyCode.KeyPad2, true },
            new object[] { KeyCode.KeyPad3, true },
            new object[] { KeyCode.KeyPad4, true },
            new object[] { KeyCode.KeyPad5, true },
            new object[] { KeyCode.KeyPad6, true },
            new object[] { KeyCode.KeyPad7, true },
            new object[] { KeyCode.KeyPad8, true },
            new object[] { KeyCode.KeyPad9, true },
            new object[] { KeyCode.A, false },
            new object[] { KeyCode.B, false },
            new object[] { KeyCode.C, false },
            new object[] { KeyCode.D, false },
            new object[] { KeyCode.E, false },
            new object[] { KeyCode.F, false },
            new object[] { KeyCode.G, false },
            new object[] { KeyCode.H, false },
            new object[] { KeyCode.I, false },
            new object[] { KeyCode.J, false },
            new object[] { KeyCode.K, false },
            new object[] { KeyCode.L, false },
            new object[] { KeyCode.M, false },
            new object[] { KeyCode.N, false },
            new object[] { KeyCode.O, false },
            new object[] { KeyCode.P, false },
            new object[] { KeyCode.Q, false },
            new object[] { KeyCode.R, false },
            new object[] { KeyCode.S, false },
            new object[] { KeyCode.T, false },
            new object[] { KeyCode.U, false },
            new object[] { KeyCode.V, false },
            new object[] { KeyCode.W, false },
            new object[] { KeyCode.X, false },
            new object[] { KeyCode.Y, false },
            new object[] { KeyCode.Z, false },
            new object[] { KeyCode.Space, false },
            new object[] { KeyCode.Unknown, false },
            new object[] { KeyCode.Apostrophe, false },
            new object[] { KeyCode.Comma, false },
            new object[] { KeyCode.Minus, false },
            new object[] { KeyCode.Period, false },
            new object[] { KeyCode.Slash, false },
            new object[] { KeyCode.Semicolon, false },
            new object[] { KeyCode.Equal, false },
            new object[] { KeyCode.LeftBracket, false },
            new object[] { KeyCode.Backslash, false },
            new object[] { KeyCode.RightBracket, false },
            new object[] { KeyCode.GraveAccent, false },
            new object[] { KeyCode.Escape, false },
            new object[] { KeyCode.Enter, false },
            new object[] { KeyCode.Tab, false },
            new object[] { KeyCode.Backspace, false },
            new object[] { KeyCode.Insert, false },
            new object[] { KeyCode.Delete, false },
            new object[] { KeyCode.Right, false },
            new object[] { KeyCode.Left, false },
            new object[] { KeyCode.Down, false },
            new object[] { KeyCode.Up, false },
            new object[] { KeyCode.PageUp, false },
            new object[] { KeyCode.PageDown, false },
            new object[] { KeyCode.Home, false },
            new object[] { KeyCode.End, false },
            new object[] { KeyCode.CapsLock, false },
            new object[] { KeyCode.ScrollLock, false },
            new object[] { KeyCode.NumLock, false },
            new object[] { KeyCode.PrintScreen, false },
            new object[] { KeyCode.Pause, false },
            new object[] { KeyCode.F1, false },
            new object[] { KeyCode.F2, false },
            new object[] { KeyCode.F3, false },
            new object[] { KeyCode.F4, false },
            new object[] { KeyCode.F5, false },
            new object[] { KeyCode.F6, false },
            new object[] { KeyCode.F7, false },
            new object[] { KeyCode.F8, false },
            new object[] { KeyCode.F9, false },
            new object[] { KeyCode.F10, false },
            new object[] { KeyCode.F11, false },
            new object[] { KeyCode.F12, false },
            new object[] { KeyCode.F13, false },
            new object[] { KeyCode.F14, false },
            new object[] { KeyCode.F15, false },
            new object[] { KeyCode.F16, false },
            new object[] { KeyCode.F17, false },
            new object[] { KeyCode.F18, false },
            new object[] { KeyCode.F19, false },
            new object[] { KeyCode.F20, false },
            new object[] { KeyCode.F21, false },
            new object[] { KeyCode.F22, false },
            new object[] { KeyCode.F23, false },
            new object[] { KeyCode.F24, false },
            new object[] { KeyCode.F25, false },
            new object[] { KeyCode.KeyPadDecimal, false },
            new object[] { KeyCode.KeyPadDivide, false },
            new object[] { KeyCode.KeyPadMultiply, false },
            new object[] { KeyCode.KeyPadSubtract, false },
            new object[] { KeyCode.KeyPadAdd, false },
            new object[] { KeyCode.KeyPadEnter, false },
            new object[] { KeyCode.KeyPadEqual, false },
            new object[] { KeyCode.LeftShift, false },
            new object[] { KeyCode.LeftControl, false },
            new object[] { KeyCode.LeftAlt, false },
            new object[] { KeyCode.LeftSuper, false },
            new object[] { KeyCode.RightShift, false },
            new object[] { KeyCode.RightControl, false },
            new object[] { KeyCode.RightAlt, false },
            new object[] { KeyCode.RightSuper, false },
            new object[] { KeyCode.Menu, false },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="InternalExtensionMethods.IsSymbolKey(KeyCode)"/> method.
    /// </summary>
    public static IEnumerable<object[]> IsSymbolKeyTestData =>
        new List<object[]>
        {
            new object[] { KeyCode.A, false },
            new object[] { KeyCode.B, false },
            new object[] { KeyCode.C, false },
            new object[] { KeyCode.D, false },
            new object[] { KeyCode.E, false },
            new object[] { KeyCode.F, false },
            new object[] { KeyCode.G, false },
            new object[] { KeyCode.H, false },
            new object[] { KeyCode.I, false },
            new object[] { KeyCode.J, false },
            new object[] { KeyCode.K, false },
            new object[] { KeyCode.L, false },
            new object[] { KeyCode.M, false },
            new object[] { KeyCode.N, false },
            new object[] { KeyCode.O, false },
            new object[] { KeyCode.P, false },
            new object[] { KeyCode.Q, false },
            new object[] { KeyCode.R, false },
            new object[] { KeyCode.S, false },
            new object[] { KeyCode.T, false },
            new object[] { KeyCode.U, false },
            new object[] { KeyCode.V, false },
            new object[] { KeyCode.W, false },
            new object[] { KeyCode.X, false },
            new object[] { KeyCode.Y, false },
            new object[] { KeyCode.Z, false },
            new object[] { KeyCode.Space, false },
            new object[] { KeyCode.Unknown, false },
            new object[] { KeyCode.D0, false },
            new object[] { KeyCode.D1, false },
            new object[] { KeyCode.D2, false },
            new object[] { KeyCode.D3, false },
            new object[] { KeyCode.D4, false },
            new object[] { KeyCode.D5, false },
            new object[] { KeyCode.D6, false },
            new object[] { KeyCode.D7, false },
            new object[] { KeyCode.D8, false },
            new object[] { KeyCode.D9, false },
            new object[] { KeyCode.Semicolon, true },
            new object[] { KeyCode.Equal, true },
            new object[] { KeyCode.Comma, true },
            new object[] { KeyCode.Minus, true },
            new object[] { KeyCode.Period, true },
            new object[] { KeyCode.Slash, true },
            new object[] { KeyCode.LeftBracket, true },
            new object[] { KeyCode.RightBracket, true },
            new object[] { KeyCode.Apostrophe, true },
            new object[] { KeyCode.KeyPadDivide, true },
            new object[] { KeyCode.KeyPadMultiply, true },
            new object[] { KeyCode.KeyPadSubtract, true },
            new object[] { KeyCode.KeyPadAdd, true },
            new object[] { KeyCode.KeyPadDecimal, true },
            new object[] { KeyCode.Backslash, true },
            new object[] { KeyCode.GraveAccent, true },
            new object[] { KeyCode.Escape, false },
            new object[] { KeyCode.Enter, false },
            new object[] { KeyCode.Tab, false },
            new object[] { KeyCode.Backspace, false },
            new object[] { KeyCode.Insert, false },
            new object[] { KeyCode.Delete, false },
            new object[] { KeyCode.Right, false },
            new object[] { KeyCode.Left, false },
            new object[] { KeyCode.Down, false },
            new object[] { KeyCode.Up, false },
            new object[] { KeyCode.PageUp, false },
            new object[] { KeyCode.PageDown, false },
            new object[] { KeyCode.Home, false },
            new object[] { KeyCode.End, false },
            new object[] { KeyCode.CapsLock, false },
            new object[] { KeyCode.ScrollLock, false },
            new object[] { KeyCode.NumLock, false },
            new object[] { KeyCode.PrintScreen, false },
            new object[] { KeyCode.Pause, false },
            new object[] { KeyCode.F1, false },
            new object[] { KeyCode.F2, false },
            new object[] { KeyCode.F3, false },
            new object[] { KeyCode.F4, false },
            new object[] { KeyCode.F5, false },
            new object[] { KeyCode.F6, false },
            new object[] { KeyCode.F7, false },
            new object[] { KeyCode.F8, false },
            new object[] { KeyCode.F9, false },
            new object[] { KeyCode.F10, false },
            new object[] { KeyCode.F11, false },
            new object[] { KeyCode.F12, false },
            new object[] { KeyCode.F13, false },
            new object[] { KeyCode.F14, false },
            new object[] { KeyCode.F15, false },
            new object[] { KeyCode.F16, false },
            new object[] { KeyCode.F17, false },
            new object[] { KeyCode.F18, false },
            new object[] { KeyCode.F19, false },
            new object[] { KeyCode.F20, false },
            new object[] { KeyCode.F21, false },
            new object[] { KeyCode.F22, false },
            new object[] { KeyCode.F23, false },
            new object[] { KeyCode.F24, false },
            new object[] { KeyCode.F25, false },
            new object[] { KeyCode.KeyPad0, false },
            new object[] { KeyCode.KeyPad1, false },
            new object[] { KeyCode.KeyPad2, false },
            new object[] { KeyCode.KeyPad3, false },
            new object[] { KeyCode.KeyPad4, false },
            new object[] { KeyCode.KeyPad5, false },
            new object[] { KeyCode.KeyPad6, false },
            new object[] { KeyCode.KeyPad7, false },
            new object[] { KeyCode.KeyPad8, false },
            new object[] { KeyCode.KeyPad9, false },
            new object[] { KeyCode.KeyPadEnter, false },
            new object[] { KeyCode.KeyPadEqual, false },
            new object[] { KeyCode.LeftShift, false },
            new object[] { KeyCode.LeftControl, false },
            new object[] { KeyCode.LeftAlt, false },
            new object[] { KeyCode.LeftSuper, false },
            new object[] { KeyCode.RightShift, false },
            new object[] { KeyCode.RightControl, false },
            new object[] { KeyCode.RightAlt, false },
            new object[] { KeyCode.RightSuper, false },
            new object[] { KeyCode.Menu, false },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="InternalExtensionMethods.IsVisibleKey(KeyCode)"/> method.
    /// </summary>
    public static IEnumerable<object[]> IsVisibleKeyTestData =>
        new List<object[]>
        {
            new object[] { KeyCode.A, true },
            new object[] { KeyCode.B, true },
            new object[] { KeyCode.C, true },
            new object[] { KeyCode.D, true },
            new object[] { KeyCode.E, true },
            new object[] { KeyCode.F, true },
            new object[] { KeyCode.G, true },
            new object[] { KeyCode.H, true },
            new object[] { KeyCode.I, true },
            new object[] { KeyCode.J, true },
            new object[] { KeyCode.K, true },
            new object[] { KeyCode.L, true },
            new object[] { KeyCode.M, true },
            new object[] { KeyCode.N, true },
            new object[] { KeyCode.O, true },
            new object[] { KeyCode.P, true },
            new object[] { KeyCode.Q, true },
            new object[] { KeyCode.R, true },
            new object[] { KeyCode.S, true },
            new object[] { KeyCode.T, true },
            new object[] { KeyCode.U, true },
            new object[] { KeyCode.V, true },
            new object[] { KeyCode.W, true },
            new object[] { KeyCode.X, true },
            new object[] { KeyCode.Y, true },
            new object[] { KeyCode.Z, true },
            new object[] { KeyCode.Space, true },
            new object[] { KeyCode.Unknown, false },
            new object[] { KeyCode.D0, true },
            new object[] { KeyCode.D1, true },
            new object[] { KeyCode.D2, true },
            new object[] { KeyCode.D3, true },
            new object[] { KeyCode.D4, true },
            new object[] { KeyCode.D5, true },
            new object[] { KeyCode.D6, true },
            new object[] { KeyCode.D7, true },
            new object[] { KeyCode.D8, true },
            new object[] { KeyCode.D9, true },
            new object[] { KeyCode.Semicolon, true },
            new object[] { KeyCode.Equal, true },
            new object[] { KeyCode.Comma, true },
            new object[] { KeyCode.Minus, true },
            new object[] { KeyCode.Period, true },
            new object[] { KeyCode.Slash, true },
            new object[] { KeyCode.LeftBracket, true },
            new object[] { KeyCode.RightBracket, true },
            new object[] { KeyCode.Apostrophe, true },
            new object[] { KeyCode.KeyPadDivide, true },
            new object[] { KeyCode.KeyPadMultiply, true },
            new object[] { KeyCode.KeyPadSubtract, true },
            new object[] { KeyCode.KeyPadAdd, true },
            new object[] { KeyCode.KeyPadDecimal, true },
            new object[] { KeyCode.Backslash, true },
            new object[] { KeyCode.GraveAccent, true },
            new object[] { KeyCode.Escape, false },
            new object[] { KeyCode.Enter, false },
            new object[] { KeyCode.Tab, false },
            new object[] { KeyCode.Backspace, false },
            new object[] { KeyCode.Insert, false },
            new object[] { KeyCode.Delete, false },
            new object[] { KeyCode.Right, false },
            new object[] { KeyCode.Left, false },
            new object[] { KeyCode.Down, false },
            new object[] { KeyCode.Up, false },
            new object[] { KeyCode.PageUp, false },
            new object[] { KeyCode.PageDown, false },
            new object[] { KeyCode.Home, false },
            new object[] { KeyCode.End, false },
            new object[] { KeyCode.CapsLock, false },
            new object[] { KeyCode.ScrollLock, false },
            new object[] { KeyCode.NumLock, false },
            new object[] { KeyCode.PrintScreen, false },
            new object[] { KeyCode.Pause, false },
            new object[] { KeyCode.F1, false },
            new object[] { KeyCode.F2, false },
            new object[] { KeyCode.F3, false },
            new object[] { KeyCode.F4, false },
            new object[] { KeyCode.F5, false },
            new object[] { KeyCode.F6, false },
            new object[] { KeyCode.F7, false },
            new object[] { KeyCode.F8, false },
            new object[] { KeyCode.F9, false },
            new object[] { KeyCode.F10, false },
            new object[] { KeyCode.F11, false },
            new object[] { KeyCode.F12, false },
            new object[] { KeyCode.F13, false },
            new object[] { KeyCode.F14, false },
            new object[] { KeyCode.F15, false },
            new object[] { KeyCode.F16, false },
            new object[] { KeyCode.F17, false },
            new object[] { KeyCode.F18, false },
            new object[] { KeyCode.F19, false },
            new object[] { KeyCode.F20, false },
            new object[] { KeyCode.F21, false },
            new object[] { KeyCode.F22, false },
            new object[] { KeyCode.F23, false },
            new object[] { KeyCode.F24, false },
            new object[] { KeyCode.F25, false },
            new object[] { KeyCode.KeyPad0, true },
            new object[] { KeyCode.KeyPad1, true },
            new object[] { KeyCode.KeyPad2, true },
            new object[] { KeyCode.KeyPad3, true },
            new object[] { KeyCode.KeyPad4, true },
            new object[] { KeyCode.KeyPad5, true },
            new object[] { KeyCode.KeyPad6, true },
            new object[] { KeyCode.KeyPad7, true },
            new object[] { KeyCode.KeyPad8, true },
            new object[] { KeyCode.KeyPad9, true },
            new object[] { KeyCode.KeyPadEnter, false },
            new object[] { KeyCode.KeyPadEqual, false },
            new object[] { KeyCode.LeftShift, false },
            new object[] { KeyCode.LeftControl, false },
            new object[] { KeyCode.LeftAlt, false },
            new object[] { KeyCode.LeftSuper, false },
            new object[] { KeyCode.RightShift, false },
            new object[] { KeyCode.RightControl, false },
            new object[] { KeyCode.RightAlt, false },
            new object[] { KeyCode.RightSuper, false },
            new object[] { KeyCode.Menu, false },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="InternalExtensionMethods.IsShiftKey(KeyCode)"/> method.
    /// </summary>
    public static IEnumerable<object[]> IsShiftKeyTestData =>
        new List<object[]>
        {
            new object[] { KeyCode.A, false },
            new object[] { KeyCode.B, false },
            new object[] { KeyCode.C, false },
            new object[] { KeyCode.D, false },
            new object[] { KeyCode.E, false },
            new object[] { KeyCode.F, false },
            new object[] { KeyCode.G, false },
            new object[] { KeyCode.H, false },
            new object[] { KeyCode.I, false },
            new object[] { KeyCode.J, false },
            new object[] { KeyCode.K, false },
            new object[] { KeyCode.L, false },
            new object[] { KeyCode.M, false },
            new object[] { KeyCode.N, false },
            new object[] { KeyCode.O, false },
            new object[] { KeyCode.P, false },
            new object[] { KeyCode.Q, false },
            new object[] { KeyCode.R, false },
            new object[] { KeyCode.S, false },
            new object[] { KeyCode.T, false },
            new object[] { KeyCode.U, false },
            new object[] { KeyCode.V, false },
            new object[] { KeyCode.W, false },
            new object[] { KeyCode.X, false },
            new object[] { KeyCode.Y, false },
            new object[] { KeyCode.Z, false },
            new object[] { KeyCode.Space, false },
            new object[] { KeyCode.Unknown, false },
            new object[] { KeyCode.Apostrophe, false },
            new object[] { KeyCode.Comma, false },
            new object[] { KeyCode.Minus, false },
            new object[] { KeyCode.Period, false },
            new object[] { KeyCode.Slash, false },
            new object[] { KeyCode.D0, false },
            new object[] { KeyCode.D1, false },
            new object[] { KeyCode.D2, false },
            new object[] { KeyCode.D3, false },
            new object[] { KeyCode.D4, false },
            new object[] { KeyCode.D5, false },
            new object[] { KeyCode.D6, false },
            new object[] { KeyCode.D7, false },
            new object[] { KeyCode.D8, false },
            new object[] { KeyCode.D9, false },
            new object[] { KeyCode.Semicolon, false },
            new object[] { KeyCode.Equal, false },
            new object[] { KeyCode.LeftBracket, false },
            new object[] { KeyCode.Backslash, false },
            new object[] { KeyCode.RightBracket, false },
            new object[] { KeyCode.GraveAccent, false },
            new object[] { KeyCode.Escape, false },
            new object[] { KeyCode.Enter, false },
            new object[] { KeyCode.Tab, false },
            new object[] { KeyCode.Backspace, false },
            new object[] { KeyCode.Insert, false },
            new object[] { KeyCode.Delete, false },
            new object[] { KeyCode.Right, false },
            new object[] { KeyCode.Left, false },
            new object[] { KeyCode.Down, false },
            new object[] { KeyCode.Up, false },
            new object[] { KeyCode.PageUp, false },
            new object[] { KeyCode.PageDown, false },
            new object[] { KeyCode.Home, false },
            new object[] { KeyCode.End, false },
            new object[] { KeyCode.CapsLock, false },
            new object[] { KeyCode.ScrollLock, false },
            new object[] { KeyCode.NumLock, false },
            new object[] { KeyCode.PrintScreen, false },
            new object[] { KeyCode.Pause, false },
            new object[] { KeyCode.F1, false },
            new object[] { KeyCode.F2, false },
            new object[] { KeyCode.F3, false },
            new object[] { KeyCode.F4, false },
            new object[] { KeyCode.F5, false },
            new object[] { KeyCode.F6, false },
            new object[] { KeyCode.F7, false },
            new object[] { KeyCode.F8, false },
            new object[] { KeyCode.F9, false },
            new object[] { KeyCode.F10, false },
            new object[] { KeyCode.F11, false },
            new object[] { KeyCode.F12, false },
            new object[] { KeyCode.F13, false },
            new object[] { KeyCode.F14, false },
            new object[] { KeyCode.F15, false },
            new object[] { KeyCode.F16, false },
            new object[] { KeyCode.F17, false },
            new object[] { KeyCode.F18, false },
            new object[] { KeyCode.F19, false },
            new object[] { KeyCode.F20, false },
            new object[] { KeyCode.F21, false },
            new object[] { KeyCode.F22, false },
            new object[] { KeyCode.F23, false },
            new object[] { KeyCode.F24, false },
            new object[] { KeyCode.F25, false },
            new object[] { KeyCode.KeyPad0, false },
            new object[] { KeyCode.KeyPad1, false },
            new object[] { KeyCode.KeyPad2, false },
            new object[] { KeyCode.KeyPad3, false },
            new object[] { KeyCode.KeyPad4, false },
            new object[] { KeyCode.KeyPad5, false },
            new object[] { KeyCode.KeyPad6, false },
            new object[] { KeyCode.KeyPad7, false },
            new object[] { KeyCode.KeyPad8, false },
            new object[] { KeyCode.KeyPad9, false },
            new object[] { KeyCode.KeyPadDecimal, false },
            new object[] { KeyCode.KeyPadDivide, false },
            new object[] { KeyCode.KeyPadMultiply, false },
            new object[] { KeyCode.KeyPadSubtract, false },
            new object[] { KeyCode.KeyPadAdd, false },
            new object[] { KeyCode.KeyPadEnter, false },
            new object[] { KeyCode.KeyPadEqual, false },
            new object[] { KeyCode.LeftShift, true },
            new object[] { KeyCode.LeftControl, false },
            new object[] { KeyCode.LeftAlt, false },
            new object[] { KeyCode.LeftSuper, false },
            new object[] { KeyCode.RightShift, true },
            new object[] { KeyCode.RightControl, false },
            new object[] { KeyCode.RightAlt, false },
            new object[] { KeyCode.RightSuper, false },
            new object[] { KeyCode.Menu, false },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="InternalExtensionMethods.IsArrowKey(KeyCode)"/> method.
    /// </summary>
    public static IEnumerable<object[]> IsArrowKeyTestData =>
        new List<object[]>
        {
            new object[] { KeyCode.A, false },
            new object[] { KeyCode.B, false },
            new object[] { KeyCode.C, false },
            new object[] { KeyCode.D, false },
            new object[] { KeyCode.E, false },
            new object[] { KeyCode.F, false },
            new object[] { KeyCode.G, false },
            new object[] { KeyCode.H, false },
            new object[] { KeyCode.I, false },
            new object[] { KeyCode.J, false },
            new object[] { KeyCode.K, false },
            new object[] { KeyCode.L, false },
            new object[] { KeyCode.M, false },
            new object[] { KeyCode.N, false },
            new object[] { KeyCode.O, false },
            new object[] { KeyCode.P, false },
            new object[] { KeyCode.Q, false },
            new object[] { KeyCode.R, false },
            new object[] { KeyCode.S, false },
            new object[] { KeyCode.T, false },
            new object[] { KeyCode.U, false },
            new object[] { KeyCode.V, false },
            new object[] { KeyCode.W, false },
            new object[] { KeyCode.X, false },
            new object[] { KeyCode.Y, false },
            new object[] { KeyCode.Z, false },
            new object[] { KeyCode.Space, false },
            new object[] { KeyCode.Unknown, false },
            new object[] { KeyCode.Apostrophe, false },
            new object[] { KeyCode.Comma, false },
            new object[] { KeyCode.Minus, false },
            new object[] { KeyCode.Period, false },
            new object[] { KeyCode.Slash, false },
            new object[] { KeyCode.D0, false },
            new object[] { KeyCode.D1, false },
            new object[] { KeyCode.D2, false },
            new object[] { KeyCode.D3, false },
            new object[] { KeyCode.D4, false },
            new object[] { KeyCode.D5, false },
            new object[] { KeyCode.D6, false },
            new object[] { KeyCode.D7, false },
            new object[] { KeyCode.D8, false },
            new object[] { KeyCode.D9, false },
            new object[] { KeyCode.Semicolon, false },
            new object[] { KeyCode.Equal, false },
            new object[] { KeyCode.LeftBracket, false },
            new object[] { KeyCode.Backslash, false },
            new object[] { KeyCode.RightBracket, false },
            new object[] { KeyCode.GraveAccent, false },
            new object[] { KeyCode.Escape, false },
            new object[] { KeyCode.Enter, false },
            new object[] { KeyCode.Tab, false },
            new object[] { KeyCode.Backspace, false },
            new object[] { KeyCode.Insert, false },
            new object[] { KeyCode.Delete, false },
            new object[] { KeyCode.Right, true },
            new object[] { KeyCode.Left, true },
            new object[] { KeyCode.Down, true },
            new object[] { KeyCode.Up, true },
            new object[] { KeyCode.PageUp, false },
            new object[] { KeyCode.PageDown, false },
            new object[] { KeyCode.Home, false },
            new object[] { KeyCode.End, false },
            new object[] { KeyCode.CapsLock, false },
            new object[] { KeyCode.ScrollLock, false },
            new object[] { KeyCode.NumLock, false },
            new object[] { KeyCode.PrintScreen, false },
            new object[] { KeyCode.Pause, false },
            new object[] { KeyCode.F1, false },
            new object[] { KeyCode.F2, false },
            new object[] { KeyCode.F3, false },
            new object[] { KeyCode.F4, false },
            new object[] { KeyCode.F5, false },
            new object[] { KeyCode.F6, false },
            new object[] { KeyCode.F7, false },
            new object[] { KeyCode.F8, false },
            new object[] { KeyCode.F9, false },
            new object[] { KeyCode.F10, false },
            new object[] { KeyCode.F11, false },
            new object[] { KeyCode.F12, false },
            new object[] { KeyCode.F13, false },
            new object[] { KeyCode.F14, false },
            new object[] { KeyCode.F15, false },
            new object[] { KeyCode.F16, false },
            new object[] { KeyCode.F17, false },
            new object[] { KeyCode.F18, false },
            new object[] { KeyCode.F19, false },
            new object[] { KeyCode.F20, false },
            new object[] { KeyCode.F21, false },
            new object[] { KeyCode.F22, false },
            new object[] { KeyCode.F23, false },
            new object[] { KeyCode.F24, false },
            new object[] { KeyCode.F25, false },
            new object[] { KeyCode.KeyPad0, false },
            new object[] { KeyCode.KeyPad1, false },
            new object[] { KeyCode.KeyPad2, false },
            new object[] { KeyCode.KeyPad3, false },
            new object[] { KeyCode.KeyPad4, false },
            new object[] { KeyCode.KeyPad5, false },
            new object[] { KeyCode.KeyPad6, false },
            new object[] { KeyCode.KeyPad7, false },
            new object[] { KeyCode.KeyPad8, false },
            new object[] { KeyCode.KeyPad9, false },
            new object[] { KeyCode.KeyPadDecimal, false },
            new object[] { KeyCode.KeyPadDivide, false },
            new object[] { KeyCode.KeyPadMultiply, false },
            new object[] { KeyCode.KeyPadSubtract, false },
            new object[] { KeyCode.KeyPadAdd, false },
            new object[] { KeyCode.KeyPadEnter, false },
            new object[] { KeyCode.KeyPadEqual, false },
            new object[] { KeyCode.LeftShift, false },
            new object[] { KeyCode.LeftControl, false },
            new object[] { KeyCode.LeftAlt, false },
            new object[] { KeyCode.LeftSuper, false },
            new object[] { KeyCode.RightShift, false },
            new object[] { KeyCode.RightControl, false },
            new object[] { KeyCode.RightAlt, false },
            new object[] { KeyCode.RightSuper, false },
            new object[] { KeyCode.Menu, false },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="InternalExtensionMethods.IsCtrlKey(KeyCode)"/> method.
    /// </summary>
    public static IEnumerable<object[]> IsCtrlKeyTestData =>
        new List<object[]>
        {
            new object[] { KeyCode.A, false },
            new object[] { KeyCode.B, false },
            new object[] { KeyCode.C, false },
            new object[] { KeyCode.D, false },
            new object[] { KeyCode.E, false },
            new object[] { KeyCode.F, false },
            new object[] { KeyCode.G, false },
            new object[] { KeyCode.H, false },
            new object[] { KeyCode.I, false },
            new object[] { KeyCode.J, false },
            new object[] { KeyCode.K, false },
            new object[] { KeyCode.L, false },
            new object[] { KeyCode.M, false },
            new object[] { KeyCode.N, false },
            new object[] { KeyCode.O, false },
            new object[] { KeyCode.P, false },
            new object[] { KeyCode.Q, false },
            new object[] { KeyCode.R, false },
            new object[] { KeyCode.S, false },
            new object[] { KeyCode.T, false },
            new object[] { KeyCode.U, false },
            new object[] { KeyCode.V, false },
            new object[] { KeyCode.W, false },
            new object[] { KeyCode.X, false },
            new object[] { KeyCode.Y, false },
            new object[] { KeyCode.Z, false },
            new object[] { KeyCode.Space, false },
            new object[] { KeyCode.Unknown, false },
            new object[] { KeyCode.Apostrophe, false },
            new object[] { KeyCode.Comma, false },
            new object[] { KeyCode.Minus, false },
            new object[] { KeyCode.Period, false },
            new object[] { KeyCode.Slash, false },
            new object[] { KeyCode.D0, false },
            new object[] { KeyCode.D1, false },
            new object[] { KeyCode.D2, false },
            new object[] { KeyCode.D3, false },
            new object[] { KeyCode.D4, false },
            new object[] { KeyCode.D5, false },
            new object[] { KeyCode.D6, false },
            new object[] { KeyCode.D7, false },
            new object[] { KeyCode.D8, false },
            new object[] { KeyCode.D9, false },
            new object[] { KeyCode.Semicolon, false },
            new object[] { KeyCode.Equal, false },
            new object[] { KeyCode.LeftBracket, false },
            new object[] { KeyCode.Backslash, false },
            new object[] { KeyCode.RightBracket, false },
            new object[] { KeyCode.GraveAccent, false },
            new object[] { KeyCode.Escape, false },
            new object[] { KeyCode.Enter, false },
            new object[] { KeyCode.Tab, false },
            new object[] { KeyCode.Backspace, false },
            new object[] { KeyCode.Insert, false },
            new object[] { KeyCode.Delete, false },
            new object[] { KeyCode.Right, false },
            new object[] { KeyCode.Left, false },
            new object[] { KeyCode.Down, false },
            new object[] { KeyCode.Up, false },
            new object[] { KeyCode.PageUp, false },
            new object[] { KeyCode.PageDown, false },
            new object[] { KeyCode.Home, false },
            new object[] { KeyCode.End, false },
            new object[] { KeyCode.CapsLock, false },
            new object[] { KeyCode.ScrollLock, false },
            new object[] { KeyCode.NumLock, false },
            new object[] { KeyCode.PrintScreen, false },
            new object[] { KeyCode.Pause, false },
            new object[] { KeyCode.F1, false },
            new object[] { KeyCode.F2, false },
            new object[] { KeyCode.F3, false },
            new object[] { KeyCode.F4, false },
            new object[] { KeyCode.F5, false },
            new object[] { KeyCode.F6, false },
            new object[] { KeyCode.F7, false },
            new object[] { KeyCode.F8, false },
            new object[] { KeyCode.F9, false },
            new object[] { KeyCode.F10, false },
            new object[] { KeyCode.F11, false },
            new object[] { KeyCode.F12, false },
            new object[] { KeyCode.F13, false },
            new object[] { KeyCode.F14, false },
            new object[] { KeyCode.F15, false },
            new object[] { KeyCode.F16, false },
            new object[] { KeyCode.F17, false },
            new object[] { KeyCode.F18, false },
            new object[] { KeyCode.F19, false },
            new object[] { KeyCode.F20, false },
            new object[] { KeyCode.F21, false },
            new object[] { KeyCode.F22, false },
            new object[] { KeyCode.F23, false },
            new object[] { KeyCode.F24, false },
            new object[] { KeyCode.F25, false },
            new object[] { KeyCode.KeyPad0, false },
            new object[] { KeyCode.KeyPad1, false },
            new object[] { KeyCode.KeyPad2, false },
            new object[] { KeyCode.KeyPad3, false },
            new object[] { KeyCode.KeyPad4, false },
            new object[] { KeyCode.KeyPad5, false },
            new object[] { KeyCode.KeyPad6, false },
            new object[] { KeyCode.KeyPad7, false },
            new object[] { KeyCode.KeyPad8, false },
            new object[] { KeyCode.KeyPad9, false },
            new object[] { KeyCode.KeyPadDecimal, false },
            new object[] { KeyCode.KeyPadDivide, false },
            new object[] { KeyCode.KeyPadMultiply, false },
            new object[] { KeyCode.KeyPadSubtract, false },
            new object[] { KeyCode.KeyPadAdd, false },
            new object[] { KeyCode.KeyPadEnter, false },
            new object[] { KeyCode.KeyPadEqual, false },
            new object[] { KeyCode.LeftShift, false },
            new object[] { KeyCode.LeftControl, true },
            new object[] { KeyCode.LeftAlt, false },
            new object[] { KeyCode.LeftSuper, false },
            new object[] { KeyCode.RightShift, false },
            new object[] { KeyCode.RightControl, true },
            new object[] { KeyCode.RightAlt, false },
            new object[] { KeyCode.RightSuper, false },
            new object[] { KeyCode.Menu, false },
        };

    /// <summary>
    /// Gets the rectangle vertice data for the <see cref="CreateRectFromLine_WhenInvoked_ReturnsCorrectResult"/> unit test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> GetExpectedRectPointData()
    {
        // X and Y axis aligned rectangle
        yield return new object[]
        {
            new LineBatchItem(new Vector2(50f, 100f), new Vector2(200f, 100f), NETColor.White, 20),
            new Vector2(50f, 90f),
            new Vector2(200f, 90f),
            new Vector2(200f, 110f),
            new Vector2(50f, 110f),
        };

        // X and Y axis aligned rectangle rotated 45 degrees clockwise
        yield return new object[]
        {
            new LineBatchItem(new Vector2(100f, 100f), new Vector2(200f, 200f), NETColor.White, 100f),
            new Vector2(135.35535f, 64.64465f),
            new Vector2(235.35535f, 164.64467f),
            new Vector2(164.64465f, 235.35533f),
            new Vector2(64.64465f, 135.35535f),
        };
    }

    // ReSharper restore HeapView.BoxingAllocation
    #endregion

    #region Method Tests
    [Fact]
    public void ToSixLaborImage_WhenInvoked_CorrectlyConvertsToSixLaborImage()
    {
        // Arrange
        var imageData = new ImageData(new NETColor[2, 3], 2, 3);

        var expectedPixels = new Rgba32[2, 3];

        // Act
        var sixLaborsImage = imageData.ToSixLaborImage();
        var actualPixels = GetSixLaborPixels(sixLaborsImage);

        // Assert
        actualPixels.Should().BeEquivalentTo(expectedPixels);
    }

    [Fact]
    public void ToImageData_WhenInvoked_CorrectlyConvertsToSixLaborImage()
    {
        // Arrange
        var rowColors = new Dictionary<uint, NETColor>
        {
            { 0, NETColor.Red },
            { 1, NETColor.Green },
            { 2, NETColor.Blue },
        };

        var sixLaborsImage = CreateSixLaborsImage(2, 3, rowColors);
        var expectedPixels = CreateImageDataPixels(2, 3, rowColors);

        // Act
        var actual = TestHelpers.ToImageData(sixLaborsImage);

        // Assert
        actual.Pixels.Should().BeEquivalentTo(expectedPixels);
    }

    [Fact]
    public void SetVertexPos_WithRectGPUDataAndInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetVertexPos(It.IsAny<Vector2>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetVertexPos_WhenInvokedWithRectGPUData_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetVertexPos(new Vector2(1111f, 2222f), vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetVertexPos(new Vector2(1111f, 2222f), vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetVertexPos(new Vector2(1111f, 2222f), vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetVertexPos(new Vector2(1111f, 2222f), vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.Should().BeFalse();
        actual.VertexPos.Should().Be(new Vector2(1111f, 2222f));
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void Scale_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expectedP1 = new Vector2(100, 100);
        var expectedP2 = new Vector2(150, 150);

        var p1 = new Vector2(100, 100);
        var p2 = new Vector2(200, 200);

        var sut = new LineBatchItem(p1, p2, NETColor.White, 0f);

        // Act
        var actual = sut.Scale(0.5f);

        // Assert
        actual.P1.Should().Be(expectedP1);
        actual.P2.Should().Be(expectedP2);
    }

    [Fact]
    public void FlipEnd_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expectedP1 = new Vector2(100, 100);
        var expectedP2 = new Vector2(0, 0);

        var p1 = new Vector2(100, 100);
        var p2 = new Vector2(200, 200);

        var sut = new LineBatchItem(p1, p2, NETColor.White, 0f);

        // Act
        var actual = sut.FlipEnd();

        // Assert
        actual.P1.Should().Be(expectedP1);
        actual.P2.Should().Be(expectedP2);
    }

    [Fact]
    public void Clamp_WhenInvoked_ClampsRadiusValues()
    {
        // Arrange
        var sut = new CornerRadius(200f, 200, -200f, -200f);

        // Act
        sut = sut.Clamp(0f, 100f);

        // Assert
        sut.TopLeft.Should().Be(100f);
        sut.BottomLeft.Should().Be(100f);
        sut.BottomRight.Should().Be(0f);
        sut.TopRight.Should().Be(0f);
    }

    [Fact]
    public void Length_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const float startX = 124.6f;
        const float startY = 187.5f;
        const float stopX = 257.3f;
        const float stopY = 302.4f;

        var line = new LineBatchItem(
            new Vector2(startX, startY),
            new Vector2(stopX, stopY),
            NETColor.White,
            0f);

        // Act
        var actual = line.Length();

        // Assert
        actual.Should().Be(175.53146f);
    }

    [Fact]
    public void SetP1_WhenInvokedForLineBatchItem_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(10, 20),
            new Vector2(2, 3),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SetP1(new Vector2(10, 20));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SetP2_WhenInvokedForLineBatchItem_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(20, 30),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SetP2(new Vector2(20, 30));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SwapEnds_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(2, 3),
            new Vector2(1, 2),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            NETColor.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SwapEnds();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SetVertexPos_WithInvalidVertexNumber_ThrowsException()
    {
        // Arrange
        var gpuData = new LineGPUData(
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty));

        // Act
        var act = () => gpuData.SetVertexPos(Vector2.Zero, (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Fact]
    public void SetRectangle_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act & Assert
        var act = () => gpuData.SetRectangle(It.IsAny<Vector4>(), (VertexNumber)1234);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetRectangle_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetRectangle(new Vector4(1111f, 2222f, 3333f, 4444f), vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetRectangle(new Vector4(1111f, 2222f, 3333f, 4444f), vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetRectangle(new Vector4(1111f, 2222f, 3333f, 4444f), vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetRectangle(new Vector4(1111f, 2222f, 3333f, 4444f), vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.Should().BeFalse();
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(new Vector4(1111f, 2222f, 3333f, 4444f));
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetRectangle_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);
        var expected = new Vector4(111, 222, 333, 444);

        // Act
        var actual = gpuData.SetRectangle(new Vector4(111, 222, 333, 444));

        // Assert
        actual.Vertex1.Rectangle.Should().Be(expected);
        actual.Vertex2.Rectangle.Should().Be(expected);
        actual.Vertex3.Rectangle.Should().Be(expected);
        actual.Vertex4.Rectangle.Should().Be(expected);
    }

    [Fact]
    public void SetAsSolid_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetAsSolid(It.IsAny<bool>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetAsSolid_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetAsSolid(true, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetAsSolid(true, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetAsSolid(true, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetAsSolid(true, vertexNumber).Vertex4,
        };

        // Assert
        actual.IsSolid.Should().BeTrue();
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetAsSolid_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var actual = gpuData.SetAsSolid(true);

        // Assert
        actual.Vertex1.IsSolid.Should().BeTrue();
        actual.Vertex2.IsSolid.Should().BeTrue();
        actual.Vertex3.IsSolid.Should().BeTrue();
        actual.Vertex4.IsSolid.Should().BeTrue();
    }

    [Fact]
    public void SetBorderThickness_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetBorderThickness(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetBorderThickness_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetBorderThickness(123f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetBorderThickness(123f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetBorderThickness(123f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetBorderThickness(123f, vertexNumber).Vertex4,
        };

        // Assert
        actual.BorderThickness.Should().Be(123f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.IsSolid.Should().Be(expectedVertex.IsSolid);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetBorderThickness_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetBorderThickness(123f);

        // Assert
        actual.Vertex1.BorderThickness.Should().Be(expected);
        actual.Vertex2.BorderThickness.Should().Be(expected);
        actual.Vertex3.BorderThickness.Should().Be(expected);
        actual.Vertex4.BorderThickness.Should().Be(expected);
    }

    [Fact]
    public void SetTopLeftCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetTopLeftCornerRadius(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetTopLeftCornerRadius_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetTopLeftCornerRadius(1234f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetTopLeftCornerRadius(1234f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetTopLeftCornerRadius(1234f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetTopLeftCornerRadius(1234f, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.Should().BeFalse();
        actual.TopLeftCornerRadius.Should().Be(1234f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetTopLeftCornerRadius_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetTopLeftCornerRadius(123f);

        // Assert
        actual.Vertex1.TopLeftCornerRadius.Should().Be(expected);
        actual.Vertex2.TopLeftCornerRadius.Should().Be(expected);
        actual.Vertex3.TopLeftCornerRadius.Should().Be(expected);
        actual.Vertex4.TopLeftCornerRadius.Should().Be(expected);
    }

    [Fact]
    public void SetBottomLeftCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetBottomLeftCornerRadius(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetBottomLeftCornerRadius_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetBottomLeftCornerRadius(1234f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetBottomLeftCornerRadius(1234f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetBottomLeftCornerRadius(1234f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetBottomLeftCornerRadius(1234f, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.Should().BeFalse();
        actual.BottomLeftCornerRadius.Should().Be(1234f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetBottomLeftCornerRadius_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetBottomLeftCornerRadius(123f);

        // Assert
        actual.Vertex1.BottomLeftCornerRadius.Should().Be(expected);
        actual.Vertex2.BottomLeftCornerRadius.Should().Be(expected);
        actual.Vertex3.BottomLeftCornerRadius.Should().Be(expected);
        actual.Vertex4.BottomLeftCornerRadius.Should().Be(expected);
    }

    [Fact]
    public void SetBottomRightCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetBottomRightCornerRadius(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetBottomRightCornerRadius_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetBottomRightCornerRadius(1234f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetBottomRightCornerRadius(1234f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetBottomRightCornerRadius(1234f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetBottomRightCornerRadius(1234f, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.Should().BeFalse();
        actual.BottomRightCornerRadius.Should().Be(1234f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetBottomRightCornerRadius_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetBottomRightCornerRadius(123f);

        // Assert
        actual.Vertex1.BottomRightCornerRadius.Should().Be(expected);
        actual.Vertex2.BottomRightCornerRadius.Should().Be(expected);
        actual.Vertex3.BottomRightCornerRadius.Should().Be(expected);
        actual.Vertex4.BottomRightCornerRadius.Should().Be(expected);
    }

    [Fact]
    public void SetTopRightCornerRadius_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetTopRightCornerRadius(It.IsAny<float>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetTopRightCornerRadius_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetTopRightCornerRadius(1234f, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetTopRightCornerRadius(1234f, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetTopRightCornerRadius(1234f, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetTopRightCornerRadius(1234f, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.Should().BeFalse();
        actual.TopRightCornerRadius.Should().Be(1234f);
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(expectedVertex.Color);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
    }

    [Fact]
    public void SetTopRightCornerRadius_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);
        const float expected = 123f;

        // Act
        var actual = gpuData.SetTopRightCornerRadius(123f);

        // Assert
        actual.Vertex1.TopRightCornerRadius.Should().Be(expected);
        actual.Vertex2.TopRightCornerRadius.Should().Be(expected);
        actual.Vertex3.TopRightCornerRadius.Should().Be(expected);
        actual.Vertex4.TopRightCornerRadius.Should().Be(expected);
    }

    [Fact]
    public void SetColor_WithInvalidVertexValue_ThrowsException()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);

        // Act
        var act = () => gpuData.SetColor(It.IsAny<NETColor>(), (VertexNumber)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("The vertex number is invalid. (Parameter 'vertexNumber')");
    }

    [Theory]
    [InlineData((int)VertexNumber.One)]
    [InlineData((int)VertexNumber.Two)]
    [InlineData((int)VertexNumber.Three)]
    [InlineData((int)VertexNumber.Four)]
    public void SetColor_WhenInvoked_ReturnsCorrectResult(int vertexNumberNumericalValue)
    {
        // Arrange
        var vertexNumber = (VertexNumber)vertexNumberNumericalValue;
        var gpuData = GenerateGPUDataInSequence(0);
        var expectedVertex = vertexNumber switch
        {
            VertexNumber.One => gpuData.Vertex1,
            VertexNumber.Two => gpuData.Vertex2,
            VertexNumber.Three => gpuData.Vertex3,
            VertexNumber.Four => gpuData.Vertex4,
        };

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetColor(NETColor.Blue, vertexNumber).Vertex1,
            VertexNumber.Two => gpuData.SetColor(NETColor.Blue, vertexNumber).Vertex2,
            VertexNumber.Three => gpuData.SetColor(NETColor.Blue, vertexNumber).Vertex3,
            VertexNumber.Four => gpuData.SetColor(NETColor.Blue, vertexNumber).Vertex4,
        };

        // Assert
        expectedVertex.IsSolid.Should().BeFalse();
        actual.VertexPos.Should().Be(expectedVertex.VertexPos);
        actual.Rectangle.Should().Be(expectedVertex.Rectangle);
        actual.Color.Should().Be(NETColor.Blue);
        actual.BorderThickness.Should().Be(expectedVertex.BorderThickness);
        actual.TopLeftCornerRadius.Should().Be(expectedVertex.TopLeftCornerRadius);
        actual.BottomLeftCornerRadius.Should().Be(expectedVertex.BottomLeftCornerRadius);
        actual.BottomRightCornerRadius.Should().Be(expectedVertex.BottomRightCornerRadius);
        actual.TopRightCornerRadius.Should().Be(expectedVertex.TopRightCornerRadius);
    }

    [Fact]
    public void SetColor_WhenUpdatingAll_ReturnsCorrectResult()
    {
        // Arrange
        var gpuData = GenerateGPUDataInSequence(0);
        var expected = NETColor.FromArgb(220, 230, 240, 250);

        // Act
        var actual = gpuData.SetColor(NETColor.FromArgb(220, 230, 240, 250));

        // Assert
        actual.Vertex1.Color.Should().Be(expected);
        actual.Vertex2.Color.Should().Be(expected);
        actual.Vertex3.Color.Should().Be(expected);
        actual.Vertex4.Color.Should().Be(expected);
    }

    [Fact]
    public void SetColor_WhenSettingLineGPUData_SetsColorToAllVertexData()
    {
        // Arrange
        var data = new LineGPUData(
            new LineVertexData(Vector2.Zero, NETColor.White),
            new LineVertexData(Vector2.Zero, NETColor.White),
            new LineVertexData(Vector2.Zero, NETColor.White),
            new LineVertexData(Vector2.Zero, NETColor.White));

        // Act
        var actual = data.SetColor(NETColor.CornflowerBlue);

        // Assert
        actual.Vertex1.Color.Should().Be(NETColor.CornflowerBlue);
        actual.Vertex2.Color.Should().Be(NETColor.CornflowerBlue);
        actual.Vertex3.Color.Should().Be(NETColor.CornflowerBlue);
        actual.Vertex4.Color.Should().Be(NETColor.CornflowerBlue);
    }

    [Fact]
    public void GetPosition_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var rect = new NETRectF(11f, 22f, 33f, 44f);

        // Act
        var actual = rect.GetPosition();

        // Assert
        actual.X.Should().Be(11f);
        actual.Y.Should().Be(22f);
    }

    [Theory]
    [InlineData(@"C:\dir1\dir2", "C:/dir1/dir2")]
    [InlineData(@"C:\dir1\dir2\", "C:/dir1/dir2/")]
    [InlineData("C:/dir1/dir2", "C:/dir1/dir2")]
    [InlineData("C:/dir1/dir2/", "C:/dir1/dir2/")]
    public void NormalizePaths_WhenInvoked_ReturnsCorrectResult(string path, string expected)
    {
        // Arrange
        var paths = new[] { path };

        // Act
        var actual = paths.NormalizePaths().ToArray();

        // Assert
        actual.Should().ContainSingle();
        actual[0].Should().Be(expected);
    }

    [Fact]
    public void ToVector2_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var point = new NETPoint(11, 22);

        // Act
        var actual = point.ToVector2();

        // Assert
        actual.X.Should().Be(11f);
        actual.Y.Should().Be(22f);
    }

    [Fact]
    public void ToPoint_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var point = new Vector2(11, 22);

        // Act
        var actual = point.ToPoint();

        // Assert
        actual.X.Should().Be(11);
        actual.Y.Should().Be(22);
    }

    [Fact]
    public void DequeueWhile_WithNoItems_DoesNotInvokedPredicate()
    {
        // Arrange
        var queue = new Queue<int>();

        var untilPredicate = new Predicate<int>(_ =>
        {
            Assert.Fail("The 'untilPredicate' should not be invoked with 0 queue items.");
            return false;
        });

        // Act & Assert
        queue.DequeueWhile(untilPredicate);
    }

    [Fact]
    public void DequeueWhile_WhenInvoked_PerformsDequeueWhenPredicateIsTrue()
    {
        // Arrange
        var totalInvokes = 0;
        var queue = new Queue<int>();
        queue.Enqueue(11);
        queue.Enqueue(22);

        var untilPredicate = new Predicate<int>(_ =>
        {
            totalInvokes += 1;
            return true;
        });

        // Act
        queue.DequeueWhile(untilPredicate);

        // Assert
        totalInvokes.Should().Be(2);
        queue.Should().BeEmpty();
    }

    [Theory]
    [InlineData(3, 2)]
    [InlineData(40, -1)]
    public void IndexOf_WithEnumerableItemsAndPredicate_ReturnsCorrectResult(int value, int expected)
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4 };

        // Act
        var actual = items.IndexOf(i => i == value);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void FirstItemIndex_WhenPredicateReturnsTrue_ReturnsCorrectIndex()
    {
        // Arrange
        var sut = new Memory<string>(new[] { "item-A", "item-C", "item-B" });

        // Act
        var actual = sut.FirstItemIndex(i => i == "item-C");

        // Assert
        actual.Should().Be(1);
    }

    [Fact]
    public void FirstItemIndex_WhenPredicateNeverReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Memory<string>(new[] { "item-A", "item-C", "item-B" });

        // Act
        var actual = sut.FirstItemIndex(i => i == "item-D");

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void FirstLayerIndex_WhenLayerExists_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 30, Item = "itemC" },
            new () { Layer = 40, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.FirstLayerIndex(30);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void FirstLayerIndex_WhenLayerDoesNotExists_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 30, Item = "itemC" },
            new () { Layer = 40, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.FirstLayerIndex(300);

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void TotalOnLayer_WithLayerGreaterThanRequestedLayer_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.TotalOnLayer(20);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void TotalOnLayer_WithNoLayersGreaterThanRequestedLayer_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.TotalOnLayer(200);

        // Assert
        actual.Should().Be(0);
    }

    [Fact]
    public void IndexOf_WithMemoryItemsAndWhenPredicateReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            default,
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.IndexOf(string.IsNullOrEmpty);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void IndexOf_WithMemoryItemsAndWhenPredicateNeverReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.IndexOf(string.IsNullOrEmpty);

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void IncreaseBy_WhenInvoked_CorrectlyIncreasesItems()
    {
        // Arrange
        var expected = new[] { 1, 2, 3, 4, 0, 0 };
        var items = new Memory<int>(new[] { 1, 2, 3, 4, });

        // Act
        items.IncreaseBy(2);

        // Assert
        items.Span.ToArray().Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToBatchItem_WithRectShapeOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new ShapeBatchItem(new Vector2(1, 2),
            100f,
            90f,
            NETColor.FromArgb(5, 6, 7, 8),
            true,
            30f,
            new CornerRadius(10, 11, 12, 13),
            ColorGradient.Horizontal,
            NETColor.FromArgb(14, 15, 16, 17),
            NETColor.FromArgb(18, 19, 20, 21));

        var sut = new RectShape
        {
            Position = new Vector2(1, 2),
            Width = 100,
            Height = 90,
            Color = NETColor.FromArgb(5, 6, 7, 8),
            IsSolid = true,
            BorderThickness = 30,
            CornerRadius = new CornerRadius(10, 11, 12, 13),
            GradientType = ColorGradient.Horizontal,
            GradientStart = NETColor.FromArgb(14, 15, 16, 17),
            GradientStop = NETColor.FromArgb(18, 19, 20, 21),
        };

        // Act
        var actual = sut.ToBatchItem();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToBatchItem_WithCircleShapeOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new ShapeBatchItem(new Vector2(1, 2),
            100f,
            100f,
            NETColor.FromArgb(4, 5, 6, 7),
            true,
            50f,
            new CornerRadius(50f),
            ColorGradient.Horizontal,
            NETColor.FromArgb(9, 10, 11, 12),
            NETColor.FromArgb(13, 14, 15, 16));

        var sut = new CircleShape
        {
            Position = new Vector2(1, 2),
            Diameter = 100,
            Color = NETColor.FromArgb(4, 5, 6, 7),
            IsSolid = true,
            BorderThickness = 50,
            GradientType = ColorGradient.Horizontal,
            GradientStart = NETColor.FromArgb(9, 10, 11, 12),
            GradientStop = NETColor.FromArgb(13, 14, 15, 16),
        };

        // Act
        var actual = sut.ToBatchItem();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(IsLetterKeyTestData))]
    public void IsLetterKey_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange & Act
        var actual = key.IsLetterKey();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsNumberKeyTestData))]
    public void IsNumberKey_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange & Act
        var actual = key.IsNumberKey();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsSymbolKeyTestData))]
    public void IsSymbolKey_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange & Act
        var actual = key.IsSymbolKey();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsVisibleKeyTestData))]
    public void IsVisibleKey_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange & Act
        var actual = key.IsVisibleKey();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsShiftKeyTestData))]
    public void IsShiftKey_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange & Act
        var actual = key.IsShiftKey();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsArrowKeyTestData))]
    public void IsArrowKey_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange & Act
        var actual = key.IsArrowKey();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsCtrlKeyTestData))]
    public void IsCtrlKey_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange & Act
        var actual = key.IsCtrlKey();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(KeyCode.Left, true, true)]
    [InlineData(KeyCode.Right, true, true)]
    [InlineData(KeyCode.Up, true, true)]
    [InlineData(KeyCode.Down, true, true)]
    [InlineData(KeyCode.Down, false, false)]
    public void AnyArrowKeysDown_WhenInvoked_ReturnsCorrectResult(
        KeyCode key,
        bool state,
        bool expected)
    {
        // Arrange
        var keyState = default(KeyboardState);
        keyState.SetKeyState(key, state);

        // Act
        var actual = keyState.AnyArrowKeysDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(VertexNumber.One)]
    [InlineData(VertexNumber.Two)]
    [InlineData(VertexNumber.Three)]
    [InlineData(VertexNumber.Four)]
    internal void SetVertexPos_WhenInvokedWithLineGPUData_ReturnsCorrectResult(VertexNumber vertexNumber)
    {
        // Arrange
        var expectedPos = new Vector2(10, 20);

        var gpuData = new LineGPUData(
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty),
            new LineVertexData(Vector2.Zero, NETColor.Empty));

        // Act
        var actual = vertexNumber switch
        {
            VertexNumber.One => gpuData.SetVertexPos(new Vector2(10, 20), VertexNumber.One).Vertex1,
            VertexNumber.Two => gpuData.SetVertexPos(new Vector2(10, 20), VertexNumber.Two).Vertex2,
            VertexNumber.Three => gpuData.SetVertexPos(new Vector2(10, 20), VertexNumber.Three).Vertex3,
            VertexNumber.Four => gpuData.SetVertexPos(new Vector2(10, 20), VertexNumber.Four).Vertex4,
        };

        // Assert
        actual.VertexPos.Should().BeEquivalentTo(expectedPos);
    }

    [Theory]
    [MemberData(nameof(GetExpectedRectPointData))]
    internal void CreateRectFromLine_WhenInvoked_ReturnsCorrectResult(
        LineBatchItem lineItem,
        Vector2 topLeftCorner,
        Vector2 topRightCorner,
        Vector2 bottomRightCorner,
        Vector2 bottomLeftCorner)
    {
        // Arrange
        var expected = new[]
        {
            topLeftCorner,
            topRightCorner,
            bottomRightCorner,
            bottomLeftCorner,
        };

        var line = new LineBatchItem(lineItem.P1, lineItem.P2, lineItem.Color, lineItem.Thickness);

        // Act
        var actual = line.CreateRectFromLine();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    /// <summary>
    /// Creates a Six Labors image type of <see cref="Image{Rgba32}"/> with the given <paramref name="width"/>
    /// and <paramref name="height"/> with each row having its own colors described by the given
    /// <paramref name="rowColors"/> dictionary.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="rowColors">The color for each row.</param>
    /// <returns>An image with the given row colors.</returns>
    /// <remarks>
    ///     The <paramref name="rowColors"/> dictionary key is the zero based row index and the
    ///     value is the color to make the entire row.
    /// </remarks>
    [ExcludeFromCodeCoverage(Justification = "Do not need to see coverage for code used for testing.")]
    private static Image<Rgba32> CreateSixLaborsImage(int width, int height, Dictionary<uint, NETColor> rowColors)
    {
        if (height != rowColors.Count)
        {
            Assert.Fail($"The height '{height}' of the image must match the total number of row colors '{rowColors.Count}'.");
        }

        var availableRows = rowColors.Keys.ToArray();

        foreach (var row in availableRows)
        {
            if (row > height - 1)
            {
                Assert.Fail($"The row '{row}' is not within the range of rows for the image height '{height}' for the definition of row colors.");
            }
        }

        var result = new Image<Rgba32>(width, height);

        for (var y = 0; y < height; y++)
        {
            var row = y;
            result.ProcessPixelRows(accessor =>
            {
                var rowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < width; x++)
                {
                    rowSpan[x] = new Rgba32(
                        rowColors[(uint)row].R,
                        rowColors[(uint)row].G,
                        rowColors[(uint)row].B,
                        rowColors[(uint)row].A);
                }
            });
        }

        return result;
    }

    private static NETColor[,] CreateImageDataPixels(int width, int height, Dictionary<uint, NETColor> rowColors)
    {
        var result = new NETColor[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                result[x, y] = NETColor.FromArgb(rowColors[(uint)y].A, rowColors[(uint)y].R, rowColors[(uint)y].G, rowColors[(uint)y].B);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the <see cref="Rgba32"/> pixels from the given <paramref name="sixLaborsImage"/>.
    /// </summary>
    /// <param name="sixLaborsImage">The six labors image.</param>
    /// <returns>The two dimensional pixel colors of the image.</returns>
    private static Rgba32[,] GetSixLaborPixels(Image<Rgba32> sixLaborsImage)
    {
        var result = new Rgba32[sixLaborsImage.Width, sixLaborsImage.Height];

        for (var y = 0; y < sixLaborsImage.Height; y++)
        {
            var row = y;
            sixLaborsImage.ProcessPixelRows(accessor =>
            {
                var pixelRow = accessor.GetRowSpan(row);

                for (var x = 0; x < sixLaborsImage.Width; x++)
                {
                    result[x, row] = pixelRow[x];
                }
            });
        }

        return result;
    }

    /// <summary>
    /// Generates GPU data with sequential, numerical values throughout
    /// the struct, starting with the given <paramref name="startValue"/> for the purpose of testing.
    /// </summary>
    /// <param name="startValue">The value to start the sequential assignment from.</param>
    /// <returns>The GPU data to test.</returns>
    private static RectGPUData GenerateGPUDataInSequence(int startValue)
    {
        var vertex1 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex2 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex3 = GenerateVertexDataInSequence(startValue);
        startValue += 11;
        var vertex4 = GenerateVertexDataInSequence(startValue);

        return new RectGPUData(vertex1, vertex2, vertex3, vertex4);
    }

    /// <summary>
    /// Generates vertex data with numerical values sequentially throughout
    /// the struct starting with the given <paramref name="startValue"/> for the purpose of testing.
    /// </summary>
    /// <param name="startValue">The value to start the sequential assignment from.</param>
    /// <returns>The vertex data to test.</returns>
    private static RectVertexData GenerateVertexDataInSequence(int startValue)
    {
        return new RectVertexData(
            new Vector2(startValue + 1f, startValue + 2f),
            new Vector4(startValue + 3, startValue + 4, startValue + 5, startValue + 6),
            NETColor.FromArgb(startValue + 7, startValue + 8, startValue + 9, startValue + 10),
            false,
            startValue + 7f,
            startValue + 8f,
            startValue + 9f,
            startValue + 10f,
            startValue + 11f);
    }
}
