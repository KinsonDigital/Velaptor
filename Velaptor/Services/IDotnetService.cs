namespace Velaptor.Services;

/// <summary>
/// Invokes Dotnet functions.
/// </summary>
public interface IDotnetService
{
    /// <summary>
    /// Invokes System.GC.KeepAlive method.
    /// </summary>
    /// <param name="obj">The object to reference.</param>
    public void GCKeepAlive(object? obj);

    /// <summary>
    /// Invokes System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi method.
    /// </summary>
    /// <param name="s">A managed string to be copied.</param>
    /// <returns>The address, in unmanaged memory, to where s was copied, or 0 if s is null.</returns>
    public nint MarshalStringToHGlobalAnsi(string? s);

    /// <summary>
    /// Invokes System.Runtime.InteropServices.Marshal.PtrToStringAnsi method.
    /// </summary>
    /// <param name="ptr">The address of the first character of the unmanaged string.</param>
    /// <returns>A managed string that holds a copy of the unmanaged string. If ptr is null, the method returns a null string.</returns>
    public string? MarshalPtrToStringAnsi(nint ptr);
}
