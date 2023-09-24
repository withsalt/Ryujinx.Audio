using System;
using Ryujinx.Cpu;
using Ryujinx.Memory;

namespace Ryujinx.HLE.HOS.Kernel.Process
{
    public interface IProcessContext : IDisposable
    {
        IVirtualMemoryManager AddressSpace { get; }

        ulong AddressSpaceSize { get; }

        IExecutionContext CreateExecutionContext(ExceptionCallbacks exceptionCallbacks);
        void Execute(IExecutionContext context, ulong codeAddress);
        void InvalidateCacheRegion(ulong address, ulong size);
    }
}
