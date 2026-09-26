// <copyright file="LibC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.MacOS;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

/// <summary>
/// P/Invoke bindings for <c>libc</c> (Apple's <c>libSystem.B.dylib</c>), exposing the BSD
/// <c>sysctlbyname</c> function used to query kernel and hardware state variables by name.
/// </summary>
/// <remarks>
/// On macOS, <c>libc</c> is provided by <c>/usr/lib/libSystem.B.dylib</c>, which is Apple's
/// unified system library incorporating the BSD C runtime, POSIX threading, and math libraries.
/// The .NET runtime resolves the import name <c>"libc"</c> to this library automatically on macOS.
/// <para/>
/// <c>sysctlbyname</c> is a BSD POSIX API that provides read (and privileged write) access to
/// a hierarchical namespace of kernel and hardware parameters, referred to as the <b>sysctl MIB</b>
/// (Management Information Base). Parameters are addressed by dotted ASCII paths such as:
/// <list type="table">
///   <listheader><term>Key</term><description>Value</description></listheader>
///   <item><term><c>hw.memsize</c></term><description>Total physical memory in bytes (<c>ulong</c>)</description></item>
///   <item><term><c>hw.optional.arm64</c></term><description><c>1</c> if running on Apple Silicon, <c>0</c> on Intel (<c>int</c>)</description></item>
///   <item><term><c>hw.perflevel0.physicalcpu</c></term><description>P-core (performance) physical CPU count on Apple Silicon (<c>int</c>)</description></item>
///   <item><term><c>hw.perflevel1.physicalcpu</c></term><description>E-core (efficiency) physical CPU count on Apple Silicon (<c>int</c>)</description></item>
///   <item><term><c>hw.cpufrequency_max</c></term><description>Maximum CPU frequency in Hz on Intel Macs (<c>ulong</c>); absent on Apple Silicon</description></item>
///   <item><term><c>machdep.cpu.brand_string</c></term><description>CPU model name string, e.g. <c>"Intel Core i9-9980HK"</c> (variable-length <c>char[]</c>)</description></item>
/// </list>
/// <para/>
/// Three overloads are provided to cover the typed output values used in this project:
/// <list type="bullet">
///   <item><c>ref ulong oldp</c> — for 64-bit integer keys such as <c>hw.memsize</c></item>
///   <item><c>ref int oldp</c>   — for 32-bit integer keys such as <c>hw.optional.arm64</c></item>
///   <item><c>IntPtr oldp</c>    — for variable-length keys such as <c>machdep.cpu.brand_string</c></item>
/// </list>
/// <para/>
/// BSD man page: https://man.freebsd.org/cgi/man.cgi?sysctl(3)<br/>
/// macOS XNU sysctl key definitions: https://opensource.apple.com/source/xnu/xnu-7195.141.2/bsd/sys/sysctl.h.auto.html.
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with native macOS interop.")]
internal static partial class LibC
{
    // ReSharper disable GrammarMistakeInComment
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Retrieves or sets a kernel or hardware state variable identified by a dotted ASCII name.
    /// </summary>
    /// <param name="name">
    /// The ASCII dotted-path name of the sysctl variable to query, e.g.
    /// <c>"hw.memsize"</c> for total physical memory, <c>"hw.optional.arm64"</c> to detect
    /// Apple Silicon, or <c>"machdep.cpu.brand_string"</c> for the CPU model string.
    /// To list all available names on a Mac, run <c>sysctl -a</c> in Terminal.
    /// </param>
    /// <param name="oldp">
    /// A pointer to a caller-allocated buffer that receives the current value of the variable.
    /// Pass <see cref="nint.Zero"/> on a first call to get the required buffer size via
    /// <paramref name="oldlenp"/> without reading any data.
    /// </param>
    /// <param name="oldlenp">
    /// On input, the size in bytes of the buffer pointed to by <paramref name="oldp"/>.
    /// On output, the number of bytes is actually written. If the buffer is too small, the function
    /// sets this to the required size and returns <c>ENOMEM</c>. Pass <see cref="nint.Zero"/>
    /// when you do not want to read the current value.
    /// </param>
    /// <param name="newp">
    /// A pointer to a buffer containing a new value to assign to the variable, or
    /// <see cref="nint.Zero"/> to perform a read-only query. Setting system values requires
    /// root privileges; pass <see cref="nint.Zero"/> for all read operations.
    /// </param>
    /// <param name="newlen">
    /// The size in bytes of the buffer pointed to by <paramref name="newp"/>.
    /// Pass <c>0</c> when <paramref name="newp"/> is <see cref="nint.Zero"/>.
    /// </param>
    /// <returns>
    /// <c>0</c> on success. On failure, returns <c>-1</c> and sets <c>errno</c>; retrieve the
    /// error code with <see cref="System.Runtime.InteropServices.Marshal.GetLastPInvokeError"/>.
    /// Common error codes: <c>ENOENT</c> (name not found), <c>ENOMEM</c> (buffer too small),
    /// <c>EPERM</c> (write requires root).
    /// </returns>
    /// <remarks>
    /// BSD man page (function signature and behavior):
    /// https://man.freebsd.org/cgi/man.cgi?sysctl(3)
    /// <para/>
    /// macOS sysctl key definitions (XNU open source):
    /// https://opensource.apple.com/source/xnu/xnu-7195.141.2/bsd/sys/sysctl.h.auto.html
    /// <para/>
    /// To browse all available sysctl keys on a live Mac, run <c>sysctl -a</c> in Terminal.
    /// <para/>
    /// This overload accepts a raw <see cref="nint"/> buffer for variable-length results such
    /// as strings (<c>machdep.cpu.brand_string</c>). Call once with <paramref name="oldp"/> =
    /// <see cref="nint.Zero"/> to get the required size, allocate with
    /// <see cref="System.Runtime.InteropServices.Marshal.AllocHGlobal(int)"/>, then call again to
    /// fill the buffer. Always free with
    /// <see cref="System.Runtime.InteropServices.Marshal.FreeHGlobal"/> in a <c>finally</c> block.
    /// </remarks>
    [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    internal static partial int sysctlbyname(string name, IntPtr oldp, ref nint oldlenp, nint newp, nint newlen);

    // ReSharper enable GrammarMistakeInComment

    /// <inheritdoc cref="sysctlbyname(string, nint, ref nint, nint, nint)"/>
    [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    internal static partial int sysctlbyname(string name, ref ulong oldp, ref nint oldlenp, nint newp, nint newlen);

    /// <inheritdoc cref="sysctlbyname(string, nint, ref nint, nint, nint)"/>
    [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
    internal static partial int sysctlbyname(string name, ref int oldp, ref nint oldlenp, nint newp, nint newlen);
}
