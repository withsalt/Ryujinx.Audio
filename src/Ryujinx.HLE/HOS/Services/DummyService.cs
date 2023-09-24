namespace Ryujinx.HLE.HOS.Services
{
    public class DummyService : IpcService
    {
        public string ServiceName { get; set; }

        public DummyService(string serviceName)
        {
            ServiceName = serviceName;
        }
    }
}
