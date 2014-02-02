using System;
using System.Runtime.InteropServices;
using System.Security;

namespace CoolFishNS.Management.CoolManager
{
    internal static class NativeMethods
    {
        // credits to http://www.pinvoke.net/ for most of this.

        [SuppressUnmanagedCodeSecurity, DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibrary(string libraryName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("d3d11.dll")]
        internal static extern unsafe int D3D11CreateDeviceAndSwapChain(void* pAdapter, int driverType, void* Software,
            int flags, void* pFeatureLevels,
            int FeatureLevels, int SDKVersion,
            void* pSwapChainDesc, void* ppSwapChain,
            void* ppDevice, void* pFeatureLevel,
            void* ppImmediateContext);

        [DllImport("d3d9.dll")]
        internal static extern IntPtr Direct3DCreate9(uint sdkVersion);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer,
            int dwSize,
            out int lpNumberOfBytesRead);
    }
}