// <copyright file="GraphicsShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using System.Globalization;
using Handles;

/// <summary>
/// Loads a WGSL shader template from disk, substitutes sprite-size placeholders,
/// and compiles the result into a GPU shader module.
/// </summary>
/// <remarks>
/// <para>
/// The <c>.wgsl</c> file may contain the following placeholder tokens, which are replaced
/// at load time with computed float values formatted with <see cref="CultureInfo.InvariantCulture"/>
/// so the WGSL decimal separator is always <c>.</c> regardless of the system locale:
/// <list type="bullet">
///   <item><description><c>{HW}</c> — sprite half-width  in world pixels (<c>imageWidth  / 2</c>).</description></item>
///   <item><description><c>{HH}</c> — sprite half-height in world pixels (<c>imageHeight / 2</c>).</description></item>
/// </list>
/// These are world-space pixel extents, not clip-space ratios. The camera's
/// view-projection matrix (uploaded each frame via <see cref="Camera2D"/>) converts
/// them to clip space at draw time.
/// </para>
/// <para>
/// Once <see cref="GraphicsPipeline"/> is built from <see cref="Handle"/>, the pipeline
/// retains its own internal reference to the compiled code. It is therefore safe — and
/// recommended — to dispose this object immediately after the pipeline is created.
/// </para>
/// </remarks>
internal sealed class GraphicsShader : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsShader"/> class.
    /// Loads the WGSL template from <paramref name="filePath"/>, substitutes the
    /// sprite half-size placeholders, and compiles the result into a shader module.
    /// </summary>
    /// <param name="gd">The graphics device used to compile the shader.</param>
    /// <param name="filePath">Path to the <c>.wgsl</c> template file.</param>
    /// <param name="spriteHalfWidth">
    /// Sprite half-width in world pixels: <c>imageWidth / 2</c>.
    /// Substituted for the <c>{HW}</c> token in the template.
    /// </param>
    /// <param name="spriteHalfHeight">
    /// Sprite half-height in world pixels: <c>imageHeight / 2</c>.
    /// Substituted for the <c>{HH}</c> token in the template.
    /// </param>
    /// <exception cref="Exception">Thrown if the shader module could not be compiled.</exception>
    public GraphicsShader(GraphicsDevice gd, string filePath, float spriteHalfWidth, float spriteHalfHeight)
    {
        var template = File.ReadAllText(filePath);

        // Pre-format floats with InvariantCulture: WGSL always requires '.' as the decimal
        // separator, but the system locale may use ',' (e.g. German, French).
        var hw = spriteHalfWidth.ToString("F6", CultureInfo.InvariantCulture);
        var hh = spriteHalfHeight.ToString("F6", CultureInfo.InvariantCulture);

        var wgsl = template
            .Replace("{HW}", hw, StringComparison.Ordinal)
            .Replace("{HH}", hh, StringComparison.Ordinal);

        Handle = gd.CreateShaderModule(wgsl);
    }

    /// <summary>
    /// Gets the compiled GPU shader module handle. Pass this (via the owning
    /// <see cref="GraphicsShader"/>) to <see cref="GraphicsPipeline"/> during construction.
    /// After the pipeline is built, this object may be safely disposed.
    /// </summary>
    public SafeShaderModuleHandle Handle { get; }

    /// <summary>
    /// Releases the shader module handle. Any <see cref="GraphicsPipeline"/> that was
    /// built from this shader retains its own internal reference to the compiled code
    /// and remains valid after disposal.
    /// </summary>
    public void Dispose() => Handle.Dispose();
}
