using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ryujinx.HLE.HOS.Services
{
    public abstract class IpcService
    {
        public IReadOnlyDictionary<int, MethodInfo> CmifCommands { get; }
        public IReadOnlyDictionary<int, MethodInfo> TipcCommands { get; }

        public ServerBase Server { get; private set; }

        private IpcService _parent;
        private int _selfId;
        private bool _isDomain;

        public IpcService(ServerBase server = null)
        {
            CmifCommands = typeof(IpcService).Assembly.GetTypes()
                .Where(type => type == GetType())
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
                .SelectMany(methodInfo => methodInfo.GetCustomAttributes(typeof(CommandCmifAttribute))
                .Select(command => (((CommandCmifAttribute)command).Id, methodInfo)))
                .ToDictionary(command => command.Id, command => command.methodInfo);

            TipcCommands = typeof(IpcService).Assembly.GetTypes()
                .Where(type => type == GetType())
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
                .SelectMany(methodInfo => methodInfo.GetCustomAttributes(typeof(CommandTipcAttribute))
                .Select(command => (((CommandTipcAttribute)command).Id, methodInfo)))
                .ToDictionary(command => command.Id, command => command.methodInfo);

            Server = server;

            _parent = this;
            _selfId = -1;
        }

        public int ConvertToDomain()
        {
            _isDomain = true;

            return _selfId;
        }

        public void ConvertToSession()
        {
            _isDomain = false;
        }

        public void CallCmifMethod(ServiceCtx context)
        {
            IpcService service = this;

            if (_isDomain)
            {
                int domainWord0 = context.RequestData.ReadInt32();
                int domainObjId = context.RequestData.ReadInt32();

                int domainCmd = (domainWord0 >> 0) & 0xff;
                int inputObjCount = (domainWord0 >> 8) & 0xff;
                int dataPayloadSize = (domainWord0 >> 16) & 0xffff;

                context.RequestData.BaseStream.Seek(0x10 + dataPayloadSize, SeekOrigin.Begin);

                context.Request.ObjectIds.EnsureCapacity(inputObjCount);

                for (int index = 0; index < inputObjCount; index++)
                {
                    context.Request.ObjectIds.Add(context.RequestData.ReadInt32());
                }

                context.RequestData.BaseStream.Seek(0x10, SeekOrigin.Begin);

                if (domainCmd == 1)
                {
                    service = GetObject(domainObjId);

                    context.ResponseData.Write(0L);
                    context.ResponseData.Write(0L);
                }
                else if (domainCmd == 2)
                {

                    context.ResponseData.Write(0L);

                    return;
                }
                else
                {
                    throw new NotImplementedException($"Domain command: {domainCmd}");
                }
            }

#pragma warning disable IDE0059 // Remove unnecessary value assignment
            long sfciMagic = context.RequestData.ReadInt64();
#pragma warning restore IDE0059
            int commandId = (int)context.RequestData.ReadInt64();

            bool serviceExists = service.CmifCommands.TryGetValue(commandId, out MethodInfo processRequest);

        }

        public void CallTipcMethod(ServiceCtx context)
        {
            int commandId = (int)context.Request.Type - 0x10;

            bool serviceExists = TipcCommands.TryGetValue(commandId, out MethodInfo processRequest);

        }

        protected void MakeObject(ServiceCtx context, IpcService obj)
        {
            obj.TrySetServer(_parent.Server);

        }

        protected T GetObject<T>(ServiceCtx context, int index) where T : IpcService
        {
            int objId = context.Request.ObjectIds[index];

            IpcService obj = _parent.GetObject(objId);

            return obj is T t ? t : null;
        }

        public bool TrySetServer(ServerBase newServer)
        {
            if (Server == null)
            {
                Server = newServer;

                return true;
            }

            return false;
        }


        private IpcService GetObject(int id)
        {
            return null;
        }

        public void SetParent(IpcService parent)
        {
            _parent = parent._parent;
        }

        public virtual void DestroyAtExit()
        {

        }
    }
}
