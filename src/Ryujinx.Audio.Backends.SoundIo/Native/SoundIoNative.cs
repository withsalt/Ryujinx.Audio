using Ryujinx.Common.Memory;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ryujinx.Audio.Backends.SoundIo.Native
{
    public static partial class SoundIo
    {
        private static string DEFAULT_LIBRARY_PATH = "runtimes";
        private static string DEFAULT_LIBRARY_NAME = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "libsoundio" : "libsoundio";

        static SoundIo()
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetAssembly(typeof(SoundIo)), (libraryName, assembly, searchPath) =>
            {
                IntPtr ptr = NativeLibrary.Load(GetLibraryFullName(), assembly, searchPath ?? DllImportSearchPath.AssemblyDirectory);
                return ptr;
            });
        }

        /// <summary>
        /// 获取库完整路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetLibraryFullName()
        {
            string name;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                name = $"{DEFAULT_LIBRARY_NAME}.dll";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                name = $"{DEFAULT_LIBRARY_NAME}.so";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                name = $"{DEFAULT_LIBRARY_NAME}.dylib";
            else
                throw new PlatformNotSupportedException($"Unsupported operating system type: {RuntimeInformation.OSDescription}");

            string archPath = GetArchitecturePath();
            if (string.IsNullOrWhiteSpace(archPath))
                throw new PlatformNotSupportedException($"Unsupported processor architecture: {RuntimeInformation.ProcessArchitecture}, system type: {RuntimeInformation.OSDescription}");

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DEFAULT_LIBRARY_PATH, GetArchitecturePath(), "native", name);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Can not find library from {path}");
            return path;
        }

        private static string GetArchitecturePath()
        {
            string architecture = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                architecture = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X86 => "win-x86",
                    Architecture.X64 => "win-x64",
                    Architecture.Arm64 => "win-arm64",
                    _ => throw new PlatformNotSupportedException($"Unsupported processor architecture: {RuntimeInformation.ProcessArchitecture}"),
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                architecture = RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => "linux-x64",
                    Architecture.Arm => "linux-arm",
                    Architecture.Arm64 => "linux-arm64",
#if NET7_0_OR_GREATER
                    Architecture.LoongArch64 => "linux-loongarch64",
#endif
                    _ => throw new PlatformNotSupportedException($"Unsupported processor architecture: {RuntimeInformation.ProcessArchitecture}"),
                };
            }
            else
            {
                throw new PlatformNotSupportedException($"Unsupported system type: {RuntimeInformation.OSDescription}");
            }
            return architecture;
        }
    }
}
