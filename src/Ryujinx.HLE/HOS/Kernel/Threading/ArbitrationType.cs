namespace Ryujinx.HLE.HOS.Kernel.Threading
{
    public enum ArbitrationType
    {
        WaitIfLessThan = 0,
        DecrementAndWaitIfLessThan = 1,
        WaitIfEqual = 2,
    }
}
