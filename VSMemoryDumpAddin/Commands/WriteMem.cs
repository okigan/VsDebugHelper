using System;
using System.Globalization;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Debugger.Interop;

namespace VSMemoryDump.Commands {

    class WriteMem : ICommand {
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
            get { return "writemem"; }
        }

        public virtual void Exec(string cmdName
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

            int fromAddress = 0;
            int lengthToRead = 0;
            bool bRet = true;
            //TODO: add more versatile robust expression handling
            bRet = bRet && Util.CallTryParse(variableOrAddressExp.Value, NumberStyles.HexNumber, out fromAddress);
            bRet = bRet && Util.CallTryParse(variableOrSizeExp.Value, NumberStyles.Integer, out lengthToRead);

            if (!bRet) {
                return bRet;
            }

            Util.WriteMemoryToFile(fileName, processId, fromAddress, lengthToRead);

            return true;
        }



        #endregion
    }
}
