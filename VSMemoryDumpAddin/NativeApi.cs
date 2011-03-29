using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;

namespace VSMemoryDumpAddin {
    class NativeApi {
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
    }

}
