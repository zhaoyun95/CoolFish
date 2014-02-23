using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CoolFishNS.Utilities;

// ReSharper disable InconsistentNaming

namespace CoolFishNS.Management.CoolManager.D3D
{
    internal sealed class D3D11Device : D3DDevice
    {
        private const int DXGI_FORMAT_R8G8B8A8_UNORM = 0x1C;
        private const int DXGI_USAGE_RENDER_TARGET_OUTPUT = 0x20;
        private const int D3D11_SDK_VERSION = 7;
        private const int D3D_DRIVER_TYPE_HARDWARE = 1;

        public D3D11Device(Process targetProc)
            : base(targetProc, "d3d11.dll")
        {
        }

        private IntPtr _swapChain;
        private IntPtr _device;

        private IntPtr _myDxgiDll;
        private IntPtr _theirDxgiDll;

        private VTableFuncDelegate _deviceRelease;
        private VTableFuncDelegate _deviceContextRelease;
        private VTableFuncDelegate _swapchainRelease;

        protected override void InitD3D(out IntPtr d3DDevicePtr)
        {
            LoadDxgiDll();
            var scd = new SwapChainDescription
                      {
                          BufferCount = 1,
                          ModeDescription = new ModeDescription {Format = DXGI_FORMAT_R8G8B8A8_UNORM},
                          Usage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
                          OutputHandle = Form.Handle,
                          SampleDescription = new SampleDescription {Count = 1},
                          IsWindowed = true
                      };

            unsafe
            {
                IntPtr pSwapChain = IntPtr.Zero;
                IntPtr pDevice = IntPtr.Zero;
                IntPtr pImmediateContext = IntPtr.Zero;
                int ret = NativeMethods.D3D11CreateDeviceAndSwapChain((void*) IntPtr.Zero, D3D_DRIVER_TYPE_HARDWARE,
                    (void*) IntPtr.Zero, 0, (void*) IntPtr.Zero, 0,
                    D3D11_SDK_VERSION, &scd, &pSwapChain, &pDevice,
                    (void*) IntPtr.Zero, &pImmediateContext);
                Logging.Log(string.Format("D3D11CreateDeviceAndSwapChain result: {0:X}", ret));
                _swapChain = pSwapChain;
                _device = pDevice;
                d3DDevicePtr = pImmediateContext;

                if (ret >= 0)
                {
                    _swapchainRelease =
                        GetDelegate<VTableFuncDelegate>(GetVTableFuncAddress(_swapChain,
                            VTableIndexes.DXGISwapChainRelease));
                    _deviceRelease =
                        GetDelegate<VTableFuncDelegate>(GetVTableFuncAddress(_device, VTableIndexes.D3D11DeviceRelease));
                    _deviceContextRelease =
                        GetDelegate<VTableFuncDelegate>(GetVTableFuncAddress(d3DDevicePtr,
                            VTableIndexes.D3D11DeviceContextRelease));
                }
            }
        }

        private void LoadDxgiDll()
        {
            _myDxgiDll = LoadLibrary("dxgi.dll");
            if (_myDxgiDll == IntPtr.Zero)
                throw new FileLoadException(String.Format("Could not load {0}", "dxgi.dll"));

            _theirDxgiDll =
                TargetProcess.Modules.Cast<ProcessModule>().First(m => m.ModuleName == "dxgi.dll").BaseAddress;
        }

        public unsafe IntPtr GetSwapVTableFuncAbsoluteAddress(int funcIndex)
        {
            IntPtr pointer = *(IntPtr*) ((void*) _swapChain);
            pointer = *(IntPtr*) ((void*) ((int) pointer + funcIndex*4));
            IntPtr offset = IntPtr.Subtract(pointer, _myDxgiDll.ToInt32());
            return IntPtr.Add(_theirDxgiDll, offset.ToInt32());
        }

        protected override void CleanD3D()
        {
            if (_swapChain != IntPtr.Zero)
                _swapchainRelease(_swapChain);

            if (_device != IntPtr.Zero)
                _deviceRelease(_device);

            if (D3DDevicePtr != IntPtr.Zero)
                _deviceContextRelease(D3DDevicePtr);
        }

        public override int BeginSceneVtableIndex
        {
            get { return VTableIndexes.D3D11DeviceContextBegin; }
        }

        public override int EndSceneVtableIndex
        {
            get { return VTableIndexes.D3D11DeviceContextEnd; }
        }

        public override int PresentVtableIndex
        {
            get { return VTableIndexes.DXGISwapChainPresent; }
        }

        #region Embedded Types

#pragma warning disable 169

        [StructLayout(LayoutKind.Sequential)]
        public struct Rational
        {
            public int Numerator;
            public int Denominator;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ModeDescription
        {
            public int Width;
            public int Height;
            public Rational RefreshRate;
            public int Format;
            public int ScanlineOrdering;
            public int Scaling;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SampleDescription
        {
            public int Count;
            public int Quality;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SwapChainDescription
        {
            public ModeDescription ModeDescription;
            public SampleDescription SampleDescription;
            public int Usage;
            public int BufferCount;
            public IntPtr OutputHandle;
            [MarshalAs(UnmanagedType.Bool)] public bool IsWindowed;
            public int SwapEffect;
            public int Flags;
        }

        public struct VTableIndexes
        {
            public const int DXGISwapChainRelease = 2;
            public const int D3D11DeviceRelease = 2;
            public const int D3D11DeviceContextRelease = 2;

            public const int DXGISwapChainPresent = 8;

            public const int D3D11DeviceContextBegin = 0x1B;
            public const int D3D11DeviceContextEnd = 0x1C;
        }


#pragma warning restore 169

        #endregion
    }
}

// ReSharper restore InconsistentNaming