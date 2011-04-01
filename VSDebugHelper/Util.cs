using System;
using System.Collections.Generic;

using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Globalization;
using System.IO;

namespace VSMemoryDump {
    public class Util {
        public static void WriteMemoryToFile(string fileName, int processId, int fromAddress, int lengthToRead) {
            IntPtr handle = NativeApi.OpenProcess(NativeApi.PROCESS_VM_READ, 0, (uint)processId);

            using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                byte[] buffer = new byte[4096];
                IntPtr read;
                for (int i = 0; i < lengthToRead; i += read.ToInt32()) {
                    NativeApi.ReadProcessMemory(handle
                        , (IntPtr)(fromAddress + i)
                        , buffer
                        , (uint)Math.Min(lengthToRead - i, buffer.Length)
                        , out read
                    );
                    fs.Write(buffer, 0, read.ToInt32());
                }
            }

            NativeApi.CloseHandle(handle);
        }

        public static void ReadFileToMemory(string fileName, int processId, int toAddress, int lengthToRead) {
            IntPtr handle = NativeApi.OpenProcess(
                NativeApi.PROCESS_VM_OPERATION
                | NativeApi.PROCESS_VM_WRITE
                //| NativeApi.PROCESS_VM_READ
                //| NativeApi.PROCESS_QUERY_INFORMATION
                , 0
                , (uint)processId
            );

            using (FileStream fs = new FileStream(fileName, FileMode.Open)) {
                byte[] buffer = new byte[4096];
                int read;
                for (int i = 0; i < lengthToRead; i += read) {
                    read = fs.Read(buffer, 0, Math.Min(lengthToRead - i, buffer.Length));

                    int written;
                    bool bret = NativeApi.WriteProcessMemory(handle
                        , (IntPtr)(toAddress + i)
                        , buffer
                        , (uint)read
                        , out written
                    );
                }
            }

            NativeApi.CloseHandle(handle);
        }

        public static bool CallTryParse(string stringToConvert, NumberStyles styles, out int number) {
            CultureInfo provider;

            // If currency symbol is allowed, use en-US culture.
            if ((styles & NumberStyles.AllowCurrencySymbol) > 0)
                provider = new CultureInfo("en-US");
            else
                provider = CultureInfo.InvariantCulture;

            bool result = Int32.TryParse(stringToConvert, styles, provider, out number);

            if (false == result && (styles & NumberStyles.AllowHexSpecifier) != 0) {
                string substring = stringToConvert.Substring(2);

                result = Int32.TryParse(substring, styles, provider, out number);

            }

            return result;

        }

        public static Type GetExcelTypeForComObject(object excelComObject, Type type) {
            // enum all the types defined in the interop assembly
            System.Reflection.Assembly assembly =
            System.Reflection.Assembly.GetAssembly(type);

            return GetExcelTypeForComObject(excelComObject, assembly);
        }

        public static Type GetExcelTypeForComObject(object excelComObject, Assembly assembly) {
            return GetExcelTypeForComObject(excelComObject, assembly.GetTypes());
        }
        public static Type GetExcelTypeForComObject(object excelComObject, Type[] types) {
            // get the com object and fetch its IUnknown
            IntPtr iunkwn = Marshal.GetIUnknownForObject(excelComObject);

            // find the first implemented interop type
            foreach (Type currType in types) {
                // get the iid of the current type
                Guid iid = currType.GUID;
                if (!currType.IsInterface || iid == Guid.Empty) {
                    // com interop type must be an interface with valid iid
                    continue;
                }

                // query supportability of current interface on object
                IntPtr ipointer = IntPtr.Zero;
                Marshal.QueryInterface(iunkwn, ref iid, out ipointer);

                if (ipointer != IntPtr.Zero) {
                    // yeah, that’s the one we’re after
                    return currType;
                }
            }

            // no implemented type found
            return null;
        }
    }
}
