namespace Ryujinx.HLE.HOS.Kernel.Ipc
{
    public enum ChannelState
    {
        NotInitialized,
        Open,
        ClientDisconnected,
        ServerDisconnected,
    }
}
