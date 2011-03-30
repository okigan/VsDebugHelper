using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;

namespace VSMemoryDump {
    class NativeApi {
        public const uint PROCESS_QUERY_INFORMATION = (0x0400);
        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = (0x0010);
        public const uint PROCESS_VM_WRITE = (0x0020);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
                UInt32 dwDesiredAccess
            ,   Int32 bInheritHandle
            ,   UInt32 dwProcessId
        );

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(
                IntPtr hProcess
            ,   IntPtr lpBaseAddress
            ,   [In, Out] byte[] buffer
            ,   UInt32 size
            ,   out IntPtr lpNumberOfBytesRead
        );
        
        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(
                IntPtr hProcess
            ,   IntPtr lpBaseAddress
            ,   byte[] lpBuffer
            ,   uint nSize
            ,   out int lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(
                IntPtr hObject
        );


        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(
                IntPtr hProcess
            ,   IntPtr lpAddress
            ,   UIntPtr dwSize
            ,   uint flNewProtect
            ,   out uint lpflOldProtect
        );


        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [DllImport("kernel32.dll")]
        public static extern bool VirtualQueryEx(
                IntPtr hProcess
            ,   IntPtr lpAddress
            ,   out MEMORY_BASIC_INFORMATION lpBuffer
            ,   uint dwLength
        );
    }

}
