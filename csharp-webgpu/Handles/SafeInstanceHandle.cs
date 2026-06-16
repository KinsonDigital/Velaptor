using csharp_webgpu.NativeInterop.WebGPU;
using Microsoft.Win32.SafeHandles;

internal sealed class SafeInstanceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    public SafeInstanceHandle(WGPUInvoker wgpu, nint handle)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle(handle);
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.InstanceRelease(this.handle);
        }

        return true;
    }
}
