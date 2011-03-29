using System;
using System.Globalization;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Debugger.Interop;

namespace VSMemoryDumpAddin.Commands {
    class ReadMem : ICommand {
        #region members
        private DTE2 _application;
        private AddIn _addin;
        #endregion


        #region ICommand Members

        public virtual void Initialize(DTE2 application, EnvDTE.AddIn addin) {
            _application = application;
            _addin = addin;
        }


        public virtual string CommandText {
            get { return "readmem"; }
        }

        public virtual void Exec(string cmdName
            , vsCommandExecOption executeOption
            , ref object variantIn
            , ref object variantOut
            , ref bool handled
        ) {
            switch (executeOption) {
            case vsCommandExecOption.vsCommandExecOptionDoDefault: {

                //in future make this behave as windbg http://msdn.microsoft.com/en-us/library/ff566176(v=vs.85).aspx
                //for now lets just make it as:
                // filename address size

                string commandline = variantIn as string;

                char[] sp = new char[] { ' ', '\t' };
                string[] argv = commandline.Split(sp);

                bool bRet = ExecuteDefault(argv);

                var commandWindow = _application.Windows.Item(EnvDTE.Constants.vsWindowKindCommandWindow).Object as CommandWindow;

                commandWindow.OutputString(CommandText + " " + (bRet ? "Succeded" : "Failed") + "\r\n");
            } break;
            }
        }

        public virtual void QueryStatus(string cmdName
            , vsCommandStatusTextWanted neededText
            , ref vsCommandStatus statusOption
            , ref object commandText
        ) {
            switch (neededText) {
            case vsCommandStatusTextWanted.vsCommandStatusTextWantedNone: {
                if (null != _application.Debugger && _application.Debugger.DebuggedProcesses.Count > 0) {
                    statusOption = vsCommandStatus.vsCommandStatusSupported
                        | vsCommandStatus.vsCommandStatusEnabled;
                }
            } break;
            }
        }

        #endregion

        #region internal
        private bool ExecuteDefault(string[] argv) {
            if (argv.Length != 3) {
                throw new ArgumentException();
            }

            string fileName = argv[0];
            string variableOrAddress = argv[1];
            string variableOrSize = argv[2];

            var variableOrAddressExp = _application.Debugger.GetExpression(variableOrAddress, false, 100);
            var variableOrSizeExp = _application.Debugger.GetExpression(variableOrSize, true, 100);
            int processId = _application.Debugger.CurrentProcess.ProcessID;

            if (!variableOrAddressExp.IsValidValue || !variableOrSizeExp.IsValidValue)
                return false;

            int toAddress = 0;
            int lengthToRead = 0;
            bool bRet = true;
            //TODO: add more versatile robust expression handling
            bRet = bRet && CallTryParse(variableOrAddressExp.Value, NumberStyles.HexNumber, out toAddress);
            bRet = bRet && CallTryParse(variableOrSizeExp.Value, NumberStyles.Integer, out lengthToRead);

            if (!bRet) {
                return bRet;
            }

            IntPtr handle = NativeApi.OpenProcess(
                NativeApi.PROCESS_VM_OPERATION
                | NativeApi.PROCESS_VM_WRITE
                | NativeApi.PROCESS_VM_READ
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

                    byte[] checkbuffer = new byte[4096];
                    IntPtr checkread;
                    NativeApi.ReadProcessMemory(handle
                        , (IntPtr)(toAddress + i)
                        , checkbuffer
                        , (uint)read
                        , out checkread
                    );
                }
            }

            NativeApi.CloseHandle(handle);

            return true;
        }

        private static bool CallTryParse(string stringToConvert, NumberStyles styles, out int number) {
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

        #endregion
    }
}
