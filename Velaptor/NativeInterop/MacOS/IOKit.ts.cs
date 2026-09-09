// <copyright file="IOKit.ts.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.MacOS;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

/// <summary>
/// P/Invoke bindings for the IOKit framework (<c>IOKit.framework</c>), specifically the
/// user-space hardware enumeration APIs declared in <c>IOKitLib.h</c>.
/// </summary>
/// <remarks>
/// IOKit is Apple's hardware abstraction layer, introduced in macOS 10.0 (2001). It exposes
/// the <b>IORegistry</b> — a live, kernel-maintained tree of every hardware service currently
/// present on the machine: GPUs, CPUs, storage controllers, USB devices, display connectors, etc.
/// <para/>
/// This class exposes only the subset of IOKit needed to locate GPU hardware services and read
/// their properties. The general pattern for every query in this codebase is:
/// <code>
/// IOServiceMatching("ServiceClass")        // 1. Build a class-name filter dictionary
///   → IOServiceGetMatchingServices(...)    // 2. Enumerate matching services into an iterator
///     → IOIteratorNext(iterator)           // 3. Walk the iterator one service at a time
///       → IORegistryEntryCreateCFProperty  // 4. Read a named property (returns a CF object)
///         → CFRelease(property)            // 5. Release the CF object when done (CoreFoundation)
///     → IOObjectRelease(service)           // 6. Release each service handle
///   → IOObjectRelease(iterator)            // 7. Release the iterator
/// </code>
/// <para/>
/// <b>IOKit service classes used in this project:</b>
/// <list type="table">
///   <listheader><term>Class name</term><description>Purpose</description></listheader>
///   <item><term><c>AGXAccelerator</c></term><description>Apple Silicon GPU driver — exposes <c>"model"</c> and <c>"gpu-core-count"</c></description></item>
///   <item><term><c>IOPCIDevice</c></term><description>Intel discrete GPU — exposes <c>"VRAM,totalMB"</c> as CFNumber or CFData</description></item>
///   <item><term><c>IOMobileFramebuffer</c></term><description>Apple Silicon display controller — exposes <c>DisplayAttributes → ProductAttributes → ProductName</c></description></item>
///   <item><term><c>IODisplayConnect</c></term><description>Intel Mac display connector — exposes <c>DisplayProductName</c> via <c>IODisplayCreateInfoDictionary</c></description></item>
/// </list>
/// <para/>
/// <b>Memory management:</b> IOKit service and iterator handles (<c>io_object_t</c>,
/// <c>io_iterator_t</c>) are released with <see cref="IOObjectRelease"/>. CF objects returned
/// by <see cref="IORegistryEntryCreateCFProperty"/> live in CoreFoundation's reference-counted
/// heap and must be released with <c>CFRelease</c> — not <c>IOObjectRelease</c>.
/// <para/>
/// Framework reference: https://developer.apple.com/documentation/iokit<br/>
/// Header source: https://opensource.apple.com/source/IOKitUser/ → IOKitLib.h.
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with native macOS interop.")]
internal static partial class IOKit
{
    private const string IOKitFramework = "/System/Library/Frameworks/IOKit.framework/IOKit";

    /// <summary>
    /// Creates a CFMutableDictionary that can be used to match IOKit services by their class name.
    /// </summary>
    /// <param name="name">
    /// The IOKit class name to match against, e.g. <c>"AGXAccelerator"</c> for Apple Silicon GPUs,
    /// <c>"IOPCIDevice"</c> for Intel discrete GPUs, or <c>"IOMobileFramebuffer"</c> for display
    /// controllers on Apple Silicon. Must be a non-null, null-terminated UTF-8 string.
    /// </param>
    /// <returns>
    /// A <c>CFMutableDictionary</c> (as <see cref="nint"/>) suitable for passing directly to
    /// <see cref="IOServiceGetMatchingServices"/>. Returns <see cref="nint.Zero"/> on failure.
    /// </returns>
    /// <remarks>
    /// IOKit framework: https://developer.apple.com/documentation/iokit
    /// Header source:   https://opensource.apple.com/source/IOKitUser/ → IOKitLib.h
    /// <para/>
    /// <b>Ownership:</b> The returned dictionary is consumed (retained then released) by
    /// <see cref="IOServiceGetMatchingServices"/> — do <b>not</b> call <c>CFRelease</c> on it
    /// separately. Doing so will cause a double-free crash.
    /// </remarks>
    [LibraryImport(IOKitFramework, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial IntPtr IOServiceMatching(string name);

    /// <summary>
    /// Looks up registered IOService objects that match the specified matching dictionary and
    /// returns an iterator over the results.
    /// </summary>
    /// <param name="masterPort">
    /// The master port obtained from <c>IOMasterPort</c>. Pass <c>0</c> to use
    /// <c>kIOMainPortDefault</c> (the default mach port), which is correct for all standard use.
    /// </param>
    /// <param name="matching">
    /// A CFDictionary containing the matching criteria, typically obtained from
    /// <see cref="IOServiceMatching"/>. The call consumes this dictionary — do not
    /// <c>CFRelease</c> it after passing it here.
    /// </param>
    /// <param name="iterator">
    /// On success, receives an <c>io_iterator_t</c> handle over the matched services. The caller
    /// owns this iterator and must release it with <see cref="IOObjectRelease"/> when done,
    /// even if no matching services were found.
    /// </param>
    /// <returns>
    /// <c>0</c> (<c>kIOReturnSuccess</c>) on success, or a non-zero IOKit error code on failure.
    /// </returns>
    /// <remarks>
    /// IOKit framework: https://developer.apple.com/documentation/iokit
    /// Header source:   https://opensource.apple.com/source/IOKitUser/ → IOKitLib.h
    /// <para/>
    /// The iterator returned in <paramref name="iterator"/> may be exhausted immediately
    /// (i.e. <see cref="IOIteratorNext"/> returns <c>0</c> on the first call) if no services
    /// match — this is not an error. Always release the iterator regardless.
    /// </remarks>
    [LibraryImport(IOKitFramework)]
    internal static partial int IOServiceGetMatchingServices(uint masterPort, IntPtr matching, out uint iterator);

    /// <summary>
    /// Returns the next object in an IOKit iterator.
    /// </summary>
    /// <param name="iterator">
    /// An <c>io_iterator_t</c> handle previously obtained from
    /// <see cref="IOServiceGetMatchingServices"/>. The iterator advances forward on each call.
    /// </param>
    /// <returns>
    /// The next <c>io_object_t</c> (service handle) in the iterator, or <c>0</c> when the
    /// iterator is exhausted. A return value of <c>0</c> means all services have been visited.
    /// </returns>
    /// <remarks>
    /// IOKit framework: https://developer.apple.com/documentation/iokit
    /// Header source:   https://opensource.apple.com/source/IOKitUser/ → IOKitLib.h.
    /// <para/>
    /// <b>Ownership:</b> Each non-zero object returned is retained and owned by the caller.
    /// It must be released with <see cref="IOObjectRelease"/> when no longer needed, typically
    /// at the end of each loop iteration.
    /// </remarks>
    [LibraryImport(IOKitFramework)]
    internal static partial uint IOIteratorNext(uint iterator);

    /// <summary>
    /// Releases an IOKit object, decrementing its retained count.
    /// </summary>
    /// <param name="object">
    ///     The <c>io_object_t</c> handle to release. This can be either a service handle returned
    ///     by <see cref="IOIteratorNext"/> or an iterator handle returned by
    ///     <see cref="IOServiceGetMatchingServices"/>.
    /// </param>
    /// <returns>
    /// <c>0</c> (<c>kIOReturnSuccess</c>) on success, or a non-zero IOKit error code on failure.
    /// </returns>
    /// <remarks>
    /// IOKit framework: https://developer.apple.com/documentation/iokit
    /// Header source:   https://opensource.apple.com/source/IOKitUser/ → IOKitLib.h.
    /// <para/>
    /// Must be called on every non-zero object obtained from <see cref="IOIteratorNext"/> and
    /// on every iterator obtained from <see cref="IOServiceGetMatchingServices"/>, even if the
    /// iterator was empty. Failure to release will leak kernel resources.
    /// </remarks>
    [LibraryImport(IOKitFramework)]
    internal static partial int IOObjectRelease(uint @object);

    /// <summary>
    /// Returns the value of a named property from an IOKit registry entry as a CoreFoundation object.
    /// </summary>
    /// <param name="entry">
    /// The <c>io_registry_entry_t</c> (service handle) to read from, typically obtained from
    /// <see cref="IOIteratorNext"/>.
    /// </param>
    /// <param name="key">
    /// A <c>CFStringRef</c> naming the property to retrieve, e.g. <c>"model"</c> on
    /// <c>AGXAccelerator</c> or <c>"VRAM,totalMB"</c> on <c>IOPCIDevice</c>. Must be created
    /// with <c>CFStringCreateWithCString</c> and released by the caller after this call.
    /// </param>
    /// <param name="allocator">
    /// The CoreFoundation allocator to use for the returned object. Pass
    /// <see cref="IntPtr.Zero"/> to use the default allocator (<c>kCFAllocatorDefault</c>).
    /// </param>
    /// <param name="options">
    /// Option bits. Pass <c>0</c> (<c>kNilOptions</c>) for standard behavior.
    /// </param>
    /// <returns>
    /// A CoreFoundation object (<c>CFTypeRef</c>) representing the property value, or
    /// <see cref="IntPtr.Zero"/> if the property does not exist on this service entry.
    /// The concrete CF type depends on the property: commonly <c>CFString</c>, <c>CFNumber</c>,
    /// <c>CFData</c>, or <c>CFDictionary</c>.
    /// </returns>
    /// <remarks>
    /// IOKit framework: https://developer.apple.com/documentation/iokit
    /// Header source:   https://opensource.apple.com/source/IOKitUser/ → IOKitLib.h.
    /// <para/>
    /// <b>Ownership (Create Rule):</b> The caller owns the returned CF object and must release
    /// it with <c>CFRelease</c> when done. A return value of <see cref="IntPtr.Zero"/> must
    /// <b>not</b> be released.
    /// <para/>
    /// The runtime CF type of the returned value can be inspected with <c>CFGetTypeID</c> and
    /// compared against <c>CFStringGetTypeID</c>, <c>CFNumberGetTypeID</c>, etc. to determine
    /// how to read the value.
    /// </remarks>
    [LibraryImport(IOKitFramework)]
    internal static partial IntPtr IORegistryEntryCreateCFProperty(uint entry, IntPtr key, IntPtr allocator, uint options);
}
