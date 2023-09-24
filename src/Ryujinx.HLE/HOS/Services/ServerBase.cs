using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Ryujinx.Common.Logging;
using Ryujinx.Common.Memory;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Process;
using Ryujinx.HLE.HOS.Kernel.Threading;

namespace Ryujinx.HLE.HOS.Services
{
    public class ServerBase : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
