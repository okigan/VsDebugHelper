using System;
using System.Collections.Generic;

using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VSMemoryDumpAddin {
    class Util {
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
