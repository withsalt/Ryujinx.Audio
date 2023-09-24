using System.Runtime.InteropServices;
using Ryujinx.Common.Memory;

namespace Ryujinx.HLE.HOS.Services.Audio.Types
{
    [StructLayout(LayoutKind.Sequential, Size = 0x110)]
    public struct OpusMultiStreamParameters
    {
        public int SampleRate;
        public int ChannelsCount;
        public int NumberOfStreams;
        public int NumberOfStereoStreams;
        public Array64<uint> ChannelMappings;
    }
}
