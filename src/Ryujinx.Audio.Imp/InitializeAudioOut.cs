using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ryujinx.Audio.Backends.CompatLayer;
using Ryujinx.Audio.Backends.Dummy;
using Ryujinx.Audio.Backends.OpenAL;
using Ryujinx.Audio.Backends.SDL2;
using Ryujinx.Audio.Backends.SoundIo;
using Ryujinx.Audio.Common;
using Ryujinx.Audio.Input;
using Ryujinx.Audio.Integration;
using Ryujinx.Audio.Output;
using Ryujinx.Audio.Renderer.Device;
using Ryujinx.Audio.Renderer.Server;
using Ryujinx.Cpu;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Threading;
using Ryujinx.HLE.HOS.Services.Audio.AudioRenderer;

namespace Ryujinx.Audio.UserInterface
{
    public class InitializeAudioOut
    {
        internal ITickSource TickSource { get; } = new TickSource(KernelConstants.CounterFrequency);
        internal AudioManager AudioManager { get; private set; }
        internal AudioOutputManager AudioOutputManager { get; private set; }
        internal AudioInputManager AudioInputManager { get; private set; }
        internal AudioRendererManager AudioRendererManager { get; private set; }
        internal VirtualDeviceSessionRegistry AudioDeviceSessionRegistry { get; private set; }
        internal KernelContext KernelContext { get; }

        private IHardwareDeviceDriver InitializeAudioDeviceDriver()
        {
            var backend = InitializeAudioBackend();
            if (backend == null)
                throw new DriveNotFoundException("Can not initialize audio backend.");
            var audioDeviceDriver = new CompatLayerHardwareDeviceDriver(InitializeAudioBackend());
            return audioDeviceDriver;
        }

        private IHardwareDeviceDriver InitializeAudioBackend()
        {
            static IHardwareDeviceDriver InitializeAudioBackendIfSupported<T>(AudioBackend backend, AudioBackend nextBackend) where T : IHardwareDeviceDriver, new()
            {
                if (T.IsSupported)
                {
                    return new T();
                }
                Console.WriteLine($"{backend} is not supported, falling back to {nextBackend}.");
                return null;
            }

            var availableBackends = new List<AudioBackend>
            {
                AudioBackend.SDL2,
                AudioBackend.SoundIo,
                AudioBackend.OpenAl,
                AudioBackend.Dummy,
            };
            IHardwareDeviceDriver deviceDriver = null;

            for (int i = 0; i < availableBackends.Count; i++)
            {
                AudioBackend currentBackend = availableBackends[i];
                AudioBackend nextBackend = i + 1 < availableBackends.Count ? availableBackends[i + 1] : AudioBackend.Dummy;

                deviceDriver = currentBackend switch
                {
                    AudioBackend.SDL2 => InitializeAudioBackendIfSupported<SDL2HardwareDeviceDriver>(AudioBackend.SDL2, nextBackend),
                    AudioBackend.SoundIo => InitializeAudioBackendIfSupported<SoundIoHardwareDeviceDriver>(AudioBackend.SoundIo, nextBackend),
                    AudioBackend.OpenAl => InitializeAudioBackendIfSupported<OpenALHardwareDeviceDriver>(AudioBackend.OpenAl, nextBackend),
                    _ => new DummyHardwareDeviceDriver(),
                };

                if (deviceDriver != null)
                {
                    break;
                }
            }

            return deviceDriver;
        }

        public void InitializeAudioRenderer(AudioInputConfiguration parameter, out AudioOutputSystem outSystem)
        {
            outSystem = null;
            int audioVolume = 100;
            var audioDeviceDriver = InitializeAudioDeviceDriver();

            AudioManager = new AudioManager();
            AudioOutputManager = new AudioOutputManager();
            AudioInputManager = new AudioInputManager();
            AudioRendererManager = new AudioRendererManager(TickSource);
            AudioRendererManager.SetVolume(audioVolume);
            AudioDeviceSessionRegistry = new VirtualDeviceSessionRegistry(audioDeviceDriver);

            IWritableEvent[] audioOutputRegisterBufferEvents = new IWritableEvent[Constants.AudioOutSessionCountMax];

            for (int i = 0; i < audioOutputRegisterBufferEvents.Length; i++)
            {
                KEvent registerBufferEvent = new(KernelContext);

                audioOutputRegisterBufferEvents[i] = new AudioKernelEvent(registerBufferEvent);
            }

            AudioOutputManager.Initialize(audioDeviceDriver, audioOutputRegisterBufferEvents);
            AudioOutputManager.SetVolume(audioVolume);

            IWritableEvent[] audioInputRegisterBufferEvents = new IWritableEvent[Constants.AudioInSessionCountMax];

            for (int i = 0; i < audioInputRegisterBufferEvents.Length; i++)
            {
                KEvent registerBufferEvent = new KEvent(KernelContext);

                audioInputRegisterBufferEvents[i] = new AudioKernelEvent(registerBufferEvent);
            }



            AudioInputManager.Initialize(audioDeviceDriver, audioInputRegisterBufferEvents);

            IWritableEvent[] systemEvents = new IWritableEvent[Constants.AudioRendererSessionCountMax];

            for (int i = 0; i < systemEvents.Length; i++)
            {
                KEvent systemEvent = new(KernelContext);

                systemEvents[i] = new AudioKernelEvent(systemEvent);
            }

            AudioManager.Initialize(audioDeviceDriver.GetUpdateRequiredEvent(), AudioOutputManager.Update, AudioInputManager.Update);

            AudioRendererManager.Initialize(systemEvents, audioDeviceDriver);

            AudioManager.Start();


            var list = AudioOutputManager.ListAudioOuts();


            string inputDeviceName = "";

            ResultCode result = AudioOutputManager
                .OpenAudioOut(out string outputDeviceName, out AudioOutputConfiguration outputConfiguration, out outSystem, null, inputDeviceName, SampleFormat.PcmInt16, ref parameter, 0, 0, 100);
            if (result != ResultCode.Success)
            {
                throw new Exception($"Can not open out device, {result}");
            }
            result = outSystem.Start();
            if (result != ResultCode.Success)
            {
                throw new Exception($"Can not start audio out system, {result}");
            }
        }

    }
}
