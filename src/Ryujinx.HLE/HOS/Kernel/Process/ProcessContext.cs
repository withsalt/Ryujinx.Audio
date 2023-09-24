using System;
using Ryujinx.Cpu;
using Ryujinx.Memory;

namespace Ryujinx.HLE.HOS.Kernel.Process
{
    public class ProcessContext : IProcessContext
    {
        public IVirtualMemoryManager AddressSpace { get; }

        public ulong AddressSpaceSize { get; }

        public ProcessContext(IVirtualMemoryManager asManager, ulong addressSpaceSize)
        {
            AddressSpace = asManager;
            AddressSpaceSize = addressSpaceSize;
        }

        public IExecutionContext CreateExecutionContext(ExceptionCallbacks exceptionCallbacks)
        {
            return new ProcessExecutionContext();
        }

        public void Execute(IExecutionContext context, ulong codeAddress)
        {
            throw new NotSupportedException();
        }

        public void InvalidateCacheRegion(ulong address, ulong size)
        {
        }

        public void Dispose()
        {
        }
    }
}
