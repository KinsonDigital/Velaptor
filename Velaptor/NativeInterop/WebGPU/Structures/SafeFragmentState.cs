// <copyright file="SafeFragmentState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Structures;

using Handles;

/// <summary>
/// A safe version of the fragment stage descriptor. Holds a safe shader module
/// handle, a managed entry-point name, and a managed array of color targets.
/// </summary>
internal struct SafeFragmentState
{
    /// <summary>
    /// The compiled fragment shader module.
    /// </summary>
    public SafeShaderModuleHandle Module;

    /// <summary>
    /// The name of the fragment entry-point function (e.g. <c>"fs_main"</c>).
    /// </summary>
    public string EntryPoint;

    /// <summary>
    /// The color targets for this fragment stage (usually one).
    /// </summary>
    public SafeColorTarget[] Targets;
}
