using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal sealed class SafeInstanceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeInstanceHandle(WebGPU wgpu)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;

        var desc = default(InstanceDescriptor);

        unsafe
        {
            SetHandle((nint)this.wgpu.CreateInstance(in desc));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.InstanceRelease((Instance*)this.handle);
            }
        }

        return true;
    }
}
