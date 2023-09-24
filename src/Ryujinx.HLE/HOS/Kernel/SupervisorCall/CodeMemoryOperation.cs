namespace Ryujinx.HLE.HOS.Kernel.SupervisorCall
{
    public enum CodeMemoryOperation : uint
    {
        Map,
        MapToOwner,
        Unmap,
        UnmapFromOwner,
    };
}
