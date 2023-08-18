// <copyright file="IWindow.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI;

using System;
using System.Numerics;
using System.Threading.Tasks;
using Batching;
using Content;
using Scene;

/// <summary>
/// Provides the core of an application window which facilitates how the
/// window behaves, its state and the ability to be used in various types
/// of applications.
/// </summary>
public interface IWindow : IDisposable
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
    /// Gets or sets a value indicating whether or not the buffers should
    /// be automatically cleared before rendering any textures.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     If this is set to true, it means you do not have to
    ///     use or invoke the <see cref="IBatcher.Clear"/>() method.
    /// </para>
    /// <para>
    ///     Set to the value of <c>false</c> if you want more control when
    ///     the back buffers will be cleared.
    /// </para>
    /// <para>
    ///     WARNING!! - To prevent performance issues, do not manually clear the
    ///     buffer with the <see cref="IBatcher.Clear"/>() method
    ///     and set this property to true.  That would be a waste of resources.
    /// </para>
    /// </remarks>
    bool AutoClearBuffer { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the mouse cursor is visible.
    /// </summary>
    bool MouseCursorVisible { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the window has been initialized.
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
    /// Gets or sets the content loader for loading content.
    /// </summary>
    IContentLoader ContentLoader { get; set; }

    /// <summary>
    /// Gets the scene manager.
    /// </summary>
    ISceneManager SceneManager { get; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the scenes should be automatically loaded.
    /// </summary>
    bool AutoSceneLoading { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the scenes should be automatically unloaded.
    /// </summary>
    public bool AutoSceneUnloading { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the scenes should be automatically updated.
    /// </summary>
    public bool AutoSceneUpdating { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the scenes should be automatically rendered.
    /// </summary>
    public bool AutoSceneRendering { get; set; }

    /// <summary>
    /// Shows the window.
    /// </summary>
    void Show();

    /// <summary>
    /// Shows the window asynchronously.
    /// </summary>
    /// <param name="afterStart">Executed after the application starts asynchronously.</param>
    /// <param name="afterUnload">Executed after the window has been unloaded.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    Task ShowAsync(Action? afterStart = null, Action? afterUnload = null);

    /// <summary>
    /// Closes the window.
    /// </summary>
    void Close();
}
