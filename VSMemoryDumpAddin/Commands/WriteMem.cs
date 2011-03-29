using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;



namespace VSMemoryDumpAddin.Commands {
    class Sys {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            UInt32 dwDesiredAccess,
            Int32 bInheritHandle,
            UInt32 dwProcessId
            );

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [In, Out] byte[] buffer,
            UInt32 size,
            out IntPtr lpNumberOfBytesRead
            );

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(
            IntPtr hObject
            );
    }

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

    class WriteMem : ICommand {
        #region members
        private DTE2 _application;
        private AddIn _addin;
        #endregion

        #region ICommand Members

        void ICommand.Initialize(EnvDTE80.DTE2 application, EnvDTE.AddIn addin) {
            _application = application;
            _addin = addin;
        }

        string ICommand.CommandText {
            get { return "writemem"; }
        }

        void ICommand.Exec(string cmdName
            , vsCommandExecOption executeOption
            , ref object variantIn
            , ref object variantOut
            , ref bool handled
        ) {

            switch (executeOption) {
            case vsCommandExecOption.vsCommandExecOptionDoDefault: {
                var x = Util.GetExcelTypeForComObject(_application.Debugger, _application.Debugger.GetType());
                var y = Util.GetExcelTypeForComObject(_application.Debugger, typeof(IDebugProperty2));

                //in future make this behave as windbg http://msdn.microsoft.com/en-us/library/ff566176(v=vs.85).aspx
                //for now lets just make it as:
                // filename address size

                string commandline = variantIn as string;

                char[]  sp = new char[]{' ', '\t'};
                string[] argv = commandline.Split(sp);

                if (argv.Length != 3) {
                    throw new NotSupportedException();
                }



                string fileName = argv[0];
                string variableOrAddress = argv[1];
                string variableOrSize = argv[2];

                var variableOrAddressExp = _application.Debugger.GetExpression(variableOrAddress, false, 100);
                var variableOrSizeExp = _application.Debugger.GetExpression(variableOrSize, true, 100);

                int processId = _application.Debugger.CurrentProcess.ProcessID;

                var process = System.Diagnostics.Process.GetProcessById(processId);
                var pmr = new ProcessMemoryReaderLib.ProcessMemoryReader();
                pmr.ReadProcess = process;
                pmr.OpenProcess();
                int bytesRead = 0;

                int address = 0;
                CallTryParse(variableOrAddressExp.Value, NumberStyles.HexNumber, out address);
                byte[] bbb = pmr.ReadProcessMemory((IntPtr) address
                    , uint.Parse(variableOrSizeExp.Value)
                    ,  out bytesRead
                );


                //variableOrAddressExp.IsValidValue 

                using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
                    fs.Write(bbb, 0, bbb.Length);
                }

                var commandWindow = _application.Windows.Item(EnvDTE.Constants.vsWindowKindCommandWindow).Object as CommandWindow;

                commandWindow.OutputString("SavedBPFormat\r\n");
            } break;
            }
        }

        private static bool CallTryParse(string stringToConvert, NumberStyles styles, out int number) {
            CultureInfo provider;

            // If currency symbol is allowed, use en-US culture.
            if ((styles & NumberStyles.AllowCurrencySymbol) > 0)
                provider = new CultureInfo("en-US");
            else
                provider = CultureInfo.InvariantCulture;

            bool result = Int32.TryParse(stringToConvert, styles,
                                         provider, out number);
            return result;
            //if (result)
            //    Console.WriteLine("Converted '{0}' to {1}.", stringToConvert, number);
            //else
            //    Console.WriteLine("Attempted conversion of '{0}' failed.",
            //                      Convert.ToString(stringToConvert));
        }

        void ICommand.QueryStatus(string cmdName
            , vsCommandStatusTextWanted neededText
            , ref vsCommandStatus statusOption
            , ref object commandText
        ) {
            switch (neededText) {
            case vsCommandStatusTextWanted.vsCommandStatusTextWantedNone: {
                if (_application.Debugger.DebuggedProcesses.Count > 0) {
                    statusOption = vsCommandStatus.vsCommandStatusSupported
                        | vsCommandStatus.vsCommandStatusEnabled;
                }
            } break;
            }

        }

        #endregion
    }
}
