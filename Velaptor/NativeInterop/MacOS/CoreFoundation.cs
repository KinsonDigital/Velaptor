// <copyright file="CoreFoundation.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable once GrammarMistakeInComment

// =============================================================================
// macOS Native Framework Overview
// =============================================================================
//
// This file uses two low-level Apple frameworks via P/Invoke: IOKit and
// CoreFoundation. Understanding their roles and relationship is important for
// reading and maintaining this code.
//
// ── CoreFoundation ────────────────────────────────────────────────────────────
//
//   Introduced: macOS 10.0 (2001) — one of Apple's oldest and most stable APIs.
//
//   Purpose: A pure-C library of fundamental data types and utilities:
//     • CFString    — immutable Unicode strings; used here to pass property key
//                     names into IOKit and to read the GPU name back out
//     • CFRelease() — reference-counted memory management for all CF objects;
//                     must be called on every CF object we own to avoid leaks
//
//   Think of it as Apple's C equivalent of .NET's System namespace primitives.
//
//   Stability: Extremely stable. These APIs are essentially unchanged since 2001.
// =============================================================================

// ReSharper enable once GrammarMistakeInComment
namespace Velaptor.NativeInterop.MacOS;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// P/Invoke bindings for the CoreFoundation framework (<c>CoreFoundation.framework</c>).
/// </summary>
/// <remarks>
/// CoreFoundation is Apple's low-level C library of fundamental data types, introduced in
/// macOS 10.0 (2001). It underpins all Objective-C and Swift collection types via toll-free
/// bridging — <c>CFString</c> and <c>NSString</c>, for example, are the same object at runtime.
/// <para/>
/// This class exposes only the subset of CoreFoundation needed to interact with IOKit: creating
/// CFString keys to pass into <c>IORegistryEntryCreateCFProperty</c>, reading the resulting
/// CFString values back out, and releasing owned CF objects when done.
/// <para/>
/// <b>Memory management — the Create/Get Rule:</b><br/>
/// Any CF object returned from a function whose name contains <c>Create</c> or <c>Copy</c>
/// is owned by the caller and <b>must</b> be released with <see cref="CFRelease"/>. Objects
/// returned by <c>Get</c> functions are <b>not</b> owned by the caller and must <b>not</b>
/// be released.
/// See https://developer.apple.com/library/archive/documentation/CoreFoundation/Conceptual/CFMemoryMgmt/Concepts/Ownership.html.
/// </remarks>
internal static partial class CoreFoundation
{
    /// <summary>
    /// The CoreFoundation encoding constant for UTF-8 (<c>kCFStringEncodingUTF8</c>, value <c>0x08000100</c>).
    /// </summary>
    /// <remarks>
    /// Pass this value as the <c>encoding</c> argument to <see cref="CFStringCreateWithCString"/> and
    /// <see cref="CFStringGetCString"/> whenever working with UTF-8 encoded C strings.
    /// See https://developer.apple.com/documentation/corefoundation/cfstringbuiltinencodings/kcfstringencodingutf8.
    /// </remarks>
    internal const uint CFStringEncodingUtf8 = 0x08000100;
    private const string CoreFoundationFramework = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    /// <summary>
    /// Convenience wrapper that converts a CoreFoundation <c>CFStringRef</c> into a managed
    /// <see cref="string"/> using a stack-allocated UTF-8 buffer.
    /// </summary>
    /// <param name="cfString">
    /// A valid <c>CFStringRef</c> returned by an IOKit or CoreFoundation API.
    /// Passing <see cref="nint.Zero"/> will return <see cref="string.Empty"/>.
    /// </param>
    /// <returns>
    /// The managed string equivalent of <paramref name="cfString"/>, or <see cref="string.Empty"/>
    /// if the conversion fails or the pointer is zero.
    /// </returns>
    /// <remarks>
    /// Internally calls <see cref="CFStringGetCString"/> with a 256-byte heap buffer and
    /// <see cref="CFStringEncodingUtf8"/> encoding, then marshals the result via
    /// <c>Marshal.PtrToStringUTF8</c>.
    /// <para/>
    /// <b>Ownership:</b> This method does <b>not</b> release <paramref name="cfString"/>.
    /// The caller retains ownership and is responsible for calling <see cref="CFRelease"/>
    /// if the string was obtained from a Create/Copy function.
    /// <para/>
    /// <b>Limitation:</b> Strings longer than 255 bytes (in UTF-8) will be truncated.
    /// For variable-length strings, use <c>CFStringGetLength</c> + <c>CFStringGetMaximumSizeForEncoding</c>
    /// to calculate the required buffer size before calling <see cref="CFStringGetCString"/> directly.
    /// </remarks>
    internal static string CFStringToString(IntPtr cfString)
    {
        const int bufferSize = 256;
        var buffer = Marshal.AllocHGlobal(bufferSize);
        try
        {
            return CFStringGetCString(cfString, buffer, bufferSize, CFStringEncodingUtf8)
                ? Marshal.PtrToStringUTF8(buffer) ?? string.Empty
                : string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Creates an immutable string from a C string.
    /// </summary>
    /// <param name="allocator">
    /// The allocator to use to allocate memory for the new string. Pass <see cref="IntPtr.Zero"/>
    /// or <c>kCFAllocatorDefault</c> to use the current default allocator.
    /// </param>
    /// <param name="cStr">The C string to convert. The string must be null-terminated.</param>
    /// <param name="encoding">
    /// The encoding of <paramref name="cStr"/>. Use <see cref="CFStringEncodingUtf8"/> (0x08000100)
    /// for UTF-8. See <c>CFStringBuiltInEncodings</c> for all values.
    /// </param>
    /// <returns>
    /// An immutable string containing <paramref name="cStr"/>, or <see cref="IntPtr.Zero"/> if
    /// there was a problem creating the object.
    /// </returns>
    /// <remarks>
    /// Official docs: https://developer.apple.com/documentation/corefoundation/1542942-cfstringcreatewithcstring
    /// <para/>
    /// <b>Ownership (Create Rule):</b> The caller owns the returned object and must release it
    /// with <see cref="CFRelease"/> when done.
    /// </remarks>
    [LibraryImport(CoreFoundationFramework, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr CFStringCreateWithCString(IntPtr allocator, string cStr, uint encoding);

    /// <summary>
    /// Releases a Core Foundation object.
    /// </summary>
    /// <param name="cf">The CFType object to release. Must not be <see cref="IntPtr.Zero"/>.</param>
    /// <remarks>
    /// Official docs: https://developer.apple.com/documentation/corefoundation/1521153-cfrelease.
    /// <para/>
    /// Decrements the reference count of <paramref name="cf"/>. If the object's reference count
    /// becomes zero, it is deallocated and destroyed. If <paramref name="cf"/> is <see cref="IntPtr.Zero"/>
    /// this will cause a runtime error — always null-check before calling.
    /// <para/>
    /// <b>The Create Rule:</b> Call <c>CFRelease</c> on any CF object returned from a function
    /// whose name contains <c>Create</c> or <c>Copy</c>. Do <b>not</b> call it on objects
    /// returned by <c>Get</c> functions — you do not own those.
    /// See https://developer.apple.com/library/archive/documentation/CoreFoundation/Conceptual/CFMemoryMgmt/Concepts/Ownership.html.
    /// </remarks>
    [LibraryImport(CoreFoundationFramework)]
    internal static partial void CFRelease(IntPtr cf);

    /// <summary>
    /// Copies the character contents of a CFString into a local C string buffer after converting
    /// the characters to a given encoding.
    /// </summary>
    /// <param name="theString">The CFString to copy.</param>
    /// <param name="buffer">
    /// The C string buffer into which the contents of <paramref name="theString"/> are copied.
    /// On return the buffer contains the string in the encoding specified by
    /// <paramref name="encoding"/>. The buffer must be at least <paramref name="bufferSize"/>
    /// bytes long and is null-terminated even on failure.
    /// </param>
    /// <param name="bufferSize">
    /// The length of <paramref name="buffer"/> in bytes, including the byte required for a
    /// null-terminator.
    /// </param>
    /// <param name="encoding">
    /// The string encoding to which the character contents of <paramref name="theString"/>
    /// should be converted. Use <see cref="CFStringEncodingUtf8"/> (0x08000100) for UTF-8.
    /// </param>
    /// <returns>
    /// <c>true</c> if the operation succeeds; <c>false</c> if the conversion fails or if the
    /// buffer is too small.
    /// </returns>
    /// <remarks>
    /// Official docs: https://developer.apple.com/documentation/corefoundation/1542721-cfstringgetcstring
    /// <para/>
    /// <b>Ownership (Get Rule):</b> The caller does <b>not</b> own the returned data. The buffer
    /// is caller-allocated and caller-freed; the CFString itself must not be released here.
    /// </remarks>
    [LibraryImport(CoreFoundationFramework)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CFStringGetCString(IntPtr theString, IntPtr buffer, nint bufferSize, uint encoding);
}
