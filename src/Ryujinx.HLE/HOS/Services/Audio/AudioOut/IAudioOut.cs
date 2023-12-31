﻿using System;
using Ryujinx.Audio.Common;
using Ryujinx.HLE.HOS.Kernel.Threading;

namespace Ryujinx.HLE.HOS.Services.Audio.AudioOut
{
    public interface IAudioOut : IDisposable
    {
        AudioDeviceState GetState();

        ResultCode Start();

        ResultCode Stop();

        ResultCode AppendBuffer(ulong bufferTag, ref AudioUserBuffer buffer);

        KEvent RegisterBufferEvent();

        ResultCode GetReleasedBuffers(Span<ulong> releasedBuffers, out uint releasedCount);

        bool ContainsBuffer(ulong bufferTag);

        uint GetBufferCount();

        ulong GetPlayedSampleCount();

        bool FlushBuffers();

        void SetVolume(float volume);

        float GetVolume();
    }
}
