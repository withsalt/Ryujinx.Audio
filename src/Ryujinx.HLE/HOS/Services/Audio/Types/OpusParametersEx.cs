using System.Runtime.InteropServices;
using Ryujinx.Common.Memory;

namespace Ryujinx.HLE.HOS.Services.Audio.Types
{
    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public struct OpusParametersEx
    {
        public int SampleRate;
        public int ChannelsCount;
        public OpusDecoderFlags Flags;

        Array4<byte> Padding1;
    }
}
