// <copyright file="IWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Numerics;
using Scene;
using WebGpu.Batching;

/// <summary>
/// Provides the core of an application window which facilitates how the
/// window behaves, its state and the ability to be used in various types
/// of applications.
/// </summary>
public interface IWindow
{
    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate to be invoked one time to initialize the window.
    /// </summary>
    Action? Initialize { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate that is invoked per frame for updating.
    /// </summary>
    Action<FrameTime>? Update { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate that is invoked per frame for rendering.
    /// </summary>
    Action<FrameTime>? Draw { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate to be invoked one time to uninitialize the window.
    /// </summary>
    Action? Uninitialize { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="Action"/> delegate that is invoked every time the window is resized.
    /// </summary>
    Action<SizeU>? WinResize { get; set; }

    /// <summary>
    /// Gets or sets the title of the window.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the position of the window.
    /// </summary>
    Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the width of the window.
    /// </summary>
    uint Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the window.
    /// </summary>
    uint Height { get; set; }

    /// <summary>
    /// Gets or sets the value of how often the update and render calls are invoked in the value of hertz.
    /// </summary>
    int UpdateFrequency { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the mouse cursor is visible.
    /// </summary>
    bool MouseCursorVisible { get; set; }

    /// <summary>
    /// Gets a value indicating whether the window has been initialized.
    /// </summary>
    bool Initialized { get; }

    /// <summary>
    /// Gets or sets the state of the window.
    /// </summary>
    StateOfWindow WindowState { get; set; }

    /// <summary>
    /// Gets or sets the type of border that the <see cref="IWindow"/> will have.
    /// </summary>
    WindowBorder TypeOfBorder { get; set; }

    /// <summary>
    /// Gets the scene manager.
    /// </summary>
    ISceneManager SceneManager { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the scenes should be automatically loaded.
    /// </summary>
    bool AutoSceneLoading { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scenes should be automatically unloaded.
    /// </summary>
    bool AutoSceneUnloading { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scenes should be automatically updated.
    /// </summary>
    bool AutoSceneUpdating { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the scenes should be automatically rendered.
    /// </summary>
    bool AutoSceneRendering { get; set; }

    /// <summary>
    /// Gets the frames per second that the main loop is running at.
    /// </summary>
    float Fps { get; }

    /// <summary>
    /// Shows the window.
    /// </summary>
    void Show();

    /// <summary>
    /// Closes the window.
    /// </summary>
    void Close();
}
