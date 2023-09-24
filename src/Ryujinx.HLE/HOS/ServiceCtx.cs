using System.IO;
using Ryujinx.HLE.HOS.Ipc;
using Ryujinx.HLE.HOS.Kernel.Process;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.Memory;

namespace Ryujinx.HLE.HOS
{
    public class ServiceCtx
    {
        public KProcess Process { get; }
        public IVirtualMemoryManager Memory { get; }
        public KThread Thread { get; }
        public IpcMessage Request { get; }
        public IpcMessage Response { get; }
        public BinaryReader RequestData { get; }
        public BinaryWriter ResponseData { get; }

        public ServiceCtx(
            KProcess process,
            IVirtualMemoryManager memory,
            KThread thread,
            IpcMessage request,
            IpcMessage response,
            BinaryReader requestData,
            BinaryWriter responseData)
        {
            Process = process;
            Memory = memory;
            Thread = thread;
            Request = request;
            Response = response;
            RequestData = requestData;
            ResponseData = responseData;
        }
    }
}
